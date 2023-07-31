
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace buddyUp.Models
{
    [Table("Tag")]
    public class Tag
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string name { get; set; } = String.Empty;
        //public List<Profile> UsersWithInterest { get; set; } = new();
        [Column("profileId")]
        [JsonIgnore]        
        public List<Profile> UsersWithInterest { get; set; } = null;
    }
}
