using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace buddyUp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [JsonIgnore] // eliminable
        public Profile? UserProfile { get; set; }
    }
}
