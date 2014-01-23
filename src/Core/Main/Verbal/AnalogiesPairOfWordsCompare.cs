/*
 * Copyright (C) 2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Collections.Generic;

namespace gbrainy.Core.Main.Verbal
{
	public class AnalogiesPairOfWordsCompare : Analogies
	{
		static protected Dictionary <int, Analogy> analogies;
		static protected ArrayListIndicesRandom analogies_indices;

		string samples, sample;

		public AnalogiesPairOfWordsCompare ()
		{
			if (analogies == null)
				analogies = AnalogiesFactory. Get (Analogy.Type.PairOfWordsCompare);
		}

		public override string Name {
			get { return String.Format (Translations.GetString ("Pair of words compare #{0}"), Variant);}
		}

		public override Dictionary <int, Analogy> List {
			get { return analogies; }
		}

		public override string Question {
			get {
				if (Current == null)
					return string.Empty;

				if (Current.answers == null)
					return Current.question;

				return String.Format (Translations.GetString (
					"Given the relationship between the two words below, which word has the same relationship to '{0}'?"),
					sample);
			}
		}

		public override string AnswerText {
			get {
				string str;
				string[] items;

				items = Answer.Correct.Split (AnalogiesFactory.Separator);

				str = String.Format (Translations.GetPluralString ("The correct answer is {0}.",
					"The possible correct answers are {0}.", items.Length),
					Answer.CorrectShow);

				if (String.IsNullOrEmpty (Rationale))
					return str;

				// Translators: answer + rationale of the answer
				return String.Format (Translations.GetString ("{0} {1}"), str, Rationale);
			}
		}


		protected override void Initialize ()
		{
			Current = GetNext ();

			if (Current == null || Current.answers == null)
				return;

			string [] items;

			items = Current.question.Split (AnalogiesFactory.Separator);

			if (items.Length == 2)
				sample = items [1].Trim ();
			else
				sample = string.Empty;

			samples = items [0].Trim ();
			Answer.Correct = Current.answers [Current.right];
			SetAnswerCorrectShow ();
		}
	
		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double y = DrawAreaY + 0.1;

			base.Draw (gr, area_width, area_height, rtl);

			if (Current == null || Current.answers == null)
				return;

			gr.SetPangoLargeFontSize ();
			gr.DrawTextCentered (0.5, y + 0.25,
				String.Format (Translations.GetString ("Words: {0}"), samples));
		}
	}
}
