using System;

namespace Szotar.Quizlet {
	public enum SetEditPermissions {
		OnlyMe,
		Groups,
		Password
	}

	public enum SetVisibility {
		Public,
		OnlyMe,
		Groups,
		Password
	}

	public static class SetPermissions {
		public static SetEditPermissions ParseEditPermissions(string str) {
			switch (str) {
				case "groups":
					return SetEditPermissions.Groups;
				case "password":
					return SetEditPermissions.Password;
				default:
					return SetEditPermissions.OnlyMe;
			}
		}

		public static SetVisibility ParseVisibility(string str) {
			switch (str) {
				case "public":
					return SetVisibility.Public;
				case "groups":
					return SetVisibility.Groups;
				case "password":
					return SetVisibility.Password;
				default:
					return SetVisibility.OnlyMe;
			}
		}

		public static string ToApiString(this SetEditPermissions permissions) {
			switch (permissions) {
				case SetEditPermissions.OnlyMe: return "only_me"; 
				case SetEditPermissions.Groups: return "groups";
				case SetEditPermissions.Password: return "password";
				default: throw new ArgumentOutOfRangeException("permissions");
			}
		}

		public static string ToApiString(this SetVisibility visibility) {
			switch (visibility) {
				case SetVisibility.Public: return "public";
				case SetVisibility.OnlyMe: return "only_me";
				case SetVisibility.Groups: return "groups";
				case SetVisibility.Password: return "password";
				default: throw new ArgumentOutOfRangeException("visibility");
			}
		}
	}
}