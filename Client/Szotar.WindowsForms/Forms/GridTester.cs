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

			//dgrid.DataSource = new Controls.CustomDictionaryResultsSource(new CustomDictionary("../../../English_Hungarian.wb.custom.txt"));
			BindingList<TranslationPair> list = new BindingList<TranslationPair>();
			list.Add(new TranslationPair("one", "egy", true));
			list.Add(new TranslationPair("two", "kettő", true));
			list.Add(new TranslationPair("three", "három", true));
			list.Add(new TranslationPair("four", "négy", true));
			list.Add(new TranslationPair("five", "öt", true));
			list.Add(new TranslationPair("six", "hat", true));
			list.Add(new TranslationPair("seven", "hét", true));
			list.Add(new TranslationPair("eight", "nyolc", true));
			list.Add(new TranslationPair("nine", "kilenc", true));
			dgrid.AllowNewItems = true;
			dgrid.DataSource = list;

			propertyGrid.SelectedObject = dgrid;
		}
	}
}