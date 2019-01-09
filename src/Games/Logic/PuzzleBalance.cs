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

using gbrainy.Core.Main;


namespace gbrainy.Games.Logic
{
	public class PuzzleBalance : Game
	{
		private const int elements = 5;
		private int group;
		private int [] balances = new int []
		{
			2,3,2,0,0,	1,3,1,1,1,
			3,3,1,0,0,	2,2,2,1,0,
			3,2,2,0,0,	0,0,0,0,0,

			2,2,3,0,0,	3,2,1,1,0,
			1,2,2,0,0,	3,1,1,0,0,
			3,3,1,0,0,	0,0,0,0,0,

			2,2,0,0,0,	2,1,1,0,0,
			3,2,0,0,0,	1,1,1,2,0,
			2,2,3,0,0,	0,0,0,0,0,
		};

		private const double figure_width = 0.1, figure_height = 0.1, space_width = 0.05, space_height = 0;

		public override string Name {
			get {return Translations.GetString ("Balance");}
		}

		public override string Question {
			get {return Translations.GetString ("Using triangles only, how many triangles are needed in the right part of the last figure to keep it balanced?");}
		}

		public override string Rationale {
			get {
				return Translations.GetString ("Every circle is equivalent to two triangles and every square to three triangles.");
			}
		}

		public override string Tip {
			get { return Translations.GetString ("Every circle is equivalent two triangles.");}
		}

		protected override void Initialize ()
		{
			int ans = 0;
			group = random.Next (3);

			for (int i = 0; i < elements; i++)
				ans += balances [(group * elements * 6) + (4 * elements) + i];

			Answer.Correct = ans.ToString ();
		}

		public void DrawBalance (CairoContextEx gr, double x, double y, int index, bool full)
		{

			double fig_x = x + 0.1, fig_y = y - 0.08;
			int total = (full == true) ? (elements * 2) : elements;

			for (int i = 0; i < total; i++) {
				switch (balances[i + index]) {
				case 1:
					gr.DrawEquilateralTriangle (fig_x, fig_y, 0.05);
					break;
				case 2:
					gr.Arc (fig_x + (0.05 / 2), fig_y + (0.05 / 2) + 0.003, (0.05 / 2), 0, 2 * Math.PI);
					gr.Stroke ();
					break;
				case 3:
					gr.Rectangle (fig_x, fig_y + 0.005, 0.045, 0.045);
					gr.Stroke ();
					break;
				}

				if (i == elements - 1)
					fig_x = x + 0.54;
				else
					fig_x += 0.07;
			}

			gr.DrawImageFromAssembly ("balance.svg", x + 0.02, y - 0.08, 0.9, 0.25);
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = 0.05, y = DrawAreaY + 0.1;

			base.Draw (gr, area_width, area_height, rtl);

			DrawBalance (gr, x, y, group * elements * 6, true);
			y += 0.3;

			DrawBalance (gr, x, y, (group * elements * 6) + 1 * elements * 2, true);
			y += 0.3;

			DrawBalance (gr, x, y, (group * elements * 6) + 2 * elements * 2, false);

			gr.SetPangoFontSize (0.05);
			gr.MoveTo (0.74, 0.71);
			gr.ShowPangoText ("?");
			gr.Stroke ();
		}
	}
}
