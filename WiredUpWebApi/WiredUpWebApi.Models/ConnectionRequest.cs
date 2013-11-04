using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class ConnectionRequest
    {
        [Key]
        [Column("ConnectionRequestId")]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        [MaxLength(ConnectionRequestConstants.SenderDisplayNameMaxLength)]
        public string SenderDisplayName { get; set; }

        public byte[] SenderPhoto { get; set; }

        public int ReceiverId { get; set; }

        public virtual User Receiver { get; set; }
    }
}
