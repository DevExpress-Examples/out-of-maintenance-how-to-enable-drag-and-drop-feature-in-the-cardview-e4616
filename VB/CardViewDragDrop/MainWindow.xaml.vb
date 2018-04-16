' Developer Express Code Central Example:
' How to implement the Drag&Drop functionality for the CardView
' 
' We have created an example demonstrating how to implement the Drag&Drop
' functionality for the CardView.
' This functionality is encapsulated in the
' CardDragDropManager class. So, all you need to do is to attach this behavior to
' the GridControl.
' 
' You can find sample updates and versions for different programming languages here:
' http://www.devexpress.com/example=E4616


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes

Namespace CardViewDragDrop
	''' <summary>
	''' Interaction logic for MainWindow.xaml
	''' </summary>
	Partial Public Class MainWindow
		Inherits Window
		Public Sub New()
			InitializeComponent()
		End Sub
	End Class
End Namespace
