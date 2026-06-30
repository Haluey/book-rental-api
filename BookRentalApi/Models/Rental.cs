namespace BookRentalApi.Models
{
    public class Rental
    {
        public int RentalIdx { get; set; }
        public int MemberIdx { get; set; }
        public int BookIdx { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
