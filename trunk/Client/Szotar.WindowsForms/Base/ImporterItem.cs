using System;

namespace Szotar.WindowsForms{
    public class ImporterItem {
        private string name, description;
        private Type type;

        public ImporterItem(Type type, string name, string description) {
            this.type = type;
            this.name = name;
            this.description = description;
        }

        public Type Type { get { return type; } }
        public string Name { get { return name; } }
        public string Description { get { return description; } }

        public override string ToString() {
            return name + " - " + description;
        }
    }
}