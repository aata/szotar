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
			list.Add(new WordListEntry(list, "one", "egy"));
			list.Add(new WordListEntry(list, "two", "kettő"));
			list.Add(new WordListEntry(list, "three", "három"));
			list.Add(new WordListEntry(list, "four", "négy"));
			list.Add(new WordListEntry(list, "five", "öt"));
			list.Add(new WordListEntry(list, "six", "hat"));
			list.Add(new WordListEntry(list, "seven", "hét"));
			list.Add(new WordListEntry(list, "eight", "nyolc"));
			list.Add(new WordListEntry(list, "nine", "kilenc"));
			dgrid.AllowNewItems = true;
			dgrid.DataSource = list;

			propertyGrid.SelectedObject = dgrid;
		}
	}
}