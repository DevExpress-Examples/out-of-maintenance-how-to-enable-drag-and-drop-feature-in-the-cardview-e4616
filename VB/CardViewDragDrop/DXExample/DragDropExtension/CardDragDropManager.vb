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
#Region "Copyright (c) 2000-2013 Developer Express Inc."
'
'{*******************************************************************}
'{                                                                   }
'{       Developer Express .NET Component Library                    }
'{                                                                   }
'{                                                                   }
'{       Copyright (c) 2000-2013 Developer Express Inc.              }
'{       ALL RIGHTS RESERVED                                         }
'{                                                                   }
'{   The entire contents of this file is protected by U.S. and       }
'{   International Copyright Laws. Unauthorized reproduction,        }
'{   reverse-engineering, and distribution of all or any portion of  }
'{   the code contained in this file is strictly prohibited and may  }
'{   result in severe civil and criminal penalties and will be       }
'{   prosecuted to the maximum extent possible under the law.        }
'{                                                                   }
'{   RESTRICTIONS                                                    }
'{                                                                   }
'{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
'{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
'{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
'{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
'{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
'{                                                                   }
'{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
'{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
'{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
'{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
'{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
'{                                                                   }
'{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
'{   ADDITIONAL RESTRICTIONS.                                        }
'{                                                                   }
'{*******************************************************************}
'
#End Region ' Copyright (c) 2000-2013 Developer Express Inc.

Imports System
Imports System.Data
Imports System.Linq
Imports System.Windows
Imports System.Collections
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.ComponentModel
Imports System.Windows.Documents
Imports System.Windows.Threading
Imports System.Collections.Generic

Imports DevExpress.Data.Access
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Core.Native
Imports DevExpress.Xpf.Grid
Imports DevExpress.Xpf.Grid.DragDrop
Imports DevExpress.Xpf.Utils

Namespace DXSample.DragDropExtension

    Public Class CardDragDropManager
        Inherits DragDropManagerBase
#Region "Properties"
        Public ReadOnly Property View() As DataViewBase
            Get
                Return If((GridControl IsNot Nothing), GridControl.View, Nothing)
            End Get
        End Property
        Public Property ScrollSpacing() As Double
            Get
                Return m_ScrollSpacing
            End Get
            Set
                m_ScrollSpacing = Value
            End Set
        End Property
        Private m_ScrollSpacing As Double
        Public Property ScrollSpeed() As Double
            Get
                Return m_ScrollSpeed
            End Get
            Set
                m_ScrollSpeed = Value
            End Set
        End Property
        Private m_ScrollSpeed As Double
        Public Property MouseDownHitInfo() As GridViewHitInfoBase
            Get
                Return m_MouseDownHitInfo
            End Get
            Protected Friend Set
                m_MouseDownHitInfo = Value
            End Set
        End Property
        Private m_MouseDownHitInfo As GridViewHitInfoBase

        Public ReadOnly Property RestoreSelection() As Boolean
            Get
                Return TypeOf DataControl.ItemsSource Is System.Collections.Specialized.INotifyCollectionChanged OrElse TypeOf DataControl.ItemsSource Is IBindingList
            End Get
        End Property

        Private ReadOnly Property CardView() As CardView
            Get
                Return TryCast(View, CardView)
            End Get
        End Property
        Private ReadOnly Property GridControl() As GridControl
            Get
                Return TryCast(DataControl, GridControl)
            End Get
        End Property
        Private ReadOnly Property DataControl() As DataControlBase
            Get
                Return TryCast(AssociatedObject, DataControlBase)
            End Get
        End Property
        Private Property LastPosition() As Point
            Get
                Return m_LastPosition
            End Get
            Set
                m_LastPosition = Value
            End Set
        End Property
        Private m_LastPosition As Point
        Private ReadOnly Property HitElement() As DependencyObject
            Get
                Return Me.m_hitElement
            End Get
        End Property

        Protected Overrides ReadOnly Property ItemsSource() As IList
            Get
                Dim source As IListSource = TryCast(DataControl.ItemsSource, IListSource)
                If source IsNot Nothing Then
                    Return source.GetList()
                End If
                Return TryCast(DataControl.ItemsSource, IList)
            End Get
        End Property
