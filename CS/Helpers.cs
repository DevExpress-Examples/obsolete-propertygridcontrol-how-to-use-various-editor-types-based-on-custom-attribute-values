using DevExpress.Xpf.PropertyGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace DXSample {
    public class ObjectToPropertyDefinitionsSourceConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(System.IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            Descriptor descriptor = value as Descriptor;
            if (descriptor != null) {
                if (descriptor.PropertyDescriptor == null)
                    value = descriptor.Data;
                else {
                    value = descriptor.PropertyDescriptor.GetValue(descriptor.Data);
                    if (value is IEnumerable) {
                        int i = 0;
                        return ((IEnumerable)value).Cast<object>().Select(x => new Descriptor() { Path = "[" + (i++) + "]", Data = x, IsCollection = true }).ToList();
                    }
                }
            }
            else if (value == null)
                return null;
            var props = TypeDescriptor.GetProperties(value);
            return props.Cast<PropertyDescriptor>().Select(x => new Descriptor { PropertyDescriptor = x, Data = value, Path = x.Name });
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new System.NotImplementedException();
        }
    }
    public class Descriptor {
        public string Path { get; set; }
        public object Data { get; set; }
        public PropertyDescriptor PropertyDescriptor { get; set; }
        public bool IsCollection { get; set; }
    }
    public class PropertyDefinitionTemplateSelector : DataTemplateSelector {
        public DataTemplate DefinitionTemplate {
            get;
            set;
        }
        public DataTemplate CollectionDefinitionTemplate {
            get;
            set;
        }
        public DataTemplate CustomDefinitionTemplate {
            get;
            set;
        }
        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            Descriptor descriptor = (Descriptor)item;
            PropertyDescriptor propertyDescriptor = descriptor.PropertyDescriptor;
            if (propertyDescriptor == null)
                return DefinitionTemplate;
            if (descriptor.IsCollection)
                return CollectionDefinitionTemplate;
            if (propertyDescriptor.Attributes.Cast<Attribute>().Where(n => n is CustomEditorAttribute).Any())
                return CustomDefinitionTemplate;
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