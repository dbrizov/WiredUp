using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WiredUpWebApi.Models
{
    public class Connection
    {
        [Key]
        [Column("ConnectionId")]
        public int Id { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public int OtherUserId { get; set; }

        public virtual User OtherUser { get; set; }
    }
}
