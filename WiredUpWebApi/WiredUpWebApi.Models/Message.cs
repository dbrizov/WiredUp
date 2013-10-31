using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(MessageConstants.ContentMaxLength)]
        public string Content { get; set; }

        public int SenderId { get; set; }

        public virtual User Sender { get; set; }

        public int ReceiverId { get; set; }

        public virtual User Receiver { get; set; }
    }
}
