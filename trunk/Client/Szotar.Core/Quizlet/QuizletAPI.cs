using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Szotar.Quizlet {
    public partial class QuizletApi : IQuizletService {
		public QuizletApi() {
			ServiceUri = new Uri("https://api.quizlet.com/2.0/");

            if (Configuration.Default.Get<string>("UserName") != null)
                Credentials = new Credentials(
                    Configuration.Default.Get("UserName", ""),
                    Configuration.Default.Get("AccessTokenExpriry", DateTime.MaxValue), 
                    Configuration.Default.Get("AccessToken", ""));
        }

        public Credentials? Credentials { get; private set; }
		public Uri ServiceUri { get; set; }

        protected async Task<JsonValue> Fetch(
            Uri endPoint, 
            RequestType requestType, 
            bool needsToken, 
            CancellationToken cancel, 
            IProgress<ProgressChangedEventArgs> progress = null, 
            IDictionary<string, string> postData = null,
            KeyValuePair<string,string>? overrideAuth = null)
        {
            
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");
            
            var wc = new WebClient();
            if (needsToken && !Credentials.HasValue)
                throw new InvalidOperationException("Cannot access this part of the Quizlet service without logging in.");

            wc.Headers["User-Agent"] = "FlashcardsWP8/0.1 (" + Environment.OSVersion + "; .NET " + Environment.Version + ")";

            // Overriding is used for basic authentication for OAuth process.
            bool useClientID = false;
            if (overrideAuth != null)
                wc.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(overrideAuth.Value.Key + ":" + overrideAuth.Value.Value));
            else if (Credentials != null)
                wc.Headers["Authorization"] = "Bearer " + Credentials.Value.ApiToken;
            else
                useClientID = true;

			try {
                string result;
                if (requestType == RequestType.Get) {
                    var uri = new Uri(ServiceUri, endPoint);
                    uri = new Uri(uri + (uri.Query == "" ? "?" : "&") + "client_id=" + clientID);
				    result = await wc.DownloadStringAsTask(uri, cancel, progress);
                } else {
                    var sb = new StringBuilder();
                    wc.Headers["Content-Type"] = "application/x-www-form-urlencoded; charset=UTF-8";
                    wc.Encoding = Encoding.UTF8;
                    if (postData != null) {
                        if (useClientID)
                            postData["client_id"] = clientID;
                        foreach (var kvp in postData) {
                            if (sb.Length > 0)
                                sb.Append('&');
                            sb.Append(kvp.Key).Append('=').Append(Uri.EscapeDataString(kvp.Value));
                        }
                    }

                    result = await wc.UploadStringAsTask(new Uri(ServiceUri, endPoint), requestType.ToString().ToUpperInvariant(), sb.ToString(), cancel, progress);
                }

				JsonValue json = null;

                // In case of HTTP status 204 (no content). This is produced, for example, as the result of 
                // a DELETE call to an endpoint.
				if (!string.IsNullOrWhiteSpace(result))
					json = JsonValue.Parse(new StringReader(result));

				return json;
            } catch (WebException e) {
				// The Quizlet API returns a JSON document explaining the error if there is one,
                // so try to use that if possible.

				try {
                    if (e.Response != null) {
                        var response = (e.Response as HttpWebResponse).GetResponseStream();
                        using (var reader = new StreamReader(response)) {
                            var dict = JsonDictionary.FromValue(JsonValue.Parse(reader));
                            var ctx = new JsonContext();
                            var errorCode = ctx.FromJson<string>(dict["error"]);
                            var errorText = ctx.FromJson<string>(dict["error_description"]);

                            if (errorCode != null && errorText != null) {
                                switch (errorCode) {
                                    case "invalid_access": throw new AccessDeniedException(errorText);
                                    case "item_not_found": throw new ItemNotFoundException(errorText);
                                    case "item_deleted": throw new ItemDeletedException(errorText);
                                    default: throw new QuizletException(errorText);
                                }
                            }
                        }
                    }

                    throw new QuizletException(string.Format("Unable to contact the Quizlet server ({0}).", e.Message), e);
                } catch (FormatException) {
					// Not JSON or invalid - just wrap the original exception
					throw new QuizletException(e.Message, e);
                } catch (KeyNotFoundException) {
					throw new QuizletException(e.Message, e);
                }
            }
        }

        public async Task<Credentials> Authenticate(string code, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null) {
            var fields = new Dictionary<string, string>();
            fields["grant_type"] = "authorization_code";
            fields["code"] = code;
            fields["redirect_uri"] = "flashcards://complete-oauth";

            var basicAuth = new KeyValuePair<string, string>(clientID, secretKey);

            var json = await Fetch(new Uri("/oauth/token", UriKind.Relative), RequestType.Post, false, cancel, progress, fields, basicAuth);
            var dict = JsonDictionary.FromValue(json);
            var ctx = new JsonContext();
            
            // TODO Should probably check token_type and scope properties to check we have the right type of token!
            return new Credentials(
                ctx.FromJson<string>(dict["user_id"]),
                DateTime.UtcNow.AddSeconds(ctx.FromJson<double>(dict["expires_in"])),
                ctx.FromJson<string>(dict["access_token"]));
        }

        public void Authenticate(Credentials credentials) {
            Credentials = credentials;
        }

        public Uri GetLoginPageUri(out string randomState) {
            randomState = Guid.NewGuid().ToString();
            return new Uri("https://quizlet.com/authorize/?response_type=code&client_id=" + clientID + "&scope=read write_set write_group&state=" + randomState);
        }

        protected Uri EndPoint(string uriString, params object[] values) {
            return new Uri(string.Format(uriString, values), UriKind.Relative);
        }

        public async Task<SetModel> FetchSetInfo(long setID, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null) {
            var ep = EndPoint("sets/{0}", setID);
            var json = await Fetch(ep, RequestType.Get, false, cancel, progress);
            return new JsonContext().FromJson<SetModel>(json);
        }

        public async Task<List<SetModel>> FetchSetInfo(IEnumerable<long> setID, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null) {
            var ep = EndPoint("sets/?set_ids={0}", string.Join(",", setID));
            var json = await Fetch(ep, RequestType.Get, false, cancel, progress);
            return new JsonContext().FromJson<List<SetModel>>(json);
        }

        public async Task<List<SetModel>> FetchUserSets(string userName, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null) {
            var ep = EndPoint("users/{0}/sets", userName);
            var json = await Fetch(ep, RequestType.Get, false, cancel, progress);
            return new JsonContext().FromJson<List<SetModel>>(json);
        }

        public async Task<List<SetModel>> SearchSets(string query, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null) {
            var ep = EndPoint("search/sets?q={0}&per_page=50", Uri.EscapeDataString(query));
            var json = await Fetch(ep, RequestType.Get, false, cancel, progress);
            return JsonDictionary.FromValue(json).Get<List<SetModel>>("sets", new JsonContext());
        }

        public async Task<UserModel> FetchUserInfo(string userName, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null) {
            var ep = EndPoint("users/{0}", Uri.EscapeDataString(userName));
            var json = await Fetch(ep, RequestType.Get, false, cancel, progress);
            return new JsonContext().FromJson<UserModel>(json);
        }

        protected enum RequestType {
            Get,
            Post,
            Put,
            Delete
        }
    }
}
