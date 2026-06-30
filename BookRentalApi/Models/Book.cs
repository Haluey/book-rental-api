namespace BookRentalApi.Models
{
    public class Book
    {
        public int BookIdx { get; set; }
        public string Author { get; set; } = string.Empty;
        public string DivCode { get; set; } = string.Empty;
        public string BookName { get; set; } = string.Empty;
        public DateTime ReleaseDt { get; set; }
        public string Isbn { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
