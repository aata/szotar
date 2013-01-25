using System;
using System.Collections.Generic;

namespace Szotar.Quizlet {
    public class SetModel : IJsonConvertible {
        public long SetID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public Uri Uri { get; set; }

        private int termCount;
        public int TermCount {
            get {
                return Terms != null ? Terms.Count : termCount;
            }
            set {
                if (Terms != null)
                    throw new InvalidOperationException("Cannot set SetModel.TermCount when the list of terms is loaded");
                termCount = value;
            }
        }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public List<string> Subjects { get; set; }
        public SetVisibility Visibility { get; set; }
        public SetEditPermissions EditPermissions { get; set; }

        public bool HasAccess { get; set; }
        public bool HasDiscussion { get; set; }

        public string TermLanguageCode { get; set; }
        public string DefinitionLanguageCode { get; set; }

        public List<TermModel> Terms { get; set; }

        public SetModel() {
        }

        public SetModel(JsonValue json, IJsonContext context) {
            var dict = JsonDictionary.FromValue(json);

            SetID = dict.Get<long>("id", context);
            Uri = new Uri(dict.Get<string>("url", context));
            Title = dict.Get<string>("title", context);
            Author = dict.Get<string>("created_by", context);
            Description = dict.Get<string>("description", context, null);
            TermCount = dict.Get<int>("term_count", context);
            Created = new DateTime(1970, 1, 1).AddSeconds(dict.Get<double>("created_date", context));
            Modified = new DateTime(1970, 1, 1).AddSeconds(dict.Get<double>("modified_date", context));
            Subjects = context.FromJson<List<string>>(dict["subjects"]);
            Visibility = SetPermissions.ParseVisibility(dict.Get<string>("visibility", context));
            EditPermissions = SetPermissions.ParseEditPermissions(dict.Get<string>("editable", context));
            HasAccess = dict.Get<bool>("has_access", context);
            HasDiscussion = dict.Get<bool>("has_discussion", context, false);
            TermLanguageCode = dict.Get<string>("lang_terms", context, null);
            DefinitionLanguageCode = dict.Get<string>("lang_definitions", context, null);

            if (dict.Items.ContainsKey("terms"))
                Terms = context.FromJson<List<TermModel>>(dict["terms"]);
        }

        public JsonValue ToJson(IJsonContext context) {
            var dict = new JsonDictionary();
            dict.Set("id", SetID);
            dict.Set("url", Uri.ToString());
            dict.Set("title", Title);
            dict.Set("created_by", Author);
            if (Description != null)
                dict.Set("description",Description);
            dict.Set("term_count", TermCount);
            dict.Set("created_date", Created.ToUnixTime());
            dict.Set("modified_date", Modified.ToUnixTime());
            dict["subjects"] = context.ToJson(Subjects);
            dict.Set("visibility", Visibility.ToApiString());
            dict.Set("editable", EditPermissions.ToApiString());
            dict.Set("has_access", HasAccess);
            dict.Set("has_discussion", HasDiscussion);
            dict.Set("lang_terms", TermLanguageCode);
            dict.Set("lang_definitions", DefinitionLanguageCode);
            if (Terms != null)
                dict["terms"] = context.ToJson(Terms);
            return dict;
        }

        // For serialization into the cache index.
        public SetModel GetBasicInfo() {
            return new SetModel {
                SetID = SetID,
                Author = Author,
                Created = Created,
                DefinitionLanguageCode = DefinitionLanguageCode,
                Description = Description,
                EditPermissions = EditPermissions,
                HasAccess = HasAccess,
                HasDiscussion = HasDiscussion,
                Modified = Modified,
                Subjects = Subjects,
                TermCount = TermCount,
                TermLanguageCode = TermLanguageCode,
                Terms = null,
                Title = Title,
                Uri = Uri, 
                Visibility = Visibility
            };
        }
    }
}