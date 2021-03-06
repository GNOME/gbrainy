/*
 * Copyright (C) 2010-2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Text.RegularExpressions;

using gbrainy.Core.Services;

namespace gbrainy.Core.Main.Xml
{
	public class GameXml : Game
	{
		// Every GameXml instance is capable of locating any XML defined game
		// This struct translates from a Variant that is global to all games
		// to a specific game + variant
		struct DefinitionLocator
		{
			public int Game { get; set; }
			public int Variant { get; set; }

			public DefinitionLocator (int game, int variant) : this ()
			{
				Game = game;
				Variant = variant;
			}
		};

		// Shared with all instances
		static List <GameXmlDefinition> games;
		static List <DefinitionLocator> locators;
		static string option_answers = "[option_answers]";

		DefinitionLocator current;
		GameXmlDefinition game;
		string question, answer, rationale, answer_show;
		ICSharpCompiler compiler;
		GameXmlDrawing xml_drawing;

		public GameXml ()
		{
			xml_drawing = new GameXmlDrawing (this);
		}

		static public List <GameXmlDefinition> Definitions {
			set {
				games = value;
				locators = new List <DefinitionLocator> ();
			}
		}

		public override GameTypes Type {
			get { return game.Type; }
		}

		public override string Name {
			get { return Translations.GetString (game.Name); }
		}

		public override string Question {
			get { return question; }
		}

		public override string Rationale {
			get { return rationale; }
		}

		public override string Tip {
			get {
				if (game.Variants.Count > 0 && game.Variants[current.Variant].Tip != null)
					return Translations.GetString (game.Variants[current.Variant].Tip);
				else
					if (String.IsNullOrEmpty (game.Tip) == false)
						return Translations.GetString (game.Tip);
					else
						return null;
			}
		}

		void SetAnswerCorrectShow ()
		{
			if (String.IsNullOrEmpty (answer_show))
				return;

			Answer.CorrectShow = answer_show;
		}

		void SetCheckExpression ()
		{
			string expression;

			if (game.Variants.Count > 0 && String.IsNullOrEmpty (game.Variants[current.Variant].AnswerCheckExpression) == false)
				expression = game.Variants[current.Variant].AnswerCheckExpression;
			else
				expression = game.AnswerCheckExpression;

			if (String.IsNullOrEmpty (expression))
				return;

			Answer.CheckExpression = expression;
		}

		void SetCheckAttributes ()
		{
			GameAnswerCheckAttributes attrib;
			if (game.Variants.Count > 0 && game.Variants[current.Variant].CheckAttributes != GameAnswerCheckAttributes.None)
				attrib = game.Variants[current.Variant].CheckAttributes;
			else
				attrib = game.CheckAttributes;

			if (attrib == GameAnswerCheckAttributes.None)
				return;

			Answer.CheckAttributes |= attrib;
		}

		protected override void Initialize ()
		{
			string variables;
			bool variants;
			LocalizableString localizable_question, localizable_rationale;

			xml_drawing.CreateDrawingObjects (game.DrawingObjects); // Draw objects shared by all variants

			if (game.Variants.Count > 0)
				xml_drawing.CreateDrawingObjects (game.Variants[current.Variant].DrawingObjects); // Draw variant specific objects

			compiler = ServiceLocator.Instance.GetService <ICSharpCompiler> ();

			variants = game.Variants.Count > 0;
			SetCheckAttributes ();

			if (variants && game.Variants[current.Variant].Variables != null)
				variables = game.Variants[current.Variant].Variables;
			else
				variables = game.Variables;

			if (variants && game.Variants[current.Variant].Question != null)
				localizable_question = game.Variants[current.Variant].Question;
			else
				localizable_question = game.Question;

			if (variants && game.Variants[current.Variant].Rationale != null)
				localizable_rationale = game.Variants[current.Variant].Rationale;
			else
				localizable_rationale = game.Rationale;

			if (String.IsNullOrEmpty (variables) == false)
			{
				compiler.EvaluateCode (variables);

				try {
					if (String.IsNullOrEmpty (localizable_question.Value) == false)
						localizable_question.ValueComputed = Int32.Parse (ReplaceVariables (localizable_question.Value));

					if (localizable_rationale != null && String.IsNullOrEmpty (localizable_rationale.Value) == false)
						localizable_rationale.ValueComputed = Int32.Parse (ReplaceVariables (localizable_rationale.Value));
				}
				catch (Exception e)
				{
					Console.WriteLine ("GameXml.Initialize. Error {0}", e);
				}
			}

			if (variants && game.Variants[current.Variant].Question != null)
				question = CatalogGetString (game.Variants[current.Variant].Question);
			else
				question = CatalogGetString (game.Question);

			if (variants && game.Variants[current.Variant].AnswerText != null)
				answer = CatalogGetString (game.Variants[current.Variant].AnswerText);
			else
				answer = CatalogGetString (game.AnswerText);

			if (variants && game.Variants[current.Variant].Rationale != null)
				rationale = CatalogGetString (game.Variants[current.Variant].Rationale);
			else
				rationale = CatalogGetString (game.Rationale);

			if (variants && game.Variants[current.Variant].AnswerShow != null)
				answer_show = CatalogGetString (game.Variants[current.Variant].AnswerShow);
			else
				answer_show = CatalogGetString (game.AnswerShow);

			if (String.IsNullOrEmpty (variables) == false)
			{
				question = ReplaceVariables (question);
				rationale = ReplaceVariables (rationale);
				answer = ReplaceVariables (answer);
				answer_show = ReplaceVariables (answer_show);
			}

			int option_answer = xml_drawing.GetOptionCorrectAnswerIndex ();

			if (option_answer != -1)
			{
				Answer.SetMultiOptionAnswer (option_answer, answer);

				// Translators {0}: list of options (A, B, C)
				string answers = String.Format (Translations.GetString ("Answer {0}."),
					Answer.GetMultiOptionsPossibleAnswers (xml_drawing.Options.Count));
				question = question.Replace (option_answers, answers);
			}
			else
			{
				Answer.Correct = answer;
			}

			SetCheckExpression ();
			SetAnswerCorrectShow ();
		}

		public override int Variants {
			get {
				if (locators.Count == 0)
					BuildLocationList ();

				return locators.Count;
			}
		}

		public override int Variant {
			set {
				base.Variant = value;

				DefinitionLocator locator;

				locator = locators [Variant];
				current.Game = locator.Game;
				current.Variant = locator.Variant;
				game = games [locator.Game];
				SetCheckAttributes ();
				SetCheckExpression ();
				SetAnswerCorrectShow ();
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);

			xml_drawing.DrawObjects (gr, game.DrawingObjects, null); // Draw objects shared by all variants

			if (game.Variants.Count > 0)
				xml_drawing.DrawObjects (gr, game.Variants[current.Variant].DrawingObjects, null); // Draw variant specific objects
		}

		static void BuildLocationList ()
		{
			locators.Clear ();

			for (int game = 0; game < games.Count; game++)
			{
				locators.Add (new DefinitionLocator (game, 0));
				for (int variant = 1; variant < games[game].Variants.Count; variant++)
					locators.Add (new DefinitionLocator (game, variant));
			}
		}

		// Protect from calling with null (exception)
		internal string CatalogGetString (string str)
		{
			if (String.IsNullOrEmpty (str))
				return str;

			return Translations.GetString (str);
		}

		// Protect from calling with null + resolve plurals
		internal string CatalogGetString (LocalizableString localizable)
		{
			if (localizable == null)
				return string.Empty;

			if (localizable.IsPlural () == false)
				return CatalogGetString (localizable.String);

			return Translations.GetPluralString (localizable.String, localizable.PluralString, localizable.ValueComputed);
		}

		// Replace compiler service variables
		internal string ReplaceVariables (string str)
		{
			const string exp = "\\[[a-z_]+\\]+";
			string var, var_value, all_vars;
			Regex regex;
			Match match;

			all_vars = compiler.GetAllVariables ();
			if (String.IsNullOrEmpty (str) ||
				String.IsNullOrEmpty (all_vars))
				return str;

			regex = new Regex (exp, RegexOptions.IgnoreCase);
			match = regex.Match (str);

			while (String.IsNullOrEmpty (match.Value) == false)
			{
				var = match.Value.Substring (1, match.Value.Length - 2);
				var_value = compiler.GetVariableValue (var);

				if (String.IsNullOrEmpty (var_value) == false)
					str = str.Replace (match.Value, var_value);

				match = match.NextMatch ();
			}
			return str;
		}
	}
}
