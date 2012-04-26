using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;

using Szotar.Json;
using System.Threading;

namespace Szotar {
    public class QuizletAPI {
        public Uri Host { get; set; }
        public const string ClientID = "k9VyKJcAXH";

        public QuizletAPI() {
            Host = new Uri("https://api.quizlet.com/2.0/");
        }

        public class SetInfo : IJsonConvertible {
            public long ID { get; set; }
            public Uri Uri { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public int TermCount { get; set; }
            public DateTime Created { get; set; }
            public DateTime Modified { get; set; }
            public List<string> Subjects { get; set; }
            public string Visibility { get; set; }
            public string Editable { get; set; }
            public bool HasAccess { get; set; }

            public SetInfo(JsonValue json, IJsonContext context) {
                var dict = json as JsonDictionary;
                if (dict == null)
                    throw new JsonConvertException("Expected a JSON dictionary");

                foreach (var k in dict.Items) {
                    switch (k.Key) {
                        case "id": ID = context.FromJson<long>(k.Value); break;
                        case "url": Uri = new Uri(context.FromJson<string>(k.Value)); break;
                        case "title": Title = context.FromJson<string>(k.Value); break;
                        case "created_by": Author = context.FromJson<string>(k.Value); break;
                        case "term_count": TermCount = context.FromJson<int>(k.Value); break;
                        case "created_date": Created = new DateTime(1970, 1, 1).AddSeconds(context.FromJson<long>(k.Value)); break;
                        case "modified_date": Modified = new DateTime(1970, 1, 1).AddSeconds(context.FromJson<long>(k.Value)); break;
                        case "subjects": Subjects = context.FromJson<List<string>>(k.Value); break;
                        case "visibility": Visibility = context.FromJson<string>(k.Value); break;
                        case "editable": Editable = context.FromJson<string>(k.Value); break;
                        case "has_access": HasAccess = context.FromJson<bool>(k.Value); break;
                    }
                }
            }

            public JsonValue ToJson(IJsonContext context) {
                throw new NotImplementedException();
            }
        }

        public void SearchSets (string query, Action<List<SetInfo>> completion, Action<Exception> errorHandler, CancellationToken token) {
            var op = System.ComponentModel.AsyncOperationManager.CreateOperation(null);

            try {
                var wr = WebRequest.Create(new Uri(Host, "search/sets?sort=most_studied&client_id=" + Uri.EscapeDataString(ClientID) + "&q=" + Uri.EscapeDataString(query)));

                token.Register(delegate { wr.Abort(); });

                wr.BeginGetResponse(new AsyncCallback(delegate(IAsyncResult result) {
                    Exception error = null;
                    try {
                        token.ThrowIfCancellationRequested();
                        var text = new StreamReader(wr.EndGetResponse(result).GetResponseStream()).ReadToEnd();
                        var json = Json.JsonValue.Parse(new StringReader(text));
                        var sets = new JsonContext().FromJson<List<SetInfo>>(((JsonDictionary)json).Items["sets"]);

                        op.PostOperationCompleted(new SendOrPostCallback(delegate {
                            completion(sets);
                        }), null);
                    } catch (WebException e) {
                        error = e;
                    } catch (JsonConvertException e) {
                        error = e;
                    } catch (InvalidOperationException e) {
                        error = e;
                    } catch (ArgumentException e) {
                        error = e;
                    } catch (OperationCanceledException e) {
                        error = e;
                    }

                    if (error != null) {
                        op.PostOperationCompleted(new SendOrPostCallback(delegate {
                            errorHandler(error);
                        }), null);
                    }
                }), null);
            } catch (WebException e) {
                errorHandler(e);
            } catch (InvalidOperationException e) {
                errorHandler(e);
            } catch (ArgumentException e) {
                errorHandler(e);
            } catch (OperationCanceledException e) {
                errorHandler(e);
            }
        }
    }
}