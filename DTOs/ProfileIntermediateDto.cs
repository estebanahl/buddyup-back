namespace buddyUp.DTOs
{
    public class ProfileIntermediateDto
    {
        public int pid { get; set; }
        public string user_id { get; set; }
        public string pname { get; set; } = string.Empty;
        public string pquote { get; set; } = string.Empty;
        public string pbio { get; set; } = string.Empty;
        public int page { get; set; }
        public string pgender { get; set; } = string.Empty;
        //public double pdistance { get; set; }
    } 
}
