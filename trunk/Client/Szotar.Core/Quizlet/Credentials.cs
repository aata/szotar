using System.Linq;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

namespace Szotar.Quizlet {
    public struct Credentials : IJsonConvertible {
        private string userName, apiToken;
        private DateTime validTo;

        public Credentials(string userName, DateTime validTo, string apiToken) : this() {
            this.userName = userName;
            this.validTo = validTo;
            this.apiToken = apiToken;
        }

        public string UserName {
            get { return userName; }
            set { 
                if (userName != null)
                    throw new InvalidOperationException("API Credentials are immutable once created");
                userName = value;
            }
        }

        public DateTime ValidTo {
            get { return validTo; } 
            set {
                if (validTo != default(DateTime))
                    throw new InvalidOperationException("API credentials are immutable once created");
                validTo = value;
            }
        }
        
        public string ApiToken {
            get { return apiToken; }
            set {
                if (apiToken != null)
                    throw new InvalidOperationException("API credentials are immutable once created");
                apiToken = value;
            }
        }

        public Credentials(JsonValue json, IJsonContext context) : this() {
            var dict = JsonDictionary.FromValue(json);
            UserName = dict.Get<string>("UserName", context);
            ValidTo = dict.Get<long>("ValidTo", context).DateTimeFromUnixTime();
            ApiToken = dict.Get<string>("ApiToken", context);
        }

        public JsonValue ToJson(IJsonContext context) {
            var dict = new JsonDictionary();
            dict["UserName"] = context.ToJson(UserName);
            dict["ValidTo"] = context.ToJson(ValidTo.ToUnixTime());
            dict["ApiToken"] = context.ToJson(ApiToken);
            return dict;
        }
    }
}