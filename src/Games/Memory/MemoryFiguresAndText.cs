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

namespace gbrainy.Games.Memory
{
	public class MemoryFiguresAndText : Core.Main.Memory
	{
		int [] figures;
		int rows, columns;
		const double start_x_ques = 0.25;
		const double start_x_ans = 0.25;
		const double start_y = 0.15;
		const double figure_size = 0.08;
		double rect_w, rect_h;
		int question_pos, figures_active;

		static internal class FigureType
		{
			internal const int Triangle = 0;
			internal const int Rectangle = 1;
			internal const int Pentagon = 2;
			internal const int Circle = 3;
			internal const int Total = Circle + 1;

			static internal string ToString (int type)
			{
				switch (type) {
				case Triangle:
					return Catalog.GetString ("Triangle");
				case Rectangle:
					return Catalog.GetString ("Rectangle");
				case Pentagon:
					return Catalog.GetString ("Pentagon");
				case Circle:
					return Catalog.GetString ("Circle");
				default:
					throw new InvalidOperationException ();
				}
			}
		}

		public override string Name {
			get {return Catalog.GetString ("Memorize figures and text");}
		}

		public override string MemoryQuestion {
			get {return Catalog.GetString ("The list below enumerates the figures shown in the previous image except for one. Which is the missing figure? Possible answers are triangle, rectangle, pentagon and circle." );}
		}

		public override void Initialize ()
		{
			ArrayListIndicesRandom figures_random;
			int pos = 0;

			switch (CurrentDifficulty) {
			case Difficulty.Easy:
				figures_active = 4;
				rows = columns = 2;
				break;
			case Difficulty.Medium:
				figures_active = 6;
				rows = 2;
				columns = 3;
				break;
			case Difficulty.Master:
				figures_active = 9;
				columns = rows = 3;
				break;
			default:
				throw new InvalidOperationException ();
			}

			rect_w = 0.65 / columns;
			rect_h = 0.65 / rows;
			figures = new int [figures_active];
			figures_random = new ArrayListIndicesRandom (FigureType.Total);
			figures_random.Initialize ();

			for (int n = 0; n < figures_active; n++)
			{
				figures[n] = figures_random [pos++];

				if (pos >= figures_random.Count) {
					pos = 0;
					figures_random.Initialize ();
				}
			}

			question_pos = random.Next (figures_active);
			right_answer = FigureType.ToString (figures[question_pos]);
			base.Initialize ();
		}

		public override void DrawPossibleAnswers (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x= DrawAreaX, y = DrawAreaY + 0.1;
			int pos = 0;
			gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, 1);

			if (DrawAnswer ==  true) {
				DrawAllFigures (gr, start_x_ans, start_y, area_width, area_height);
				return;
			}

			gr.SetPangoNormalFontSize ();
			for (int i = 0; i < figures.Length; i++)
			{
				if (i == question_pos)
					continue;

				gr.MoveTo (x, y);
				gr.ShowPangoText (FigureType.ToString (figures[i]));

				if ((pos + 1) % 3 == 0) {
					y += 0.2;
					x = DrawAreaX;
				} else {
					x+= 0.30;
				}
				pos++;
			}
		}

		public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.DrawObjectToMemorize (gr, area_width, area_height, rtl);
			DrawAllFigures (gr, start_x_ques, start_y, area_width, area_height);
		}

		void DrawAllFigures (CairoContextEx gr, double x, double y, int area_width, int area_height)
		{
			int col = 1, fig;
			double org_x = x;

			DrawGrid (gr, x, y, area_width, area_height);
			gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha);
			for (int figure = 0; figure < figures.Length; figure++, col++)
			{
				fig = (int)figures[figure];
				if (fig >= FigureType.Total)
					fig -= FigureType.Total;

				DrawFigure (gr, x, y, fig);

				if (col >= columns) {
					col = 0;
					y += rect_h;
					x = org_x;
				} else {
					x += rect_w;
				}
			}
		}

		void DrawFigure (CairoContextEx gr, double x, double y, int type)
		{
			double space_x, space_y;

			space_x = (rect_w - figure_size) / 2;
			space_y = (rect_h - figure_size) / 2;

			switch (type) {
			case FigureType.Triangle:
				gr.DrawEquilateralTriangle (x + space_x, y + space_y, figure_size);
				break;
			case FigureType.Rectangle:
				gr.Rectangle (x + space_x, y + space_y, figure_size, figure_size);
				gr.Stroke ();
				break;
			case FigureType.Pentagon:
				gr.DrawPentagon (x + space_x, y + space_y, figure_size);
				break;
			case FigureType.Circle:
				gr.Arc (x + space_x + figure_size / 2, y + space_y + figure_size / 2, figure_size / 2, 0, 2 * Math.PI);
				gr.Stroke ();
				break;
			default:
				throw new InvalidOperationException ();
			}
		}

		void DrawGrid (CairoContextEx gr, double x, double y, int area_width, int area_height)
		{
			gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha);
			for (int column = 0; column < columns; column++) {
				for (int row = 0; row < rows; row++) {
					gr.Rectangle (x + column * rect_w, y + row * rect_h, rect_w, rect_h);
				}
			}
			gr.Stroke ();
		}
	}
}
