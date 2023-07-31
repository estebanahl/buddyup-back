using buddyUp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace buddyUp.DTOs
{
    public class MatchOutDto
    {
        public int id { get; set; }
       
        public int matchedUserPid { get; set; }
             
    }
}
