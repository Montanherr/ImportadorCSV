using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebContracts.Models
{
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("user_email")]
        public string UserEmail { get; set; }

        [Column("user_passwordHash")]
        public string UserPasswordHash { get; set; }
    }
}
