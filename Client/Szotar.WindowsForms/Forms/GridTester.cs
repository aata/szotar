using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class GridTester : Form {
		public GridTester() {
			InitializeComponent();

			var store = new Sqlite.SqliteDataStore(":memory:");
			components.Add(new DisposableComponent(store));

			menuStrip1.Renderer = toolStrip1.Renderer = new ToolStripAeroRenderer(ToolbarTheme.Toolbar);

			//dgrid.DataSource = new Controls.CustomDictionaryResultsSource(new CustomDictionary("../../../English_Hungarian.wb.custom.txt"));

			var list = store.CreateSet("Test", Environment.UserName, "Klingon", "http://example.org/", DateTime.Now);
			list.Add(new WordListEntry(list, "one", "egy", 0, 0));
			list.Add(new WordListEntry(list, "two", "kettő", 0, 0));
			list.Add(new WordListEntry(list, "three", "három", 0, 0));
			list.Add(new WordListEntry(list, "four", "négy", 0, 0));
			list.Add(new WordListEntry(list, "five", "öt", 0, 0));
			list.Add(new WordListEntry(list, "six", "hat", 0, 0));
			list.Add(new WordListEntry(list, "seven", "hét", 0, 0));
			list.Add(new WordListEntry(list, "eight", "nyolc", 0, 0));
			list.Add(new WordListEntry(list, "nine", "kilenc", 0, 0));
			dgrid.AllowNewItems = true;
			dgrid.DataSource = list;

			propertyGrid.SelectedObject = dgrid;
		}
	}
}