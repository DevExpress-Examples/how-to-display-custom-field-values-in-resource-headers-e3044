Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Data.OleDb
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data
Imports DevExpress.Xpf.Scheduler
Imports DevExpress.XtraScheduler

Namespace SchedulerResHeaderCustFieldWpf
	''' <summary>
	''' Interaction logic for MainWindow.xaml
	''' </summary>
	Partial Public Class MainWindow
		Inherits Window

		Private dataSet As New CarsDBDataSet()
		Private tableAdapterAppointments As New CarsDBDataSetTableAdapters.CarSchedulingTableAdapter()
		Private tableAdapterResources As New CarsDBDataSetTableAdapters.CarsTableAdapter()

		Public Sub New()
			InitializeComponent()

			tableAdapterAppointments.Fill(dataSet.CarScheduling)
			tableAdapterResources.Fill(dataSet.Cars)

			schedulerControl1.Storage.ResourceStorage.DataSource = dataSet.Cars
			schedulerControl1.Storage.AppointmentStorage.DataSource = dataSet.CarScheduling

			If schedulerControl1.Storage.AppointmentStorage.Count > 0 Then
				schedulerControl1.Start = schedulerControl1.Storage.AppointmentStorage(0).Start
			End If

			ApplyResourceHeadersStyle()

			AddHandler schedulerControl1.Storage.AppointmentsInserted, AddressOf Storage_AppointmentsModified
			AddHandler schedulerControl1.Storage.AppointmentsChanged, AddressOf Storage_AppointmentsModified
			AddHandler schedulerControl1.Storage.AppointmentsDeleted, AddressOf Storage_AppointmentsModified

			AddHandler tableAdapterAppointments.Adapter.RowUpdated, AddressOf adapter_RowUpdated
		End Sub

		Private Sub ApplyResourceHeadersStyle()
			Dim verticalResourceHeaderStyle As Style = CType(Me.FindResource("VerticalResourceHeaderStyle"), Style)
			schedulerControl1.Views.TimelineView.VerticalResourceHeaderStyle = verticalResourceHeaderStyle
			Dim resourceIdConverter As ResourceIdConverter = CType(verticalResourceHeaderStyle.Resources("resourceIdConverter"), ResourceIdConverter)
			resourceIdConverter.Storage = schedulerControl1.Storage
			resourceIdConverter.CustomFieldName = "WebSite"
		End Sub

		#Region "Data-Related Events"

		Private Sub Storage_AppointmentsModified(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs)
			Me.tableAdapterAppointments.Adapter.Update(Me.dataSet)
			Me.dataSet.AcceptChanges()
		End Sub

		Private Sub adapter_RowUpdated(ByVal sender As Object, ByVal e As System.Data.OleDb.OleDbRowUpdatedEventArgs)
			If e.Status = UpdateStatus.Continue AndAlso e.StatementType = StatementType.Insert Then
				Dim id As Integer = 0
				Using cmd As New OleDbCommand("SELECT @@IDENTITY", tableAdapterAppointments.Connection)
					id = CInt(Fix(cmd.ExecuteScalar()))
				End Using
				e.Row("ID") = id
			End If
		End Sub

		#End Region ' Data-Related Events

	End Class

	Public Class ResourceIdConverter
		Implements IValueConverter
		Private privateStorage As SchedulerStorage
		Public Property Storage() As SchedulerStorage
			Get
				Return privateStorage
			End Get
			Set(ByVal value As SchedulerStorage)
				privateStorage = value
			End Set
		End Property
		Private privateCustomFieldName As String
		Public Property CustomFieldName() As String
			Get
				Return privateCustomFieldName
			End Get
			Set(ByVal value As String)
				privateCustomFieldName = value
			End Set
		End Property

		#Region "IValueConverter Members"
		Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
			Return Storage.ResourceStorage.Items.Find(Function(e) System.Convert.ToInt32(e.Id) = System.Convert.ToInt32(value)).CustomFields(CustomFieldName)
		End Function

		Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
			Throw New NotImplementedException()
		End Function
		#End Region
	End Class

End Namespace
