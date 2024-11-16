namespace B3cBonsai.Models.ViewModels
{
    public class FilterValueVM
    {

        public List<DanhMucSanPham> Categories { get; set; }
        public int MinimumPrice { get; set; }
        public int MaximumPrice { get; set; }
        public int InStock { get; set; }
        public int OutOfStock { get; set; }
    }
}
