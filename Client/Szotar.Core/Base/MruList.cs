using System;
using System.Collections.Generic;

namespace Szotar {
	public interface IKeyCompare<T> {
		int CompareKeyWith(T b);
	}

	[Serializable]
	public class MruList<T>
		: IJsonConvertible
		where T : IKeyCompare<T> 
	{
		public List<T> Entries { get; set; }
		int maximumSize;

		public int MaximumSize {
			get {
				return maximumSize;
			}
			set {
				maximumSize = value;
				Clip();
			}
		}

		public MruList() : this(10) { }

		public MruList(int size) {
			Entries = new List<T>();
			MaximumSize = size;
		}

		void Clip() {
			if (Entries.Count > MaximumSize)
				Entries.RemoveRange(MaximumSize, Entries.Count - MaximumSize);
		}

		public void Update(T newItem) {
			int index = Entries.FindIndex(e => e.CompareKeyWith(newItem) == 0);
			if (index != -1)
				Entries.RemoveAt(index);
			Entries.Insert(0, newItem);

			Clip();
		}

		protected MruList(JsonValue json, IJsonContext context) {
			var dict = json as JsonDictionary;
			if (dict == null)
				throw new JsonConvertException("Expected a JSON dictionary");

			foreach (var k in dict.Items) {
				switch (k.Key) {
					case "Entries":
						Entries = context.FromJson<List<T>>(k.Value);
						break;

					case "MaximumSize":
						MaximumSize = context.FromJson<int>(k.Value);
						break;
				}
			}
		}

		JsonValue IJsonConvertible.ToJson(IJsonContext context) {
			var dict = new JsonDictionary();

			dict.Items.Add("Entries", context.ToJson(Entries));
			dict.Items.Add("MaximumSize", context.ToJson(MaximumSize));

			return dict;
		}
	}
}