using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;

using Szotar.Json;
using System.Threading;
using System.Globalization;

namespace Szotar {
    public class QuizletAPI {
        public Uri Host { get; set; }
        public const string ClientID = "k9VyKJcAXH";

        public QuizletAPI() {
            Host = new Uri("https://api.quizlet.com/2.0/");
        }

        [Serializable]
        public class QuizletException : Exception {
            public QuizletException() { }
            public QuizletException(string message) : base(message) { }
            public QuizletException(string message, Exception inner) : base(message, inner) { }
            protected QuizletException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }

        [Serializable]
        public class SetNotFoundException : QuizletException {
            public SetNotFoundException() { }
            public SetNotFoundException(string message) : base(message) { }
            public SetNotFoundException(string message, Exception inner) : base(message, inner) { }
            protected SetNotFoundException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
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
            public List<TranslationPair> Terms { get; set; }

            public SetInfo(JsonValue json, IJsonContext context) {
                var dict = json as JsonDictionary;
                if (dict == null)
                    throw new JsonConvertException("Expected a JSON dictionary to be converted to a SetInfo");

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
                        case "terms":
                            var list = new List<TranslationPair>();
                            if (k.Value is JsonArray) {
                                foreach (var term in ((JsonArray)k.Value).Items) {
                                    if (!(term is JsonDictionary))
                                        throw new JsonConvertException("Expected SetInfo.Terms to be an array of JSON dictionaries");
                                    TranslationPair pair = new TranslationPair();
                                    pair.Phrase = context.FromJson<string>(((JsonDictionary)term).Items["term"]);
                                    pair.Translation = context.FromJson<string>(((JsonDictionary)term).Items["definition"]);
                                    if (pair.Phrase == null || pair.Translation == null)
                                        throw new JsonConvertException("Either term or definition was not set when convering from JSON to SetInfo");
                                    list.Add(pair);
                                }
                            } else {
                                throw new JsonConvertException("Expected SetInfo.Terms to be an array");
                            }
                            Terms = list;
                            break;
                    }
                }
            }

            public JsonValue ToJson(IJsonContext context) {
                throw new NotImplementedException();
            }
        }

        protected void FetchJSON(Uri uri, Action<JsonValue> completion, Action<Exception> errorHandler, CancellationToken token) {
            var op = System.ComponentModel.AsyncOperationManager.CreateOperation(null);
            try {
                var wr = WebRequest.Create(uri);
                token.Register(delegate { wr.Abort(); });

                wr.BeginGetResponse(new AsyncCallback(delegate(IAsyncResult result) {
                    Exception error = null;

                    try {
                        token.ThrowIfCancellationRequested();
                        HttpWebResponse response;
                        try {
                            response = (HttpWebResponse)wr.EndGetResponse(result);
                        } catch (WebException e) {
                            response = (HttpWebResponse)e.Response;
                        }

                        var text = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        var json = JsonValue.Parse(new StringReader(text));

                        if (response.StatusCode != HttpStatusCode.OK) {
                            try {
                                var dict = (JsonDictionary)json;
                                string errorCode = new JsonContext().FromJson<string>(dict.Items["error"]);
                                string errorText = new JsonContext().FromJson<string>(dict.Items["error_description"]);
                                if (errorCode != null && errorText != null) {
                                    if (errorCode == "item_not_found")
                                        throw new SetNotFoundException(errorText);
                                    else
                                        throw new QuizletException(errorText);
                                }
                                throw new QuizletException("The Quizlet server returned an invalid document.");
                            } catch (KeyNotFoundException e) {
                                throw new QuizletException("The Quizlet server returned an invalid document.", e);
                            } catch (InvalidCastException e) {
                                throw new QuizletException("The Quizlet server returned an invalid document.", e);
                            } catch (JsonConvertException e) {
                                throw new QuizletException("The Quizlet server returned an invalid document.", e);
                            }
                        } 

                        op.PostOperationCompleted(new SendOrPostCallback(delegate {
                            completion(json);
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
                    } catch (QuizletException e) {
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
            } catch (FormatException e) {
                errorHandler(e);
            } catch (QuizletException e) {
                errorHandler(e);
            }
        }

        public void GetSetInfo(long setID, Action<SetInfo> completion, Action<Exception> errorHandler, CancellationToken token) {
            FetchJSON(
                new Uri(Host, "sets/" + setID.ToString(CultureInfo.InvariantCulture) + "?client_id=" + Uri.EscapeDataString(ClientID)),
                json => {
                    try {
                        var set = new JsonContext().FromJson<SetInfo>(json);
                        completion(set);
                    } catch (JsonConvertException e) {
                        errorHandler(e);
                    }
                },
                errorHandler,
                token);
        }

        public void SearchSets (string query, Action<List<SetInfo>> completion, Action<Exception> errorHandler, CancellationToken token) {
            FetchJSON(
                new Uri(Host, "search/sets?sort=most_studied&client_id=" + Uri.EscapeDataString(ClientID) + "&q=" + Uri.EscapeDataString(query)),
                json => {
                    try {
                        var sets = new JsonContext().FromJson<List<SetInfo>>(((JsonDictionary)json).Items["sets"]);
                        completion(sets);
                    } catch (JsonConvertException e) {
                        errorHandler(e);
                    }
                },
                errorHandler,
                token);
        }
    }
}