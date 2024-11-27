using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Admin
{
	[Area("Employee")]
	public class DashboardController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public DashboardController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Authorize(Roles = SD.Role_Admin)]
		public async Task<IActionResult> Index()
		{
			IEnumerable<DonHang> donHangs = await _unitOfWork.DonHang.GetAll();

			// Calculate total for approved orders using decimal
			decimal totalApproved = donHangs
				.Where(dh => dh.TrangThaiDonHang == SD.StatusApproved)
				.Sum(dh => (decimal)dh.TongTienDonHang);

			// Check and handle potential overflow before converting to int
			TempData["total"] = totalApproved < int.MinValue || totalApproved > int.MaxValue
				? int.MaxValue
				: (int)totalApproved;

			// Calculate total for cancelled orders using decimal
			decimal totalCancelled = donHangs
				.Where(dh => dh.TrangThaiDonHang == SD.StatusCancelled)
				.Sum(dh => (decimal)dh.TongTienDonHang);

			// Check and handle potential overflow before converting to int
			TempData["totalLoss"] = totalCancelled < int.MinValue || totalCancelled > int.MaxValue
				? int.MaxValue
				: (int)totalCancelled;

			return View();
		}

		public IActionResult LaySanPhamBanChayNhat(int soLuongSanPham)
		{
			var chiTietDonHangs = _unitOfWork.ChiTietDonHang.GetAll().Result;
			var sanPhams = _unitOfWork.SanPham.GetAll().Result;

			var sanPhamBanChay = chiTietDonHangs
				.Where(ct => ct.SanPhamId != null)
				.GroupBy(ct => ct.SanPhamId)
				.Select(group => new
				{
					SanPhamId = group.Key,
					TongSoLuongBan = group.Sum(ct => ct.SoLuong)
				})
				.OrderByDescending(x => x.TongSoLuongBan)
				.Take(soLuongSanPham)
				.ToList();

			var topSanPhams = sanPhams
				.Where(sp => sanPhamBanChay.Select(x => x.SanPhamId).Contains(sp.Id))
				.ToList();

			topSanPhams.ForEach(sp =>
				sp.SoLuong = sanPhamBanChay.First(x => x.SanPhamId == sp.Id).TongSoLuongBan);

			return Json(new { data = topSanPhams });
		}

		#region API Calls

		public async Task<IActionResult> GetAll()
		{
			return Json(new { data = await _unitOfWork.DonHang.GetAll() });
		}

		public async Task<JsonResult> GetEarningData(string timeRange)
		{

			var listOrders = (await _unitOfWork.DonHang.GetAll())
				.Where(x => x.TrangThaiDonHang == SD.StatusApproved);

			var columnData = new List<int>();
			var categories = new List<string>();

			switch (timeRange)
			{
				case "day":
					{
						var daysOfWeek = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
						columnData = daysOfWeek.Select(day =>
							listOrders
								.Where(x => x.NgayNhanHang.GetValueOrDefault().DayOfWeek.ToString().StartsWith(day.Substring(0, 3)))
								.Sum(x => Convert.ToInt32(x.TongTienDonHang))
						).ToList();
						categories = daysOfWeek.ToList();
						break;
					}
				case "week":
					{
						var weeksInMonth = Enumerable.Range(1, 4);
						columnData = weeksInMonth.Select(week =>
							listOrders
								.Where(x => x.NgayNhanHang.GetValueOrDefault().Month == DateTime.Today.Month)
								.Where(x => (x.NgayNhanHang.GetValueOrDefault().Day - 1) / 7 + 1 == week)
								.Sum(x => Convert.ToInt32(x.TongTienDonHang))
						).ToList();
						categories = weeksInMonth.Select(week => "Week " + week).ToList();
						break;
					}
				case "month":
					{
						int currentYear = DateTime.Today.Year;
						columnData = Enumerable.Range(1, 12).Select(month =>
							listOrders
								.Where(x => x.NgayNhanHang.GetValueOrDefault().Month == month && x.NgayNhanHang.GetValueOrDefault().Year == currentYear)
								.Sum(x => Convert.ToInt32(x.TongTienDonHang))
						).ToList();
						categories = Enumerable.Range(1, 12).Select(month => "Month " + month).ToList();
						break;
					}
				case "year":
					{
						int currentYear = DateTime.Today.Year;
						var recentYears = Enumerable.Range(currentYear - 5, 5);
						columnData = recentYears.Select(year =>
							listOrders
								.Where(x => x.NgayNhanHang.GetValueOrDefault().Year == year)
								.Sum(x => Convert.ToInt32(x.TongTienDonHang))
						).ToList();
						categories = recentYears.Select(year => year.ToString()).ToList();
						break;
					}
				default:
					{
						columnData = new List<int> { 0 };
						categories = new List<string> { "Unknown" };
						break;
					}
			}

			return Json(new
			{
				name = "Đơn hàng",
				data = columnData,
				categories = categories
			});
		}
        public async Task<JsonResult> GetAllOrderData(string timeRange)
        {
            var listOrders = (await _unitOfWork.DonHang.GetAll());

            int statusApproved = listOrders.Count(lo => lo.TrangThaiDonHang == SD.StatusApproved);
            int statusPending = listOrders.Count(lo => lo.TrangThaiDonHang == SD.StatusPending);
            int statusInAllProgress = listOrders.Count() - statusApproved - statusPending;

            return Json(new
            {
                approved = statusApproved,
                pending = statusPending,
                inAllProgress = statusInAllProgress
            });
        }



        public async Task<IActionResult> GetOrderStatusData(string timeRange)
		{
			var listOrders = (await _unitOfWork.DonHang.GetAll())
		.Where(x => x.NgayNhanHang != null);

			// Lọc đơn hàng theo khoảng thời gian
			switch (timeRange)
			{
				case "day":
					listOrders = listOrders.Where(x => x.NgayNhanHang.GetValueOrDefault().Date == DateTime.Today);
					break;
				case "week":
					var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
					listOrders = listOrders.Where(x => x.NgayNhanHang >= startOfWeek && x.NgayNhanHang <= startOfWeek.AddDays(6));
					break;
				case "month":
					listOrders = listOrders.Where(x => x.NgayNhanHang.GetValueOrDefault().Month == DateTime.Today.Month
													&& x.NgayNhanHang.GetValueOrDefault().Year == DateTime.Today.Year);
					break;
				case "year":
					listOrders = listOrders.Where(x => x.NgayNhanHang.GetValueOrDefault().Year == DateTime.Today.Year);
					break;
			}

			// Đếm số lượng đơn hàng theo trạng thái
			var orderStatuses = new Dictionary<string, int>
			{
				{ SD.StatusInProcess, 0 },
				{ SD.StatusPending, 0 },
				{ SD.StatusCancelled, 0 },
				{ SD.StatusShipped, 0 },
				{ SD.StatusApproved, 0 }
			};

			foreach (var order in listOrders)
			{
				if (orderStatuses.ContainsKey(order.TrangThaiDonHang))
				{
					orderStatuses[order.TrangThaiDonHang]++;
				}
			}

			return Json(new
			{
				labels = new string[] {"Đang giao", "Đang chờ", "Bị hủy","Đã giao","Đã xác nhận"}.ToList(),
				data = orderStatuses.Values.ToList()
			});
		}
        public async Task<JsonResult> GetOrderOverViewData(string timeRange)
        {
            var listOrders = (await _unitOfWork.DonHang.GetAll())
                .Where(x => x.TrangThaiDonHang != null);

            // Khởi tạo dữ liệu cho các trạng thái đơn hàng
            var orderStatuses = new string[] { SD.StatusInProcess, SD.StatusPending, SD.StatusCancelled, SD.StatusShipped, SD.StatusApproved };

            var orderStatusesVietnamese = new string[] { "Đang xử lý", "Chờ xử lý", "Đã hủy", "Đã giao", "Đã duyệt" };

            var statusData = new Dictionary<string, List<int>>();

            // Khởi tạo các danh mục thời gian cho biểu đồ
            var categories = new List<string>();
			int amountOrders = 0;
			int amountSuccessOrders = 0;

            switch (timeRange)
            {
                case "day":
                    var daysOfWeek = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
                    foreach (var status in orderStatuses)
                    {
                        statusData[status] = daysOfWeek.Select(day =>
						{
							amountOrders++;
							return listOrders
									.Where(x => x.TrangThaiDonHang == status)
									.Where(x => x.NgayNhanHang.GetValueOrDefault().DayOfWeek.ToString().StartsWith(day.Substring(0, 3)))
									.Count();
						}
                        ).ToList();
						if (status == SD.StatusApproved)
						{
							amountSuccessOrders = statusData[status].Count;
						}
                    }
                    categories = daysOfWeek.ToList();
                    break;

                case "week":
                    var weeksInMonth = Enumerable.Range(1, 4);
                    foreach (var status in orderStatuses)
                    {
                        statusData[status] = weeksInMonth.Select(week =>
						{
							amountOrders++;
							return listOrders
									.Where(x => x.TrangThaiDonHang == status)
									.Where(x => x.NgayNhanHang.GetValueOrDefault().Month == DateTime.Today.Month)
									.Where(x => (x.NgayNhanHang.GetValueOrDefault().Day - 1) / 7 + 1 == week)
									.Count();
						}
                        ).ToList();
						if (status == SD.StatusApproved)
						{
							amountSuccessOrders = statusData[status].Count;
						}
                    }
                    categories = weeksInMonth.Select(week => "Week " + week).ToList();
                    break;

                case "month":
                    var currentYear = DateTime.Today.Year;
                    foreach (var status in orderStatuses)
                    {
                        statusData[status] = Enumerable.Range(1, 12).Select(month =>
						{
							amountOrders++;
							return listOrders
									.Where(x => x.TrangThaiDonHang == status)
									.Where(x => x.NgayNhanHang.GetValueOrDefault().Month == month && x.NgayNhanHang.GetValueOrDefault().Year == currentYear)
									.Count();
						}
                        ).ToList();
                        if (status == SD.StatusApproved)
                        {
                            amountSuccessOrders = statusData[status].Count;
                        }
                    }
                    categories = Enumerable.Range(1, 12).Select(month => "Month " + month).ToList();
                    break;

                case "year":
                    var recentYears = Enumerable.Range(DateTime.Today.Year - 5, 5);
                    foreach (var status in orderStatuses)
                    {
                        statusData[status] = recentYears.Select(year =>
						{
							amountOrders++;
							return listOrders
									.Where(x => x.TrangThaiDonHang == status)
									.Where(x => x.NgayNhanHang.GetValueOrDefault().Year == year)
									.Count();
						}
                        ).ToList();
                        if (status == SD.StatusApproved)
                        {
                            amountSuccessOrders = statusData[status].Count;
                        }
                    }
                    categories = recentYears.Select(year => year.ToString()).ToList();
                    break;
                default:
                    foreach (var status in orderStatuses)
                    {
                        statusData[status] = new List<int> { 0 };
                    }
                    categories = new List<string> { "Unknown" };
                    break;
            }

            var result = orderStatuses.Select((status,i) => new
            {
				name = status,
                nameVietNamese = orderStatusesVietnamese[i],
                data = statusData[status]
            }).ToList();

            return Json(new
            {
                data = result,
                categories = categories,
				amountOrders = amountOrders,
				amountSuccessOrders = amountSuccessOrders,
            });
        }

        public async Task<IActionResult> SanPhamList()
		{
			var sanPhams = await _unitOfWork.SanPham.GetAll() ?? new List<SanPham>();
			return Json(new { data = sanPhams });
		}
		public async Task<IActionResult> DonHangList(int top = 10)
		{
			var donHangs = (await _unitOfWork.DonHang.GetAll())
				.OrderByDescending(dh => dh.NgayDatHang)
				.Take(top)  // Đây là chỗ giới hạn số lượng đơn hàng.
				.ToList();

			return Json(new { data = donHangs });
		}
		#endregion
	}
}
