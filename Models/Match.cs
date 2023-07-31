using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace buddyUp.Models
{
    public class Match
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int id { get; set; }
        [ForeignKey("user1")]
        public int userp1_id { get; set; }
        [ForeignKey("userp1_id")]
        [JsonIgnore]
        public Profile user1 { get; set; }
        [ForeignKey("user2")]
        public int userp2_id { get; set; }
        [ForeignKey("userp2_id")]
        [JsonIgnore]
        public Profile user2 { get; set; }
        public bool isMatch { get; set; }
    }
}
