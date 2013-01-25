namespace Szotar.Quizlet {
    public class GroupModel : IJsonConvertible {
        public long GroupID { get; set; }

        public GroupModel() {
        }

        public GroupModel GetBasicInfo() {
            return new GroupModel();
        }

        public GroupModel(JsonValue json, IJsonContext context) {
        }

        public JsonValue ToJson(IJsonContext context) {
            return new JsonDictionary();
        }
    }
}