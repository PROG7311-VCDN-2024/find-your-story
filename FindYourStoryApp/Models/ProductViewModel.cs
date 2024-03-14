namespace FindYourStoryApp.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        public string Base64BookCover { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int Price { get; set; }

        public int InStock { get; set; }

    }
}
