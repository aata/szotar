using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

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

			return guess == Fix(answer) || answer.Split(';', ',').Any(s => guess == Fix(s));
		}

		// TODO: Possible enhancements: regex-based or word-based replacements in given answer, such as:
		//       sg -> something
		//       vkit -> valakit
		string Fix(string s) {
			var sb = new StringBuilder();

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
					}

					// This only occurs if c wasn't a punctuation mark -- thus, " ... " will be collapsed into " ".
					wasSpace = isSpace;
				}

				if (add)
					sb.Append(c);
			}

			return sb.ToString().Trim();
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
		readonly TableLayoutPanel table;
		readonly Label roundNo = new Label();
		readonly Label correctLabel = new Label(), incorrectLabel = new Label();
		readonly Label correctCount = new Label(), incorrectCount = new Label();
		readonly Label correctRatio = new Label(), incorrectRatio = new Label();
		readonly Label prompt = new Label();

		public RoundOverview() : this(0, 0, 0) { }

		public void UpdateScore(int round, int count, int correct) {
			roundNo.Text = string.Format(Resources.LearnMode.EndOfRound, round);

			int incorrect = count - correct;
			correctLabel.Text = Resources.LearnMode.Correct;
			incorrectLabel.Text = Resources.LearnMode.Incorrect;
			correctCount.Text = correct.ToString(CultureInfo.CurrentUICulture);
			incorrectCount.Text = incorrect.ToString(CultureInfo.CurrentUICulture);
			correctRatio.Text = RatioToPercentage(correct, count);
			incorrectRatio.Text = RatioToPercentage(incorrect, count);
			prompt.Text = Resources.LearnMode.PressAnyKey;

			Layout();
		}

		public RoundOverview(int round, int count, int correct) {
			// TODO: Make theme-aware.
			correctCount.ForeColor = correctLabel.ForeColor = correctRatio.ForeColor = AnswerColors.Correct;
			incorrectCount.ForeColor = incorrectLabel.ForeColor = incorrectRatio.ForeColor = AnswerColors.Incorrect;

			base.Font = new Font(base.Font.FontFamily, base.Font.Size * 1.4f);
			FontFamily titleFontFamily = Array.Exists(FontFamily.Families, x => x.Name == "Cambria") 
				? new FontFamily("Cambria")
				: roundNo.Font.FontFamily;

			roundNo.Font = new Font(titleFontFamily, base.Font.Size * 2.0f, FontStyle.Bold);
			Disposed += delegate {
				Font.Dispose();
				roundNo.Font.Dispose();
			};
			
			prompt.ForeColor = SystemColors.GrayText;

			table = new TableLayoutPanel { RowCount = 4, ColumnCount = 3 }; 
			
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

		static string RatioToPercentage(int nom, int denom) {
			return denom == 0 
				? "100%" 
				: string.Format("{0:D}%", nom * 100 / denom);
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
		readonly ListView list;
		readonly List<ListViewGroup> groups;

		public GameOverview(LearnMode mode) {
			list = new ListView { 
				Dock = DockStyle.Fill, 
				Margin = new Padding(3),
				FullRowSelect = true,
				View = View.Details,
				HeaderStyle = ColumnHeaderStyle.None
			};
			groups = new List<ListViewGroup>();

			Controls.Add(list);
			ThemeHelper.UseExplorerTheme(list);

			InitializeContextMenu();

			list.Resize += delegate { DistributeColumns(); };
			Load += delegate { DistributeColumns(); };
		}

		void DistributeColumns() {
			if (list.Columns.Count < 3)
				return;
			
			list.Columns[0].Width = list.ClientSize.Width / 2;
			list.Columns[1].Width = list.ClientSize.Width / 2;
			list.Columns[2].Width = 0;
		}

		public void UpdateOverview(IList<IList<Attempt>> rounds) {
			list.BeginUpdate();

			try {
				list.Clear();
				list.Groups.Clear();
				groups.Clear();

				list.Columns.Add(new ColumnHeader());
				list.Columns.Add(new ColumnHeader());
				list.Columns.Add(new ColumnHeader());

				foreach (var round in rounds) {
					var group = new ListViewGroup();
					groups.Add(group);
					group.Header = string.Format(Resources.LearnMode.RoundN, groups.Count);
					list.Groups.Add(group);

					foreach (var attempt in round) {
						string phrase, translation;
						if (!attempt.Swapped) {
							phrase = attempt.Item.Phrase;
							translation = attempt.Item.Translation;
						} else {
							phrase = attempt.Item.Translation;
							translation = attempt.Item.Phrase;
						}

						var item = new ListViewItem(new[] { phrase, translation, "" }, group);
						if(!attempt.Correct)
							item.ForeColor = SystemColors.GrayText;
						item.Tag = attempt;
						list.Items.Add(item);
					}
				}
			} finally {
				list.EndUpdate();
			}

			DistributeColumns();
		}

		#region Context Menu
		ContextMenuStrip contextMenu;
		void InitializeContextMenu() {
			contextMenu = new ContextMenuStrip();

			var viewInList = new ToolStripMenuItem(Resources.LearnMode.ViewInList);
			EventHandler viewInListClick = (s, e) => {                
				foreach (ListViewItem item in list.SelectedItems) {
					var attempt = item.Tag as Attempt;
					if (attempt == null)
						continue;
					var form = Forms.ListBuilder.Open(attempt.Item.SetID);
					if(form != null)
						form.ScrollToItem(attempt.Item.Phrase, attempt.Item.Translation);
				}
			};
			viewInList.Click += viewInListClick;
			list.ItemActivate += viewInListClick;
			contextMenu.Items.Add(viewInList);

			list.ContextMenuStrip = contextMenu;
		}
		#endregion
	}

	// TODO: Add items in the file menu that open the word lists currently being learnt.
	class OptionsMenu : MenuStrip {
		readonly LearnMode mode;
		readonly ToolStripMenuItem swapItem, optionsItem;

		public OptionsMenu(LearnMode mode) {
			this.mode = mode;

			optionsItem = new ToolStripMenuItem(Resources.LearnMode.Options);
			base.Items.Add(optionsItem);

			swapItem = new ToolStripMenuItem(Resources.LearnMode.Swap);
			swapItem.Click += delegate {
				mode.Swap = !mode.Swap;
			};
			swapItem.ShortcutKeys = Keys.Control | Keys.I;

			optionsItem.DropDownOpening += OptionsItemDropDownOpening;
			optionsItem.DropDownItems.Add(swapItem);
			optionsItem.DropDownItems.Add(new ToolStripSeparator());
			
			AddFixupItem(Resources.LearnMode.IgnoreSpaces, "PracticeFixSpaces", optionsItem);
			AddFixupItem(Resources.LearnMode.IgnorePunctuation, "PracticeFixPunctuation", optionsItem);
			AddFixupItem(Resources.LearnMode.IgnoreParenthesized, "PracticeFixParentheses", optionsItem);
			AddFixupItem(Resources.LearnMode.IgnoreCase, "PracticeFixCase", optionsItem);
		}

		void OptionsItemDropDownOpening(object sender, EventArgs e) {
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
	public class LearnMode : Mode {
		Label phrase, scoreLabel, answer, history;
		TextBox translation; // The user enters their guess into this.
		TextBox affirmation; // When the user gets an answer wrong, make them type it in so that they learn it.
		Button overrideButton;
		Button editButton;
		Button viewButton;
		Button deleteButton;
		RoundOverview roundOverview;
		GameOverview gameOverview;

		Font font, smallFont, extraSmallFont, scoreFont;
		readonly Random rng;
		readonly AnswerChecker answerChecker = new AnswerChecker();

		enum State {
			Guessing,
			ViewingAnswer,
			RoundOverview,
			GameOverview
		}

		State state;
		Timer timer;
		const int fadeOutInterval = 250;
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
			rng = new Random();
		}

		public override void Start(IPracticeWindow owner) {
			base.Start(owner);

			GameArea.Resize += OnGameAreaResize;

			// State
			rounds = new List<IList<Attempt>>();
			StartRound();

			// UI
			var form = GameArea.FindForm();
			var formFont = form != null ? form.Font : SystemFonts.DefaultFont;
			var fontFamily = formFont.FontFamily;

			font = new Font(fontFamily, 24);
			smallFont = new Font(fontFamily, 16);
			extraSmallFont = new Font(fontFamily, 12);
			scoreFont = new Font(fontFamily, 16);

			phrase = new Label { AutoSize = true, BackColor = Color.Transparent, Font = font };
			answer = new Label { AutoSize = true, BackColor = Color.Transparent, Font = font };
			scoreLabel = new Label { AutoSize = true, BackColor = Color.Transparent, Font = scoreFont, Dock = DockStyle.Bottom, TextAlign = ContentAlignment.BottomRight };
			history = new Label { AutoSize = true, BackColor = Color.Transparent, Font = extraSmallFont, Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleLeft };
			translation = new TextBox { Font = font };
			affirmation = new TextBox { Font = font };
			overrideButton = new Button { Font = formFont, Text = Resources.LearnMode.OverrideButton };
			editButton = new Button { Font = formFont, Text = Resources.LearnMode.EditButton };
			viewButton = new Button { Font = formFont, Text = Resources.LearnMode.ViewInListButton };
			deleteButton = new Button { Font = formFont, Text = Resources.LearnMode.DeleteButton };
			roundOverview = new RoundOverview { Dock = DockStyle.Fill };
			gameOverview = new GameOverview(this) { Dock = DockStyle.Fill };

			GameArea.Controls.Add(phrase);
			GameArea.Controls.Add(scoreLabel);
			GameArea.Controls.Add(answer);
			GameArea.Controls.Add(history);
			GameArea.Controls.Add(translation);
			GameArea.Controls.Add(affirmation);
			GameArea.Controls.Add(overrideButton);
			GameArea.Controls.Add(editButton);
			GameArea.Controls.Add(viewButton);
			GameArea.Controls.Add(deleteButton);
			GameArea.Controls.Add(roundOverview);
			GameArea.Controls.Add(gameOverview);

			translation.TextChanged += delegate { Layout(); };
			translation.KeyUp += TranslationKeyUp;
			affirmation.TextChanged += AffirmationTextChanged;
			overrideButton.Click += delegate { Override(items[index], lastGuess); };
			editButton.Click += EditButtonClick;
			viewButton.Click += ViewButtonClick;
			deleteButton.Click += DeleteButtonClick;
			roundOverview.KeyUp += RoundOverviewKeyUp;

			// Stop the beep sound being produced when Enter is pressed.
			// (Handling KeyUp is not sufficient to stop this.)
			translation.KeyPress += (s, e) => {
				if (e.KeyChar == '\r')
					e.Handled = true;
			};

			MergeMenu(new OptionsMenu(this));

			Update();
			Layout();
		}

		void EditButtonClick(object sender, EventArgs e) {
			var newItem = Dialogs.EditPracticeItem.Show(items[index]);
			if (newItem != null)
				items[index] = newItem;
			Update();
			Layout();
		}

		void DeleteButtonClick(object sender, EventArgs e) {
			var dr = MessageBox.Show(
				GameArea,
				Properties.Resources.ConfirmDeletePracticeItem,
				Properties.Resources.ConfirmDeletePracticeItemTitle,
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.None,
				MessageBoxDefaultButton.Button2);

			if (dr != DialogResult.OK)
				return;
			
			var list = DataStore.Database.GetWordList(items[index].SetID);
			for (int i = 0; i < list.Count; i++) {
				var item = list[i];
				if (item.Phrase == items[index].Phrase && item.Translation == items[index].Translation)
					list.RemoveAt(i);
			}
		}

		void ViewButtonClick(object sender, EventArgs e) {
			var form = Forms.ListBuilder.Open(items[index].SetID);
			if (form != null)
				form.ScrollToItem(items[index].Phrase, items[index].Translation);
		}

		public override void Stop() {
			base.Stop();

			answerChecker.RemoveEventHandlers();
			GameArea.Resize -= OnGameAreaResize;
		}

		void StartRound() {
			int round = rounds.Count + 1;

			var previousRound = currentRound;
			if (currentRound != null)
				rounds.Add(currentRound);
			currentRound = new List<Attempt>();

			items = previousRound != null 
				? new List<PracticeItem>(from x in previousRound where !x.Correct select x.Item) 
				: Owner.GetAllItems();

			items.Shuffle(rng);

			index = 0;
			score = 0;

			state = items.Count > 0 ? State.Guessing : State.GameOverview;
		}

		public bool Swap {
			get { return swap; }
			set {
				if (swap == value)
					return;
				swap = value;

				// Shuffle the remaining items so you don't get the same one again (which would be too easy.)
				// This could also be done by simply moving the current item to the back of the list and shifting
				// the rest frontwards.
				items.Shuffle(rng, index, items.Count - index);
				Update();
				Layout();
			}
		}

		string CurrentPhrase { get { return swap ? items[index].Translation : items[index].Phrase; } }
		string CurrentTranslation { get { return swap ? items[index].Phrase : items[index].Translation; } }

		static void ReportGuess(PracticeItem item, bool correct) {
			item.History.Add(DateTime.Now, correct);
			DataStore.Database.AddPracticeHistory(item.SetID, item.Phrase, item.Translation, correct);
		}

		void NextPhrase() {
			state = ++index >= items.Count ? State.RoundOverview : State.Guessing;

			Update();
			Layout();
		}

		void Correct(PracticeItem item, string guess) {
			ReportGuess(item, true);
			currentRound.Add(new Attempt { Correct = true, Guess = guess, Item = item, Swapped = Swap });
			score++;

			NextPhrase();
		}

		void Wrong() {
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

		bool fadingOutAnswer;
		void AffirmationTextChanged(object sender, EventArgs e) {
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

		void FadeOutText(int ms, Control textBox, Action whenDone) {
			Color fg = textBox.ForeColor, bg = textBox.BackColor;

			Func<int, float> clamp = x => (byte)Math.Max(0, Math.Min((int)x, 255));
			Fade(ms, p => {
				var r = clamp(fg.R + p * (bg.R - fg.R));
				var g = clamp(fg.G + p * (bg.G - fg.G));
				var b = clamp(fg.B + p * (bg.B - fg.B));
				textBox.ForeColor = Color.FromArgb(r, g, b);
			}, delegate {
				whenDone();
				textBox.ForeColor = fg;
			});
		}

		void Fade(int ms, Action<float> action, Action completion) {
			timer = timer ?? new Timer { Interval = 2, Tag = 1, Enabled = false };

			var sw = new Stopwatch();
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

				float progress = Math.Max(0.0f, Math.Min(elapsed / (float)ms, 1.0f));
				action(progress);
				if(complete)
					completion();
			};

			timer.Tick += onTick;
			timer.Start();
		}

		bool fadingOutGuess;
		void TranslationKeyUp(object sender, KeyEventArgs e) {
			if (fadingOutGuess) {
				e.Handled = e.SuppressKeyPress = true;
				return;
			}

			if (e.KeyCode != Keys.Return)
				return;
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
					Wrong();
			});

			e.Handled = e.SuppressKeyPress = true;
		}

		void RoundOverviewKeyUp(object sender, KeyEventArgs e) {
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
				scoreLabel.Text = string.Format(Resources.LearnMode.Score, score, currentRound.Count, items.Count - currentRound.Count);
				history.Text = string.Format(Resources.LearnMode.History, items[index].History.History.Count(x => x.Value), items[index].History.History.Count, items[index].History.Importance);
			}

			affirmation.Tag = affirmation.Visible = state == State.ViewingAnswer;
			translation.Tag = translation.Visible = state == State.Guessing;
			scoreLabel.Tag = scoreLabel.Visible = state != State.RoundOverview && state != State.GameOverview;
			phrase.Tag = phrase.Visible = state != State.RoundOverview && state != State.GameOverview;
			history.Tag = history.Visible = state == State.Guessing || state == State.ViewingAnswer;
			answer.Tag = answer.Visible = state == State.ViewingAnswer;
			overrideButton.Tag = overrideButton.Visible = state == State.ViewingAnswer;
			viewButton.Tag = viewButton.Visible = state == State.ViewingAnswer;
			deleteButton.Tag = deleteButton.Visible = state == State.ViewingAnswer;
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
		void SizeToWidth(TextBoxBase textBox) {
			if (!(textBox.Visible || (bool?)textBox.Tag == true))
				return;

			using (Graphics g = GameArea.CreateGraphics()) {
				float min = g.MeasureString("abcdefghi", font).Width;
				float max = g.MeasureString("abcdefghijklmoqrstuvwxyz", font).Width;
				float actual = g.MeasureString(textBox.Text + "m", font).Width;

				bool fits = true;
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

		void ChooseLabelFont(Label label) {
			if ((bool?)label.Tag != true)
				return;

			using (var g = label.CreateGraphics()) {
				int sizeRatio = (int)g.MeasureString(label.Text, font).Width / GameArea.ClientSize.Width;
				if (sizeRatio >= 2)
					label.Font = extraSmallFont;
				else if (sizeRatio >= 1)
					label.Font = smallFont;
				else
					label.Font = font;
			}
		}

		protected void Layout() {
			SizeToWidth(translation);
			SizeToWidth(affirmation);

			ChooseLabelFont(phrase);
			ChooseLabelFont(answer);

			phrase.MaximumSize = answer.MaximumSize = new Size(
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
			foreach (var button in new[] { overrideButton, editButton, viewButton, deleteButton }) {
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
