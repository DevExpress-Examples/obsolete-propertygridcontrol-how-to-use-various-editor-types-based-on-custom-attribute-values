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
                value = If(descriptor.PropertyDescriptor Is Nothing, descriptor.Data, descriptor.PropertyDescriptor.GetValue(descriptor.Data))
            End If
            Dim collection As IEnumerable = TryCast(value, IEnumerable)
            If collection IsNot Nothing Then
                Dim i = 0
                Return collection.Cast(Of Object)().Select(Function(x)
                                                               Return New Descriptor("[" + i.ToString() + "]", x, Nothing)
                                                               i += 1
                                                           End Function)
            End If
            If value Is Nothing Then
                Return Nothing
            End If
            Return TypeDescriptor.GetProperties(value).Cast(Of PropertyDescriptor)().Select(Function(x) New Descriptor(x.Name, value, x))
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class

    Public Class Descriptor
        Public ReadOnly Property Path As String
        Public ReadOnly Property Data As Object
        Public ReadOnly Property PropertyDescriptor As PropertyDescriptor
        Public ReadOnly Property IsCollection As Boolean
            Get
                If PropertyDescriptor IsNot Nothing And Data IsNot Nothing Then
                    Return TypeOf PropertyDescriptor.GetValue(Data) Is IEnumerable
                End If
                Return TypeOf Data Is IEnumerable
            End Get
        End Property
        Public Sub New(path As String, data As Object, propertyDescriptor As PropertyDescriptor)
            Me.Path = path
            Me.Data = data
            Me.PropertyDescriptor = propertyDescriptor
        End Sub
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
            If propertyDescriptor.Attributes.Cast(Of Attribute)().Where(Function(n) TypeOf n Is CustomEditorAttribute).Any() Then Return CustomDefinitionTemplate
            If descriptor.IsCollection Then Return CollectionDefinitionTemplate
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
