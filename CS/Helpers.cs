using DevExpress.Xpf.PropertyGrid;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace DXSample {
    public class ObjectToPropertyDefinitionsSourceConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            Descriptor descriptor = value as Descriptor;
            if (descriptor != null)
                value = descriptor.PropertyDescriptor == null ? descriptor.Data : descriptor.PropertyDescriptor.GetValue(descriptor.Data);
            IEnumerable collection = value as IEnumerable;
            if (collection != null) {
                int i = 0;
                return collection.Cast<object>().Select(x => new Descriptor("[" + (i++) + "]", x, null));
            }
            return value == null ? null : TypeDescriptor.GetProperties(value).Cast<PropertyDescriptor>().Select(x => new Descriptor(x.Name, value, x));
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class Descriptor {
        public string Path { get; }
        public object Data { get; }
        public PropertyDescriptor PropertyDescriptor { get; }
        public bool IsCollection {
            get {
                if (PropertyDescriptor != null && Data != null)
                    return PropertyDescriptor.GetValue(Data) is IEnumerable;
                return Data is IEnumerable;
            }
        }
        public Descriptor(string path, object data, PropertyDescriptor propertyDescriptor) {
            Path = path;
            Data = data;
            PropertyDescriptor = propertyDescriptor;
        }
    }
    public class PropertyDefinitionTemplateSelector : DataTemplateSelector {
        public DataTemplate DefinitionTemplate { get; set; }
        public DataTemplate CollectionDefinitionTemplate { get; set; }
        public DataTemplate CustomDefinitionTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            Descriptor descriptor = (Descriptor)item;
            PropertyDescriptor propertyDescriptor = descriptor.PropertyDescriptor;
            if (propertyDescriptor == null)
                return DefinitionTemplate;
            if (propertyDescriptor.Attributes.Cast<Attribute>().Where(n => n is CustomEditorAttribute).Any())
                return CustomDefinitionTemplate;
            if (descriptor.IsCollection)
                return CollectionDefinitionTemplate;
            return DefinitionTemplate;
        }
    }
    public class CellTemplateSelector : DataTemplateSelector {
        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            RowData data = (RowData)item;
            var descr = (Descriptor)data.Definition.DataContext;
            var attr = (CustomEditorAttribute)descr.PropertyDescriptor.Attributes.Cast<Attribute>().First(x => x is CustomEditorAttribute);
            if (attr.Name == "ButtonEdit") {
                return (DataTemplate)((FrameworkElement)container).FindResource("ButtonEditTemplate");
            }
            return base.SelectTemplate(item, container);
        }
    }
}