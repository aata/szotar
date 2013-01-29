using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Szotar {
    /// <summary>
    /// An object which can be converted to JSON or constructed from a JSON value.
    /// </summary>
    /// <remarks>
    /// The construction from a JSON value is implemented by a constructor, which C#'s
    /// interfaces cannot express. However, that constructor must be present, or the 
    /// system will fail.
    /// </remarks>
    public interface IJsonConvertible {
        /// <summary>
        /// Converts the object to a JSON value.
        /// </summary>
        /// <param name="context">The context in which to convert (the group of system converters).</param>
        /// <remarks>This should not throw exceptions, but it may happen.</remarks>
        JsonValue ToJson(IJsonContext context);
    }

    /// <summary>
    /// Capable of converting everything into or from a JSON object, provided it knows
    /// what type of object it is dealing with.
    /// </summary>
    /// <remarks>
    /// The JSON context is an abstraction of the group of system converters employed by the 
    /// JsonConfiguration class to implement IJsonConvertible for system types.
    /// </remarks>
    public interface IJsonContext {
        /// <summary>
        /// Convert some .NET object to JSON.
        /// </summary>
        /// <exception cref="JsonConvertException">The object could not be converted to JSON, perhaps because no converter was found.</exception>
        /// <exception cref="InvalidCastException">The passed object was of the wrong type for the converter which was found (probably an internal error).</exception>
        JsonValue ToJson(object value);

        /// <summary>
        /// Convert a JSON value to a specific type of .NET object.
        /// </summary>
        /// <exception cref="JsonConvertException">The object could not be converted to JSON, perhaps because no converter was found.</exception>
        /// <exception cref="InvalidCastException">The passed object was of the wrong type for the converter which was found.</exception>
        T FromJson<T>(JsonValue json);

        bool RelaxedNumericConversion { get; set; }
    }

    /// <summary>
    /// A specific converter than only converts to or from one .NET type.
    /// </summary>
    public interface IJsonConverter {
        /// <summary>
        /// Converts an object to a JSON value.
        /// </summary>
        /// <param name="value">The object to be converted.</param>
        /// <param name="context">The context in which to convert (the group of system converters).</param>
        /// <exception cref="JsonConvertException">The object could not be converted to JSON, perhaps because it was of the wrong type.</exception>
        /// <exception cref="InvalidCastException">The passed object was of the wrong type.</exception>
        JsonValue ToJson(object value, IJsonContext context);

        /// <summary>
        /// Converts a JSON value to a specific type of JSON object.
        /// </summary>
        /// <param name="json">The json object to convert from.</param>
        /// <param name="context">The context in which to convert (the group of system converters).</param>
        /// <exception cref="JsonConvertException">The object could not be converted to JSON, perhaps because it was the wrong type of JSON primitive.</exception>
        /// <exception cref="InvalidCastException">The passed object was of the wrong type.</exception>
        object FromJson(JsonValue json, IJsonContext context);
    }

    public class JsonConvertException : FormatException {
        public JsonConvertException() { }
        public JsonConvertException(string message) : base(message) { }
        public JsonConvertException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Converts objects to and from JSON using a group of converters for System types.
    /// </summary>
    /// <remarks>
    /// Converters for system types (which are not modifiable by the program) cannot be created by
    /// implementing IJsonConvertible, so we must implement a system of handlers for system types.
    /// It's a shame C# doesn't have type classes.
    /// 
    /// There are converters for: bool, int, double, float, long, short and string. There is also
    /// a special case for List&lt;T&gt;. It would be nice to have instances for all IList&lt;T&gt;-
    /// and IDictionary&lt;T&gt;-derived classes (obviously, in these cases, the class would have to 
    /// be default-constructible).</remarks>
    public class JsonContext : IJsonContext {
        protected readonly Dictionary<Type, IJsonConverter> converters;

        public JsonContext() {
            RelaxedNumericConversion = false;
            converters = new Dictionary<Type, IJsonConverter> {
				{ typeof (string), new JsonStringConverter() },
				{ typeof (double), new JsonFloatingConverter<double>() },
				{ typeof (float), new JsonFloatingConverter<float>() },
				{ typeof (int), new JsonIntegralConverter<int>() },
				{ typeof (long), new JsonIntegralConverter<long>() },
				{ typeof (short), new JsonIntegralConverter<short>() },
				{ typeof (byte), new JsonIntegralConverter<byte>() },
				{ typeof (bool), new JsonBoolConverter() },
				{ typeof (DateTime), new JsonDateTimeConverter() }
			};
        }

        public JsonValue ToJson(object value) {
            if (value == null)
                return null;

            return GetConverter(value.GetType()).ToJson(value, this);
        }

        public T FromJson<T>(JsonValue json) {
            if (json == null) {
                if (typeof(T).IsValueType)
                    throw new JsonConvertException("Tried to convert a null value from JSON.");
                return (T)(object)null;
            }

            return (T)GetConverter<T>().FromJson(json, this);
        }

        protected IJsonConverter GetConverter<T>() {
            return GetConverter(typeof(T));
        }

        public bool RelaxedNumericConversion { get; set; }

        // Converts an object to and from JSON by using its IJsonConvertible instance.
        class ReflectionConverter : IJsonConverter {
            readonly Type type;

            public ReflectionConverter(Type type) {
                // It is not known yet if the type even implements IJsonConvertible.
                // If it does not, a conversion exception will be thrown.
                this.type = type;
            }

            public JsonValue ToJson(object value, IJsonContext context) {
                if (value == null)
                    return null;

                var c = value as IJsonConvertible;
                if (c != null)
                    return c.ToJson(context);

                throw new JsonConvertException("There is no converter registered for the type " + type.Namespace + "." + type.Name + " and it does not implement IJsonConvertible");
            }

            public object FromJson(JsonValue json, IJsonContext context) {
                var c = type.GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[] { typeof(JsonValue), typeof(IJsonContext) },
                    null
                    );

                if (c != null)
                    return c.Invoke(new object[] { json, context });

                throw new JsonConvertException("There is no converter registered for the type " + type.Namespace + "." + type.Name + " and it does not have a constructor taking a JsonValue and an IJsonContext");
            }
        }

        protected IJsonConverter GetConverter(Type type) {
            IJsonConverter converter;
            if (converters.TryGetValue(type, out converter))
                return converter;

            // A special case for List<T>.
            if (type.IsGenericType) {
                if (type.GetGenericTypeDefinition() == typeof(List<>)) {
                    var elemT = type.GetGenericArguments()[0];
                    var convT = typeof(JsonListConverter<>).MakeGenericType(elemT);
                    return (IJsonConverter)Activator.CreateInstance(convT);
                }
            }

            return new ReflectionConverter(type);
        }

        public class JsonBoolConverter : IJsonConverter {
            public JsonValue ToJson(object value, IJsonContext context) {
                return new JsonBool(Convert.ToBoolean(value, CultureInfo.InvariantCulture));
            }

            public object FromJson(JsonValue value, IJsonContext context) {
                var b = value as JsonBool;
                if (b != null)
                    return b.Value;

                throw new JsonConvertException("Value was not a boolean");
            }
        }

        public class JsonDateTimeConverter : IJsonConverter {
            public JsonValue ToJson(object value, IJsonContext context) {
                if (value is DateTime) {
                    var d = (DateTime)value;
                    return new JsonString(d.ToString(CultureInfo.InvariantCulture));
                }
                throw new JsonConvertException("Value was not a DateTime");
            }

            public object FromJson(JsonValue value, IJsonContext context) {
                var b = value as JsonString;
                if (b != null)
                    return Convert.ToDateTime(b.Value, CultureInfo.InvariantCulture);

                throw new JsonConvertException("Value was not a DateTime");
            }
        }

        public class JsonFloatingConverter<T> : IJsonConverter {
            public JsonValue ToJson(object value, IJsonContext context) {
                return new JsonNumber(Convert.ToDouble(value, CultureInfo.InvariantCulture));
            }

            public object FromJson(JsonValue value, IJsonContext context) {
                var n = value as JsonNumber;
                if (n != null)
                    return Convert.ChangeType(n.DoubleValue, typeof(T), CultureInfo.InvariantCulture);

                throw new JsonConvertException("Value was not a number");
            }
        }

        public class JsonIntegralConverter<T> : IJsonConverter {
            public JsonValue ToJson(object value, IJsonContext context) {
                return new JsonNumber(Convert.ToDouble(value, CultureInfo.InvariantCulture));
            }

            public object FromJson(JsonValue value, IJsonContext context) {
                var n = value as JsonNumber;
                if (n != null)
                    return Convert.ChangeType(n.LongValue, typeof(T), CultureInfo.InvariantCulture);

                if (context.RelaxedNumericConversion) {
                    var s = value as JsonString;
                    try {
                        if (s != null)
                            return Convert.ChangeType(s.Value, typeof(T), CultureInfo.InvariantCulture);
                    } catch (FormatException) {
                        throw new JsonConvertException("Value was not a number");
                    } catch (OverflowException) {
                        throw new JsonConvertException("Value was too large");
                    }
                }

                throw new JsonConvertException("Value was not a number");
            }
        }

        public class JsonStringConverter : IJsonConverter {
            public JsonValue ToJson(object value, IJsonContext context) {
                return new JsonString(Convert.ToString(value, CultureInfo.InvariantCulture));
            }

            public object FromJson(JsonValue json, IJsonContext context) {
                var s = json as JsonString;
                if (s != null)
                    return s.Value;

                throw new JsonConvertException("Value was not a string");
            }
        }

        public class JsonListConverter<T> : IJsonConverter {
            public JsonValue ToJson(object value, IJsonContext context) {
                var list = value as List<T>;
                if (list == null)
                    throw new JsonConvertException("Value was not a List<" + typeof(T) + ">");

                var array = new JsonArray();
                foreach (var item in list)
                    array.Items.Add(context.ToJson(item));

                return array;
            }

            public object FromJson(JsonValue json, IJsonContext context) {
                var array = json as JsonArray;
                if (array == null)
                    throw new JsonConvertException("Value was not an array");

                return array.Items.Select(context.FromJson<T>).ToList();
            }
        }
    }

    public class ParseException : FormatException {
        public ParseException() { }

        public ParseException(string message)
            : base(message) { }

        public ParseException(string message, Exception inner)
            : base(message, inner) { }
    }

    public abstract class JsonValue {
        static protected void SkipWhitespace(TextReader tr) {
            while (char.IsWhiteSpace((char)tr.Peek()))
                tr.Read();

            if (tr.Peek() == -1)
                throw new ParseException("Expected value, found EOF");
        }

        protected static bool Expect(TextReader tr, string expectedText) {
            return expectedText.All(c => tr.Read() == c);
        }

        protected static void ExpectLiteral(TextReader tr, string literal) {
            if (!Expect(tr, literal))
                throw new ParseException(string.Format("Expected {0}, found something else", literal));
        }

        public static JsonValue Parse(TextReader tr) {
            SkipWhitespace(tr);

            var c = (char)tr.Peek();
            switch (c) {
                case 't': ExpectLiteral(tr, "true"); return new JsonBool(true);
                case 'f': ExpectLiteral(tr, "false"); return new JsonBool(false);
                case 'n': ExpectLiteral(tr, "null"); return null;
                case '"': return new JsonString(tr);
                case '[': return new JsonArray(tr);
                case '{': return new JsonDictionary(tr);
                case '-': return new JsonNumber(tr);
                default:
                    if (char.IsDigit(c))
                        goto case '-';
                    throw new ParseException(string.Format("Expected a value, found '{0}'", c));
            }
        }

        protected abstract void Write(TextWriter tw, int indentation);

        public static void Write(JsonValue value, TextWriter tw) {
            if (value != null)
                value.Write(tw, 0);
            else
                tw.Write("null");
        }

        protected static void Write(JsonValue value, TextWriter tw, int indentation) {
            if (value != null)
                value.Write(tw, indentation);
            else
                tw.Write("null");
        }

        public static void WriteMin(JsonValue value, TextWriter tw) {
            if (value != null)
                value.Write(tw, -1);
            else
                tw.Write("null");
        }

        protected static void WriteIndentation(int indentation, TextWriter tw) {
            tw.Write(Environment.NewLine);
            for (int i = 0; i < indentation; ++i)
                tw.Write("    ");
        }

        protected static int NextIndentationLevel(int level) {
            return level < 0 ? level : level + 1;
        }
    }

    [DebuggerDisplay("{Value}")]
    public class JsonBool : JsonValue {
        public bool Value { get; set; }

        public JsonBool() { }
        public JsonBool(bool value) { Value = value; }

        protected override void Write(TextWriter tw, int indentation) {
            tw.Write(Value ? "true" : "false");
        }
    }

    [DebuggerDisplay("{Value}")]
    public class JsonString : JsonValue {
        public string Value { get; set; }

        public JsonString() { }
        public JsonString(string value) { Value = value; }

        public JsonString(TextReader tr) {
            SkipWhitespace(tr);

            if (!Expect(tr, "\""))
                throw new ParseException("Error parsing string constant: expected opening '\"'");

            var sb = new StringBuilder();

            while (true) {
                int c = tr.Read();
                switch (c) {
                    case -1:
                        throw new ParseException("Unterminated string constant");
                    case '"':
                        Value = sb.ToString();
                        return;

                    case '\\':
                        int esc = tr.Read();
                        if (esc == -1)
                            goto case -1;

                        switch (esc) {
                            case '"':
                            case '\\':
                            case '/':
                                sb.Append((char)esc);
                                break;
                            case 'b': sb.Append('\b'); break;
                            case 'f': sb.Append('\f'); break;
                            case 'n': sb.Append('\n'); break;
                            case 'r': sb.Append('\r'); break;
                            case 't': sb.Append('\t'); break;
                            case 'u':
                                int c1 = tr.Read(),
                                    c2 = tr.Read(),
                                    c3 = tr.Read(),
                                    c4 = tr.Read();

                                if (c1 == -1 || c2 == -1 || c3 == -1 || c4 == -1)
                                    throw new ParseException("Unterminated string constant");

                                int result =
                                    (ReadHex(c1) << 12)
                                    + (ReadHex(c2) << 8)
                                    + (ReadHex(c3) << 4)
                                    + ReadHex(c4);

                                sb.Append((char)result);
                                break;

                            default:
                                throw new ParseException("Invalid escape character: '" + (char)esc + "'");
                        }
                        break;

                    default:
                        sb.Append((char)c);
                        break;
                }
            }
        }

        protected int ReadHex(int c) {
            if (c >= '0' && c <= '9')
                return c - '0';
            if (c >= 'a' && c <= 'f')
                return 10 + c - 'a';
            if (c >= 'A' && c <= 'F')
                return 10 + c - 'A';

            throw new ParseException("Invalid hexadecimal constant in unicode escape character");
        }

        protected override void Write(TextWriter tw, int indentation) {
            tw.Write('\"');
            foreach (char c in Value) {
                switch (c) {
                    case '\n': tw.Write("\\n"); break;
                    case '\r': tw.Write("\\r"); break;
                    case '\b': tw.Write("\\b"); break;
                    case '\f': tw.Write("\\f"); break;
                    case '\t': tw.Write("\\t"); break;
                    case '\\': tw.Write("\\\\"); break;
                    case '/': tw.Write("\\/"); break;
                    case '"': tw.Write("\\\""); break;
                    default:
                        if (c < 32) {
                            var h = (int)c;
                            tw.Write("\\u");
                            tw.Write(HexDigit(h >> 12));
                            tw.Write(HexDigit(h >> 8));
                            tw.Write(HexDigit(h >> 4));
                            tw.Write(HexDigit(h));
                        } else {
                            tw.Write(c);
                        }
                        break;
                }
            }
            tw.Write('\"');
        }

        protected char HexDigit(int digit) {
            digit &= 0xF;
            return digit < 10
                ? (char)('0' + digit)
                : (char)('a' + (digit - 10));
        }
    }

    [DebuggerDisplay("{Value}")]
    public class JsonNumber : JsonValue {
        /// <summary>True if the value is within the range of a long.</summary>
        public bool IsIntegral { get; protected set; }

        public long LongValue { get; set; }
        public double DoubleValue { get; set; }

        public JsonNumber() { }

        public JsonNumber(double value) {
            IsIntegral
                = value == Math.Round(value)
                && value <= long.MaxValue
                && value >= long.MinValue;

            LongValue = (long)value;
            DoubleValue = value;
        }

        public JsonNumber(int value) : this((long)value) { }

        public JsonNumber(long value) {
            LongValue = value;
            DoubleValue = value;
            IsIntegral = true;
        }

        public JsonNumber(TextReader tr) {
            SkipWhitespace(tr);

            var sb = new StringBuilder();

            if (tr.Peek() == '-') {
                sb.Append('-');
                tr.Read();
            }

            if (tr.Peek() == '0') {
                sb.Append((char)tr.Read());

            } else if (char.IsDigit((char)tr.Peek())) {
                while (char.IsDigit((char)tr.Peek()))
                    sb.Append((char)tr.Read());

            } else if (tr.Peek() == -1) {
                throw new ParseException("EOF in numeric constant (expected digit)");
            }

            // Optional fractional part
            if (tr.Peek() == '.') {
                sb.Append((char)tr.Read());
                while (char.IsDigit((char)tr.Peek()))
                    sb.Append((char)tr.Read());
            }

            // Optional exponent
            if (tr.Peek() == 'e' || tr.Peek() == 'E') {
                sb.Append((char)tr.Peek());
                if (tr.Peek() == '+' || tr.Peek() == '-')
                    sb.Append((char)tr.Peek());
                else if (tr.Peek() == -1)
                    throw new ParseException("Expected '+' or '-' in string constant, got EOF");
                else
                    throw new ParseException("Expected '+' or '-' in string constant, got '" + (char)tr.Peek() + "'");

                while (char.IsDigit((char)tr.Peek()))
                    sb.Append((char)tr.Read());
            }

            long integral;
            double floating;

            if (long.TryParse(sb.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out integral)) {
                LongValue = integral;
                DoubleValue = integral;
                IsIntegral = true;
            } else if (double.TryParse(sb.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out floating)) {
                DoubleValue = floating;
                LongValue = (long)floating; // This could result in garbage. What to do in that case?
                IsIntegral = false;
            } else {
                throw new ParseException(string.Format("The numeric constant '{0}' is not a valid number", sb));
            }
        }

        protected override void Write(TextWriter tw, int indentation) {
            tw.Write(IsIntegral
                ? LongValue.ToString(CultureInfo.InvariantCulture)
                : DoubleValue.ToString(CultureInfo.InvariantCulture));
        }
    }

    public class JsonArray : JsonValue {
        public List<JsonValue> Items { get; set; }

        public JsonArray() { Items = new List<JsonValue>(); }
        public JsonArray(IEnumerable<JsonValue> items) { Items = new List<JsonValue>(items); }

        public JsonArray(TextReader tr) {
            SkipWhitespace(tr);

            if (!Expect(tr, "["))
                throw new ParseException("Expected array ('[')");

            Items = new List<JsonValue>();
            while (true) {
                SkipWhitespace(tr);
                if (tr.Peek() == ']')
                    break;

                Items.Add(Parse(tr));

                SkipWhitespace(tr);
                if (tr.Peek() != ',')
                    break;

                tr.Read();
            }

            if (tr.Peek() == ']')
                tr.Read();
            else
                throw new ParseException("Expected ']' at end of array literal");
        }

        protected override void Write(TextWriter tw, int indentation) {
            tw.Write('[');

            bool first = true;
            foreach (var v in Items) {
                if (first)
                    first = false;
                else
                    tw.Write(',');

                WriteIndentation(NextIndentationLevel(indentation), tw);

                Write(v, tw, NextIndentationLevel(indentation));
            }

            if (Items.Count > 0) {
                WriteIndentation(indentation, tw);
            }

            tw.Write(']');
        }
    }

    public class JsonDictionary : JsonValue {
        public Dictionary<string, JsonValue> Items { get; protected set; }

        public JsonDictionary() {
            Items = new Dictionary<string, JsonValue>();
        }

        public JsonDictionary(IDictionary<string, JsonValue> items) {
            if (items == null)
                throw new ArgumentNullException("items");

            Items = new Dictionary<string, JsonValue>(items);
        }

        public static JsonDictionary FromValue(JsonValue value) {
            var dict = value as JsonDictionary;
            if (dict != null)
                return dict;
            throw new JsonConvertException("Expected a JSON dictionary.");
        }

        public JsonDictionary Set(string key, string value) {
            Items[key] = new JsonString(value);
            return this;
        }

        public JsonDictionary Set(string key, long value) {
            Items[key] = new JsonNumber(value);
            return this;
        }

        public JsonDictionary Set(string key, double value) {
            Items[key] = new JsonNumber(value);
            return this;
        }

        public JsonDictionary Set(string key, bool value) {
            Items[key] = new JsonBool(value);
            return this;
        }

        public JsonDictionary Set(string key, JsonValue value) {
            Items[key] = value;
            return this;
        }

        public JsonDictionary GetSubDictionary(string key) {
            return FromValue(this[key]);
        }

        public T Get<T>(string key, IJsonContext context) {
            return context.FromJson<T>(this[key]);
        }

        public T Get<T>(string key, IJsonContext context, T defaultValue) {
            JsonValue json;
            if (Items.TryGetValue(key, out json))
                return context.FromJson<T>(json);
            return defaultValue;
        }

        public JsonValue this[string key] {
            get {
                JsonValue value;
                if (Items.TryGetValue(key, out value))
                    return value;
                throw new JsonConvertException(string.Format("Expected dictionary to contain '{0}' entry", key));
            }
            set {
                Items[key] = value;
            }
        }

        public JsonDictionary(TextReader tr) {
            SkipWhitespace(tr);

            if (!Expect(tr, "{"))
                throw new ParseException("Expected object ('{')");

            Items = new Dictionary<string, JsonValue>();
            while (true) {
                SkipWhitespace(tr);
                string key = new JsonString(tr).Value;
                SkipWhitespace(tr);
                ExpectLiteral(tr, ":");
                SkipWhitespace(tr);
                var value = Parse(tr);

                try {
                    Items.Add(key, value);
                } catch (ArgumentException) {
                    throw new ParseException("The object already contained the key \"" + key + "\"");
                }

                SkipWhitespace(tr);
                if (tr.Peek() != ',')
                    break;

                tr.Read();
            }

            if (tr.Peek() == '}')
                tr.Read();
            else
                throw new ParseException("Expected '}' at end of object literal");
        }

        protected override void Write(TextWriter tw, int indentation) {
            tw.Write('{');
            bool first = true;
            foreach (var v in Items) {
                if (first)
                    first = false;
                else
                    tw.Write(',');

                if (indentation >= 0) {
                    WriteIndentation(NextIndentationLevel(indentation), tw);
                }

                Write(new JsonString(v.Key), tw);
                if (indentation >= 0)
                    tw.Write(' ');
                tw.Write(':');
                if (indentation >= 0)
                    tw.Write(' ');
                Write(v.Value, tw, NextIndentationLevel(indentation));
            }

            if (Items.Count > 0) {
                WriteIndentation(indentation, tw);
            }

            tw.Write('}');
        }
    }
}