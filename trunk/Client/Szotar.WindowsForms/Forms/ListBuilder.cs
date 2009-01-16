using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class ListBuilder : Form {
		WordList list;
		BindingList<TranslationPair> editingList;
		bool saved;

		public ListBuilder()
			: this(new WordList()) 
		{
		}

		public ListBuilder(WordList wordList) {
			InitializeComponent();

			this.list = wordList;
			editingList = new BindingList<TranslationPair>(list);
			editingList.AllowNew = true;
			editingList.AllowEdit = true;
			editingList.AllowRemove = true;
			editingList.RaiseListChangedEvents = true;

			grid.DataSource = editingList;
			this.UpdateTitle();
			name.Text = list.Name;
			author.Text = list.Author;
			url.Text = list.Url;

			meta.Height = GuiConfiguration.ListBuilderMetadataSectionHeight;

			editingList.ListChanged += new ListChangedEventHandler(editingList_ListChanged);
			this.Layout += new LayoutEventHandler(ListBuilder_Layout);
			this.Closing += new CancelEventHandler(ListBuilder_Closing);
			editMetadata.Click += new EventHandler(editMetadata_Click);
			copyAsCsv.Click += new EventHandler(copyAsCsv_Click);
			save.Click += new EventHandler(save_Click);
			saveAs.Click += new EventHandler(saveAs_Click);
			sort.Click += new EventHandler(sort_Click);
			swapAll.Click += new EventHandler(swapAll_Click);
			shadow.MouseDown += new MouseEventHandler(shadow_MouseDown);
			shadow.MouseMove += new MouseEventHandler(shadow_MouseMove);
			list.PropertyChanged += new PropertyChangedEventHandler(list_PropertyChanged);
			name.TextChanged += new EventHandler(name_TextChanged);
			author.TextChanged += new EventHandler(author_TextChanged);
			url.TextChanged += new EventHandler(url_TextChanged);

			swap.Click += new EventHandler(swap_Click);
			remove.Click += new EventHandler(remove_Click);

			saved = true;
		}

		void remove_Click(object sender, EventArgs e) {
			foreach (int i in grid.SelectedIndices)
				editingList[i] = null;
			while (editingList.Remove(null))
				;
		}

		void swap_Click(object sender, EventArgs e) {
			foreach (int i in grid.SelectedIndices) {
				TranslationPair p = editingList[i];
				editingList[i] = new TranslationPair(p.Translation, p.Phrase);
			}
		}

		void swapAll_Click(object sender, EventArgs e) {
			//Swap whole list
			for (int i = 0; i < editingList.Count; ++i) {
				TranslationPair p = editingList[i];
				editingList[i] = new TranslationPair(p.Translation, p.Phrase);
			}	
		}

		void sort_Click(object sender, EventArgs e) {
			List<TranslationPair> items = new List<TranslationPair>(editingList);
			items.Sort();
			editingList.RaiseListChangedEvents = false;
			editingList.Clear();
			foreach (TranslationPair pair in items)
				editingList.Add(pair);
			editingList.RaiseListChangedEvents = true;
			editingList.ResetBindings();
		}

		private void UpdateTitle() {
			Text = string.Format("{0} - {1}", list.Name, Application.ProductName);
		}

		#region Metadata Bindings
		void url_TextChanged(object sender, EventArgs e) {
			list.Url = url.Text;
		}

		void author_TextChanged(object sender, EventArgs e) {
			list.Author = author.Text;
		}

		void name_TextChanged(object sender, EventArgs e) {
			list.Name = name.Text;
		}

		void list_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch (e.PropertyName) {
				case "Name":
					if(!name.Text.Equals(list.Name))
						name.Text = list.Name;
					this.UpdateTitle();
					break;
				case "Author":
					if(!author.Text.Equals(list.Author))
						author.Text = list.Author;
					break;
				case "Url":
					if (!url.Text.Equals(list.Url))
						url.Text = list.Url;
					break;
			}
		}
		#endregion

		public class ShadowTag {
			public System.Drawing.Point Down { get; set; }
			public int OriginalHeight { get; set; }
		}

		private void shadow_MouseMove(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				ShadowTag tag = (ShadowTag)shadow.Tag;
				System.Drawing.Point down = tag.Down;
				int delta = down.Y - e.Y;
				meta.Height += delta;
				UpdateGridHeight();
				GuiConfiguration.ListBuilderMetadataSectionHeight = meta.Height;
			}
		}

		private void shadow_MouseDown(object sender, MouseEventArgs e) {
			shadow.Tag = new ShadowTag { Down = e.Location, OriginalHeight = shadow.Height };
		}

		private void saveAs_Click(object sender, EventArgs e) {
			SaveAs();
		}

		private void save_Click(object sender, EventArgs e) {
			if (list.HasPath)
				SaveTo(list.Path);
			else
				SaveAs();
		}

		private void ListBuilder_Closing(object sender, CancelEventArgs e) {
			if (!saved) {
				if (list.HasPath) {
					list.Save();
				} else {
					//Choosing Yes as the default based on what Notepad does.
					DialogResult result = MessageBox.Show(Properties.Resources.WouldYouLikeToSaveThisList, Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
					if (result == DialogResult.Cancel) {
						e.Cancel = true;
						return;
					} else if (result == DialogResult.Yes) {
						list.Save();
					}
				}
			}

			list.PropertyChanged += new PropertyChangedEventHandler(list_PropertyChanged);
		}

		private void SaveAs() {
			if (DialogResult.OK == saveFileDialog.ShowDialog()) {
				if (SaveTo(saveFileDialog.FileName)) {
					saved = true;

					List<ListInfo> recentLists =
						GuiConfiguration.RecentLists;
					if (recentLists == null)
						recentLists = new List<ListInfo>();

					list.Path = saveFileDialog.FileName;

					recentLists.Insert(0, list.GetInfo());

					int max = GuiConfiguration.RecentListsSize;
					if (recentLists.Count >= max)
						recentLists.RemoveAt(max - 1);

					GuiConfiguration.RecentLists = recentLists;
					GuiConfiguration.Save();
				}
			}
		}

		private bool SaveTo(string path) {
			try {
				list.Save(path);
				return true;
			} catch (IOException e) {
				string msg = String.Format(CultureInfo.CurrentUICulture, Resources.Errors.CouldNotSaveListMessage, path, "\n\n", e.Message);
				RtlAwareMessageBox.Show(this, msg, Resources.Errors.CouldNotSaveListCaption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return false;
			}
		}

		private void editMetadata_Click(object sender, EventArgs e) {
			editMetadata.Checked = !editMetadata.Checked;
			if (editMetadata.Checked)
				ShowMeta();
			else
				HideMeta();
		}

		private void copyAsCsv_Click(object sender, EventArgs e) {
			var sb = new StringBuilder();

			foreach (TranslationPair pair in editingList) {
				bool quotePhrase = pair.Phrase.Contains("\"") || pair.Phrase.Contains(",") || pair.Phrase.Contains("\n"),
					quoteTranslation = pair.Translation.Contains("\"") || pair.Translation.Contains(",") || pair.Translation.Contains("\n");
				if (quotePhrase)
					sb.Append('"');
				sb.Append(pair.Phrase.Replace("\"", "\"\""));
				if (quotePhrase)
					sb.Append('"');

				sb.Append(',');

				if (quoteTranslation)
					sb.Append('"');
				sb.Append(pair.Translation.Replace("\"", "\"\""));
				if (quoteTranslation)
					sb.Append('"');

				sb.AppendLine();
			}

			if(sb.Length > 0)
				Clipboard.SetText(sb.ToString());
		}

		private void ListBuilder_Layout(object sender, LayoutEventArgs e) {
			grid.Top = mainMenu.Bottom;
			UpdateGridHeight();
			grid.Width = ClientSize.Width;
		}

		private void UpdateGridHeight() {
			grid.Height = ClientSize.Height - grid.Top - (meta.Visible ? meta.Height : 0);
		}

		private void editingList_ListChanged(object sender, ListChangedEventArgs e) {
			this.saved = false;
		}

		//Metadata
		private void ShowMeta() {
			meta.Visible = true;
			grid.Height -= meta.Height;
		}

		private void HideMeta() {
			meta.Visible = false;
			grid.Height += meta.Height;
		}

		//Public methods
		public void AddPair(string phrase, string translation) {
			System.Diagnostics.Debug.Assert(!InvokeRequired);

			editingList.Add(new TranslationPair(phrase, translation, true));
			grid.DataSource = editingList;
		}

		public void AddPair(TranslationPair pair) {
			System.Diagnostics.Debug.Assert(!InvokeRequired);
			System.Diagnostics.Debug.Assert(pair.Mutable);

			editingList.Add(pair);
			grid.DataSource = editingList;
		}

		public bool ShowMetadata {
			get {
				return meta.Visible;
			}
			set {
				if (value != ShowMetadata) {
					if (value)
						ShowMeta();
					else
						HideMeta();
				}
			}
		}
	}
}
