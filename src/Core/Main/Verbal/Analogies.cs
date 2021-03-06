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

using gbrainy.Core.Libraries;

namespace gbrainy.Core.Main.Verbal
{
	public abstract class Analogies : Game
	{
		public Analogy Current { get; set; }

		public override string Question {
			get {
				if (Current == null)
					return string.Empty;

				return Current.question;
			}
		}

		public override string Tip {
			get {
				if (Current == null)
					return null;
				else
					return Current.tip;
			}
		}

		public override string Rationale {
			get {
				if (Current == null)
					return string.Empty;

				return Current.rationale;
			}
		}

		protected void SetAnswerCorrectShow ()
		{
			if (Current == null || 
			    Current.MultipleAnswers == false ||
			    String.IsNullOrEmpty (Answer.Correct))
				return;

			string [] items;
			string str = string.Empty;

			items = Answer.Correct.Split (AnalogiesFactory.Separator);

			for (int i = 0 ; i < items.Length; i++)
			{
				str += items [i].Trim ();
				if (i + 1 < items.Length) {
					// Translators: this the separator used when concatenating multiple possible answers for verbal analogies
					// For example: "Possible correct answers are: sleep, rest."
					str += Translations.GetString (", ");
				}
			}
			Answer.CorrectShow = str;
		}

		public override GameTypes Type {
			get { return GameTypes.VerbalAnalogy;}
		}

		public abstract Dictionary <int, Analogy> List {
			get;
		}

		public override int Variants {
			get { return List.Count;}
		}

		public Analogy GetNext ()
		{
			Analogy analogy; // Holds a deep copy
			Analogy analogy_ref; // Holds reference to the object
			ArrayListIndicesRandom indices = null;
			int new_right = 0;
			bool localized = true;

			List.TryGetValue (Variant, out analogy_ref);
			analogy = analogy_ref.Copy ();

			if (analogy.answers != null) { // Randomize answers order
				string [] answers;
			
				indices = new ArrayListIndicesRandom (analogy.answers.Length);
				answers = new string [analogy.answers.Length];

				indices.Initialize ();

				for (int i = 0; i < indices.Count; i++)
				{
					if (GetText.StringExists (analogy.answers [indices[i]]) == false)
						localized = false;

					answers [i] = Translations.GetString (analogy.answers [indices[i]]);
					if (indices[i] == analogy.right)
						new_right = i;
				}
				analogy.right = new_right;
				analogy.answers = answers;
			}

			if ((GetText.StringExists (analogy.question) == false) ||
				(String.IsNullOrEmpty (analogy.tip) == false && GetText.StringExists (analogy.tip) == false) ||
				(String.IsNullOrEmpty (analogy.rationale) == false && GetText.StringExists (analogy.rationale) == false)) 
				localized = false;

			if (localized == true) {
				analogy.question = Translations.GetString (analogy.question);

				if (String.IsNullOrEmpty (analogy.tip) == false)
					analogy.tip = Translations.GetString (analogy.tip);

				if (String.IsNullOrEmpty (analogy.rationale) == false)
					analogy.rationale = Translations.GetString (analogy.rationale);
			} else {

				// Get analogy again
				List.TryGetValue (Variant, out analogy_ref);
				analogy = analogy_ref.Copy ();

				if (analogy.answers != null) { // Randomize answers order
					string [] answers;

					answers = new string [analogy.answers.Length];

					for (int i = 0; i < indices.Count; i++)
						answers [i] = analogy.answers [indices[i]];

					analogy.right = new_right;
					analogy.answers = answers;
				}
			}
			return analogy;
		}
	}
}
