using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using buddyUp.Models;
using Newtonsoft.Json.Serialization;

namespace buddyUp.Models
{
    public class Photo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [JsonIgnore]
        public string Image { get; set; }= string.Empty; 
        [ForeignKey("User")]
        public int UserProfileId { get; set; }
        [ForeignKey("UserProfileId")]
        [JsonIgnore]
        [Column("userOwnerId")]
        public Profile UserOwner { get; set; } = null!;
    }
}
