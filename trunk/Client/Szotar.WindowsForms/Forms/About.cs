using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class About : Form {
		public About() {
			InitializeComponent();

			Text = string.Format(Text, Application.ProductName);
			productName.Text = string.Format(productName.Text, Application.ProductName);
			version.Text = string.Format(version.Text, Application.ProductVersion.ToString());
			webLink.Text = string.Format(webLink.Text, Application.ProductName);

			webLink.LinkClicked += delegate {
				System.Diagnostics.Process.Start("http://code.google.com/p/szotar/");
			};
		}
	}
}
