using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using WebContracts.Models;

public class ContractImport
{
    /// <summary>
    /// Lê um arquivo CSV e converte cada linha em um objeto do tipo ContractCsv.
    /// </summary>
    /// <param name="filePath">Caminho completo do arquivo CSV a ser lido.</param>
    /// <returns>Uma lista de contratos convertidos do arquivo CSV.</returns>
    public List<ContractCsv> ReadContractsFromCsv(string filePath)
    {
        // Configura o leitor CSV para usar a cultura pt-BR, com delimitador ";"
        var config = new CsvConfiguration(new CultureInfo("pt-BR"))
        {
            Delimiter = ";", // Espera arquivos com separador ponto e vírgula
            HeaderValidated = null,       // Ignora validação de cabeçalho
            MissingFieldFound = null      // Ignora campos ausentes
        };

        // Abre o arquivo e inicializa o leitor CSV
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, config))
        {
            // Converte cada linha em um objeto ContractCsv
            var records = csv.GetRecords<ContractCsv>().ToList();

            // Retorna todos os contratos lidos
            return records;
        }
    }
}
