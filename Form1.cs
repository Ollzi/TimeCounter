using System;
using System.Data;
using System.Windows.Forms;

namespace TimeCounter
{
    public partial class Timecounter : Form
    {
        private DataTable _table;

        public Timecounter()
        {
            InitializeComponent();
            _table = new DataTable();
            _table.Columns.Add(new DataColumn("Hours", typeof(int)));
            _table.Columns.Add(new DataColumn("Minutes", typeof(int)));
            _table.Columns.Add(new DataColumn("Total", typeof(int)));

            dgData.AutoGenerateColumns = false;
            dgData.DataSource = _table;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddRow();
        }

        private void AddRow()
        {
            var hours = string.IsNullOrEmpty(txtHours.Text) ? 0 : Convert.ToInt32(txtHours.Text);
            var minutes = string.IsNullOrEmpty(txtMinutes.Text) ? 0 : Convert.ToInt32(txtMinutes.Text);

            var row = _table.NewRow();
            row["Hours"] = hours;
            row["Minutes"] = minutes;
            row["Total"] = (60*hours) + minutes;

            _table.Rows.InsertAt(row, 0);

            UpdateTotal();

            txtHours.Text = string.Empty;
            txtMinutes.Text = string.Empty;
            txtHours.Focus();
        }

        private void UpdateTotal()
        {
            double totalMinutes = 0;

            foreach (DataRow row in _table.Rows)
            {
                totalMinutes = totalMinutes + Convert.ToDouble(row["Total"]);
            }

            var hours = Math.Floor((totalMinutes / 60));
            var minutes = totalMinutes - (hours*60);


            lblTotal.Text = string.Format("{0} hours, {1} minutes", hours, minutes);

            try
            {
                var hoursWorked = new TimeSpan(0, Convert.ToInt32(hours), Convert.ToInt32(minutes), 0);
                var startHour = Convert.ToInt32(txtStartTime.Text.Split(':')[0]);
                var startMinute = Convert.ToInt32(txtStartTime.Text.Split(':')[1]);
                var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startHour, startMinute, 0);
                var workHours = Convert.ToInt32(txtWorkDays.Text) * 8;

                var remaining = new TimeSpan(0, workHours, 0, 0) - hoursWorked;
                var endTime = startTime.Add(remaining).AddMinutes(30);
                lblEndTime.Text = hours > 8 && remaining.Hours < 8 ? endTime.ToShortTimeString() : "";
            }
            catch (Exception e)
            {
                lblEndTime.Text = e.Message;
            }
            

        }

        private void txtMinutes_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddRow();
            }
        }

        private void txtWorkDays_TextChanged(object sender, EventArgs e)
        {
            UpdateTotal();
        }

        private void txtStartTime_TextChanged(object sender, EventArgs e)
        {
            UpdateTotal();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            _table.Rows.Clear();
            UpdateTotal();
        }
    }
}
