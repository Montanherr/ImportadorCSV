using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebContracts.Models
{
    public class Import
    {
        [Key]
        public int im_id { get; set; }

        public DateTime im_dateImport { get; set; } = DateTime.Now;

        public string im_fileName { get; set; }

        public int user_id { get; set; }

        [ForeignKey("user_id")]
        public User User { get; set; }

        public ICollection<Contract> Contracts { get; set; }
    }
}
