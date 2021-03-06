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

using gbrainy.Core.Main;

namespace gbrainy.Games.Logic
{
	public class PuzzleSquares : Game
	{
		private double rows, columns;
		private int type;

		public override string Name {
			get {return Translations.GetString ("Squares");}
		}

		public override string Question {
			get {return Translations.GetString ("How many squares of any size do you count in the figure below?");} 
		}

		public override string Tip {
			get { return Translations.GetString ("A square is a rectangle with sides of equal length. A square can also be built from other squares.");}
		}

		public override string Rationale {
			get {
				switch (type) {
				case 0:
					return Translations.GetString ("There are 16 single squares, 9 squares made by 4 single squares, 4 squares made by 9 single squares and 1 square made by 16 single squares.");
				case 1:
					return Translations.GetString ("There are 9 single squares, 4 squares made by 4 single squares and 1 square made by 9 single squares.");
				default:
					return string.Empty;
				}
			}
		}

		protected override void Initialize ()
		{
			if (CurrentDifficulty==GameDifficulty.Easy)
				type = 0;
			else
				type = random.Next (2);

			rows = 3;
			columns = 3;		

			if (type == 0) {
				rows++;
				columns++;
				Answer.Correct = "30";
			} else {
				Answer.Correct = "14";
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double rect_w = DrawAreaWidth / rows;
			double rect_h = DrawAreaHeight / columns;

			base.Draw (gr, area_width, area_height, rtl);

			for (int column = 0; column < columns; column++) {
				for (int row = 0; row < rows; row++) {
					gr.Rectangle (DrawAreaX + row * rect_w, DrawAreaY + column * rect_h, rect_w, rect_h);
				}
			}

			gr.Stroke ();
		}
	}
}
