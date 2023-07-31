using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace buddyUp.Models
{
    [Table("Message")]
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int mId { get; set; }
        [ForeignKey("Chat")]
        public int chatId { get; set; }
        [JsonIgnore]
        [ForeignKey("chatId")]
        public Match Chat { get; set; } = null!;
        public int senderPId { get; set; }
        public string text { get; set; } = string.Empty;    
        public DateTime timestamp { get; set; }
    }
}
