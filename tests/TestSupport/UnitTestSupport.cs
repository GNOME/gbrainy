/*
 * Copyright (C) 2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using NUnit.Framework;
using System.IO;

using gbrainy.Core.Main;
using gbrainy.Core.Services;
using gbrainy.Core.Libraries;

namespace gbrainy.Test
{
	public class UnitTestSupport
	{
		public void RegisterDefaultServices ()
		{
			new DefaultServices ().RegisterServices ();

			string mono_path = Environment.GetEnvironmentVariable ("MONO_PATH");
            Console.WriteLine("**** RegisterDefaultServices. Dir 1" + mono_path);

			if (String.IsNullOrEmpty (mono_path))
				mono_path = ".";

			// Configuration
            Console.WriteLine("**** RegisterDefaultServices. Dir 2" + mono_path);
            mono_path = Path.GetFullPath(mono_path);

            Console.WriteLine("**** RegisterDefaultServices. Dir 3" + mono_path);
			ServiceLocator.Instance.GetService <IConfiguration> ().Set (ConfigurationKeys.AssembliesDir, mono_path);
		}
	}
}
