using ProjectSolutisDevTrail.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace ProjectSolutisDevTrail.Services.Implementations;

public class ReportService
{
    public void GeneratePdf(Evento evento, List<Participante> participantes, string filePath)
    {
        using (PdfWriter writer = new PdfWriter(filePath))
        using (PdfDocument pdf = new PdfDocument(writer))
        {
            Document document = new Document(pdf);

            // Adicionando título do evento
            document.Add(new Paragraph($"Título do Evento: {evento.Titulo}").SetBold().SetFontSize(14));
            document.Add(new Paragraph($"Descrição: {evento.Descricao}"));
            document.Add(new Paragraph($"Local: {evento.Local}"));
            document.Add(new Paragraph($"Data e Hora: {evento.DataHora.ToString("dd/MM/yyyy HH:mm")}"));
            document.Add(new Paragraph("\nParticipantes:"));

            // Criando uma tabela para os participantes
            Table table = new Table(3);
            table.AddHeaderCell("Nome");
            table.AddHeaderCell("Sobrenome");
            table.AddHeaderCell("E-mail");

            foreach (var participante in participantes)
            {
                table.AddCell(participante.Nome);
                table.AddCell(participante.Sobrenome);
                table.AddCell(participante.Email);
            }

            document.Add(table);
            document.Close();
        }
    }
}