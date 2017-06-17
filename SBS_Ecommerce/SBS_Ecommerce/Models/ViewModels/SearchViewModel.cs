namespace SBS_Ecommerce.Models
{
    public class SearchViewModel
    {
        public string Keyword { get; set; }
        public string Sort { get; set; }
        public string SortType { get; set; }
        public int? CgID { get; set; }
        public int[] BrandID { get; set; }
        public int[] RangeID { get; set; }
        public bool Filter { get; set; }
        public int CurrentPage { get; set; } = 1;

    }
}