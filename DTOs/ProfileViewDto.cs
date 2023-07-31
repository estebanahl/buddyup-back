namespace buddyUp.DTOs
{
    public class ProfileViewDto
    {
        public int id { get; set; }
        public string user_id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string quote { get; set; } = string.Empty;
        public string bio { get; set; } = string.Empty;
        public int age { get; set; }
        public string gender { get; set; } = string.Empty;
        //public int distance_in_km { get; set; }
        public List<string> tags { get; set; } = null!;
        public List<PhotoViewDto> photos { get; set; } = null!;
    }
}
