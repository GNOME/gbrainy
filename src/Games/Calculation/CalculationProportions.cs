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
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using System;
using Cairo;
using Mono.Unix;

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;

namespace gbrainy.Games.Calculation
{
	public class CalculationProportions : Game
	{
		const int options_cnt = 4;
		const int correct_pos = 0;
		double []options;
		ArrayListIndicesRandom random_indices;
		double num, den, percentage, correct;

		public override string Name {
			get {return Catalog.GetString ("Proportions");}
		}

		public override Types Type {
			get { return Game.Types.MathTrainer;}
		}

		public override string Question {
			get {
				return String.Format (
					Catalog.GetString ("What is the {0}% of {1}/{2}? Answer {3}, {4}, {5} or {6}."), 
					percentage, num, den, GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2), GetPossibleAnswer (3));}
		}

		public override void Initialize ()
		{
			int options_next, random_max, which = 0;
		
			switch (CurrentDifficulty) {
			case Difficulty.Easy:
				random_max = 30;
				break;
			default:
			case Difficulty.Medium:
				random_max = 50;
				break;
			case Difficulty.Master:
				random_max = 80;
				break;
			}

			do {
				// Fraction
				num = 10 + random.Next (random_max);
				den = 2 + random.Next (random_max / 5);
				percentage = 10 + random.Next (random_max) ;
				correct = percentage / 100d * num / den;
			} while (correct < 1);

			options = new double [options_cnt];

			options_next = 0;
			options [options_next++] = correct;
			options [options_next++] = percentage / 70d * num / den;
			options [options_next++] = percentage / 120d * num / den;
			options [options_next++] = percentage / 150d * num / den;;

			random_indices = new ArrayListIndicesRandom (options_cnt);
			random_indices.Initialize ();
		
			for (int i = 0; i < options_cnt; i++)
			{
				if (random_indices [i] == correct_pos) {
					which = i;
					break;
				}
			}

			right_answer += GetPossibleAnswer (which);
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{	
			double x = DrawAreaX + 0.25, y = DrawAreaY + 0.16;
			int indx;

			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();

			for (int i = 0; i < options_cnt; i++)
			{
				gr.MoveTo (x, y);
				indx = random_indices[i];
				gr.ShowPangoText (String.Format ("{0}) {1:##0.##}", GetPossibleAnswer (i) , options [indx]));

				y = y + 0.15;
			}
		}
	}
}
