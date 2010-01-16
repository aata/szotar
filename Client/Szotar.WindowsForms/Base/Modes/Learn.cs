using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Szotar.WindowsForms {
	// Checks answers, ignoring case, extraneous spaces and punctuation, and parenthesised 
	// clauses (based on configuration).
	// TODO: Uncouple this from GuiConfiguration, or at least remove the SettingChanged handler.
	class AnswerChecker {
		bool fixSpaces, fixPunct, fixParens, fixCase;

		public AnswerChecker() {
			fixSpaces = GuiConfiguration.PracticeFixSpaces;
			fixPunct = GuiConfiguration.PracticeFixPunctuation;
			fixParens = GuiConfiguration.PracticeFixParentheses;
			fixCase = GuiConfiguration.PracticeFixCase;

			Configuration.Default.SettingChanged += new EventHandler<SettingChangedEventArgs>(OnSettingChanged);
		}

		public void RemoveEventHandlers() {
			Configuration.Default.SettingChanged -= new EventHandler<SettingChangedEventArgs>(OnSettingChanged);
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

	static class AnswerColors {
		public static Color Correct {
			get {
				return Color.FromArgb(32, 128, 32);
			}
		}

		public static Color Incorrect {
			get {
				return Color.FromArgb(128, 16, 16);
			}
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
			correctCount.ForeColor = correctLabel.ForeColor = correctRatio.ForeColor = AnswerColors.Correct;
			incorrectCount.ForeColor = incorrectLabel.ForeColor = incorrectRatio.ForeColor = AnswerColors.Incorrect;

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

	class GameOverview : UserControl {
		FlowLayoutPanel flow;

		Font headerFont, textFont;

		public GameOverview(LearnMode mode) {
			flow = new FlowLayoutPanel() { 
				Dock = DockStyle.Fill, 
				FlowDirection = FlowDirection.TopDown, 
				WrapContents = false,
				AutoScroll = true
			};

			headerFont = new Font(mode.GameArea.Font.FontFamily, mode.GameArea.Font.Size * 1.8f, FontStyle.Bold);
			textFont = new Font(mode.GameArea.Font.FontFamily, mode.GameArea.Font.Size, FontStyle.Regular);

			Controls.Add(flow);
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);

			if (disposing) {
				headerFont.Dispose();
				textFont.Dispose();
			}
		}

		public void UpdateOverview(IList<IList<Attempt>> rounds) {
			flow.Controls.Clear();
			flow.WrapContents = false;

			int i = 1;
			foreach (var round in rounds) {
				var headerFlow = new FlowLayoutPanel {
					FlowDirection = FlowDirection.LeftToRight,
					WrapContents = false,
					AutoSize = true
				};

				var header = new Label() { Font = headerFont, AutoSize = true };
				header.Text = string.Format("Round {0}", i++);
				headerFlow.Controls.Add(header);

				var score = new Label() { Font = headerFont, AutoSize = true, ForeColor = AnswerColors.Correct };
				int correct = 0;
				foreach (var attempt in round)
					if (attempt.Correct)
						correct++;
				score.Text = string.Format("{0}/{1} — {2}%", 
					correct, 
					round.Count, 
					round.Count > 0 ? correct * 100 / round.Count : 100);

				headerFlow.Controls.Add(score);

				flow.Controls.Add(headerFlow);
				flow.Controls.Add(MakeTable(round));
			}

			Layout();
		}

		public TableLayoutPanel MakeTable(IList<Attempt> round) {
			var table = new TableLayoutPanel {
				RowCount = round.Count,
				ColumnCount = 4
			};

			table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

			int i = 0;
			foreach (var attempt in round) {
				table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

				var ordinal = new Label {
					Text = string.Format("{0}.", ++i),
					TextAlign = ContentAlignment.MiddleRight
				};

				var word = new Label {
					Text = attempt.Swapped ? attempt.Item.Translation : attempt.Item.Phrase,
					ForeColor = attempt.Correct ? AnswerColors.Correct : AnswerColors.Incorrect,
					TextAlign = ContentAlignment.MiddleLeft
				};

				var arrow = new Label {
					Text = "→",
					TextAlign = ContentAlignment.MiddleCenter
				};

				var translation = new Label {
					Text = Text = attempt.Swapped ? attempt.Item.Phrase : attempt.Item.Translation,
					TextAlign = ContentAlignment.MiddleLeft
				};

				int j = 0;
				foreach (Label label in new[] { ordinal, word, arrow, translation }) {
					label.AutoSize = true;
					label.Font = textFont;
					table.Controls.Add(label);
					table.SetRow(label, i - 1);
					table.SetColumn(label, j++);
				}
			}

			table.AutoSize = true;
			table.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			table.Margin = new Padding(table.Margin.Left * 4, table.Margin.Top, table.Margin.Right, table.Margin.Bottom);
			return table;
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);

			Layout();
		}

		// TODO: This is still pretty slow. Find a faster way.
		new void Layout() {
			flow.SuspendLayout();
			foreach (var control in flow.Controls) {
				var table = control as TableLayoutPanel;
				if (table != null) {
					table.SuspendLayout();
					foreach(var tableControl in table.Controls) {
						var label = tableControl as Label;
						if (label != null)
							label.MaximumSize = new Size(ClientSize.Width * 3 / 7, int.MaxValue);
					}
					table.ResumeLayout();
				}
			}
			flow.ResumeLayout();
		}
	};

	// TODO: Add items in the file menu that open the word lists currently being learnt.
	class OptionsMenu : MenuStrip {
		LearnMode mode;
		ToolStripMenuItem swapItem, optionsItem;

		public OptionsMenu(LearnMode mode) {
			this.mode = mode;

			optionsItem = new ToolStripMenuItem("&Options");
			Items.Add(optionsItem);

			swapItem = new ToolStripMenuItem("Swap phrase/translation");
			swapItem.Click += delegate {
				mode.Swap = !mode.Swap;
			};

			optionsItem.DropDownOpening += new EventHandler(optionsItem_DropDownOpening);
			optionsItem.DropDownItems.Add(swapItem);
			optionsItem.DropDownItems.Add(new ToolStripSeparator());

			AddFixupItem("Ignore spaces in answer", "PracticeFixSpaces", optionsItem);
			AddFixupItem("Ignore punctuation in answer", "PracticeFix", optionsItem);
			AddFixupItem("Ignore parenthesized text in answer", "PracticeFixSpaces", optionsItem);
			AddFixupItem("Ignore case in answer", "PracticeFixSpaces", optionsItem);
		}

		void optionsItem_DropDownOpening(object sender, EventArgs e) {
			swapItem.Checked = mode.Swap;
			foreach (ToolStripItem item in optionsItem.DropDownItems) {
				var menuItem = item as ToolStripMenuItem;
				if (menuItem != null && menuItem.Tag is string)
					menuItem.Checked = GuiConfiguration.GetPracticeFixupSetting((string)menuItem.Tag);
			}
		}

		void AddFixupItem(string text, string setting, ToolStripMenuItem parent) {
			var item = new ToolStripMenuItem(text);
			item.Tag = setting;
			item.Click += delegate {
				GuiConfiguration.SetPracticeFixupSetting(setting, !GuiConfiguration.GetPracticeFixupSetting(setting));
			};
			parent.DropDownItems.Add(item);
		}
	}

	class Attempt {
		public PracticeItem Item { get; set; }
		public string Guess { get; set; }
		public bool Correct { get; set; }
		public bool Swapped { get; set; }
	};

	// TODO: Actually log the results
	// TODO: Overview at end of game
	// TODO: Still get the "default beep" sound when pressing enter on the translation (even when correct). Why?
	public class LearnMode : Mode {
		Label phrase, scoreLabel, answer;
		TextBox translation; // The user enters their guess into this.
		TextBox affirmation; // When the user gets an answer wrong, make them type it in so that they learn it.
		Button overrideButton;
		Button editButton;
		RoundOverview roundOverview;
		GameOverview gameOverview;

		Font font, smallFont, extraSmallFont, scoreFont;
		Random random;

		AnswerChecker answerChecker = new AnswerChecker();

		enum State {
			Guessing,
			ViewingAnswer,
			RoundOverview,
			GameOverview
		}

		State state;
		Timer timer;
		int fadeOutInterval = 250;
		string lastGuess;

		List<IList<Attempt>> rounds;
		IList<Attempt> currentRound;
		IList<PracticeItem> items;
		int index;
		int score;
		bool swap;

		public LearnMode()
			: base("Learn")
		{
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
			smallFont = new Font(GameArea.FindForm().Font.FontFamily, 16);
			extraSmallFont = new Font(GameArea.FindForm().Font.FontFamily, 12);
			scoreFont = new Font(GameArea.FindForm().Font.FontFamily, 16);

			phrase = new Label() { AutoSize = true, BackColor = Color.Transparent, Font = font };
			answer = new Label() { AutoSize = true, BackColor = Color.Transparent, Font = font };
			scoreLabel = new Label() { AutoSize = true, BackColor = Color.Transparent, Font = scoreFont, Dock = DockStyle.Bottom, TextAlign = ContentAlignment.BottomRight };
			translation = new TextBox() { Font = font };
			affirmation = new TextBox() { Font = font };
			overrideButton = new Button() { Font = GameArea.FindForm().Font, Text = "&override" };
			editButton = new Button() { Font = GameArea.FindForm().Font, Text = "&edit" };
			roundOverview = new RoundOverview() { Dock = DockStyle.Fill };
			gameOverview = new GameOverview(this) { Dock = DockStyle.Fill };

			GameArea.Controls.Add(phrase);
			GameArea.Controls.Add(scoreLabel);
			GameArea.Controls.Add(answer);
			GameArea.Controls.Add(translation);
			GameArea.Controls.Add(affirmation);
			GameArea.Controls.Add(overrideButton);
			GameArea.Controls.Add(editButton);
			GameArea.Controls.Add(roundOverview);
			GameArea.Controls.Add(gameOverview);

			translation.TextChanged += delegate { Layout(); };
			translation.KeyUp += new KeyEventHandler(translation_KeyUp);
			affirmation.TextChanged += new EventHandler(affirmation_TextChanged);
			overrideButton.Click += delegate { Override(items[index], lastGuess); };
			editButton.Click += new EventHandler(editButton_Click);
			roundOverview.KeyUp += new KeyEventHandler(roundOverview_KeyUp);

			MergeMenu(new OptionsMenu(this));

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

			answerChecker.RemoveEventHandlers();
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
				state = State.GameOverview;
		}

		void Shuffle<T>(IList<T> items) {
			ShuffleRange(items, 0, items.Count);
		}

		void ShuffleRange<T>(IList<T> items, int start, int count) {
			// Fisher-Yates shuffle
			int n = count;
			while (n > 1) {
				n--;
				int k = random.Next(n + 1);
				var t = items[start + k];
				items[start + k] = items[start + n];
				items[start + n] = t;
			}
		}

		public bool Swap {
			get { return swap; }
			set {
				if (swap != value) {
					swap = value;

					// Shuffle the remaining items so you don't get the same one again (which would be too easy.)
					// This could also be done by simply moving the current item to the back of the list and shifting
					// the rest frontwards.
					ShuffleRange(items, index, items.Count - index);
					Update();
					Layout();
				}
			}
		}

		string CurrentPhrase { get { return swap ? items[index].Translation : items[index].Phrase; } }
		string CurrentTranslation { get { return swap ? items[index].Phrase : items[index].Translation; } }

		void ReportGuess(PracticeItem item, bool correct) {
			// TODO: Add to practice log.
		}

		void NextPhrase() {
			if (++index >= items.Count)
				state = State.RoundOverview;
			else
				state = State.Guessing;

			Update();
			Layout();
		}

		void Correct(PracticeItem item, string guess) {
			ReportGuess(item, true);
			currentRound.Add(new Attempt { Correct = true, Guess = guess, Item = item, Swapped = Swap });
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
			currentRound.Add(new Attempt { Item = item, Guess = guess, Correct = false, Swapped = Swap });
			NextPhrase();
		}

		bool fadingOutAnswer = false;
		void affirmation_TextChanged(object sender, EventArgs e) {
			if (fadingOutAnswer)
				return;

			if (answerChecker.IsAcceptable(affirmation.Text, CurrentTranslation)) {
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
				bool correct = answerChecker.IsAcceptable(translation.Text, CurrentTranslation);
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
				smallFont.Dispose();
				extraSmallFont.Dispose();
			}

			base.Dispose(disposing);
		}

		protected void Update() {
			if (state == State.RoundOverview)
				roundOverview.UpdateScore(rounds.Count + 1, currentRound.Count, score);
			else if (state == State.GameOverview)
				gameOverview.UpdateOverview(rounds);

			if (items.Count > 0 && index < items.Count) {
				phrase.Text = CurrentPhrase;
				answer.Text = CurrentTranslation;
				if (state == State.Guessing)
					translation.Clear();
				if (state == State.ViewingAnswer)
					affirmation.Clear();
				scoreLabel.Text = string.Format("score: {0}/{1}, {2} remaining", score, currentRound.Count, items.Count - currentRound.Count);
			}

			affirmation.Tag = affirmation.Visible = state == State.ViewingAnswer;
			translation.Tag = translation.Visible = state == State.Guessing;
			scoreLabel.Tag = scoreLabel.Visible = state != State.RoundOverview && state != State.GameOverview;
			phrase.Tag = phrase.Visible = state != State.RoundOverview && state != State.GameOverview;
			answer.Tag = answer.Visible = state == State.ViewingAnswer;
			overrideButton.Tag = overrideButton.Visible = state == State.ViewingAnswer;
			editButton.Tag = editButton.Visible = state == State.ViewingAnswer || state == State.Guessing;
			roundOverview.Tag = roundOverview.Visible = state == State.RoundOverview;
			gameOverview.Tag = gameOverview.Visible = state == State.GameOverview;

			if (state == State.Guessing)
				translation.Focus();
			else if (state == State.ViewingAnswer)
				affirmation.Focus();
			else if (state == State.RoundOverview)
				roundOverview.Focus();
			else if (state == State.GameOverview)
				gameOverview.Focus();
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

			using (var g = phrase.CreateGraphics()) {
				int sizeRatio = (int)g.MeasureString(phrase.Text, font).Width / GameArea.ClientSize.Width;
				if (sizeRatio >= 2)
					phrase.Font = extraSmallFont;
				else if (sizeRatio >= 1)
					phrase.Font = smallFont;
				else
					phrase.Font = font;
			}

			phrase.MaximumSize = new Size(
				GameArea.ClientSize.Width - phrase.Padding.Horizontal,
				GameArea.ClientSize.Height - phrase.Padding.Vertical);

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
