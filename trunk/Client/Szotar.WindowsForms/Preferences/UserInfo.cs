using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Preferences {
	[PreferencePage("User Info", Parent=typeof(Categories.General))]
	public partial class UserInfo : PreferencePage {
		public UserInfo() {
			InitializeComponent();

			name.Text = GuiConfiguration.UserRealName;
			nickname.Text = GuiConfiguration.UserNickname;
		}

		//TODO: commit properly
		public override void Commit() {
			GuiConfiguration.UserRealName = name.Text;
			GuiConfiguration.UserNickname = nickname.Text;
		}
	}
}
