/*
 * Copyright (C) 2007 Jordi Mas i Hernàndez <jmas@softcatala.org>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Timers;
using System.ComponentModel;

using gbrainy.Core.Views;
using gbrainy.Core.Services;

namespace gbrainy.Core.Main
{
	public class GameSession : IDrawable, IDrawRequest, IMouseEvent
	{
		[Flags]
		public enum Types
		{	
			None			= 0,
			LogicPuzzles		= 2,
			Memory			= 4,
			Calculation		= 8,
			VerbalAnalogies		= 16,
			Custom			= 32,
			AllGames		= Memory | Calculation | LogicPuzzles | VerbalAnalogies
		}

		public enum SessionStatus
		{
			NotPlaying,
			Playing,
			Answered,
			Finished,
		}

		private TimeSpan game_time;
		private Game current_game;
		private GameManager game_manager;
		private System.Timers.Timer timer;
		private bool paused;
		private TimeSpan one_sec = TimeSpan.FromSeconds (1);
		private SessionStatus status;
		private ViewsControler controler;
		private ISynchronizeInvoke synchronize;
		private PlayerHistory player_history;
		private int id;
		private GameSessionHistoryExtended history;
		private GameSessionPlayList play_list;
		
		public event EventHandler DrawRequest;
		public event EventHandler <UpdateUIStateEventArgs> UpdateUIElement;
		
	
		public GameSession (ITranslations translations)
		{
			Translations = translations;
			id = 0;
			game_manager = new GameManager ();
			play_list = new GameSessionPlayList (game_manager);
			game_time = TimeSpan.Zero;

			timer = new System.Timers.Timer ();
			timer.Elapsed += TimerUpdater;
			timer.Interval = (1 * 1000); // 1 second

			controler = new ViewsControler (translations, this);
			Status = SessionStatus.NotPlaying;
			player_history = new PlayerHistory ();
			history = new GameSessionHistoryExtended ();
		}

		public int ID {
			get {return id;}
		}

		public GameSessionHistoryExtended History {
			get {return history;}
		}

		public GameTypes AvailableGames {
			get { return game_manager.AvailableGameTypes; }
		}

		public PlayerHistory PlayerHistory { 
			set { player_history = value; }
			get { return player_history; }
		}

		public ISynchronizeInvoke SynchronizingObject { 
			set { synchronize = value; }
			get { return synchronize; }
		}
	
		public Types Type {
			get { return play_list.GameType; }
			set { play_list.GameType = value; }
		}

		public GameDifficulty Difficulty {
			get { return play_list.Difficulty; }
			set { play_list.Difficulty = value; }
		}
	
		public string GameTime {
			get { return TimeSpanToStr (game_time);}
		}

		public bool Paused {
			get {return paused; }
			set {paused = value; }
		}

		public GameSessionPlayList PlayList {
			get { return play_list; }
			set { play_list = value; }
		}

		public Game CurrentGame {
			get {return current_game; }
			set {
				current_game = value; 
				controler.Game = value;
			}
		}

		public bool EnableTimer {
			get {return timer.Enabled; }
			set {timer.Enabled = value; }
		}

		public SessionStatus Status {
			get {return status; }
			set {
				status = value;
				controler.Status = value;

				if (status == SessionStatus.Answered && CurrentGame != null)
					CurrentGame.EnableMouseEvents (false);
			}
		}

		public GameManager GameManager {
			get { return game_manager;}
			set {
				game_manager = value;
				play_list.GameManager = value;
			}
		}

		public string TimePerGame {
			get {
				TimeSpan average;

				average = (history.GamesPlayed > 0) ? TimeSpan.FromSeconds (game_time.TotalSeconds / history.GamesPlayed) : game_time;
				return TimeSpanToStr (average);
			}
		}

		private ITranslations Translations { get; set; }

		public string StatusText {
			get {
				if (Status == SessionStatus.NotPlaying || Status == SessionStatus.Finished)
					return string.Empty;

				string played, time, game;

				played = String.Format (Translations.GetString ("Games played: {0} (Score: {1})"), history.GamesPlayed, history.TotalScore);
				time = String.Format (Translations.GetString ("Time: {0}"),
					paused == false ? GameTime : Translations.GetString ("Paused"));

				if (CurrentGame != null) {
					// Translators: {0} is the name of the game
	 				game = String.Format (Translations.GetString ("Game: {0}"), CurrentGame.Name);
					// Translators: text in the status bar: games played - time - game name
					return String.Format (Translations.GetString ("{0} - {1} - {2}"), played, time, game);
				} else {
					// Translators: text in the status bar: games played - time
					return String.Format (Translations.GetString ("{0} - {1}"), played, time);
				}	
			}
		}
	
		public void New ()
		{
			if (Type == Types.None)
				throw new InvalidOperationException ("You have to setup the GameSession type");

			id++;
			if (Status != SessionStatus.NotPlaying)
				End ();

			history.Clear ();
			game_time = TimeSpan.Zero;
			timer.SynchronizingObject = SynchronizingObject;
			EnableTimer = true;
		}

		public void End ()
		{
			// Making a deep copy of GameSessionHistory type (base class) for serialization
			player_history.SaveGameSession (history.Copy ());

			if (CurrentGame != null) {
				CurrentGame.DrawRequest -= GameDrawRequest;
				CurrentGame.UpdateUIElement -= GameUpdateUIElement;
				CurrentGame.Finish ();
			}

			EnableTimer = false;
			timer.SynchronizingObject = null;

			paused = false;
			CurrentGame = null;
			Status = SessionStatus.Finished;
		}

		public void NextGame ()
		{
			try
			{
				if (CurrentGame != null) {
					CurrentGame.DrawRequest -= GameDrawRequest;
					CurrentGame.UpdateUIElement -= GameUpdateUIElement;
					CurrentGame.Finish ();
				}

				history.GamesPlayed++;
				CurrentGame = play_list.GetPuzzle ();
				CurrentGame.Translations = Translations;
				CurrentGame.SynchronizingObject = SynchronizingObject;
				CurrentGame.DrawRequest += GameDrawRequest;
				CurrentGame.UpdateUIElement += GameUpdateUIElement;

				CurrentGame.Begin ();

				CurrentGame.GameTime = TimeSpan.Zero;
				Status = SessionStatus.Playing;
			}
			catch (Exception e)
			{
				Console.WriteLine ("GameSession.NextGame {0}", e);
			}
		}

		public void Pause ()
		{
			EnableTimer = false;
			paused = true;

			if (CurrentGame != null)
				CurrentGame.EnableMouseEvents (false);
		}

		public void Resume ()
		{
			EnableTimer = true;
			paused = false;

			if (CurrentGame != null)
				CurrentGame.EnableMouseEvents (true);
		}

		public bool ScoreGame (string answer)
		{
			int game_score;

			if (CurrentGame == null || Status == SessionStatus.Answered)
				return false;

			game_score = CurrentGame.Score (answer);
			history.UpdateScore (CurrentGame.Type, Difficulty, game_score);

			Status = SessionStatus.Answered;
			return (game_score > 0 ? true : false);
		}

		private void TimerUpdater (object source, ElapsedEventArgs e)
		{
			lock (this) {
				if (CurrentGame == null)
					return;

				game_time = game_time.Add (one_sec);
				CurrentGame.GameTime = CurrentGame.GameTime + one_sec;
			}

			if (UpdateUIElement == null)
				return;

			UpdateUIElement (this, new UpdateUIStateEventArgs (UpdateUIStateEventArgs.EventUIType.Time, null));
		}

		static private string TimeSpanToStr (TimeSpan time)
		{
			DateTime dtime;

			// Convert it to DateTime to be able to use CultureSensitive formatting
			dtime = new DateTime (1970, 1, 1, time.Hours, time.Minutes, time.Seconds);
			
			return dtime.Hour > 0 ? dtime.ToString ("hh:mm:ss") : dtime.ToString ("mm:ss");
		}

		public void GameUpdateUIElement (object obj, UpdateUIStateEventArgs args)
		{
			if (UpdateUIElement != null)
				UpdateUIElement (this, args);
		}

		// A game has requested a redraw, scale the request to the object
		// subscribed to GameSession.GameDrawRequest
		public void GameDrawRequest (object o, EventArgs args)
		{
			if (DrawRequest != null)
				DrawRequest (this, EventArgs.Empty);
		}

		public virtual void Draw (CairoContextEx gr, int width, int height, bool rtl)
		{
			controler.CurrentView.Draw (gr, width, height, rtl);
		}

		public void MouseEvent (object obj, MouseEventArgs args)
		{
			if (controler.CurrentView as IMouseEvent != null)
				(controler.CurrentView as IMouseEvent).MouseEvent (this, args);
		}
	}
}
