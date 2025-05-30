using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebContracts.Models
{
    public class Contract
    {
        [Key]
        public int con_id { get; set; }

        public int im_id { get; set; }

        public string con_customerName { get; set; }
        public string con_cpf { get; set; }
        public string con_contractNumber { get; set; }
        public string con_product { get; set; }
        public DateTime con_dueDate { get; set; }
        public decimal con_mount { get; set; }

        [ForeignKey("im_id")]
        public Import Import { get; set; }
    }
}
