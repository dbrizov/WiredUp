using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WiredUpWebApi.Models
{
    public class ConnectionRequest
    {
        [Key]
        [Column("ConnectionRequestId")]
        public int Id { get; set; }

        public int SenderId { get; set; }

        public virtual User Sender { get; set; }

        public int ReceiverId { get; set; }

        public virtual User Receiver { get; set; }
    }
}
