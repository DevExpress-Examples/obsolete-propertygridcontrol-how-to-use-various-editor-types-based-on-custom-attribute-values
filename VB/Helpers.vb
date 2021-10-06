Imports DevExpress.Xpf.PropertyGrid
Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Markup

Namespace DXSample

    Public Class ObjectToPropertyDefinitionsSourceConverter
        Inherits MarkupExtension
        Implements IValueConverter

        Public Overrides Function ProvideValue(ByVal serviceProvider As IServiceProvider) As Object
            Return Me
        End Function

        Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
            Dim descriptor As Descriptor = TryCast(value, Descriptor)
            If descriptor IsNot Nothing Then
                If descriptor.PropertyDescriptor Is Nothing Then
                    value = descriptor.Data
                Else
                    value = descriptor.PropertyDescriptor.GetValue(descriptor.Data)
                    If TypeOf value Is IEnumerable Then
                        Dim i As Integer = 0
                        Return CType(value, IEnumerable).Cast(Of Object)().[Select](Function(x) New Descriptor() With {.Path = "[" & Math.Min(Threading.Interlocked.Increment(i), i - 1) & "]", .Data = x, .IsCollection = True}).ToList()
                    End If
                End If
            ElseIf value Is Nothing Then
                Return Nothing
            End If

            Dim props = TypeDescriptor.GetProperties(value)
            Return props.Cast(Of PropertyDescriptor)().[Select](Function(x) New Descriptor With {.PropertyDescriptor = x, .Data = value, .Path = x.Name})
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class

    Public Class Descriptor

        Public Property Path As String

        Public Property Data As Object

        Public Property PropertyDescriptor As PropertyDescriptor

        Public Property IsCollection As Boolean
    End Class

    Public Class PropertyDefinitionTemplateSelector
        Inherits DataTemplateSelector

        Public Property DefinitionTemplate As DataTemplate

        Public Property CollectionDefinitionTemplate As DataTemplate

        Public Property CustomDefinitionTemplate As DataTemplate

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) As DataTemplate
            Dim descriptor As Descriptor = CType(item, Descriptor)
            Dim propertyDescriptor As PropertyDescriptor = descriptor.PropertyDescriptor
            If propertyDescriptor Is Nothing Then Return DefinitionTemplate
            If descriptor.IsCollection Then Return CollectionDefinitionTemplate
            If propertyDescriptor.Attributes.Cast(Of Attribute)().Where(Function(n) TypeOf n Is CustomEditorAttribute).Any() Then Return CustomDefinitionTemplate
            Return DefinitionTemplate
        End Function
    End Class

    Public Class CellTemplateSelector
        Inherits DataTemplateSelector

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) As DataTemplate
            Dim data As RowData = CType(item, RowData)
            Dim descr = CType(data.Definition.DataContext, Descriptor)
            Dim attr = CType(descr.PropertyDescriptor.Attributes.Cast(Of Attribute)().First(Function(x) TypeOf x Is CustomEditorAttribute), CustomEditorAttribute)
            If Equals(attr.Name, "ButtonEdit") Then
                Return CType(CType(container, FrameworkElement).FindResource("ButtonEditTemplate"), DataTemplate)
            End If

            Return MyBase.SelectTemplate(item, container)
        End Function
    End Class
End Namespace
