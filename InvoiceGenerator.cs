using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinUi_Inventory_Management.Models;

namespace WinUi_Inventory_Management
{
    public class InvoiceGenerator
    {
        public static void GenerateInvoicePDF(Order order, User user, Stream stream)
        {
            Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            // Fonts
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.Black);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.Black);
            var tableHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.White);

            // Company Header
            PdfPTable headerTable = new PdfPTable(2) { WidthPercentage = 100 };
            headerTable.SetWidths(new float[] { 70f, 30f });

            PdfPCell companyCell = new PdfPCell();
            companyCell.Border = Rectangle.NO_BORDER;
            companyCell.AddElement(new Paragraph("Retail Rythm", titleFont));
            headerTable.AddCell(companyCell);

            PdfPCell invoiceCell = new PdfPCell();
            invoiceCell.Border = Rectangle.NO_BORDER;
            invoiceCell.AddElement(new Paragraph($"Invoice #{order.Id}", titleFont));
            invoiceCell.AddElement(new Paragraph($"Date: {order.CreatedAt:dd/MM/yyyy}", normalFont));
            headerTable.AddCell(invoiceCell);

            doc.Add(headerTable);
            doc.Add(new Paragraph("\n"));

            // Table
            PdfPTable table = new PdfPTable(5) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 40f, 10f, 15f, 15f, 20f });

            AddCellToHeader(table, "Product Name", tableHeaderFont, BaseColor.Gray);
            AddCellToHeader(table, "Qty", tableHeaderFont, BaseColor.Gray);
            AddCellToHeader(table, "Price", tableHeaderFont, BaseColor.Gray);
            AddCellToHeader(table, "Discount", tableHeaderFont, BaseColor.Gray);
            AddCellToHeader(table, "Total", tableHeaderFont, BaseColor.Gray);

            foreach (var item in order.Items)
            {
                AddCellToBody(table, item.ItemName, normalFont);
                AddCellToBody(table, item.ItemQuantity.ToString(), normalFont);
                AddCellToBody(table, $"{item.ItemPrice.ToString("0.00")}", normalFont);
                AddCellToBody(table, item.Discount.ToString("0.00"), normalFont);
                AddCellToBody(table, item.Total.ToString("0.00"), normalFont);
            }

            doc.Add(table);

            // Grand Total and user info
            doc.Add(new Paragraph("\n"));
            var totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.Black);
            doc.Add(new Paragraph($"Issued to:\n"));
            doc.Add(new Paragraph($"{user.FullName}\n{user.Email}\n", normalFont));
            doc.Add(new Paragraph($"Grand Total: {order.Items.Sum(i => i.Total):0.00}", totalFont));
            doc.Add(new Paragraph("\nThank you for your business!", normalFont));

            doc.Close();
        }

        // Helper methods for table
        private static void AddCellToHeader(PdfPTable table, string text, Font font, BaseColor backgroundColor)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.BackgroundColor = backgroundColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private static void AddCellToBody(PdfPTable table, string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        internal static void GenerateInvoicePDF(List<Order> orders, User loggedInUser, Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
