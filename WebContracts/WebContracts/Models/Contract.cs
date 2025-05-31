using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebContracts.Converters;

namespace WebContracts.Models
{
    public class Contract
    {
        internal DateTime con_dueDate;

        [Key]
        public int con_id { get; set; }

        public int im_id { get; set; }

        public string con_customerName { get; set; }
        public string con_cpf { get; set; }
        public string con_contractNumber { get; set; }
        public string con_product { get; set; }

        [TypeConverter(typeof(CustomDateTimeConverter))]
        public DateTime dueDate { get; set; }
        public decimal con_amount { get; set; }


        [ForeignKey("im_id")]
        public Import Import { get; set; }
    }
}
