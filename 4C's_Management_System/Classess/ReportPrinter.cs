using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms; 

namespace _4C_s_Management_System
{
    public static class ReportPrinter
    {
        /// <summary>Opens the Windows print preview for a table report.</summary>
        public static void Preview(DataTable data, string title, string subtitle = null)
        {
            using (var document = CreateDocument(data, title, subtitle))
            using (var preview = new PrintPreviewDialog { Document = document, Width = 1000, Height = 750 })
                preview.ShowDialog();
        }

        /// <summary>Shows the Windows printer dialog and prints a table report.</summary>
        public static void Print(DataTable data, string title, string subtitle = null)
        {
            using (var document = CreateDocument(data, title, subtitle))
            using (var dialog = new PrintDialog { Document = document, UseEXDialog = true })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    document.Print();
            }
        }

        internal static PrintDocument CreateDocument(DataTable data, string title, string subtitle)
        {
            var document = new PrintDocument
            {
                DocumentName = string.IsNullOrWhiteSpace(title) ? "Report" : title
            };
            document.DefaultPageSettings.Margins = new Margins(44, 44, 52, 44);
            document.DefaultPageSettings.Landscape = data != null && data.Columns.Count >= 6;

            var currentRow = 0;
            var pageNumber = 1;
            document.BeginPrint += (sender, args) => { currentRow = 0; pageNumber = 1; };
            document.PrintPage += (sender, args) =>
            {
                var result = PrintPageRenderer.Render(
                    args.Graphics,
                    args.MarginBounds,
                    data,
                    title,
                    subtitle,
                    currentRow,
                    pageNumber);
                currentRow = result.NextRow;
                args.HasMorePages = result.HasMorePages;
                pageNumber++;
            };
            return document;
        }
    }

    internal sealed class ReportPageResult
    {
        internal int NextRow;
        internal bool HasMorePages;
    }

    internal static class PrintPageRenderer
    {
        private const float HeaderHeight = 25f;
        private const float RowHeight = 25f;
        private const float FooterHeight = 20f;

        internal static ReportPageResult Render(
            Graphics graphics,
            Rectangle bounds,
            DataTable data,
            string title,
            string subtitle,
            int firstRow,
            int pageNumber)
        {
            if (graphics == null) throw new ArgumentNullException(nameof(graphics));
            var result = new ReportPageResult { NextRow = Math.Max(0, firstRow) };
            var left = (float)bounds.Left;
            var top = (float)bounds.Top;
            var width = Math.Max(1f, bounds.Width);
            var footerTop = bounds.Bottom - FooterHeight;

            DrawTitle(graphics, left, top, width, title, subtitle);
            top += string.IsNullOrWhiteSpace(subtitle) ? 42f : 58f;

            using (var footerFont = new Font("Segoe UI", 8f))
            using (var footerBrush = new SolidBrush(Color.DimGray))
            using (var footerFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center })
                graphics.DrawString("Page " + pageNumber, footerFont, footerBrush,
                    new RectangleF(left, footerTop, width, FooterHeight), footerFormat);

            if (data == null || data.Columns.Count == 0 || data.Rows.Count == 0)
            {
                using (var emptyFont = new Font("Segoe UI", 10f))
                using (var emptyBrush = new SolidBrush(Color.Gray))
                    graphics.DrawString("No data to display.", emptyFont, emptyBrush, left, top + 12f);
                result.HasMorePages = false;
                return result;
            }

            var columnCount = data.Columns.Count;
            var columnWidth = width / columnCount;
            var bodyFontSize = Math.Max(6f, 9f - Math.Max(0, columnCount - 6) * 0.35f);
            DrawTableHeader(graphics, data, left, top, width, columnWidth, bodyFontSize);
            top += HeaderHeight;

            if (top + RowHeight > footerTop)
                throw new InvalidOperationException("The page is too small to print a report row.");
            var rowIndex = Math.Max(0, firstRow);
            while (rowIndex < data.Rows.Count && top + RowHeight <= footerTop)
            {
                var rowBounds = new RectangleF(left, top, width, RowHeight);
                if (rowIndex % 2 == 1)
                {
                    using (var alternate = new SolidBrush(Color.FromArgb(245, 247, 250)))
                        graphics.FillRectangle(alternate, rowBounds);
                }
                DrawRow(graphics, data.Rows[rowIndex], left, top, columnWidth, bodyFontSize);
                using (var line = new Pen(Color.FromArgb(220, 225, 230)))
                    graphics.DrawLine(line, left, top + RowHeight, left + width, top + RowHeight);
                top += RowHeight;
                rowIndex++;
            }

            result.NextRow = rowIndex;
            result.HasMorePages = rowIndex < data.Rows.Count;
            return result;
        }

        private static void DrawTitle(Graphics graphics, float left, float top, float width, string title, string subtitle)
        {
            using (var titleFont = new Font("Segoe UI", 18f, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.FromArgb(31, 55, 78)))
                graphics.DrawString(string.IsNullOrWhiteSpace(title) ? "Report" : title, titleFont, titleBrush,
                    new RectangleF(left, top, width, 28f));

            if (!string.IsNullOrWhiteSpace(subtitle))
                using (var subtitleFont = new Font("Segoe UI", 9f))
                using (var subtitleBrush = new SolidBrush(Color.DimGray))
                    graphics.DrawString(subtitle, subtitleFont, subtitleBrush,
                        new RectangleF(left, top + 30f, width, 18f));
        }

        private static void DrawTableHeader(Graphics graphics, DataTable data, float left, float top, float width, float columnWidth, float fontSize)
        {
            using (var background = new SolidBrush(Color.FromArgb(37, 78, 112)))
                graphics.FillRectangle(background, left, top, width, HeaderHeight);
            using (var headerFont = new Font("Segoe UI", fontSize, FontStyle.Bold))
            using (var headerBrush = new SolidBrush(Color.White))
            using (var format = CellFormat())
            {
                for (var column = 0; column < data.Columns.Count; column++)
                {
                    var cell = new RectangleF(left + column * columnWidth + 5f, top + 2f,
                        Math.Max(1f, columnWidth - 10f), HeaderHeight - 4f);
                    graphics.DrawString(data.Columns[column].ColumnName, headerFont, headerBrush, cell, format);
                }
            }
        }

        private static void DrawRow(Graphics graphics, DataRow row, float left, float top, float columnWidth, float fontSize)
        {
            using (var bodyFont = new Font("Segoe UI", fontSize))
            using (var bodyBrush = new SolidBrush(Color.FromArgb(40, 47, 54)))
            using (var format = CellFormat())
            {
                for (var column = 0; column < row.Table.Columns.Count; column++)
                {
                    var cell = new RectangleF(left + column * columnWidth + 5f, top + 2f,
                        Math.Max(1f, columnWidth - 10f), RowHeight - 4f);
                    graphics.DrawString(FormatValue(row[column]), bodyFont, bodyBrush, cell, format);
                }
            }
        }

        private static StringFormat CellFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };
        }

        private static string FormatValue(object value)
        {
            if (value == null || value == DBNull.Value) return string.Empty;
            if (value is DateTime date) return date.ToString("d");
            if (value is decimal number) return number.ToString("N2");
            return Convert.ToString(value);
        }
    }
}
