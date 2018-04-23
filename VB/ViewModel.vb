Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Linq
Imports System.Text

Namespace DXSample
    Public Class ViewModel
        Public Property Department() As Department
        Public Sub New()
            Department = New Department With {.Name = "Department"}
            Department.Employees.Add(New Employee With {.Name = "Jett Mitchell"})
            Department.Employees.Add(New Employee With {.Name = "Garrick Stiedemann DVM"})
            Department.Employees.Add(New Employee With {.Name = "Hettie Runte"})
        End Sub
    End Class
    Public Class Department
        Public Sub New()
            Employees = New BindingList(Of Employee)()
        End Sub
        Public Property Name() As String
        Public Property Employees() As BindingList(Of Employee)
    End Class
    Public Class Employee
        <CustomEditorAttribute("ButtonEdit")> _
        Public Property Name() As String
    End Class
    Public Class CustomEditorAttribute
        Inherits Attribute

        Private privateName As String
        Public Property Name() As String
            Get
                Return privateName
            End Get
            Private Set(ByVal value As String)
                privateName = value
            End Set
        End Property
        Public Sub New(ByVal _name As String)
            Name = _name
        End Sub
    End Class
End Namespace