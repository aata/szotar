using System;

namespace Szotar.Quizlet {
    public class TermModel : IJsonConvertible {
        public long TermID { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }

        public Uri ImageUri { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        public TermModel() {
        }

        public TermModel(JsonValue json, IJsonContext context) {
            if (json == null)
                throw new ArgumentNullException("json");
            if (context == null)
                throw new ArgumentNullException("context");

            var dict = JsonDictionary.FromValue(json);
            TermID = context.FromJson<long>(dict["id"]);
            Term = context.FromJson<string>(dict["term"]);
            Definition = context.FromJson<string>(dict["definition"]);
            if (dict.Items.ContainsKey("image") && dict.Items["image"] is JsonDictionary) {
                var imageDict = dict.GetSubDictionary("image");
                ImageWidth = imageDict.Get<int>("width", context);
                ImageHeight = imageDict.Get<int>("height", context);
            }
        }

        public JsonValue ToJson(IJsonContext context) {
            throw new NotImplementedException();
        }
    }
}