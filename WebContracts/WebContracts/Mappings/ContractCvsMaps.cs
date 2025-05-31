using CsvHelper.Configuration;
using System.Globalization;
using WebContracts.Models;

public class ContractCsvMaps : ClassMap<ContractCsv>
{
    public ContractCsvMaps()
    {
        Map(m => m.customerName).Name("customerName");
        Map(m => m.cpf).Name("cpf");
        Map(m => m.contractNumber).Name("contractNumber");
        Map(m => m.product).Name("product");
        Map(m => m.dueDate).Name("dueDate").TypeConverterOption.Format("dd/MM/yyyy");
        Map(m => m.amount).Name("amount");
    }
}
