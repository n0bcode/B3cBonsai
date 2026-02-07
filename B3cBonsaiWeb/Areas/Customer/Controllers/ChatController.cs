using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Utility.Services.AI;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly IAIService _aiService;
        private readonly IUnitOfWork _unitOfWork;

        public ChatController(IAIService aiService, IUnitOfWork unitOfWork)
        {
            _aiService = aiService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Message cannot be empty.");
            }

            string context = "";
            if (request.ProductId.HasValue)
            {
                var product = await _unitOfWork.SanPham.Get(x => x.Id == request.ProductId.Value, "DanhMuc");
                if (product != null)
                {
                    context = $"Sản phẩm đang xem: {product.TenSanPham}. Danh mục: {product.DanhMuc.TenDanhMuc}. Giá: {product.Gia.ToString("N0")} VND. Mô tả: {product.MoTa}.\n";
                    
                    // Lấy thêm các sản phẩm cùng danh mục để gợi ý
                    var relatedProducts = await _unitOfWork.SanPham.GetAll(x => x.DanhMucId == product.DanhMucId && x.Id != product.Id && x.TrangThai);
                    if (relatedProducts.Any())
                    {
                        context += "Các sản phẩm tương tự có thể gợi ý: " + string.Join(", ", relatedProducts.Take(3).Select(p => $"{p.TenSanPham} ({p.Gia.ToString("N0")} VND)"));
                    }
                }
            }
            else
            {
                // Nếu không có sản phẩm cụ thể, lấy một số sản phẩm nổi bật
                var products = await _unitOfWork.SanPham.GetAll(x => x.TrangThai);
                var recommendations = products.OrderByDescending(x => x.NgayTao).Take(5);
                context = "Danh sách sản phẩm nổi bật: " + string.Join(", ", recommendations.Select(p => $"{p.TenSanPham} ({p.Gia.ToString("N0")} VND)"));
            }

            var response = await _aiService.GetChatResponseAsync(request.Message, context);
            return Ok(new { response });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
        public int? ProductId { get; set; }
    }
}
