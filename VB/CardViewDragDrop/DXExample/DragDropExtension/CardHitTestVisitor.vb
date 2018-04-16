Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports DevExpress.Xpf.Grid

Namespace DXSample.DragDropExtension
	Friend MustInherit Class FindCardElementHitTestVisitorBase
		Inherits DevExpress.Xpf.Grid.CardViewHitTestVisitorBase
		Protected ReadOnly dragDropManager As DragDropManagerBase
		Private privateStoredHitElement As FrameworkElement
		Public Property StoredHitElement() As FrameworkElement
			Get
				Return privateStoredHitElement
			End Get
			Protected Set(ByVal value As FrameworkElement)
				privateStoredHitElement = value
			End Set
		End Property
		Protected Sub New(ByVal dragDropManager As DragDropManagerBase)
			Me.dragDropManager = dragDropManager
		End Sub
		Protected Sub StoreHitElement()
			StoredHitElement = TryCast(HitElement, FrameworkElement)
		End Sub
	End Class
	Friend Class FindCardViewRowElementHitTestVisitor
		Inherits FindCardElementHitTestVisitorBase
		Public Sub New(ByVal dragDropManager As DragDropManagerBase)
			MyBase.New(dragDropManager)
		End Sub
		Public Overrides Sub VisitCard(ByVal rowHandle As Integer)
			StoreHitElement()
		End Sub
		Public Overrides Sub VisitCardHeader(ByVal rowHandle As Integer)
			StoreHitElement()
		End Sub
		Public Overrides Sub VisitCardHeaderButton(ByVal rowHandle As Integer)
			StoreHitElement()
		End Sub
		Public Overrides Sub VisitGroupRow(ByVal rowHandle As Integer)
			StoreHitElement()
			StopHitTesting()
		End Sub
	End Class
	Friend Class FindCardViewDataAreaElementHitTestVisitor
		Inherits FindCardElementHitTestVisitorBase
		Public Sub New(ByVal dragDropManager As DragDropManagerBase)
			MyBase.New(dragDropManager)
		End Sub
		Public Overrides Sub VisitDataArea()
			StoreHitElement()
		End Sub
	End Class
End Namespace
