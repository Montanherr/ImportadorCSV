using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using WebContracts.Models;

public class ContractImport
{
    
    /// Lê um arquivo CSV e converte cada linha em um objeto do tipo ContractCsv.
   
    /// <param name="filePath">Caminho completo do arquivo CSV a ser lido.</param>
    /// <returns>Uma lista de contratos convertidos do arquivo CSV.</returns>
    public List<ContractCsv> ReadContractsFromCsv(string filePath)
    {
        var config = new CsvConfiguration(new CultureInfo("pt-BR"))
        {
            Delimiter = ";",
            HeaderValidated = null,
            MissingFieldFound = null,
            TrimOptions = TrimOptions.Trim // evitar espaços
        };

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Context.RegisterClassMap<ContractCsvMaps>(); // Classe referenciada do mapeamento. 
            var records = csv.GetRecords<ContractCsv>().ToList();
            return records;
        }
    }

}
