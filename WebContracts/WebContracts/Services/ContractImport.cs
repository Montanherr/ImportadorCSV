using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using WebContracts.Models;

public class ContractImport
{

    public List<ContractCsv> ReadContractsFromCsv(string filePath)
    {
        Console.WriteLine("Conteúdo do CSV:");
        Console.WriteLine(File.ReadAllText(filePath));

        var config = new CsvConfiguration(new CultureInfo("pt-BR"))
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null,
            BadDataFound = null // Adicione isso para ignorar caracteres inválidos, como "imobili�rio"
        };

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Context.RegisterClassMap<ContractCsvMaps>();
            var records = csv.GetRecords<ContractCsv>().ToList();
            return records;
        }
    }
}
