/*
 * Copyright (C) 2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

namespace gbrainy.Core.Main.Xml
{
	public class TextDrawingObject : DrawingObject
	{
		public enum Sizes
		{
			Small,
			Medium,
			Large,
			XLarge,
			XXLarge,
		}

		public string Text { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
		public bool Centered { get; set; }
		public Sizes Size { get; set; }

		public TextDrawingObject ()
		{
			Size = Sizes.Medium;
		}
	}
}
