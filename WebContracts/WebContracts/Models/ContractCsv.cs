using System;

namespace WebContracts.Models
{
    public class ContractCsv
    {
        public string customerName { get; set; }
        public string cpf { get; set; }
        public string contractNumber { get; set; }
        public string product { get; set; }
        public DateTime dueDate { get; set; }
        public decimal amount { get; set; } 
    }
}
