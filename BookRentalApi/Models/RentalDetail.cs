namespace BookRentalApi.Models
{
    public class RentalDetail
    {
        public int RentalIdx { get; set; }

        public int MemberIdx { get; set; }
        public string MemberName { get; set; } = string.Empty;

        public int BookIdx { get; set; }
        public string BookName { get; set; } = string.Empty;

        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
