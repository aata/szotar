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

		public static MruList<DictionaryInfo> RecentDictionaries {
			get { return Default.Get<MruList<DictionaryInfo>>("RecentDictionaries", null); }
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

		public static float ListBuilderColumnRatio {
			get { return Default.Get<float>("ListBuilderColumnRatio", 0.5f); }
			set { Default.Set("ListBuilderColumnRatio", value); }
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

		public static bool LogViewerShowMetrics {
			get { return Default.Get<bool>("LogViewerShowMetrics", true); }
			set { Default.Set("LogViewerShowMetrics", value); }
		}

		public static bool LogViewerShowDebug {
			get { return Default.Get<bool>("LogViewerShowDebug", true); }
			set { Default.Set("LogViewerShowDebug", value); }
		}

		public static bool LogViewerShowWarnings {
			get { return Default.Get<bool>("LogViewerShowWarnings", true); }
			set { Default.Set("LogViewerShowWarnings", value); }
		}

		public static bool LogViewerShowErrors {
			get { return Default.Get<bool>("LogViewerShowErrors", true); }
			set { Default.Set("LogViewerShowErrors", value); }
		}

		// This makes it easier to change the default if necessary. (See OptionsMenu in Learn.cs, for example.)
		public static bool GetPracticeFixupSetting(string setting) {
			return Default.Get<bool>(setting, true);
		}

		public static void SetPracticeFixupSetting(string setting, bool value) {
			Default.Set(setting, value);
		}

		public static bool PracticeFixSpaces {
			get { return Default.Get<bool>("PracticeFixSpaces", true); }
			set { Default.Set("PracticeFixSpaces", value); }
		}

		public static bool PracticeFixPunctuation {
			get { return Default.Get<bool>("PracticeFixPunctuation", true); }
			set { Default.Set("PracticeFixPunctuation", value); }
		}

		public static bool PracticeFixParentheses {
			get { return Default.Get<bool>("PracticeFixParentheses", true); }
			set { Default.Set("PracticeFixParentheses", value); }
		}

		public static bool PracticeFixCase {
			get { return Default.Get<bool>("PracticeFixCase", true); }
			set { Default.Set("PracticeFixCase", value); }
		}

		public static int PracticeDefaultCount {
			get { return Default.Get<int>("PracticeDefaultCount", 20); }
			set { Default.Set("PracticeDefaultCount", value); }
		}

		public static string UiLanguage {
			get { return Default.Get<string>("UiLanguage", ""); }
			set { Default.Set("UiLanguage", value); }
		}
	}
}