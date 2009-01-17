using System;
using System.Collections.Generic;

namespace Szotar.WindowsForms {
	public static class GuiConfiguration {
		static IConfiguration Default { 
			get { return Szotar.Configuration.Default; }
		}

		public static void Save() {
			Default.Save();
		}

		public static string StartupAction {
			get { return Default.Get<string>("StartupAction", "StartPage"); }
			set { Default.Set("StartupAction", value); }
		}

		public static string StartupDictionary {
			get { return Default.Get<string>("StartupDictionary", null); }
			set { Default.Set("StartupDictionary", value); }
		}

		public static int ListBuilderMetadataSectionHeight {
			get { return Default.Get<int>("ListBuilderMetadataSectionHeight", 75); }
			set { Default.Set("ListBuilderMetadataSectionHeight", value); }
		}

		public static MruList RecentDictionaries {
			get { return Default.Get<MruList>("RecentDictionaries", null); }
			set { Default.Set("RecentDictionaries", value); }
		}

		public static bool IgnoreAccents {
			get { return Default.Get<bool>("IgnoreAccents", false); }
			set { Default.Set("IgnoreAccents", value); }
		}

		public static bool IgnoreCase {
			get { return Default.Get<bool>("IgnoreCase", true); }
			set { Default.Set("IgnoreCase", value); }
		}

		public static float LookupFormColumn1FillWeight {
			get { return Default.Get<float>("LookupFormColumn1FillWeight", 100.0f); }
			set { Default.Set("LookupFormColumn1FillWeight", value); }
		}

		public static string UserRealName {
			get { return Default.Get<string>("UserRealName", null); }
			set { Default.Set("UserRealName", value); }
		}

		public static string UserNickname {
			get { return Default.Get<string>("UserNickname", null); }
			set { Default.Set("UserNickname", value); }
		}

		public static string ListFontName {
			get { return Default.Get<string>("ListFontName", null); }
			set { Default.Set("ListFontName", value); }
		}

		public static float ListFontSize {
			get { return Default.Get<float>("ListFontSize", 9.0f); }
			set { Default.Set("ListFontSize", value); }
		}

		public static System.Drawing.Font GetListFont() {
			if (ListFontName == null)
				return null;
			return new System.Drawing.Font(ListFontName, ListFontSize);
		}
	}
}