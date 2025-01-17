using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DotnetBackend.Models;
using iText.IO.Font.Constants;

namespace DotnetBackend.Services
{
    public class RelatorioService
    {
        private readonly ClientService _clientService;
        private readonly PurchaseService _purchaseService;

        public RelatorioService(ClientService clientService, PurchaseService purchaseService)
        {
            _clientService = clientService;
            _purchaseService = purchaseService;
        }

        public async Task<MemoryStream> GenerateClientReportPdfAsync(List<Client> clients)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var pdfWriter = new PdfWriter(memoryStream))
                {
                    using (var pdf = new PdfDocument(pdfWriter))
                    {
                        var document = new Document(pdf);

                        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                        document.Add(new Paragraph("Relatório de Clientes")
                            .SetFont(boldFont)
                            .SetFontSize(20)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)); // Centraliza o título

                        var table = new Table(new float[] { 3, 3, 3, 3 })
                            .SetWidth(UnitValue.CreatePercentValue(100)); // Define a largura da tabela para 100%

                        table.AddHeaderCell("Nome");
                        table.AddHeaderCell("ID");
                        table.AddHeaderCell("Data de Cadastro");
                        table.AddHeaderCell("Telefone");

                        foreach (var client in clients)
                        {
                            table.AddCell(client.Name);
                            table.AddCell(client.Id);
                            table.AddCell(client.DateCreated.ToString("dd/MM/yyyy"));
                            table.AddCell(client.Phone);
                        }

                        document.Add(table);
                        document.Close();
                    }
                }

                return memoryStream;
            }
        }

        public async Task<MemoryStream> GeneratePurchaseReportPdfAsync(List<Purchase> purchases)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var pdfWriter = new PdfWriter(memoryStream))
                {
                    using (var pdf = new PdfDocument(pdfWriter))
                    {
                        var document = new Document(pdf);

                        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                        document.Add(new Paragraph("Relatório de Compras")
                            .SetFont(boldFont)
                            .SetFontSize(20)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                        var table = new Table(new float[] { 3, 3, 3, 3, 3 })
                            .SetWidth(UnitValue.CreatePercentValue(100)); // Define a largura da tabela para 100%

                        table.AddHeaderCell("ID");
                        table.AddHeaderCell("Cliente Id");
                        table.AddHeaderCell("Data da Compra");
                        table.AddHeaderCell("Valor De Compra");
                        table.AddHeaderCell("Valor Pago");

                        foreach (var purchase in purchases)
                        {
                            table.AddCell(purchase.PurchaseId);
                            table.AddCell(purchase.ClientId);
                            table.AddCell(purchase.PurchaseDate?.ToString("dd/MM/yyyy"));
                            table.AddCell("R$" + purchase.TotalPrice.ToString("").Replace(".", ","));
                            table.AddCell("R$" + purchase.AmountPaid.ToString("").Replace(".", ","));
                        }

                        document.Add(table);
                        document.Close();
                    }
                }

                return memoryStream;
            }
        }

        public async Task<MemoryStream> GenerateWithdrawalReportPdfAsync(List<Withdrawal> withdrawals)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var pdfWriter = new PdfWriter(memoryStream))
                {
                    using (var pdf = new PdfDocument(pdfWriter))
                    {
                        var document = new Document(pdf);

                        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                        document.Add(new Paragraph("Relatório de Saques")
                            .SetFont(boldFont)
                            .SetFontSize(20)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                        var table = new Table(new float[] { 3, 3, 3, 3, 3, 3 })
                            .SetWidth(UnitValue.CreatePercentValue(100)); // Define a largura da tabela para 100%

                        table.AddHeaderCell("ID");
                        table.AddHeaderCell("Cliente Id");
                        table.AddHeaderCell("Data do Saque");
                        table.AddHeaderCell("Valor");
                        table.AddHeaderCell("Valor Recebível");
                        table.AddHeaderCell("Retirado De");

                        foreach (var withdrawal in withdrawals)
                        {
                            table.AddCell(withdrawal.WithdrawalId);
                            table.AddCell(withdrawal.ClientId);
                            table.AddCell(withdrawal.DateCreated?.ToString("dd/MM/yyyy"));
                            table.AddCell("R$" + withdrawal.AmountWithdrawn.ToString("").Replace(".", ","));
                            table.AddCell("R$" + withdrawal.AmountWithdrawnReceivable?.ToString("").Replace(".", ","));
                            table.AddCell(string.Join(", ", withdrawal.WithdrawnItems));
                        }

                        document.Add(table);
                        document.Close();
                    }
                }

                return memoryStream;
            }
        }
    }
}