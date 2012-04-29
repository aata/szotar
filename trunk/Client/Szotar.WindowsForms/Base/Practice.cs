using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Szotar.WindowsForms {
	public enum PracticeMode {
		Default,
		SearchMode,
		Flashcards,
		Learn,
		MultipleChoice
	}

	public class PracticeOptions {
		public PracticeMode Mode { get; set; }
		public bool SwapTerms { get; set; }

		public PracticeOptions() {
			Mode = PracticeMode.Default;
			SwapTerms = false;
		}
	}

	public interface IPracticeWindow {
		void MarkSuccess(PracticeItem item);
		void MarkFailure(PracticeItem item);

		PracticeItem FetchNextItem();
		IList<PracticeItem> GetAllItems();
		void ReplaceItem(PracticeItem existing, PracticeItem replacement);

		ToolStrip Controls { get; }
		Panel GameArea { get; }

		void SetMode(IPracticeMode mode);

		int ItemCount { get; }
		int Position { get; }
	}

	public interface IPracticeMode : IDisposable {
		void Start(IPracticeWindow owner);
		void Stop();
		string Name { get; }
	}

	public class Mode : IPracticeMode, IDisposable {
		public string Name { get; protected set; }
		public IPracticeWindow Owner { get; private set; }
		public Panel GameArea { get { return Owner.GameArea; } }

		List<ToolStrip> mergedMenus = new List<ToolStrip>();

		public Mode(string name) {
			Name = name;
		}

		public virtual void Start(IPracticeWindow owner) {
			Owner = owner;
		}

		public virtual void Stop() {
			GameArea.Controls.Clear();
			foreach (var menu in mergedMenus)
				ToolStripManager.RevertMerge(Owner.Controls, menu);
			mergedMenus.Clear();
		}

		protected void MergeMenu(ToolStrip menu) {
			ToolStripManager.Merge(menu, Owner.Controls);
			if (!mergedMenus.Contains(menu))
				mergedMenus.Add(menu);
		}

		protected void UnmergeMenu(ToolStrip menu) {
			ToolStripManager.RevertMerge(Owner.Controls, menu);
			mergedMenus.Remove(menu);
		}

		#region Disposable
		public void Dispose() {
			Dispose(true);

			GameArea.Controls.Clear();
		}

		protected virtual void Dispose(bool disposing) {
			GC.SuppressFinalize(this);
		}

		~Mode() {
			Dispose(false);
		}
		#endregion
	}

	public class DefaultMode : Mode {
		public DefaultMode()
			: base("")
		{ }

		public override void Start(IPracticeWindow owner) {
			base.Start(owner);
		}

		public override void Stop() {
			base.Stop();
		}
	}

	/// <summary>
	/// Stores a backwards/forwards history and navigates through it.
	/// </summary>
	public class Navigator {
		Stack<PracticeItem>
			back = new Stack<PracticeItem>(),
			fore = new Stack<PracticeItem>();

		public IPracticeWindow Source { get; private set; }
		public PracticeItem CurrentItem { get; private set; }
		public void UpdateCurrentItem(PracticeItem newItem) {
			Source.ReplaceItem(CurrentItem, newItem);
			CurrentItem = newItem;
		}
		public int Length { get { return Source.ItemCount; } }
		public int Position { 
			get {
				var pos = Source.Position - fore.Count;

				if (CurrentItem != null)
					pos--;

				while (pos < 0)
					pos += Source.ItemCount;

				return pos;
			}
		}

		public Navigator(IPracticeWindow source) {
			Source = source;

			CurrentItem = source.FetchNextItem();
		}

		/// <summary>
		/// Advance to the last item that has already been seen.
		/// </summary>
		public void AdvanceToEnd() {
			back.Push(CurrentItem);

			while (fore.Count > 0) {
				back.Push(CurrentItem);
				CurrentItem = fore.Pop();
			}
		}

		public void Advance() {
			if (fore.Count == 0) {
				back.Push(CurrentItem);
				CurrentItem = Source.FetchNextItem();
				return;
			}

			back.Push(CurrentItem);
			CurrentItem = fore.Pop();
		}

		public bool Retreat() {
			if (back.Count == 0)
				return false;

			fore.Push(CurrentItem);
			CurrentItem = back.Pop();
			return true;
		}
	}

	/// <summary>
	/// The player is shown the phrase, and tries to guess the translation. Advancing
	/// again reveals the translation.
	/// </summary>
	public class FlashcardMode : Mode {
		Control phraseLabel, translationLabel;

		Navigator nav;
		NavigatorMenu navMenu;

		Font bigFont, smallFont;
		bool swap;

		public FlashcardMode()
			: base("Flashcards") 
		{ }

		public override void Start(IPracticeWindow owner) {
			base.Start(owner);

			swap = false;
			nav = new Navigator(owner);
			navMenu = new NavigatorMenu(nav, true);

			MergeMenu(navMenu.Menu);

			phraseLabel = new Label();
			translationLabel = new Label();
			translationLabel.ForeColor = System.Drawing.SystemColors.GrayText;

			bigFont = new System.Drawing.Font(GameArea.FindForm().Font.FontFamily, 24);
			smallFont = new System.Drawing.Font(GameArea.FindForm().Font.FontFamily, 16);

			foreach (var l in new[] { phraseLabel, translationLabel }) {
				l.AutoSize = true;
				l.Font = bigFont;
				l.BackColor = Color.Transparent;
				GameArea.Controls.Add(l);
			}

			GameArea.Resize += new EventHandler(GameArea_Resize);
			GameArea.PreviewKeyDown += new PreviewKeyDownEventHandler(GameArea_PreviewKeyDown);

			foreach (Control c in new Control[] { phraseLabel, translationLabel, GameArea }) {
				c.MouseUp += new MouseEventHandler(GameArea_MouseUp);
			}

			navMenu.Back += delegate { GoBack(); };
			navMenu.Forward += delegate { GoForward(); };
			navMenu.End += delegate { GoToEnd(); };
			navMenu.Edit += delegate {
				var newItem = Dialogs.EditPracticeItem.Show(nav.CurrentItem);
				if (newItem != null)
					nav.UpdateCurrentItem(newItem);
				Update();
				Layout();
			};
			navMenu.Swap += delegate { SwapItems(); };

			translationLabel.Visible = false;
			Update();
			Layout();
		}

		void GameArea_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
			if (e.Modifiers != Keys.None) {
				// Do nothing.
			} else if (e.KeyCode == Keys.Left) {
				GoBack();
			} else if (e.KeyCode == Keys.End) {
				GoToEnd();
			} else if (e.KeyCode == Keys.Home) {
				GoToStart();
			} else {
				GoForward();
			}
		}

		private void SwapItems() {
			swap = !swap;

			Update();
			Layout();
		}

		public override void Stop() {
			base.Stop();

			GameArea.Resize -= new EventHandler(GameArea_Resize);
			GameArea.PreviewKeyDown -= new PreviewKeyDownEventHandler(GameArea_PreviewKeyDown);

			foreach (Control c in new Control[] { phraseLabel, translationLabel, GameArea })
				c.MouseUp -= new MouseEventHandler(GameArea_MouseUp);
		}

		void GoForward() {
			if (translationLabel.Visible) {
				nav.Advance();
				translationLabel.Visible = false;
			} else {
				translationLabel.Visible = true;
			}

			Update();
			Layout();
		}

		private void GoToStart() {
			while(nav.Position > 0)
				nav.Retreat();
			translationLabel.Visible = false;

			Update();
			Layout();
		}

		private void GoToEnd() {
			nav.AdvanceToEnd();
			translationLabel.Visible = false;

			Update();
			Layout();
		}

		void GoBack() {
			if (translationLabel.Visible) {
				translationLabel.Visible = false;
			} else {
				if (!nav.Retreat()) {
					// TODO: Show a message saying the start of the stream has been reached.
				} else {
					translationLabel.Visible = true;
				}
			}

			Update();
			Layout();
		}

		void GameArea_MouseUp(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.XButton2)
				GoForward();
			else if (e.Button == MouseButtons.Right || e.Button == MouseButtons.XButton1)
				GoBack();
		}

		void GameArea_Resize(object sender, EventArgs e) {
			Layout();
		}

		void Update() {
			navMenu.Update();

			if (swap) {
				phraseLabel.Text = nav.CurrentItem.Translation;
				translationLabel.Text = nav.CurrentItem.Phrase;
			} else {
				phraseLabel.Text = nav.CurrentItem.Phrase;
				translationLabel.Text = nav.CurrentItem.Translation;
			}
		}

		void Layout() {
			// Choose a smaller font if it doesn't fit
			foreach (var label in new[] { phraseLabel, translationLabel }) {
				if (label.Font == bigFont && label.Width > GameArea.Width) {
					label.Font = smallFont;
				} else if (label.Font == smallFont) {
					using (Graphics g = GameArea.CreateGraphics()) {
						if (g.MeasureString(label.Text, bigFont).Width < GameArea.Width)
							label.Font = bigFont;
					}
				}
			}

			var height = phraseLabel.Height + translationLabel.Height;

			var px = (GameArea.Width - phraseLabel.Width) / 2;
			var tx = (GameArea.Width - translationLabel.Width) / 2;

			var py = (GameArea.Height - height) / 2;
			var ty = py + phraseLabel.Height;

			phraseLabel.Location = new System.Drawing.Point(px, py);
			translationLabel.Location = new System.Drawing.Point(tx, ty);
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				Stop();
				bigFont.Dispose();
				smallFont.Dispose();
			}

			base.Dispose(disposing);
		}
	}

	/// <summary>
	/// Maintains a menu with navigation buttons that raise events when clicked. This menu
	/// should be merged with some other menu.
	/// </summary>
	public class NavigatorMenu {
		public Navigator Navigator { get; protected set; }
		public ToolStrip Menu { get; protected set; }

		ToolStripButton back, fore, end, edit, swap;
		ToolStripLabel position;

		public NavigatorMenu(Navigator nav, bool swapItem) {
			Navigator = nav;

			back = new ToolStripButton { Text = "←" };
			fore = new ToolStripButton { Text = "→" };
			end = new ToolStripButton { Text = "end" };
			edit = new ToolStripButton { Text = "edit" };
			if (swapItem)
				swap = new ToolStripButton { Text = "swap phrase/translation" };
			position = new ToolStripLabel { Alignment = ToolStripItemAlignment.Right };

			Menu = new ToolStrip();
			foreach (var button in Buttons) {
				button.Alignment = ToolStripItemAlignment.Right;
				Menu.Items.Add(button);
			}
			Menu.Items.Add(position);

			back.Click += delegate { Raise(Back); };
			fore.Click += delegate { Raise(Forward); };
			end.Click += delegate { Raise(End); };
			edit.Click += delegate { Raise(Edit); };
			if (swapItem)
				swap.Click += delegate { Raise(Swap); };
		}

		protected IEnumerable<ToolStripItem> Buttons {
			get {
				yield return fore;
				yield return back;
				yield return end;
				yield return edit;
				if (swap != null)
					yield return swap;
			}
		}

		public event EventHandler Back;
		public event EventHandler Forward;
		public event EventHandler End;
		public event EventHandler Edit;
		public event EventHandler Swap;

		void Raise(EventHandler handler) {
			if (handler != null)
				handler(this, new EventArgs());
		}

		public void Update() {
			position.Text = string.Format("{0}/{1}", Navigator.Position + 1, Navigator.Length);
		}
	}
}