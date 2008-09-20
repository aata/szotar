using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Szotar {
	public interface IStringTable {
		string this[string stringName] { get; }
	}

	public abstract class LocalizationProvider {
		public abstract IStringTable ErrorStringTable { get; }
		public abstract IStringTable GetTypeDescriptionStringTable(Type type);

		public static LocalizationProvider Default { get; set; }
	}

	internal class LocalizedTypeDescriptor : CustomTypeDescriptor {
		Type type;
		PropertyDescriptorCollection baseProperties;
		IStringTable typeStringTable;

		public LocalizedTypeDescriptor(Type type) {
			baseProperties = TypeDescriptor.GetProvider(typeof(Object)).GetTypeDescriptor(type).GetProperties();
			typeStringTable = LocalizationProvider.Default.GetTypeDescriptionStringTable(type);
			this.type = type;
		}

		public LocalizedTypeDescriptor(Type type, ICustomTypeDescriptor parent)
			: base(parent) {
			baseProperties = parent.GetProperties();
			typeStringTable = LocalizationProvider.Default.GetTypeDescriptionStringTable(type);
			this.type = type;
		}

		public override PropertyDescriptorCollection GetProperties() {
			return GetProperties(null);
		}

		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			List<PropertyDescriptor> properties = new List<PropertyDescriptor>();

			foreach (PropertyDescriptor defaultPD in baseProperties) {
				properties.Add(new LocalizedPropertyDescriptor(defaultPD, typeStringTable));
			}

			return new PropertyDescriptorCollection(properties.ToArray(), true);
		}
	}

	internal class LocalizedPropertyDescriptor : PropertyDescriptor {
		PropertyDescriptor actual;
		IStringTable typeStringTable;

		string category, description, displayName;

		public LocalizedPropertyDescriptor(PropertyDescriptor actual, IStringTable typeStringTable)
			: base(actual) 
		{
			this.actual = actual;
			this.typeStringTable = typeStringTable;
			category = typeStringTable[actual.Category + "$Category"] ?? actual.Category;
			description = typeStringTable[actual.Description + "$Description"] ?? actual.Description;
			displayName = typeStringTable[actual.DisplayName + "$Name"] ?? actual.DisplayName;
		}

		public override string Category {
			get {
				return category;
			}
		}

		public override string Description {
			get {
				return description;
			}
		}

		public override string DisplayName {
			get {
				return displayName;
			}
		}

		public override bool CanResetValue(object component) {
			return actual.CanResetValue(component);
		}

		public override Type ComponentType {
			get { return actual.ComponentType; }
		}

		public override object GetValue(object component) {
			return actual.GetValue(component);
		}

		public override bool IsReadOnly {
			get { return actual.IsReadOnly; }
		}

		public override Type PropertyType {
			get { return actual.PropertyType; }
		}

		public override void ResetValue(object component) {
			actual.ResetValue(component);
		}

		public override void SetValue(object component, object value) {
			actual.SetValue(component, value);
		}

		public override bool ShouldSerializeValue(object component) {
			return actual.ShouldSerializeValue(component);
		}
	}

	public class LocalizedTypeDescriptionProvider<T> : TypeDescriptionProvider {
		public LocalizedTypeDescriptionProvider()
			: base(TypeDescriptor.GetProvider(typeof(T)))
		{
		}

		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
			return new LocalizedTypeDescriptor(objectType, base.GetTypeDescriptor(objectType, instance));
		}
	}
}