#End Region

        Private m_hitElement As DependencyObject
        Private HoverRowHandle As Integer

#Region "Inner classes"
        Public Class CardDragSource
            Inherits SupportDragDropBase
            Private ReadOnly Property CardDragDropManager() As CardDragDropManager
                Get
                    Return DirectCast(dragDropManager, CardDragDropManager)
                End Get
            End Property
            Protected Overrides ReadOnly Property Owner() As FrameworkElement
                Get
                    Return CardDragDropManager.DataControl
                End Get
            End Property
            Public Sub New(dragDropManager As DragDropManagerBase)
                MyBase.New(dragDropManager)
            End Sub
            Protected Overrides ReadOnly Property SourceElementCore() As FrameworkElement
                Get
                    Return CardDragDropManager.DataControl
                End Get
            End Property
        End Class
        Private Delegate Sub MoveRowsDelegate(sourceManager As DragDropManagerBase, targetRowHandle As Integer, hitElement As DependencyObject)
        Protected Class DragDropHitTestResult
            Inherits DragDropObjectBase
            Public Property Element() As UIElement
                Get
                    Return m_Element
                End Get
                Private Set
                    m_Element = Value
                End Set
            End Property
            Private m_Element As UIElement
            Public Sub New(manager As DragDropManagerBase)
                MyBase.New(manager)
            End Sub
            Public Function CallBack(result As HitTestResult) As HitTestResultBehavior
                Element = TryCast(result.VisualHit, UIElement)
                If Element Is Nothing OrElse Not UIElementHelper.IsVisibleInTree(TryCast(Element, FrameworkElement)) OrElse Not Element.IsHitTestVisible Then
                    Return HitTestResultBehavior.[Continue]
                End If
                Return HitTestResultBehavior.[Stop]
            End Function
        End Class
#End Region

#Region "Behavior"
        Protected Overrides Sub OnAttached()
            MyBase.OnAttached()
            Dim dataCtrl As DataControlBase = DataControl
            If dataCtrl IsNot Nothing Then
                DataViewDragDropManager.SetDragDropManager(dataCtrl, Me)
                DragDropHelper = New RowDragDropElementHelper(CreateDragSource(Me))
                AddHandler AutoExpandTimer.Tick, AddressOf AutoExpandTimer_Tick
                DragManager.SetDropTargetFactory(dataCtrl, New DragDropManagerDropTargetFactory())
            End If
        End Sub
        Protected Overrides Sub OnDetaching()
            MyBase.OnDetaching()
            If DataControl IsNot Nothing Then
                RemoveHandler AutoExpandTimer.Tick, AddressOf AutoExpandTimer_Tick
            End If
        End Sub
#End Region

#Region "AutoExpander"
        Public Shared ReadOnly AllowAutoExpandProperty As DependencyProperty = DependencyPropertyManager.Register("AllowAutoExpandGroups", GetType(Boolean), GetType(CardDragDropManager), New PropertyMetadata(True))
        Public Shared ReadOnly AutoExpandDelayProperty As DependencyProperty = DependencyPropertyManager.Register("AutoExpandGroupsDelay", GetType(Integer), GetType(CardDragDropManager), New PropertyMetadata(1000, Function(s, e)
                                                                                                                                                                                                                          DirectCast(s, CardDragDropManager).AutoExpandTimer.Interval = TimeSpan.FromMilliseconds(CInt(e.NewValue))

                                                                                                                                                                                                                      End Function))

        Public Property AllowAutoExpandGroups() As Boolean
            Get
                Return CBool(GetValue(AllowAutoExpandProperty))
            End Get
            Set
                SetValue(AllowAutoExpandProperty, Value)
            End Set
        End Property
        Public Property AutoExpandGroupsDelay() As Integer
            Get
                Return CInt(GetValue(AutoExpandDelayProperty))
            End Get
            Set
                SetValue(AutoExpandDelayProperty, Value)
            End Set
        End Property

        Private AutoExpandTimer As DispatcherTimer
        Private ReadOnly Property IsExpandable() As Boolean
            Get
                If HoverRowHandle <> GridControl.InvalidRowHandle AndAlso HoverRowHandle <= 0 AndAlso HoverRowHandle <> GridControl.NewItemRowHandle AndAlso HoverRowHandle <> GridControl.AutoFilterRowHandle Then
                    Return Not GridControl.IsGroupRowExpanded(HoverRowHandle)
                Else
                    Return False
                End If
            End Get
        End Property

        Private Sub PerformAutoExpand()
            GridControl.ExpandGroupRow(HoverRowHandle)
        End Sub
        Private Sub AutoExpandTimer_Tick(sender As Object, e As EventArgs)
            If AllowAutoExpandGroups Then
                DataControl.Dispatcher.BeginInvoke(New Action(Sub() PerformAutoExpand()))
            End If
        End Sub
        Private Sub StopAutoExpandTimer()
            If AutoExpandTimer.IsEnabled Then
                AutoExpandTimer.[Stop]()
            End If
        End Sub
