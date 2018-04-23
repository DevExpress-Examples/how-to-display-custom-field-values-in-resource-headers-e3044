using System;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Scheduler;
using DevExpress.XtraScheduler;

namespace SchedulerResHeaderCustFieldWpf {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private CarsDBDataSet dataSet = new CarsDBDataSet();
        private CarsDBDataSetTableAdapters.CarSchedulingTableAdapter tableAdapterAppointments = new CarsDBDataSetTableAdapters.CarSchedulingTableAdapter();
        private CarsDBDataSetTableAdapters.CarsTableAdapter tableAdapterResources = new CarsDBDataSetTableAdapters.CarsTableAdapter();

        public MainWindow() {
            InitializeComponent();

            tableAdapterAppointments.Fill(dataSet.CarScheduling);
            tableAdapterResources.Fill(dataSet.Cars);

            schedulerControl1.Storage.ResourceStorage.DataSource = dataSet.Cars;
            schedulerControl1.Storage.AppointmentStorage.DataSource = dataSet.CarScheduling;

            if (schedulerControl1.Storage.AppointmentStorage.Count > 0)
                schedulerControl1.Start = schedulerControl1.Storage.AppointmentStorage[0].Start;

            ApplyResourceHeadersStyle();

            schedulerControl1.Storage.AppointmentsInserted +=
                new PersistentObjectsEventHandler(Storage_AppointmentsModified);
            schedulerControl1.Storage.AppointmentsChanged +=
                new PersistentObjectsEventHandler(Storage_AppointmentsModified);
            schedulerControl1.Storage.AppointmentsDeleted +=
                new PersistentObjectsEventHandler(Storage_AppointmentsModified);

            tableAdapterAppointments.Adapter.RowUpdated +=
                new System.Data.OleDb.OleDbRowUpdatedEventHandler(adapter_RowUpdated);
        }

        private void ApplyResourceHeadersStyle() {
            Style verticalResourceHeaderStyle = (Style)this.FindResource("VerticalResourceHeaderStyle");
            schedulerControl1.Views.TimelineView.VerticalResourceHeaderStyle = verticalResourceHeaderStyle;
            ResourceIdConverter resourceIdConverter = (ResourceIdConverter)verticalResourceHeaderStyle.Resources["resourceIdConverter"];
            resourceIdConverter.Storage = schedulerControl1.Storage;
            resourceIdConverter.CustomFieldName = "WebSite";
        }

        #region Data-Related Events

        void Storage_AppointmentsModified(object sender, PersistentObjectsEventArgs e) {
            this.tableAdapterAppointments.Adapter.Update(this.dataSet);
            this.dataSet.AcceptChanges();
        }

        private void adapter_RowUpdated(object sender, System.Data.OleDb.OleDbRowUpdatedEventArgs e) {
            if (e.Status == UpdateStatus.Continue && e.StatementType == StatementType.Insert) {
                int id = 0;
                using (OleDbCommand cmd = new OleDbCommand("SELECT @@IDENTITY", tableAdapterAppointments.Connection)) {
                    id = (int)cmd.ExecuteScalar();
                }
                e.Row["ID"] = id;
            }
        }

        #endregion Data-Related Events

    }

    public class ResourceIdConverter : IValueConverter {
        public SchedulerStorage Storage { get; set; }
        public string CustomFieldName { get; set; }

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return Storage.ResourceStorage.Items.Find(e => System.Convert.ToInt32(e.Id) == System.Convert.ToInt32(value)).CustomFields[CustomFieldName];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
        #endregion
    }

}
