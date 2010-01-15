using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Szotar.WindowsForms {
	// Checks answers, ignoring case, extraneous spaces and punctuation, and parenthesised 
	// clauses (based on configuration).
	class AnswerChecker {
		bool fixSpaces, fixPunct, fixParens, fixCase;

		public AnswerChecker() {
			fixSpaces = GuiConfiguration.PracticeFixSpaces;
			fixPunct = GuiConfiguration.PracticeFixPunctuation;
			fixParens = GuiConfiguration.PracticeFixParentheses;
			fixCase = GuiConfiguration.PracticeFixCase;

			Configuration.Default.SettingChanged += new EventHandler<SettingChangedEventArgs>(OnSettingChanged);
		}

		void OnSettingChanged(object sender, SettingChangedEventArgs e) {
			switch (e.SettingName) {
				case "PracticeFixSpaces":
					fixSpaces = GuiConfiguration.PracticeFixSpaces;
					break;
				case "PracticeFixPunctuation":
					fixPunct = GuiConfiguration.PracticeFixPunctuation;
					break;
				case "PracticeFixParentheses":
					fixParens = GuiConfiguration.PracticeFixParentheses;
					break;
				case "PracticeFixCase":
					fixCase = GuiConfiguration.PracticeFixCase;
					break;
			}
		}

		public bool IsAcceptable(string guess, string answer) {
			guess = Fix(guess);

			if (guess == Fix(answer))
				return true;

			foreach (string s in answer.Split(';'))
				if (guess == Fix(s))
					return true;

			foreach (string s in answer.Split(','))
				if (guess == Fix(s))
					return true;

			return false;
		}

		// TODO: Possible enhancements: regex-based or word-based replacements in given answer, such as:
		//       sg -> something
		//       vkit -> valakit
		string Fix(string s) {
			var sb = new System.Text.StringBuilder();

			bool wasSpace = false;
			int parens = 0;
			foreach (var ch in s.Trim().Normalize()) {
				char c = ch;
				bool add = true;

				if (fixCase)
					c = char.ToLowerInvariant(c);

				if (c == '(' || c == '[')
					parens++;
				else if (c == ')' || c == ']')
					parens--;

				if (parens < 0)
					parens = 0;

				if (parens > 0)
					add = false;
				else if (fixPunct && char.IsPunctuation(c))
					add = false;
				else if (fixSpaces) {
					bool isSpace = char.IsWhiteSpace(c);
					if (wasSpace) {
						if (isSpace)
							add = false;
						else
							sb.Append(' ');
					}

					// This only occurs if c wasn't a punctuation mark -- thus, " ... " will be collapsed into " ".
					wasSpace = isSpace;
				}

				if (add)
					sb.Append(c);
			}

			return sb.ToString();
		}
	}

	class RoundOverview : UserControl {
		TableLayoutPanel table;
		Label roundNo = new Label();
		Label correctLabel = new Label(), incorrectLabel = new Label();
		Label correctCount = new Label(), incorrectCount = new Label();
		Label correctRatio = new Label(), incorrectRatio = new Label();
		Label prompt = new Label();

		public RoundOverview() : this(0, 0, 0) { }

		public void UpdateScore(int round, int count, int correct) {
			roundNo.Text = string.Format("End of round {0}", round);

			int incorrect = count - correct;
			correctLabel.Text = "Correct";
			incorrectLabel.Text = "Incorrect";
			correctCount.Text = correct.ToString();
			incorrectCount.Text = incorrect.ToString();
			correctRatio.Text = RatioToPercentage(correct, count);
			incorrectRatio.Text = RatioToPercentage(incorrect, count);
			prompt.Text = "Press any key to continue";

			Layout();
		}

		public RoundOverview(int round, int count, int correct) {
			// TODO: Make theme-aware.
			correctCount.ForeColor = correctLabel.ForeColor = correctRatio.ForeColor = Color.FromArgb(32, 128, 32);
			incorrectCount.ForeColor = incorrectLabel.ForeColor = incorrectRatio.ForeColor = Color.FromArgb(128, 16, 16);

			Font = new Font(Font.FontFamily, Font.Size * 1.4f);
			FontFamily titleFontFamily = Array.Exists(FontFamily.Families, x => x.Name == "Cambria") 
				? new FontFamily("Cambria")
				: roundNo.Font.FontFamily;

			roundNo.Font = new Font(titleFontFamily, Font.Size * 2.0f, FontStyle.Bold);
			Disposed += delegate {
				Font.Dispose();
				roundNo.Font.Dispose();
			};
			
			prompt.ForeColor = SystemColors.GrayText;

			table = new TableLayoutPanel() { RowCount = 4, ColumnCount = 3 }; 
			
			foreach (var label in new[] { roundNo, correctLabel, incorrectLabel, correctCount, incorrectCount, correctRatio, incorrectRatio, prompt }) {
				label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
				label.TextAlign = ContentAlignment.MiddleLeft;
				label.AutoSize = true;
				table.Controls.Add(label);
			}

			prompt.TextAlign = ContentAlignment.MiddleCenter;

			table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			table.RowStyles.Add(new RowStyle(SizeType.Percent, 0.5f));
			table.RowStyles.Add(new RowStyle(SizeType.Percent, 0.5f));
			table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0f));
			table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

			// Row 0: Header
			table.SetRow(roundNo, 0); 
			table.SetColumn(roundNo, 0);
			table.SetColumnSpan(roundNo, 3);

			// Row 1: Correct guesses
			table.SetRow(correctLabel, 1);
			table.SetRow(correctCount, 1);
			table.SetRow(correctRatio, 1);
			table.SetColumn(correctLabel, 0);
			table.SetColumn(correctCount, 1);
			table.SetColumn(correctRatio, 2);

			// Row 2: Incorrect guesses
			table.SetRow(incorrectLabel, 2);
			table.SetRow(incorrectCount, 2);
			table.SetRow(incorrectRatio, 2);
			table.SetColumn(incorrectLabel, 0);
			table.SetColumn(incorrectCount, 1);
			table.SetColumn(incorrectRatio, 2);

			// Row 3: Prompt
			table.SetRow(prompt, 3);
			table.SetColumn(prompt, 0);
			table.SetColumnSpan(prompt, 3);

			Controls.Add(table);

			Resize += delegate { Layout(); };

			UpdateScore(round, count, correct);
		}

		string RatioToPercentage(int nom, int denom) {
			if (denom == 0)
				return "100%";

			return string.Format("{0:D}%", nom * 100 / denom);
		}

		new void Layout() {
			int width = Math.Max(
				Math.Max(correctLabel.PreferredWidth, incorrectLabel.PreferredWidth)
					+ Math.Max(correctCount.PreferredWidth, incorrectCount.PreferredWidth)
					+ Math.Max(correctRatio.PreferredWidth, incorrectRatio.PreferredWidth),
				Math.Max(prompt.PreferredWidth, roundNo.PreferredWidth));
			table.Width = Math.Min(width * 7 / 5, ClientSize.Width);

			int height = roundNo.PreferredHeight + correctLabel.PreferredHeight + incorrectLabel.PreferredHeight + prompt.PreferredHeight;
			table.Height = Math.Min(height * 7 / 5, ClientSize.Height);

			table.Left = (ClientSize.Width - table.Width) / 2;
			table.Top = (ClientSize.Height - table.Height) / 2;
		}
	};

	// TODO: Actually log the results, allowing swapping phrase/translation, smaller phrase font for long phrases
	// TODO: Overview at end of round and end of game
	// TODO: Still get the "default beep" sound when pressing enter on the translation (even when correct). Why?
	public class LearnMode : Mode {
		Label phrase, scoreLabel, answer;
		TextBox translation; // The user enters their guess into this.
		TextBox affirmation; // When the user gets an answer wrong, make them type it in so that they learn it.
		Button overrideButton;
		Button editButton;
		RoundOverview roundOverview;

		Font font, scoreFont;
		Random random;

		AnswerChecker answerChecker = new AnswerChecker();

		enum State {
			Guessing,
			ViewingAnswer,
			ViewingResults
		}

		State state;
		Timer timer;
		int fadeOutInterval = 250;
		string lastGuess;

		class Attempt {
			public PracticeItem Item { get; set; }
			public string Guess { get; set; }
			public bool Correct { get; set; }
		};

		List<IList<Attempt>> rounds;
		IList<Attempt> currentRound;
		IList<PracticeItem> items;
		int index;
		int score;

		public LearnMode()
			: base("Learn") {
			random = new Random();
		}

		public override void Start(IPracticeWindow owner) {
			base.Start(owner);

			GameArea.Resize += new EventHandler(OnGameAreaResize);

			// State
			rounds = new List<IList<Attempt>>();
			StartRound();

			// UI
			font = new Font(GameArea.FindForm().Font.FontFamily, 24);
			scoreFont = new Font(GameArea.FindForm().Font.FontFamily, 16);

			phrase = new Label() { AutoSize = true, BackColor = Color.Transparent, Font = font };
			answer = new Label() { AutoSize = true, BackColor = Color.Transparent, Font = font };
			scoreLabel = new Label() { AutoSize = true, BackColor = Color.Transparent, Font = scoreFont, Dock = DockStyle.Bottom, TextAlign = ContentAlignment.BottomRight };
			translation = new TextBox() { Font = font };
			affirmation = new TextBox() { Font = font };
			overrideButton = new Button() { Font = GameArea.FindForm().Font, Text = "&override" };
			editButton = new Button() { Font = GameArea.FindForm().Font, Text = "&edit" };
			roundOverview = new RoundOverview() { Dock = DockStyle.Fill };

			GameArea.Controls.Add(phrase);
			GameArea.Controls.Add(scoreLabel);
			GameArea.Controls.Add(answer);
			GameArea.Controls.Add(translation);
			GameArea.Controls.Add(affirmation);
			GameArea.Controls.Add(overrideButton);
			GameArea.Controls.Add(editButton);
			GameArea.Controls.Add(roundOverview);

			translation.TextChanged += delegate { Layout(); };
			translation.KeyUp += new KeyEventHandler(translation_KeyUp);
			affirmation.TextChanged += new EventHandler(affirmation_TextChanged);
			overrideButton.Click += delegate { Override(items[index], lastGuess); };
			editButton.Click += new EventHandler(editButton_Click);
			roundOverview.KeyUp += new KeyEventHandler(roundOverview_KeyUp);

			Update();
			Layout();
		}

		void editButton_Click(object sender, EventArgs e) {
			var dialog = new Dialogs.EditPracticeItem(items[index]);
			if (dialog.ShowDialog() == DialogResult.OK) {
				var existed = DataStore.Database.UpdateWordListEntry(
					items[index].SetID,
					items[index].Phrase,
					items[index].Translation,
					dialog.Phrase,
					dialog.Translation);

				// TODO: This should possibly be made into an actual message, since it would go against 
				// the user's expectations. However, explaining the reason why it didn't work would 
				// be equally confusing.
				// A third (and perhaps best) option would be to modify the item based on its unique ID 
				// (which word list entries do actually have, even though they are not currently 
				// retrieved by SqliteWordList.)
				if (!existed)
					ProgramLog.Default.AddMessage(LogType.Debug, "Attempting to update word list entry that no longer exists: {0}, {1}", items[index].Phrase, items[index].Translation);
			}
		}

		public override void Stop() {
			base.Stop();

			GameArea.Resize -= new EventHandler(OnGameAreaResize);
		}

		void StartRound() {
			int round = rounds.Count + 1;

			var previousRound = currentRound;
			if (currentRound != null)
				rounds.Add(currentRound);
			currentRound = new List<Attempt>();

			if (previousRound != null) {
				items = new List<PracticeItem>();
				foreach (var attempt in previousRound) {
					if (!attempt.Correct)
						items.Add(attempt.Item);
					Shuffle(items);
				}
			} else {
				items = Owner.GetAllItems();
			}

			index = 0;
			score = 0;

			if (items.Count > 0)
				state = State.Guessing;
			else
				// TODO: Add a new state, completed, which shows the final scores and all rounds of the game.
				state = State.ViewingResults;
		}

		void Shuffle<T>(IList<T> items) {
			// Fisher-Yates shuffle
			int n = items.Count;
			while (n > 1) {
				n--;
				int k = random.Next(n + 1);
				var t = items[k];
				items[k] = items[n];
				items[n] = t;
			}
		}

		void ReportGuess(PracticeItem item, bool correct) {
			// TODO: Add to practice log.
		}

		void NextPhrase() {
			if (++index >= items.Count)
				state = State.ViewingResults;
			else
				state = State.Guessing;

			Update();
			Layout();
		}

		void Correct(PracticeItem item, string guess) {
			ReportGuess(item, true);
			currentRound.Add(new Attempt { Correct = true, Guess = guess, Item = item });
			score++;

			NextPhrase();
		}

		void Wrong(PracticeItem item, string guess) {
			// Don't report the answer as wrong yet -- the user may still override.
			state = State.ViewingAnswer;

			Update();
			Layout();
		}

		// The user can override and say that they were actually right.
		void Override(PracticeItem item, string guess) {
			Correct(item, guess);
		}

		// The user accepts that they are pathetic and worthless and will never amount to anything,
		// and continues their miserable existence.
		void AcceptFailure(PracticeItem item, string guess) {
			ReportGuess(item, false);
			currentRound.Add(new Attempt { Item = item, Guess = guess, Correct = false });
			NextPhrase();
		}

		bool fadingOutAnswer = false;
		void affirmation_TextChanged(object sender, EventArgs e) {
			if (fadingOutAnswer)
				return;

			if (answerChecker.IsAcceptable(affirmation.Text, items[index].Translation)) {
				fadingOutAnswer = true;

				FadeOutText(fadeOutInterval, affirmation, delegate {
					fadingOutAnswer = false;
					AcceptFailure(items[index], lastGuess);
				});
			}

			Layout();
		}

		void FadeOutText(int ms, TextBox textBox, Action whenDone) {
			Color fg = textBox.ForeColor, bg = textBox.BackColor;

			Func<int, float> clamp = x => (byte)Math.Max(0, Math.Min((int)x, 255));
			Fade(ms, p => {
				var r = clamp((float)fg.R + p * (float)(bg.R - fg.R));
				var g = clamp((float)fg.G + p * (float)(bg.G - fg.G));
				var b = clamp((float)fg.B + p * (float)(bg.B - fg.B));
				textBox.ForeColor = Color.FromArgb(r, g, b);
			}, delegate {
				whenDone();
				textBox.ForeColor = fg;
			});
		}

		void Fade(int ms, Action<float> action, Action completion) {
			timer = timer ?? new Timer() { Interval = 2, Tag = 1, Enabled = false };

			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			EventHandler onTick = null;
			onTick = delegate {
				long elapsed = sw.ElapsedMilliseconds;
				bool complete = false; 

				if (elapsed >= ms) {
					timer.Tag = (int)timer.Tag - 1;
					if ((int)timer.Tag == 0)
						timer.Enabled = false;
					timer.Tick -= onTick;
					complete = true;
				}

				float progress = Math.Max(0.0f, Math.Min((float)elapsed / (float)ms, 1.0f));
				action(progress);
				if(complete)
					completion();
			};

			timer.Tick += onTick;
			timer.Start();
		}

		bool fadingOutGuess = false;
		void translation_KeyUp(object sender, KeyEventArgs e) {
			if (fadingOutGuess) {
				e.Handled = e.SuppressKeyPress = true;
				return;
			}

			if (e.KeyCode == Keys.Return) {
				bool correct = answerChecker.IsAcceptable(translation.Text, items[index].Translation);
				lastGuess = translation.Text;

				// TODO: Remove this or make it optional
				if (!correct && !string.IsNullOrEmpty(translation.Text.Trim()))
					System.Media.SystemSounds.Asterisk.Play();

				FadeOutText(fadeOutInterval, translation, delegate {
					fadingOutGuess = false;
					if (correct)
						Correct(items[index], translation.Text);
					else
						Wrong(items[index], translation.Text);
				});

				e.Handled = e.SuppressKeyPress = true;
			}
		}

		void roundOverview_KeyUp(object sender, KeyEventArgs e) {
			StartRound();
			Update();
			Layout();
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				Stop();

				if(timer != null)
					timer.Dispose();
				font.Dispose();
			}

			base.Dispose(disposing);
		}

		protected void Update() {
			if (state == State.ViewingResults)
				roundOverview.UpdateScore(rounds.Count + 1, currentRound.Count, score);

			if (items.Count > 0 && index < items.Count) {
				phrase.Text = items[index].Phrase;
				answer.Text = items[index].Translation;
				if (state == State.Guessing)
					translation.Clear();
				if (state == State.ViewingAnswer)
					affirmation.Clear();
				scoreLabel.Text = string.Format("score: {0}/{1}, {2} remaining", score, currentRound.Count, items.Count - currentRound.Count);
			}

			affirmation.Tag = affirmation.Visible = state == State.ViewingAnswer;
			translation.Tag = translation.Visible = state == State.Guessing;
			scoreLabel.Tag = scoreLabel.Visible = state != State.ViewingResults;
			phrase.Tag = phrase.Visible = state != State.ViewingResults;
			answer.Tag = answer.Visible = state == State.ViewingAnswer;
			overrideButton.Tag = overrideButton.Visible = state == State.ViewingAnswer;
			editButton.Tag = editButton.Visible = state == State.ViewingAnswer || state == State.Guessing;
			roundOverview.Tag = roundOverview.Visible = state == State.ViewingResults;

			if (state == State.Guessing)
				translation.Focus();
			else if (state == State.ViewingAnswer)
				affirmation.Focus();
			else if (state == State.ViewingResults)
				roundOverview.Focus();
		}

		// TODO: Make the font smaller when the form is small?
		void SizeToWidth(TextBox textBox) {
			if (!(textBox.Visible || (bool?)textBox.Tag == true))
				return;

			using (Graphics g = GameArea.CreateGraphics()) {
				float min = g.MeasureString("abcdefghi", font).Width;
				float max = g.MeasureString("abcdefghijklmoqrstuvwxyz", font).Width;
				float actual = g.MeasureString(textBox.Text + "m", font).Width;

				bool fits = true;
				float width = actual;
				if (actual < min) {
					actual = min;
				} else if (actual >= max) {
					actual = max;
					fits = false;
				}

				// Room for the caret
				actual += g.MeasureString("mm", font).Width;

				int availableWidth = GameArea.ClientSize.Width - textBox.Margin.Horizontal;
				if(actual > availableWidth)
					actual = availableWidth;

				textBox.Width = (int)actual;

				if (fits) {
					// Make sure the text is positioned correctly in the textbox.
					int ss = textBox.SelectionStart, sl = textBox.SelectionLength;
					textBox.SelectionStart = 0;
					textBox.SelectionStart = ss;
					textBox.SelectionLength = sl;
				}
			}
		}

		protected void Layout() {
			SizeToWidth(translation);
			SizeToWidth(affirmation);

			var height = 0;
			foreach (var c in new Control[] { phrase, answer, affirmation, translation })
				if ((bool?)c.Tag == true)
					height += c.Height;

			var y = (GameArea.Height - height) / 2;
			foreach (var c in new Control[] { phrase, answer, affirmation, translation }) {
				if ((bool?)c.Tag == true) {
					c.Location = new Point((GameArea.Width - c.Width) / 2, y);
					y += c.Height;
				}
			}

			int right = GameArea.ClientSize.Width;
			foreach (var button in new[] { overrideButton, editButton }) {
				if ((bool?)button.Tag == true) {
					button.Top = 0;
					button.Left = right - button.Width;
					right -= button.Width + button.Margin.Right;
				}
			}

			if ((bool?)roundOverview.Tag == true) {
				roundOverview.Left = (GameArea.Width - roundOverview.Width) / 2;
				roundOverview.Top = (GameArea.Height - roundOverview.Height) / 2;
			}
		}

		protected virtual void OnGameAreaResize(object sender, EventArgs e) {
			Layout();
		}
	}
}