#End Region

        Public Sub New()
            AutoExpandTimer = New DispatcherTimer() With {
            .Interval = TimeSpan.FromMilliseconds(AutoExpandGroupsDelay)
        }
        End Sub

#Region "DragDropHandlers"
        Public Overrides Sub OnDragOver(sourceManager As DragDropManagerBase, source As UIElement, pt As Point)
            LastPosition = pt
            UpdateHoverRowHandle(source, pt)
            If IsExpandable Then
                AutoExpandTimer.[Stop]()
                AutoExpandTimer.Start()
            Else
                AutoExpandTimer.[Stop]()
            End If
            Me.m_hitElement = GetVisibleHitTestElement(pt)
            PerformDropToViewCore(sourceManager)
            MyBase.OnDragOver(sourceManager, source, pt)
        End Sub
        Protected Overrides Sub OnDragLeave()
            StopAutoExpandTimer()
            HoverRowHandle = GridControl.InvalidRowHandle
            MouseDownHitInfo = Nothing
            MyBase.OnDragLeave()
        End Sub
        Protected Overrides Sub OnDrop(sourceManager As DragDropManagerBase, source As UIElement, pt As Point)
            If DropEventIsLocked Then
                Return
            End If
            'GridDropEventArgs e = RaiseDropEvent(sourceManager, pt);
            'if (!e.Handled)
            PerformDropToView(sourceManager, TryCast(GetHitInfo(HitElement), CardViewHitInfo), pt, AddressOf MoveSelectedRows, Function(x) New MoveRowsDelegate(Sub(s, g, h) MoveSelectedRowsToGroup(s, g, h, x)), AddressOf AddRows)
            'RaiseDroppedEvent(sourceManager, e);
            MyBase.OnDrop(sourceManager, source, pt)


            'if (DropEventIsLocked) return;
            'PerformDropToView(sourceManager, GetHitInfo(HitElement) as CardViewHitInfo, pt, MoveSelectedRows, MoveSelectedRowsToGroup, AddRows);
            'base.OnDrop(sourceManager, source, pt);
        End Sub
#End Region

#Region "Overrides"
        Protected Overrides Function CanShowDropMarker() As Boolean
            Return Not View.RenderSize.IsZero()
        End Function
        Protected Overrides Function CustomAllowDrag(e As IndependentMouseEventArgs) As Boolean
            DraggingRows = CalcDraggingRows(e)
            Dim startDragArgs As StartDragEventArgs = RaiseStartDragEvent(e)
            Return startDragArgs.CanDrag
        End Function
        Protected Overrides Function CalcDraggingRows(e As IndependentMouseEventArgs) As IList
            Dim hitInfo As CardViewHitInfo = TryCast(MouseDownHitInfo, CardViewHitInfo)
            If IsInDataRow(hitInfo) Then
                Return GetSelectedRowsCopy()
            End If
            If IsInGroupRow(hitInfo) Then
                Return GetChildRows(hitInfo.RowHandle)
            End If
            Return Nothing
        End Function
        Protected Overrides Function CreateLogicalOwner() As FrameworkElement
            Return View
        End Function
        Protected Overrides Function CanStartDrag(e As MouseButtonEventArgs) As Boolean
            If View.IsEditing Then
                Return False
            End If
            MouseDownHitInfo = GetHitInfo(TryCast(e.OriginalSource, DependencyObject))
            Dim hitInfo As CardViewHitInfo = TryCast(MouseDownHitInfo, CardViewHitInfo)
            Return hitInfo.InRow
        End Function
        Protected Overrides Function RaiseStartDragEvent(e As IndependentMouseEventArgs) As StartDragEventArgs
            Dim result As New StartDragEventArgs() With {
            .CanDrag = True
        }
            Return result
        End Function
