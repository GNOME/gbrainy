/*
 * Copyright (C) 2007-2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Cairo;
using Gtk;
using Gdk;
using Gnome;
using Mono.Unix;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

#if MONO_ADDINS
using Mono.Addins;
using Mono.Addins.Setup;
#endif

public class gbrainy: Program
{
	[Glade.Widget("gbrainy")] Gtk.Window app_window;
	[Glade.Widget ("toolbar_item")] Gtk.CheckMenuItem toolbar_menuitem;
	[Glade.Widget] Box drawing_vbox;
	[Glade.Widget] Gtk.VBox question_vbox;
	[Glade.Widget] Gtk.VBox solution_vbox;
	[Glade.Widget] Gtk.Entry answer_entry;
	[Glade.Widget] Gtk.Button answer_button;
	[Glade.Widget] Gtk.Button tip_button;
	[Glade.Widget] Gtk.Button next_button;
	[Glade.Widget] Gtk.Statusbar statusbar;
	[Glade.Widget] Gtk.Toolbar toolbar;
	[Glade.Widget] Gtk.Menu settings_menu;
	[Glade.Widget] Gtk.Menu help_menu;
	[Glade.Widget] Gtk.MenuItem pause_menuitem;
	[Glade.Widget] Gtk.MenuItem finish_menuitem;
	[Glade.Widget] Gtk.MenuItem newgame_menuitem;
	GameDrawingArea drawing_area;
	GameSession session;
	ToolButton all_tbbutton, logic_tbbutton, calculation_tbbutton, memory_tbbutton, verbal_tbbutton, pause_tbbutton, finish_tbbutton;
	TextTag tag_green;
	bool low_res;
	bool full_screen;
	SimpleLabel question_label;
	SimpleLabel solution_label;
	
	public static PlayerHistory history = null;
	public static Preferences preferences = null;
 
	public gbrainy (string [] args, params object [] props)
	: base ("gbrainy", Defines.VERSION, Modules.UI,  args, props)
	{
		Catalog.Init ("gbrainy", Defines.GNOME_LOCALE_DIR);
		FixLocaleInfo ();

		IconFactory icon_factory = new IconFactory ();
                AddIcon (icon_factory, "logic-games", "logic-games-32.png");
		AddIcon (icon_factory, "math-games", "math-games-32.png");
		AddIcon (icon_factory, "memory-games", "memory-games-32.png");
		AddIcon (icon_factory, "verbal-games", "verbal-games-32.png");
		AddIcon (icon_factory, "pause", "pause-32.png");
		AddIcon (icon_factory, "resume", "resume-32.png");
		AddIcon (icon_factory, "endgame", "endgame-32.png");
		AddIcon (icon_factory, "allgames", "allgames-32.png");
		icon_factory.AddDefault ();

		Glade.XML gXML = new Glade.XML (null, "gbrainy.glade", "gbrainy", null);
		gXML.Autoconnect (this);

		toolbar.IconSize = Gtk.IconSize.Dnd;
		toolbar.ShowArrow = false;
	
		Tooltips tooltips = new Tooltips ();
		all_tbbutton = new ToolButton ("allgames");
		all_tbbutton.SetTooltip (tooltips, Catalog.GetString ("Play all the games"), null);
		all_tbbutton.Label = Catalog.GetString ("All");
		all_tbbutton.Clicked += OnAllGames;
		toolbar.Insert (all_tbbutton, -1);

		logic_tbbutton = new ToolButton ("logic-games");
		logic_tbbutton.SetTooltip (tooltips, Catalog.GetString ("Play games that challenge your reasoning and thinking"), null);
		logic_tbbutton.Label = Catalog.GetString ("Logic");
		logic_tbbutton.Clicked += OnLogicOnly;
		toolbar.Insert (logic_tbbutton, -1);

		calculation_tbbutton = new ToolButton ("math-games");
		calculation_tbbutton.Label = Catalog.GetString ("Calculation");
		calculation_tbbutton.SetTooltip (tooltips, Catalog.GetString ("Play games that challenge your mental calculation skills"), null);
		calculation_tbbutton.Clicked += OnMathOnly;
		toolbar.Insert (calculation_tbbutton, -1);

		memory_tbbutton = new ToolButton ("memory-games");
		memory_tbbutton.Label = Catalog.GetString ("Memory");
		memory_tbbutton.SetTooltip (tooltips, Catalog.GetString ("Play games that challenge your short term memory"), null);
		memory_tbbutton.Clicked += OnMemoryOnly;
		toolbar.Insert (memory_tbbutton, -1);

		verbal_tbbutton = new ToolButton ("verbal-games");
		verbal_tbbutton.Label = Catalog.GetString ("Verbal");
		verbal_tbbutton.SetTooltip (tooltips, Catalog.GetString ("Play games that challenge your verbal aptitude"), null);
		verbal_tbbutton.Clicked += OnVerbalOnly;
		toolbar.Insert (verbal_tbbutton, -1);

		pause_tbbutton = new ToolButton ("pause");
		pause_tbbutton.Label = Catalog.GetString ("Pause");
		pause_tbbutton.SetTooltip (tooltips, Catalog.GetString ("Pause or resume the game"), null);
		pause_tbbutton.Clicked += OnPauseGame;
		toolbar.Insert (pause_tbbutton, -1);

		finish_tbbutton = new ToolButton ("endgame");
		finish_tbbutton.SetTooltip (tooltips, Catalog.GetString ("End the game and show score"), null);
		finish_tbbutton.Label = Catalog.GetString ("Finish");
		finish_tbbutton.Clicked += OnEndGame;
		toolbar.Insert (finish_tbbutton, -1);

		session = new GameSession (this);

		if (history == null)
			history = new PlayerHistory ();

		if (preferences == null)
			preferences = new Preferences ();

		session.GameManager.Difficulty = (Game.Difficulty) preferences.GetIntValue (Preferences.DifficultyKey);
		drawing_area = new GameDrawingArea (this);
		GameSensitiveUI ();

		// For low resolutions, hide the toolbar and made the drawing area smaller
		if (drawing_area.Screen.Width> 0 && drawing_area.Screen.Height > 0) {
			if (drawing_area.Screen.Height < 700) {
				drawing_vbox.HeightRequest = 300;
				low_res = true;
			}
		}

		question_label = new SimpleLabel ();
		question_label.HeightMargin = 2;
		question_vbox.Add (question_label);

		solution_label = new SimpleLabel ();
		solution_label.HeightMargin = 2;
		solution_vbox.Add (solution_label);

	#if MONO_ADDINS
		Gtk.MenuItem item = new Gtk.MenuItem (Catalog.GetString ("Extensions"));
		settings_menu.Append (item);
		item.Activated += delegate (object sender, EventArgs ar) { Mono.Addins.Gui.AddinManagerWindow.Run (app_window);};
	#endif

		item = new Gtk.MenuItem (Catalog.GetString ("How to Extend gbrainy's Functionality"));
		help_menu.Prepend (item);
		item.Activated += delegate (object sender, EventArgs ar) { Process.Start ("http://live.gnome.org/gbrainy/Extending");};

		drawing_vbox.Add (drawing_area);
		app_window.IconName = "gbrainy";
		app_window.ShowAll ();

		if (preferences.GetBoolValue (Preferences.Toolbar) == false || low_res == true)
			toolbar_menuitem.Active = false;

		ActiveInputControls (false);
	}

	/* Taken from locale.h  */
	[StructLayout (LayoutKind.Sequential)]
	public struct lconv
	{
		public string decimal_point;
		public string thousands_sep;		
		public string grouping;
		public string int_curr_symbol;
		public string currency_symbol;
		public string mon_decimal_point;
		public string mon_thousands_sep;
		public string mon_grouping;
		public string positive_sign;
		public string negative_sign;
		char int_frac_digits;
		char frac_digits;
		char p_cs_precedes;
		char p_sep_by_space;
		char n_cs_precedes;
		char n_sep_by_space;
		char p_sign_posn;
		char n_sign_posn;
		char int_p_cs_precedes;
		char int_p_sep_by_space;
		char int_n_cs_precedes;
		char int_n_sep_by_space;
		char int_p_sign_posn;
		char int_n_sign_posn;
	}

	[DllImport("libc")]
	static extern IntPtr localeconv ();

	// Mono supports less locales that Unix systems
	// To overcome this limitation we setup the right locale parameters
	// when the Mono locale is InvariantCulture, that is, when the user's locale
	// has not been identified and the default Mono locale is used
	//
	// See: https://bugzilla.novell.com/show_bug.cgi?id=420468
	// 
	static void FixLocaleInfo ()
	{
		IntPtr st = IntPtr.Zero;
		lconv lv;
		int platform = (int) Environment.OSVersion.Platform;
		
		if (platform != 4 && platform != 128) // Only in Unix based systems
			return;

		if (CultureInfo.CurrentCulture != CultureInfo.InvariantCulture) // Culture well supported
			return;

		try {
			st = localeconv ();
			if (st == IntPtr.Zero) return;

			lv = (lconv) Marshal.PtrToStructure (st, typeof (lconv));
			CultureInfo culture =  (CultureInfo) CultureInfo.CurrentCulture.Clone ();
			culture.NumberFormat.NumberDecimalSeparator = lv.decimal_point;
			Thread.CurrentThread.CurrentCulture = culture;
		}
		catch (Exception) {}
	}

	public void UpdateStatusBar ()
	{
		statusbar.Push (0, session.StatusText);
	}

	public void ActiveInputControls (bool active)
	{
		bool answer, entry, next, tip;

		answer = entry = next = tip = active;

		if (active == true && session.CurrentGame != null && session.CurrentGame.ButtonsActive == true && String.IsNullOrEmpty (session.CurrentGame.Tip ) == false)
			tip = true;
		else
			tip = false;
	
		switch (session.Status) {
		case GameSession.SessionStatus.NotPlaying:
			answer = false;
			entry =  false;
			next = false;
			tip = false;
			break;
		case GameSession.SessionStatus.Playing:
			break;
		case GameSession.SessionStatus.Answered:
			answer = false;
			entry =  false;
			tip = false;
			break;
		}

		answer_button.Sensitive = answer;
		answer_entry.Sensitive = entry;
		next_button.Sensitive = next;
		tip_button.Sensitive = tip;

		if (entry == true)
			answer_entry.GrabFocus ();
	}

	public void UpdateQuestion (string question)
	{
		question_label.Text = question;
	}

	private void UpdateSolution (string solution)
	{		
		solution_label.Text = solution;
	}

	public void QueueDraw ()
	{
		drawing_area.QueueDraw ();
	}

	public void SetMargin (int margin)
	{
		question_label.WidthMargin = margin;
		solution_label.WidthMargin = margin;
	}

	void GameSensitiveUI () 
	{
		//Toolbar buttons and menu items that are sensitive when the user is playing
		bool playing;

		if (session.Status != GameSession.SessionStatus.NotPlaying || drawing_area.Mode == GameDrawingArea.Modes.CountDown)
			playing = true;
		else
			playing = false;
	
		finish_tbbutton.Sensitive = pause_tbbutton.Sensitive = playing;
		all_tbbutton.Sensitive = calculation_tbbutton.Sensitive = memory_tbbutton.Sensitive = logic_tbbutton.Sensitive = !playing;
		pause_menuitem.Sensitive = finish_menuitem.Sensitive = playing;
		newgame_menuitem.Sensitive = !playing;
	}

	void OnNextGameAfterCountDown (object source, EventArgs e)
	{
		drawing_area.Mode = GameDrawingArea.Modes.Puzzle;
		ActiveInputControls (session.CurrentGame.ButtonsActive);
		next_button.Sensitive = true;
		drawing_area.puzzle = session.CurrentGame;		
		UpdateQuestion (session.CurrentGame.Question);
		answer_entry.Text = string.Empty;
		UpdateStatusBar ();
		session.CurrentGame.DrawAnswer = false;
		drawing_area.QueueDraw ();

		if (session.CurrentGame as Memory != null)
			(session.CurrentGame as Memory).StartTimer ();
	}	

	private void GetNextGame ()
	{
		UpdateSolution (String.Empty);
		UpdateQuestion (String.Empty);
		session.NextGame ();
		
		if (preferences.GetBoolValue (Preferences.MemQuestionWarnKey) && session.Type != GameSession.Types.MemoryTrainers && ((session.CurrentGame as Memory)  != null)) {
			drawing_area.OnDrawCountDown (OnNextGameAfterCountDown);
		}
		else
			OnNextGameAfterCountDown (this, EventArgs.Empty);
	}
	
	void OnMenuAbout (object sender, EventArgs args)
	{
		AboutDialog about = new AboutDialog ();
		about.Run ();
	}	

	void OnAnswerButtonClicked (object sender, EventArgs args)
	{
		string answer;

		if (session.CurrentGame == null)
			return;
	
		if (answer_button.Sensitive == true && session.CurrentGame.CheckAnswer (answer_entry.Text) == true) {
			session.GamesWon++;
			session.CurrentGame.Won = true;
			answer = "<span color='#00A000'>" + Catalog.GetString ("Congratulations.") + "</span>";
		} else
			answer = Catalog.GetString ("Incorrect answer.");

		session.ScoreGame ();
		session.EnableTimer = false;
		answer_entry.Text = String.Empty;
		UpdateStatusBar ();
		UpdateSolution (answer + " " + session.CurrentGame.Answer);

		session.CurrentGame.DrawAnswer = true;
		session.Status = GameSession.SessionStatus.Answered;
		ActiveInputControls (true);
		next_button.GrabFocus ();
		drawing_area.QueueDraw ();
	}		

	void OnQuit (object sender, EventArgs args)
	{
		Quit ();	
	}	

	void OnDeleteWindow (object sender, DeleteEventArgs args)
	{
		Quit ();	
	}	

	void OnNextButtonClicked (object sender, EventArgs args)
	{
		if (answer_entry.Text.Length > 0) {
			OnAnswerButtonClicked (sender, args);
			return;
		}

		session.ScoreGame ();
		GetNextGame ();
		session.EnableTimer = true;
	}

	void OnTip (object sender, EventArgs args)
	{
		if (session.CurrentGame == null)
			return;

		UpdateSolution (session.CurrentGame.TipString);
	}

	void OnNewGame ()
	{
		session.NewSession ();
		GetNextGame ();
		GameSensitiveUI ();
		UpdateSolution (Catalog.GetString ("Once you have an answer type it in the \"Answer:\" entry box and press the \"OK\" button."));
		UpdateStatusBar ();
	}

	void OnMathOnly (object sender, EventArgs args)
	{
		session.Type = GameSession.Types.CalculationTrainers;
		OnNewGame ();
	}


	void OnVerbalOnly (object sender, EventArgs args)
	{
		session.Type = GameSession.Types.VerbalAnalogies;
		OnNewGame ();
	}

	void OnMemoryOnlyAfterCountDown (object source, EventArgs e)
	{
		OnNewGame ();
	}

	void OnMemoryOnly (object sender, EventArgs args)
	{
		session.Type = GameSession.Types.MemoryTrainers;
		UpdateSolution (String.Empty);
 		UpdateQuestion (String.Empty);

		if (preferences.GetBoolValue (Preferences.MemQuestionWarnKey)) {
			drawing_area.OnDrawCountDown (OnMemoryOnlyAfterCountDown);
			GameSensitiveUI ();
			next_button.Sensitive = true;
		}
		else
			OnNewGame ();
	}

	void OnPreferences (object sender, EventArgs args)
	{
		PreferencesDialog dialog;

		dialog = new PreferencesDialog ();
		if (dialog.Run () == ResponseType.Ok) {
			session.GameManager.Difficulty = (Game.Difficulty) preferences.GetIntValue (Preferences.DifficultyKey);
		}
		dialog.Dialog.Destroy ();
	}

	void OnCustomGame (object sender, EventArgs args)
	{
		ResponseType rslt;
		CustomGameDialog dialog;

		dialog = new CustomGameDialog (session.GameManager);		
		rslt = dialog.Run ();
		dialog.Dialog.Destroy ();

		if (rslt == ResponseType.Ok && dialog.NumOfGames > 0) {
			session.Type = GameSession.Types.Custom;
			OnNewGame ();
		}
	}

	void OnLogicOnly (object sender, EventArgs args)
	{
		session.Type = GameSession.Types.LogicPuzzles;
		OnNewGame ();
	}

	void OnAllGames (object sender, EventArgs args)
	{
		session.Type = GameSession.Types.AllGames;
		OnNewGame ();		
	}

	void OnTrainersOnly (object sender, EventArgs args)
	{
		session.Type = GameSession.Types.TrainersOnly;
		OnNewGame ();		
	}

	void OnAnswerActivate (object sender, EventArgs args)
	{
		if (answer_entry.Text.Length > 0) {
			OnAnswerButtonClicked (sender, args);
			return;
		}
	}

	void OnEndGame (object sender, EventArgs args)
	{
		drawing_area.Mode = GameDrawingArea.Modes.Scores;
		drawing_area.GameSession = session.Copy ();
	
		history.SaveGameSession (session);
		session.EndSession ();
		drawing_area.EndDrawCountDown ();
		drawing_area.puzzle = null;

		UpdateSolution (String.Empty);
		UpdateQuestion (String.Empty);
		UpdateStatusBar ();
		GameSensitiveUI ();
		drawing_area.QueueDraw ();
		ActiveInputControls (false);
		SetPauseResumeButton (true);
	}

	void SetPauseResumeButton (bool pause)
	{
		if (pause) {
			pause_tbbutton.StockId = "pause";
			pause_tbbutton.Label = Catalog.GetString ("Pause");
 			session.Resume ();
			ActiveInputControls (true);
		} else {
			pause_tbbutton.StockId = "resume";
			pause_tbbutton.Label = Catalog.GetString ("Resume");
			session.Pause ();
			ActiveInputControls (false);
		}
		UpdateStatusBar ();
	}

	void OnPauseGame (object sender, EventArgs args)
	{
		SetPauseResumeButton (session.Paused);
	}

	private void OnToolbarActivate (object sender, System.EventArgs args)
	{
		int width, height;
		Requisition requisition;

		requisition =  toolbar.SizeRequest ();
		app_window.GetSize (out width, out height);
		toolbar.Visible = !toolbar.Visible;
		preferences.SetBoolValue (Preferences.Toolbar, toolbar.Visible);
		preferences.Save ();
		app_window.Resize (width, height - requisition.Height);
	}

	void OnHistory (object sender, EventArgs args)
	{
		PlayerHistoryDialog dialog;

		dialog = new PlayerHistoryDialog ();
		dialog.Run ();
		dialog.Dialog.Destroy ();	
	}	

	private void AddIcon (IconFactory stock, string stockid, string resource)
	{
		Gtk.IconSet iconset = stock.Lookup (stockid);
		
		if (iconset != null)
			return;

		iconset = new Gtk.IconSet ();
		Gdk.Pixbuf img = Gdk.Pixbuf.LoadFromResource (resource);
		IconSource source = new IconSource ();
		source.Pixbuf = img;
		iconset.AddSource (source);
		stock.Add (stockid, iconset);		
	}

	void OnFullscreen (object sender, EventArgs args)
	{
		if (full_screen == false) {
			drawing_area.Margins = true;
			app_window.Fullscreen ();
		}
		else {
			drawing_area.Margins = false;
			app_window.Unfullscreen ();
		}

		full_screen = !full_screen;
	}

	[DllImport ("libc")] // Linux
	private static extern int prctl (int option, byte [] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);

	[DllImport ("libc")] // BSD
	private static extern void setproctitle (byte [] fmt, byte [] str_arg);
 
	public static void SetProcessName (string name)
	{
		int platform = (int) Environment.OSVersion.Platform;		
		if (platform != 4 && platform != 128)
			return;

		try {
			if (prctl (15 /* PR_SET_NAME */, Encoding.ASCII.GetBytes (name + "\0"),
				IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 0) {
				throw new ApplicationException ("Error setting process name: " + 
					Mono.Unix.Native.Stdlib.GetLastError ());
			}
		} catch (EntryPointNotFoundException) {
			setproctitle (Encoding.ASCII.GetBytes ("%s\0"), 
				Encoding.ASCII.GetBytes (name + "\0"));
		}
	}
	
	public static void Main (string [] args) 
	{
		try {
			SetProcessName ("gbrainy");
		} catch {}

		gbrainy gui = new gbrainy (args);
		gui.Run ();	
	}
}
