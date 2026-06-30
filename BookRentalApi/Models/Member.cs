namespace BookRentalApi.Models
{
    public class Member
    {
        public int MemberIdx { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string Levels { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