#End Region

        Private Function GetHitInfo(element As DependencyObject) As GridViewHitInfoBase
            Dim result As CardViewHitInfo = CardView.CalcHitInfo(element)
            Return result
        End Function

        Private Function GetDragIndicatorPositionForRowElement(rowElement As FrameworkElement) As TableDragIndicatorPosition
            If rowElement Is Nothing Then
                Return TableDragIndicatorPosition.None
            End If
            Dim point As Double = LastPosition.Y - LayoutHelper.GetRelativeElementRect(rowElement, DataControl).Top
            Return If(point > rowElement.ActualHeight / 2, TableDragIndicatorPosition.Bottom, TableDragIndicatorPosition.Top)
        End Function

        Private Function BanDrop(insertRowHandle As Integer, hitInfo As GridViewHitInfoBase, sourceManager As DragDropManagerBase) As Boolean
            SetDropMarkerVisibility(True)
            Return InlineAssignHelper(DropEventIsLocked, False)
        End Function

        Private Function GetHitInfo(e As IndependentMouseEventArgs) As GridViewHitInfoBase
            Return GetHitInfo(TryCast(e.OriginalSource, DependencyObject))
        End Function

        Private Function CreateDragSource(dataViewDragDropManager As CardDragDropManager) As ISupportDragDrop
            Return New CardDragSource(dataViewDragDropManager)
        End Function

        Private Function GetSelectedRowsCopy() As List(Of Object)
            Return New List(Of Object)(GridControl.SelectedItems.Cast(Of Object)())
        End Function
        Private Function IsInDataRow(info As CardViewHitInfo) As Boolean
            Return IsInRowCore(info) AndAlso Not GridControl.IsGroupRowHandle(info.RowHandle)
        End Function
        Private Function IsInGroupRow(info As CardViewHitInfo) As Boolean
            Return IsInRowCore(info) AndAlso GridControl.IsGroupRowHandle(info.RowHandle)
        End Function
        Private Function IsInRowCore(info As CardViewHitInfo) As Boolean
            Return info.InRow
        End Function

        Private Function GetDropTargetTypeByHitElement(hitElement As DependencyObject) As DropTargetType
            Dim rowElement As FrameworkElement
            Dim dragIndicatorPosition As TableDragIndicatorPosition
            Return GetDropTargetTypeByHitElement(hitElement, rowElement, dragIndicatorPosition)
        End Function
        Private Function GetDropTargetTypeByHitElement(hitElement As DependencyObject, ByRef rowElement As FrameworkElement, ByRef dragIndicatorPosition As TableDragIndicatorPosition) As DropTargetType
            rowElement = GetRowElement(hitElement)
            dragIndicatorPosition = GetDragIndicatorPositionForRowElement(rowElement)
            Select Case dragIndicatorPosition
                Case TableDragIndicatorPosition.Top
                    Return DropTargetType.InsertRowsBefore
                Case TableDragIndicatorPosition.Bottom
                    Return DropTargetType.InsertRowsAfter
                Case TableDragIndicatorPosition.InRow
                    Return DropTargetType.InsertRowsIntoNode
                Case Else
                    Return DropTargetType.None
            End Select
        End Function

        Private Sub PerformDropToViewCore(sourceManager As DragDropManagerBase)
            Dim hitInfo As GridViewHitInfoBase = GetHitInfo(HitElement)
            If BanDrop(hitInfo.RowHandle, hitInfo, sourceManager) Then
                ClearDragInfo(sourceManager)
                Return
            End If
            PerformDropToView(sourceManager, TryCast(hitInfo, CardViewHitInfo), LastPosition, AddressOf SetReorderDropInfo, Function(x) New MoveRowsDelegate(Sub(s, g, h) MoveSelectedRowsToGroup(s, g, h, x)), AddressOf SetAddRowsDropInfo)
        End Sub
        Private Sub PerformDropToView(sourceManager As DragDropManagerBase, hitInfo As CardViewHitInfo, pt As Point, reorderDelegate As MoveRowsDelegate, groupDelegateExtractor As Func(Of Boolean, MoveRowsDelegate), addRowsDelegate As MoveRowsDelegate)
            Dim insertRowHandle As Integer = hitInfo.RowHandle
            If GridControl.IsGroupRowHandle(insertRowHandle) Then
                groupDelegateExtractor(True)(sourceManager, insertRowHandle, HitElement)
                Return
            End If
            If IsSortedButNotGrouped() OrElse hitInfo.HitTest = CardViewHitTest.DataArea Then
                If sourceManager.DraggingRows.Count > 0 AndAlso GetDataAreaElement(HitElement) IsNot Nothing AndAlso Not ReferenceEquals(sourceManager, Me) Then
                    addRowsDelegate(sourceManager, insertRowHandle, HitElement)
                Else
                    ClearDragInfo(sourceManager)
                End If
                Return
            End If
            If insertRowHandle = GridControl.InvalidRowHandle OrElse insertRowHandle = GridControl.AutoFilterRowHandle OrElse insertRowHandle = GridControl.NewItemRowHandle Then
                ClearDragInfo(sourceManager)
                Return
            End If
            If GridControl.GroupCount > 0 Then
                Dim groupRowHandle As Integer = GridControl.GetParentRowHandle(insertRowHandle)
                If ShouldReorderGroup() Then
                    If Not IsSameGroup(sourceManager, GetGroupInfos(groupRowHandle), HitElement) Then
                        groupDelegateExtractor(False)(sourceManager, groupRowHandle, HitElement)
                    End If
                    reorderDelegate(sourceManager, insertRowHandle, HitElement)
                Else
                    groupDelegateExtractor(True)(sourceManager, groupRowHandle, HitElement)
                End If
            Else
                reorderDelegate(sourceManager, insertRowHandle, HitElement)
            End If
        End Sub

        Private Function GetChildRows(groupRowHandle As Integer) As List(Of Object)
            Dim list As New List(Of Object)()
            CollectGroupRowChildren(groupRowHandle, list)
            Return list
        End Function

        Private Sub UpdateHoverRowHandle(source As UIElement, pt As Point)
            HoverRowHandle = GetOverRowHandle(source, pt)
        End Sub

        Private Function GetOverRowHandle(source As UIElement, pt As Point) As Integer
            Dim element As UIElement = GetVisibleHitTestElement(pt)
            Dim hitInfo As GridViewHitInfoBase = GetHitInfo(element)
            Return hitInfo.RowHandle
        End Function

        Private Function ShouldReorderGroup() As Boolean
            Return (GridControl.SortInfo.Count - GridControl.GroupCount) <= 0
        End Function

        Private Sub AddRow(sourceManager As DragDropManagerBase, row As Object, insertRowHandle As Integer)
            sourceManager.RemoveObject(row)
            ItemsSource.Add(sourceManager.GetObject(row))
        End Sub
        Private Sub SetAddRowsDropInfo(sourceManager As DragDropManagerBase, insertRowHandle As Integer, hitElement As DependencyObject)
            sourceManager.SetDropTargetType(DropTargetType.DataArea)
            sourceManager.ShowDropMarker(GetDataAreaElement(hitElement), TableDragIndicatorPosition.None)
        End Sub
        Private Sub AddRows(sourceManager As DragDropManagerBase, insertRowHandle As Integer, hitElement As DependencyObject)
            DataControl.BeginDataUpdate()
            For Each row As Object In sourceManager.DraggingRows
                AddRow(sourceManager, row, insertRowHandle)
            Next
            DataControl.EndDataUpdate()
        End Sub

        Private Sub SetMoveToGroupRowDropInfo(sourceManager As DragDropManagerBase, insertRowHandle As Integer, hitElement As DependencyObject)
            Dim groupInfo As GroupInfo() = GetGroupInfos(insertRowHandle)
            If CanMoveSelectedRowsToGroup(sourceManager, groupInfo, hitElement) Then
                sourceManager.SetDropTargetType(DropTargetType.InsertRowsIntoGroup)
                sourceManager.ViewInfo.GroupInfo = groupInfo
                sourceManager.ShowDropMarker(GetRowElement(hitElement), TableDragIndicatorPosition.None)
            Else
                ClearDragInfo(sourceManager)
            End If
        End Sub

        Private Sub CollectGroupRowChildren(groupRowHandle As Integer, list As IList)
            Dim childCount As Integer = GridControl.GetChildRowCount(groupRowHandle)
            For i As Integer = 0 To childCount - 1
                Dim childRowHandle As Integer = GridControl.GetChildRowHandle(groupRowHandle, i)
                If GridControl.IsGroupRowHandle(childRowHandle) Then
                    CollectGroupRowChildren(childRowHandle, list)
                Else
                    list.Add(GridControl.GetRow(childRowHandle))
                End If
            Next
        End Sub
        Private Function IsSortedButNotGrouped() As Boolean
            Return IsSorted() AndAlso Not IsGrouped()
        End Function
        Private Function IsGrouped() As Boolean
            Return GridControl.GroupCount > 0
        End Function
        Private Function IsSorted() As Boolean
            Return GridControl.SortInfo.Count > 0
        End Function
        Private Sub MoveSelectedRows(sourceManager As DragDropManagerBase, insertRowHandle As Integer, hitElement As DependencyObject)
            Dim insertObject As Object = GridControl.GetRow(insertRowHandle)
            If insertObject Is Nothing OrElse sourceManager.DraggingRows.Contains(GridControl.GetRow(insertRowHandle)) Then
                Return
            End If
            Dim row As FrameworkElement = GetRowElement(hitElement)
            Dim dropTargetType__1 As DropTargetType = GetDropTargetType(row)
            If dropTargetType__1 = DropTargetType.None Then
                Return
            End If
            Dim index As Integer = ItemsSource.IndexOf(insertObject)
            If dropTargetType__1 = DropTargetType.InsertRowsAfter Then
                index += 1
            End If

            GridControl.BeginDataUpdate()
            For Each obj As Object In sourceManager.DraggingRows
                Dim sourceObject As Object = sourceManager.GetObject(obj)

                If ReferenceEquals(ItemsSource, sourceManager.GetSource(obj)) Then
                    If index > ItemsSource.IndexOf(sourceObject) Then
                        index -= 1
                    End If
                End If
                sourceManager.RemoveObject(obj)
                ItemsSource.Insert(index, sourceObject)
                index += 1
            Next
            GridControl.EndDataUpdate()

            If RestoreSelection Then
                Dim startRowHandle As Integer = GridControl.GetRowHandleByListIndex(ItemsSource.IndexOf(sourceManager.DraggingRows(0)))
                Dim endRowHandle As Integer = GridControl.GetRowHandleByListIndex(ItemsSource.IndexOf(sourceManager.DraggingRows(sourceManager.DraggingRows.Count - 1)))
                GridControl.SelectRange(startRowHandle, endRowHandle)
            Else
                GridControl.UnselectAll()
            End If
            If sourceManager.DraggingRows.Count > 0 Then
                GridControl.CurrentItem = sourceManager.DraggingRows(0)
            End If
        End Sub
        Private Sub MoveSelectedRowsToGroup(sourceManager As DragDropManagerBase, groupRowHandle As Integer, hitElement As DependencyObject, allowChangeSource As Boolean)
            Dim groupInfo As GroupInfo() = GetGroupInfos(groupRowHandle)
            If Not CanMoveSelectedRowsToGroup(sourceManager, groupInfo, hitElement) Then
                Return
            End If
            For Each obj As Object In sourceManager.DraggingRows
                For Each item As GroupInfo In groupInfo
                    DevExpress.Xpf.Grid.DragDrop.Utils.SetPropertyValue(sourceManager.GetObject(obj), item.FieldName, item.Value)
                Next
                If allowChangeSource AndAlso Not ItemsSource.Contains(obj) Then
                    sourceManager.RemoveObject(sourceManager.GetObject(obj))
                    ItemsSource.Add(sourceManager.GetObject(obj))
                End If
            Next
        End Sub
        'void MoveSelectedRowsToGroup(DragDropManagerBase sourceManager, int groupRowHandle, DependencyObject hitElement) {
        '    GroupInfo[] groupInfo = GetGroupInfos(groupRowHandle);
        '    if (!CanMoveSelectedRowsToGroup(sourceManager, groupInfo, hitElement))
        '        return;

        '    GridControl.BeginDataUpdate();
        '    foreach (object obj in sourceManager.DraggingRows) {
        '        foreach (GroupInfo item in groupInfo) {
        '            DevExpress.Xpf.Grid.DragDrop.Utils.SetPropertyValue(sourceManager.GetObject(obj), item.FieldName, item.Value);
        '        }
        '        if (!ItemsSource.Contains(obj)) {
        '            sourceManager.RemoveObject(sourceManager.GetObject(obj));
        '            ItemsSource.Add(sourceManager.GetObject(obj));
        '        }
        '    }
        '    GridControl.EndDataUpdate();
        '}
        Private Function CanMoveSelectedRowsToGroup(sourceManager As DragDropManagerBase, groupInfos As GroupInfo(), hitElement As DependencyObject) As Boolean
            If GetRowElement(hitElement) Is Nothing Then
                Return False
            Else
                Return Not IsSameGroup(sourceManager, groupInfos, hitElement)
            End If
        End Function

        Private Sub SetReorderDropInfo(sourceManager As DragDropManagerBase, insertRowHandle As Integer, hitElement As DependencyObject)
            Dim rowElement As FrameworkElement = GetRowElement(hitElement)
            Dim dragIndicatorPosition As TableDragIndicatorPosition = GetDragIndicatorPositionForRowElement(rowElement)
            If dragIndicatorPosition <> TableDragIndicatorPosition.None Then
                Dim dropTargetType__1 As DropTargetType = If(dragIndicatorPosition = TableDragIndicatorPosition.Bottom, DropTargetType.InsertRowsAfter, DropTargetType.InsertRowsBefore)
                sourceManager.SetDropTargetType(dropTargetType__1)
                sourceManager.ViewInfo.DropTargetRow = GridControl.GetRow(insertRowHandle)
                sourceManager.ShowDropMarker(rowElement, dragIndicatorPosition)
            Else
                ClearDragInfo(sourceManager)
            End If
        End Sub

        Private Function IsSameGroup(sourceManager As DragDropManagerBase, groupInfos As GroupInfo(), hitElement As DependencyObject) As Boolean
            'foreach (object obj in sourceManager.DraggingRows) {
            '    if (!ItemsSource.Contains(obj))
            '        return false;
            '    foreach (GroupInfo groupInfo in groupInfos) {
            '        object value;
            '        if (groupInfo.FieldName.Contains(".")) {
            '            ComplexPropertyDescriptor complexDescr = new ComplexPropertyDescriptor(obj, groupInfo.FieldName);
            '            value = complexDescr.GetValue(obj);
            '        }
            '        else
            '            value = TypeDescriptor.GetProperties(obj)[groupInfo.FieldName].GetValue(obj);
            '        if (!object.Equals(value, groupInfo.Value))
            '            return false;
            '    }
            '}
            'return true;
            For Each obj As Object In sourceManager.DraggingRows
                If Not ItemsSource.Contains(obj) Then
                    Return False
                End If
                For Each groupInfo As GroupInfo In groupInfos
                    If Not Object.Equals(DevExpress.Xpf.Grid.DragDrop.Utils.GetPropertyValue(obj, groupInfo.FieldName), groupInfo.Value) Then
                        Return False
                    End If
                Next
            Next
            Return True
        End Function
        Private Function GetGroupInfos(rowHandle As Integer) As GroupInfo()
            Dim rowLevel As Integer = GridControl.GetRowLevelByRowHandle(rowHandle)
            Dim groupInfo As GroupInfo() = New GroupInfo(rowLevel) {}
            Dim currentGroupRowHandle As Integer = rowHandle
            For i As Integer = rowLevel To 0 Step -1
                groupInfo(i) = New GroupInfo() With {
                .Value = GridControl.GetGroupRowValue(currentGroupRowHandle),
                .FieldName = GridControl.SortInfo(i).FieldName
            }
                currentGroupRowHandle = GridControl.GetParentRowHandle(currentGroupRowHandle)
            Next
            Return groupInfo
        End Function
        Private Function GetElementAcceptVisitor(hitElement As DependencyObject, visitor As DataViewHitTestVisitorBase) As FrameworkElement
            GetHitInfo(hitElement).Accept(visitor)
            Return TryCast(visitor, FindCardElementHitTestVisitorBase).StoredHitElement
        End Function
        Private Function CreateFindDataAreaElementHitTestVisitor(dataViewDragDropManager As DragDropManagerBase) As DataViewHitTestVisitorBase
            Return New FindCardViewDataAreaElementHitTestVisitor(dataViewDragDropManager)
        End Function
        Private Function CreateFindRowElementHitTestVisitor(dataViewDragDropManager As DragDropManagerBase) As DataViewHitTestVisitorBase
            Return New FindCardViewRowElementHitTestVisitor(dataViewDragDropManager)
        End Function

        Private Function GetVisibleHitTestElement(pt As Point) As UIElement
            Dim result As New DragDropHitTestResult(Me)
            VisualTreeHelper.HitTest(DataControl, Nothing, New HitTestResultCallback(AddressOf result.CallBack), New PointHitTestParameters(pt))
            Return result.Element
        End Function
        Private Function GetRowElement(hitElement As DependencyObject) As FrameworkElement
            Dim visitor As DataViewHitTestVisitorBase = CreateFindRowElementHitTestVisitor(Me)
            Return GetElementAcceptVisitor(hitElement, visitor)
        End Function
        Private Function GetDataAreaElement(hitElement As DependencyObject) As FrameworkElement
            Dim visitor As DataViewHitTestVisitorBase = CreateFindDataAreaElementHitTestVisitor(Me)
            Return GetElementAcceptVisitor(hitElement, visitor)
        End Function
        'void SetPropertyValue(object obj, string propertyName, object value) {
        'if (propertyName.Contains(".")) {
        '    DevExpress.Data.Access.ComplexPropertyDescriptor complexDesc = new DevExpress.Data.Access.ComplexPropertyDescriptor(obj, propertyName);
        '    complexDesc.SetValue(obj, value);
        '}
        'else
        '    TypeDescriptor.GetProperties(obj)[propertyName].SetValue(obj, value);
        '}
        Private Function GetPropertyValue(obj As Object, propertyName As String) As Object
            Return TypeDescriptor.GetProperties(obj)(propertyName).GetValue(obj)
        End Function
        Private Function GetDropTargetType(row As FrameworkElement) As DropTargetType
            Select Case GetDragIndicatorPositionForRowElement(row)
                Case TableDragIndicatorPosition.Top
                    Return DropTargetType.InsertRowsBefore
                Case TableDragIndicatorPosition.Bottom
                    Return DropTargetType.InsertRowsAfter
                Case TableDragIndicatorPosition.InRow
                    Return DropTargetType.InsertRowsIntoNode
                Case Else
                    Return DropTargetType.None
            End Select
        End Function
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
        'void SetDropMarkerVisibility(bool visible) {
        '    IsDropMarkerVisible = visible;
        '    if (!visible)
        '        HideDropMarker();
        '}
    End Class

End Namespace
