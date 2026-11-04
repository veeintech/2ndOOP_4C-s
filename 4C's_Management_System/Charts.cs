using _4C_s_Management_System;
using Guna.Charts.Interfaces;
using Guna.Charts.WinForms;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;

namespace _4C_s_Management_System
{
    public static class Charts
    {
        private static readonly Color[] DefaultDoughnutColors =
        {
            Color.Maroon,
            Color.Goldenrod,
            Color.LightGray,
            Color.SteelBlue,
            Color.SeaGreen
        };

        public static GunaBarDataset? BindBar(
            GunaChart chart,
            DataTable? data,
            string categoryColumn,
            string valueColumn,
            string datasetLabel,
            Color? barColor = null)
        {
            ArgumentNullException.ThrowIfNull(chart);

            LPoint[] points = GetPoints(data, categoryColumn, valueColumn);
            chart.Datasets.Clear();

            if (points.Length == 0)
                return null;

            var dataset = new GunaBarDataset
            {
                Label = datasetLabel,
                TargetChart = chart
            };

            dataset.DataPoints.AddRange(points);

            var colors = new Color[points.Length];
            Array.Fill(colors, barColor ?? Color.Maroon);
            dataset.FillColors.AddRange(colors);

            chart.Datasets.Add(dataset);
            return dataset;
        }

        public static GunaDoughnutDataset? BindDoughnut(
            GunaChart chart,
            DataTable? data,
            string categoryColumn,
            string valueColumn,
            string datasetLabel)
        {
            ArgumentNullException.ThrowIfNull(chart);

            LPoint[] points = GetPoints(data, categoryColumn, valueColumn);
            chart.Datasets.Clear();

            if (points.Length == 0)
                return null;

            var dataset = new GunaDoughnutDataset
            {
                Label = datasetLabel,
                TargetChart = chart
            };

            dataset.DataPoints.AddRange(points);

            var colors = new Color[points.Length];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = DefaultDoughnutColors[i % DefaultDoughnutColors.Length];

            dataset.FillColors.AddRange(colors);
            chart.Datasets.Add(dataset);
            return dataset;
        }

        private static LPoint[] GetPoints(
            DataTable? data,
            string categoryColumn,
            string valueColumn)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(categoryColumn);
            ArgumentException.ThrowIfNullOrWhiteSpace(valueColumn);

            if (data is null || data.Rows.Count == 0)
                return Array.Empty<LPoint>();

            if (!data.Columns.Contains(categoryColumn))
                throw new ArgumentException(
                    $"Column '{categoryColumn}' was not found.", nameof(categoryColumn));

            if (!data.Columns.Contains(valueColumn))
                throw new ArgumentException(
                    $"Column '{valueColumn}' was not found.", nameof(valueColumn));

            var points = new System.Collections.Generic.List<LPoint>();

            foreach (DataRow row in data.Rows)
            {
                if (row.IsNull(categoryColumn) || row.IsNull(valueColumn))
                    continue;

                points.Add(new LPoint
                {
                    Label = Convert.ToString(
                        row[categoryColumn], CultureInfo.CurrentCulture) ?? string.Empty,
                    Y = Convert.ToDouble(
                        row[valueColumn], CultureInfo.CurrentCulture)
                });
            }

            return points.ToArray();
        }
    }
}




//private void UserControlDB_Load(object sender, EventArgs e)
//{
//    var monthlyIncome = new DataTable();
//    monthlyIncome.Columns.Add("Month", typeof(string));
//    monthlyIncome.Columns.Add("Income", typeof(double));

//    monthlyIncome.Rows.Add("Nov", 13000d);
//    monthlyIncome.Rows.Add("Dec", 18000d);
//    monthlyIncome.Rows.Add("Jan", 20000d);

//    Charts.BindBar(
//        gunaChart3,
//        monthlyIncome,
//        "Month",
//        "Income",
//        "Monthly Income Overview");

//    var occupancy = new DataTable();
//    occupancy.Columns.Add("Status", typeof(string));
//    occupancy.Columns.Add("Count", typeof(double));

//    occupancy.Rows.Add("Occupied", 26d);
//    occupancy.Rows.Add("Available", 18d);
//    occupancy.Rows.Add("Reserved", 24d);

//    Charts.BindDoughnut(
//        gunaChart1,
//        occupancy,
//        "Status",
//        "Count",
//        "Room Occupancy");
//}