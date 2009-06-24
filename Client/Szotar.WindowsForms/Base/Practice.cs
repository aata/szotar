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

	public interface IPracticeOverseer {
		void MarkSuccess(PracticeItem item);
		void MarkFailure(PracticeItem item);

		PracticeItem FetchNextItem();
	}

	public interface IPracticeMode : IDisposable {
		void Start(Panel panel, IPracticeOverseer owner);
		void Stop();
		string Name { get; }
	}

	public class Mode : IPracticeMode, IDisposable {
		public string Name { get; protected set; }

		public IPracticeOverseer Owner { get; private set; }
		public Panel Panel { get; private set; }

		public virtual void Start(Panel panel, IPracticeOverseer owner) {
			Owner = owner;
			Panel = panel;
		}

		public virtual void Stop() {
		}

		public void Dispose() {
			Dispose(true);

			Panel.Controls.Clear();
		}

		protected virtual void Dispose(bool disposing) {
			GC.SuppressFinalize(this);
		}

		~Mode() {
			Dispose(false);
		}
	}

	public class DefaultMode : Mode {
		public void Start(Panel panel, IPracticeOverseer owner) {
			base.Start(panel, owner);
		}

		public void Stop() {
			base.Stop();
		}
	}

	public class Navigator {
		Stack<PracticeItem>
			back = new Stack<PracticeItem>(),
			fore = new Stack<PracticeItem>();

		public IPracticeOverseer Source { get; private set; }

		public Navigator(IPracticeOverseer source) {
			Source = source;

			CurrentItem = source.FetchNextItem();
		}

		/// <invariant>CurrentItem != null</invariant>
		public PracticeItem CurrentItem { get; private set; }

		public void AdvanceToEnd() {
			back.Push(CurrentItem);

			while (fore.Count > 0)
				back.Push(fore.Pop());

			CurrentItem = Source.FetchNextItem();
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

	public class FlashcardMode : Mode {
		Control phraseLabel, translationLabel;
		Navigator nav;

		Font bigFont, smallFont;

		public override void Start(Panel panel, IPracticeOverseer owner) {
			base.Start(panel, owner);

			Name = "Flashcards";

			nav = new Navigator(owner);

			phraseLabel = new Label();
			translationLabel = new Label();
			translationLabel.ForeColor = System.Drawing.SystemColors.GrayText;

			bigFont = new System.Drawing.Font(panel.FindForm().Font.FontFamily, 24);
			smallFont = new System.Drawing.Font(panel.FindForm().Font.FontFamily, 16);

			foreach (var l in new [] { phraseLabel, translationLabel }) {
				l.AutoSize = true;
				l.Font = bigFont;
				l.BackColor = Color.Transparent;
				panel.Controls.Add(l);
			}

			Panel.Resize += new EventHandler(Panel_Resize);

			foreach(Control c in new Control[]{ phraseLabel, translationLabel, Panel }) {
				c.MouseUp += new MouseEventHandler(Panel_MouseUp);
			}

			Panel.Controls.Add(new NavigatorControl());

			translationLabel.Visible = false;
			Update();
			Layout();
		}

		public override void Stop() {
			base.Stop();

			Panel.Resize -= new EventHandler(Panel_Resize);

			foreach (Control c in new Control[] { phraseLabel, translationLabel, Panel }) {
				c.MouseUp -= new MouseEventHandler(Panel_MouseUp);
			}
		}

		void Panel_MouseUp(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left || e.Button == MouseButtons.XButton2) {
				if (translationLabel.Visible) {
					nav.Advance();
					translationLabel.Visible = false;
				} else {
					translationLabel.Visible = true;
				}
			} else if (e.Button == MouseButtons.Right || e.Button == MouseButtons.XButton1) {
				if (translationLabel.Visible) {
					translationLabel.Visible = false;
				} else {
					if (!nav.Retreat()) {
						// TODO: Show a message saying the start of the stream has been reached.
					} else {
						translationLabel.Visible = true;
					}
				}
			}

			Update();
			Layout();
		}

		void Panel_Resize(object sender, EventArgs e) {
			Layout();
		}

		void Update() {
			phraseLabel.Text = nav.CurrentItem.Phrase;
			translationLabel.Text = nav.CurrentItem.Translation;
		}

		void Layout() {
			// Choose a smaller font if it doesn't fit
			foreach (var label in new [] { phraseLabel, translationLabel }) {
				if (label.Font == bigFont && label.Width > Panel.Width) {
					label.Font = smallFont;
				} else if (label.Font == smallFont) {
					using (Graphics g = Panel.CreateGraphics()) {
						if (g.MeasureString(label.Text, bigFont).Width < Panel.Width)
							label.Font = bigFont;
					}
				}
			}

			var height = phraseLabel.Height + translationLabel.Height;
			
			var px = (Panel.Width - phraseLabel.Width) / 2;
			var tx = (Panel.Width - translationLabel.Width) / 2;

			var py = (Panel.Height - height) / 2;
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

	public class NavigatorControl : UserControl {
		Button back, fore, end, edit;

		public EventHandler Back, Forward, SkipToEnd, Edit;

		public NavigatorControl() {
			back = new Button { Text = "←" };
			fore = new Button { Text = "→" };
			end = new Button { Text = "end" };
			edit = new Button { Text = "edit" };

			int margin = 3;

			BackColor = Color.Transparent;
			Controls.Add(back);
			Controls.Add(fore);
			Controls.Add(end);
			Controls.Add(edit);

			foreach (Button b in Controls) { 
				b.AutoSize = true;
				b.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			}

			Anchor = AnchorStyles.Right | AnchorStyles.Top;

			this.ParentChanged += delegate {
				if (Parent == null)
					return;

				Font = Parent.Font;

				Width = margin * 5 + back.Width + fore.Width + end.Width + edit.Width;
				Height = Math.Max(back.Height, Math.Max(fore.Height, Math.Max(end.Height, edit.Height))) + margin * 2; 

				Left = Parent.ClientSize.Width - Width;
				Top = 0;

				back.Left = margin;
				fore.Left = back.Right + margin;
				end.Left = fore.Right + margin;
				edit.Left = end.Right + margin;
			};
		}
	}
}