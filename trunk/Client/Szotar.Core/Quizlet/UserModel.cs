using System;
using System.Collections.Generic;

namespace Szotar.Quizlet {
    public class UserModel : IJsonConvertible {
        public string UserName { get; set; }
        public string AccountType { get; set; }
        public DateTime SignUpDate { get; set; }
        public Uri ProfileImage { get; set; }

        // Only contains basic set/group info
        public List<SetModel> Sets { get; set; }
        public List<SetModel> FavouriteSets { get; set; }
        public List<GroupModel> Groups { get; set; }

        public int? StudySessionCount;
        public int? MessageCount;
        public int? TotalAnswerCount;
        public int? PublicSetsCreated;
        public int? PublicTermsEntered;

        public UserModel() {
        }

        public UserModel(JsonValue json, IJsonContext context) {
            var dict = JsonDictionary.FromValue(json);

            bool wasRelaxed = context.RelaxedNumericConversion;
            context.RelaxedNumericConversion = true;

            UserName = dict.Get<string>("username", context);
            AccountType = dict.Get<string>("account_type", context, "free");
            SignUpDate = new DateTime(1970,1,1).AddSeconds(dict.Get<double>("sign_up_date", context));

            if (dict.Items.ContainsKey("profile_image"))
                ProfileImage = new Uri(dict.Get<string>("profile_image", context), UriKind.Absolute);

            Sets = dict.Get<List<SetModel>>("sets", context);
            FavouriteSets = dict.Get<List<SetModel>>("favorite_sets", context);
            Groups = dict.Get<List<GroupModel>>("groups", context);

            if (dict.Items.ContainsKey("statistics")) {
                var stats = dict.GetSubDictionary("statistics");
                foreach (var k in stats.Items) {
                    switch (k.Key) {
                        case "study_session_count": StudySessionCount = context.FromJson<int>(k.Value); break;
                        case "message_count": MessageCount = context.FromJson<int>(k.Value); break;
                        case "total_answer_count": TotalAnswerCount = context.FromJson<int>(k.Value); break;
                        case "public_sets_created": PublicSetsCreated = context.FromJson<int>(k.Value); break;
                        case "public_terms_entered": PublicTermsEntered = context.FromJson<int>(k.Value); break;
                    }
                }
            }

            context.RelaxedNumericConversion = wasRelaxed;
        }

        public JsonValue ToJson(IJsonContext context) {
            throw new NotImplementedException();
        }
    }
}