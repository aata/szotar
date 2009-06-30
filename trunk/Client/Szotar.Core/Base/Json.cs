using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using System.Diagnostics;

namespace Szotar.Json {
	[Serializable]
	public class ParseException : Exception {
		public ParseException() { }
	  
		public ParseException(string message) 
		  : base(message) 
		{ }

		public ParseException(string message, Exception inner) 
			: base(message, inner) 
		{ }

		protected ParseException( 
			System.Runtime.Serialization.SerializationInfo info, 
			System.Runtime.Serialization.StreamingContext context) 
			: base(info, context) 
		{ }
	}

	public abstract class JsonValue {
		static protected void SkipWhitespace(TextReader tr) {
			while(char.IsWhiteSpace((char)tr.Peek()))
				tr.Read();

			if (tr.Peek() == -1)
				throw new ParseException("Expected value, found EOF");
		}

		protected static bool Expect(TextReader tr, string expectedText) {
			foreach (char c in expectedText) {
				if(tr.Read() != c)
					return false;
			}

			return true;
		}

		protected static void ExpectLiteral(TextReader tr, string literal) {
			if (!Expect(tr, literal))
				throw new ParseException(string.Format("Expected {0}, found something else", literal));
		}

		public static JsonValue Parse(TextReader tr) {
			SkipWhitespace(tr);

			char c = (char)tr.Peek();
			switch (c) {
				case 't': ExpectLiteral(tr, "true"); return new JsonBool(true);
				case 'f': ExpectLiteral(tr, "false"); return new JsonBool(true);
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
			if (level < 0)
				return level;
			else
				return level + 1;
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

			if(!Expect(tr, "\""))
				throw new ParseException("Error parsing string constant: expected opening '\"'");

			var sb = new StringBuilder();

			while (true) {
				int c = tr.Read();
				switch(c) {
					case -1:
						throw new ParseException("Unterminated string constant");
					case '"':
						Value = sb.ToString();
						return;

					case '\\':
						int esc = tr.Read();
						if(esc == -1)
							goto case -1;

						switch(esc) {
							case '"': case '\\': case '/':
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

								if(c1 == -1 || c2 == -1 || c3 == -1 || c4 == -1)
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
			if (c >= '0' || c <= '9')
				return c - '0';
			if (c >= 'a' || c <= 'f')
				return 10 + c - 'a';
			if (c >= 'A' || c <= 'F')
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
							int h = (int)c;
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
			if(digit < 10)
				return (char)((int)'0' + digit);
			else 
				return (char)((int)'a' + (digit - 10));
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
				= value == Math.Truncate(value) 
				&& value <= long.MaxValue 
				&& value >= long.MinValue;

			LongValue = (long)value;
			DoubleValue = value; 
		}

		public JsonNumber(int value) : this((long)value) { }

		public JsonNumber(long value) { 
			LongValue = value;
			DoubleValue = (double)value;
			IsIntegral = true;
		} 

		public JsonNumber(TextReader tr) {
			SkipWhitespace(tr);

			var sb = new StringBuilder();
			
			if(tr.Peek() == '-') {
				sb.Append('-');
				tr.Read();
			}

			if(tr.Peek() == '0') {
				sb.Append((char)tr.Read());

			} else if(char.IsDigit((char)tr.Peek())) {
				while(char.IsDigit((char)tr.Peek()))
					sb.Append((char)tr.Read());

			} else if(tr.Peek() == -1) {
				throw new ParseException("EOF in numeric constant (expected digit)");
			}

			// Optional fractional part
			if(tr.Peek() == '.') {
				sb.Append((char)tr.Read());
				while(char.IsDigit((char)tr.Peek()))
					sb.Append((char)tr.Read());
			}

			// Optional exponent
			if(tr.Peek() == 'e' || tr.Peek() == 'E') {
				sb.Append((char)tr.Peek());
				if(tr.Peek() == '+' || tr.Peek() == '-')
					sb.Append((char)tr.Peek());
				else if(tr.Peek() == -1)
					throw new ParseException("Expected '+' or '-' in string constant, got EOF");
				else
					throw new ParseException("Expected '+' or '-' in string constant, got '" + (char)tr.Peek() + "'");
				
				while(char.IsDigit((char)tr.Peek()))
					sb.Append((char)tr.Read());
			}

			long integral;
			double floating;

			if(long.TryParse(sb.ToString(), out integral)) {
				LongValue = integral;
				DoubleValue = integral;
				IsIntegral = true;
			} else if (double.TryParse(sb.ToString(), out floating)) {
				DoubleValue = floating;
				LongValue = (long)floating; // This could result in garbage. What to do in that case?
				IsIntegral = false;
			} else {
				throw new ParseException(string.Format("The numeric constant '{0}' is not a valid number", sb));
			}
		}

		protected override void Write(TextWriter tw, int indentation) {
			if(IsIntegral)
				tw.Write(LongValue);
			else
				tw.Write(DoubleValue);
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

				Items.Add(JsonValue.Parse(tr));

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
				if(first)
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
		public Dictionary<string, JsonValue> Items { get; set; }

		public JsonDictionary() { Items = new Dictionary<string, JsonValue>(); }
		public JsonDictionary(IDictionary<string, JsonValue> items) {
			Items = new Dictionary<string, JsonValue>(items);
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
				JsonValue value = JsonValue.Parse(tr);

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