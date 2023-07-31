using buddyUp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace buddyUp.DTOs
{
    public class ProfileSimple
    {
        public string uid { get; set; } = string.Empty;
        public string uemail { get; set; } = string.Empty;
        public int pid { get; set; }
        public string pname { get; set; } = string.Empty;
        public string pquote { get; set; } = string.Empty;
        public string pbio { get; set; } = string.Empty;
        public int page { get; set; } 
        public string pgender { get; set; } = string.Empty;
        public List<PhotoViewDto> images { get; set; } = null!;
        public List<Tag> tags { get; set; } = null!;
    }
}
