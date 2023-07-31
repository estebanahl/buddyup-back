using System.Buffers.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace buddyUp.DTOs
{
    public class PhotoInputDto
    {
        public string Image { get; set; } = string.Empty;     
        public string UserId { get; set; } = string.Empty;
    }
}
