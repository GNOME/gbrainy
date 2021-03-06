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

using System;
using System.Web.UI.WebControls;

namespace gbrainy.Clients.WebForms
{
	public partial class Default : System.Web.UI.Page
	{
		WebSession web_session;
		const string CookieName = "Language";
		const string DefaultLocale = "en_US";

		void Page_Load (object o, EventArgs e)
        	{
			Logger.Debug ("Default.Page_Load. IsPostBack {0}", IsPostBack);

			if (IsPostBack == true)
				return;
			
			web_session = Global.Sessions [Session.SessionID];

			for (int i = 0; i <LanguageSupport.Languages.Length; i++)
			{
				languages_drop.Items.Add (new ListItem (LanguageSupport.Languages[i].Name,
					LanguageSupport.Languages[i].LangCode));
			}

			if (Request.Cookies [CookieName] != null)
    				languages_drop.SelectedValue = Request.Cookies[CookieName].Value;
			else // Default language value
				languages_drop.SelectedValue = DefaultLocale;

			Global.Sessions [Session.SessionID].LanguageCode = languages_drop.SelectedValue;
	        }
		
		protected void OnStartGame (Object sender, EventArgs e)
		{
			web_session = Global.Sessions [Session.SessionID];
			web_session.GameState = null;

			// Collect language cookie
			Global.Sessions [Session.SessionID].LanguageCode = languages_drop.SelectedValue;
			
			Response.Cookies[CookieName].Value = languages_drop.SelectedValue;
			Response.Cookies[CookieName].Expires = DateTime.Now.AddYears (1);

			Logger.Debug ("Default.OnStartGame. Start game button click");
			Response.Redirect ("Game.aspx");
		}
	}
}

