using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Preferences {
	[PreferencePage("Practice", Parent = typeof(Categories.General), Importance = 5)]
	public partial class Practice : PreferencePage {
		public Practice() {
			InitializeComponent();

			itemCount.Value = GuiConfiguration.PracticeDefaultCount;
		}

		public override void Commit() {
			GuiConfiguration.PracticeDefaultCount = (int)itemCount.Value;
		}
	}
}
