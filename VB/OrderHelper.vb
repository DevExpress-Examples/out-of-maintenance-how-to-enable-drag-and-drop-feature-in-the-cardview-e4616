Imports DevExpress.Mvvm
Imports System
Imports System.Collections.ObjectModel

Namespace DXSample

    Public Class OrderHelper

        Private _Orders As ObservableCollection(Of DXSample.Order)

        Public Property Orders As ObservableCollection(Of Order)
            Get
                Return _Orders
            End Get

            Private Set(ByVal value As ObservableCollection(Of Order))
                _Orders = value
            End Set
        End Property

        Public Sub New()
            Dim rnd As Random = New Random()
            Orders = New ObservableCollection(Of Order)()
            For i As Integer = 0 To rnd.Next(100, 200) - 1
                Orders.Add(New Order())
            Next
        End Sub
    End Class

    Public Class Order
        Inherits ViewModelBase

        Private Shared rnd As Random = New Random()

        Private _Name As String

        Private _OrderDate As Date

        Private _Amount As Integer

        Private _Price As Integer

        Private _IsProcessed As Boolean

#Region "Properties"
        Public Property Name As String
            Get
                Return _Name
            End Get

            Set(ByVal value As String)
                SetProperty(_Name, value, Function() Name)
            End Set
        End Property

        Public Property OrderDate As Date
            Get
                Return _OrderDate
            End Get

            Set(ByVal value As Date)
                SetProperty(_OrderDate, value, Function() OrderDate)
            End Set
        End Property

        Public Property Amount As Integer
            Get
                Return _Amount
            End Get

            Set(ByVal value As Integer)
                SetProperty(_Amount, value, Function() Amount)
            End Set
        End Property

        Public Property Price As Integer
            Get
                Return _Price
            End Get

            Set(ByVal value As Integer)
                SetProperty(_Price, value, Function() Price)
            End Set
        End Property

        Public Property IsProcessed As Boolean
            Get
                Return _IsProcessed
            End Get

            Set(ByVal value As Boolean)
                SetProperty(_IsProcessed, value, Function() IsProcessed)
            End Set
        End Property

#End Region
        Public Sub New()
            Name = RandomStringHelper.GetRandomString()
            OrderDate = New DateTime(rnd.Next(1998, 2012), rnd.Next(1, 12), rnd.Next(1, 28))
            Amount = rnd.Next(-1000, 1000)
            Price = rnd.Next(0, 10000)
            IsProcessed = rnd.NextDouble() > 0.5
        End Sub
    End Class

    Public Class RandomStringHelper

        Private Shared rnd As Random = New Random()

        Private Shared letters As String = "abcdefghijklmnopqrstuvwxyz"

        Public Shared Function GetRandomString() As String
            Dim length As Integer = rnd.Next(6, 20)
            Dim retVal As String =("" & letters(rnd.Next(25))).ToUpper()
            For i As Integer = 0 To length - 1 - 1
                retVal += letters(rnd.Next(25))
            Next

            Return retVal
        End Function
    End Class
End Namespace
