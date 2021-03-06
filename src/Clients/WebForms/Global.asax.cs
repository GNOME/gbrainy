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
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

using gbrainy.Core.Services;
using gbrainy.Core.Main;

namespace gbrainy.Clients.WebForms
{
	public class Global : System.Web.HttpApplication
	{
		// Application counters
		static public int TotalSessions { get; set; }
		static public int TotalEndedSessions { get; set; }
		static public int TotalGamesSessions { get; set; }
		static public int TotalGames { get; set; }
		static public int TotalTimeSeconds { get; set; }
		static public DateTime Started { get; set; }
		
		
		static public Dictionary <string, WebSession> Sessions = new Dictionary <string, WebSession> ();

		protected virtual void Application_Start (Object sender, EventArgs e)
		{
			 Started = DateTime.Now;
			
			// Init log system
			if (String.Compare (Environment.GetEnvironmentVariable ("GBRAINY_DEBUG"), "false", false) != 0)
			{
				Logger.LogLevel = Level.DEBUG;
				Logger.LogDevice = new FileLogger ();
			} else
			{
				Logger.LogLevel = Level.INFO;
				Logger.LogDevice = new ConsoleLogger ();
			}
			
			// Register services
			DefaultServices services = new DefaultServices ();
			services.RemoveService <ITranslations> ();
			services.RegisterServices ();
			
			ServiceLocator.Instance.RegisterService <ITranslations> (new TranslationsWeb ());
			ThemeManager.ConfigPath = Defines.THEMES_DIR;
			
			// Configuration
			ServiceLocator.Instance.GetService <IConfiguration> ().Set (ConfigurationKeys.GamesGraphics, "images/");	
			ServiceLocator.Instance.GetService <IConfiguration> ().Set (ConfigurationKeys.AssembliesDir, "bin/");

			Logger.Info ("Global.Application_Start.gbrainy web starting");
		}

		protected virtual void Session_Start (Object sender, EventArgs e)
		{
			Logger.Debug ("Global.Session_Start. Session {0}", Session.SessionID);

			WebSession details = new WebSession (HttpContext.Current.Session);
			
			lock (Sessions)
			{
				if (Sessions.ContainsKey (Session.SessionID))
				{
					Sessions [Session.SessionID] = details;
				}
				else
				{
					TotalSessions++;
					Sessions.Add (Session.SessionID, details);
				}
			}
		}

		protected virtual void Application_BeginRequest (Object sender, EventArgs e)
		{
		}

		protected virtual void Application_EndRequest (Object sender, EventArgs e)
		{
		}

		protected virtual void Application_AuthenticateRequest (Object sender, EventArgs e)
		{
		}

		protected virtual void Application_Error (Object sender, EventArgs e)
		{
		}

		protected virtual void Session_End (Object sender, EventArgs e)
		{
			const string CachePrefix = "@@@InProc@";
			string sessionid;

			// Needed due to a bug in Mono < 2.7.
			// See: http://bugzilla.novell.com/show_bug.cgi?id=629990
			if (Session.SessionID.StartsWith (CachePrefix, StringComparison.OrdinalIgnoreCase))
				sessionid = Session.SessionID.Substring (CachePrefix.Length);
			else
				sessionid = Session.SessionID;

			Logger.Debug ("Global.Session_End. Session {0}", sessionid);
			
			lock (Sessions)
			{
				if (Sessions.ContainsKey (sessionid)) {
					Sessions.Remove (sessionid);
	
					try
					{
						File.Delete (GameImage.GetImageFileName (Session.SessionID));
					}
					catch (Exception ex)
					{
						Logger.Error ("Global.Session_End. Could not delete {0}, exception: {1}",
								GameImage.GetImageFileName (Session.SessionID), ex);
					}
				}
				else
					Logger.Error ("Global.Session_End. Could not find session " + Session.SessionID);
			}
		}

		protected virtual void Application_End (Object sender, EventArgs e)
		{
			Logger.Debug ("Global.Application_End.");
		}
	}
}
