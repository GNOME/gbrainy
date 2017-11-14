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
using System.Xml.Serialization;

using gbrainy.Core.Services;

namespace gbrainy.Core.Main
{
	[Serializable]
	// Old class name, to keep compatibility when serializing with previous PlayerHistory files
	[XmlType("GameHistory")]
	public class GameSessionHistory
	{
		[XmlElementAttribute ("games_played")]
		public int GamesPlayed { get; set; }	

		[XmlElementAttribute ("games_won")]
		public int GamesWon { get; set; }

		[XmlElementAttribute ("total_score")]
		public int TotalScore { get; set; }

		[XmlElementAttribute ("math_score")]
		public int MathScore { get; set; }

		[XmlElementAttribute ("logic_score")]
		public int LogicScore { get; set; }

		[XmlElementAttribute ("memory_score")]
		public int MemoryScore { get; set; }

		[XmlElementAttribute ("verbal_score")]
		public int VerbalScore { get; set; }

		public virtual void Clear ()
		{	
			GamesPlayed = GamesWon = TotalScore = MathScore = LogicScore = MemoryScore = VerbalScore = 0;
		}

		// Deep copy
		public GameSessionHistory Copy ()
		{
			GameSessionHistory history = new GameSessionHistory ();

			history.GamesPlayed = GamesPlayed;
			history.GamesWon = GamesWon;
			history.TotalScore = TotalScore;
			history.MathScore = MathScore;
			history.LogicScore = LogicScore;
			history.MemoryScore = MemoryScore;
			history.VerbalScore = VerbalScore;
			return history;
		}

		public string GetResult (ITranslations translations)
		{
			string s;

			if (GamesPlayed >= 10) {
				int percentage_won = (int) (100 * GamesWon / GamesPlayed);
				if (percentage_won >= 90)
					s = translations.GetString ("Outstanding results");
				else if (percentage_won >= 70)
					s = translations.GetString ("Excellent results");
				else if (percentage_won >= 50)
					s = translations.GetString ("Good results");
				else if (percentage_won >= 30)
					s = translations.GetString ("Poor results");
				else s = translations.GetString ("Disappointing results");
			} else
				s = string.Empty;

			return s;
		}
	}
}

