var dataTable;
$(document).ready(function () {
    dataTable = $('#ClientOrderData').DataTable({
        "ajax": {
            "url": "/Customer/ClientProfile/ClientOrderData",
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                return json.data.map(order => ({
                    maDonHang: order.id,
                    ngayDatHang: new Date(order.ngayDatHang).toLocaleDateString('vi-VN'),
                    trangThai: order.trangThaiDonHang,
                    tongTien: new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(order.tongTienDonHang)
                }));
            }
        },
        "columns": [
            { "data": "maDonHang", "className": "text-center" },
            { "data": "ngayDatHang", "className": "text-center" },
            {
                "data": "trangThai", "className": "text-center",
                "render": function (data) {
                    let trangThaiText = "";
                    let colorClass = "";

                    switch (data) {
                        case "Pending":
                            trangThaiText = "Chờ xử lý";
                            colorClass = "text-warning";
                            break;
                        case "Approved":
                            trangThaiText = "Đã duyệt";
                            colorClass = "text-success";
                            break;
                        case "Processing":
                            trangThaiText = "Đang xử lý";
                            colorClass = "text-primary";
                            break;
                        case "Shipped":
                            trangThaiText = "Đã giao";
                            colorClass = "text-info";
                            break;
                        case "Cancelled":
                            trangThaiText = "Đã hủy";
                            colorClass = "text-danger";
                            break;
                        case "Refunded":
                            trangThaiText = "Đã hoàn tiền";
                            colorClass = "text-secondary";
                            break;
                        default:
                            trangThaiText = "Không xác định";
                            colorClass = "text-dark";
                            break;
                    }

                    return `<span class="text ${colorClass}">${trangThaiText}</span>`;
                }
            },
            { "data": "tongTien", "className": "text-right" },
            {
                "data": "maDonHang", "className": "text-right",
                "render": function (data) {
                    return `
                        <a href="/Customer/Payment/OrderComplete?orderId=${data}" class="btn btn-primary btn-sm me-2">Chi tiết</a>
                        <a href="/Customer/Payment/ExportBill?orderId=${data}" class="btn btn-secondary btn-sm">Xuất hóa đơn</a>
                    `;
                }
            }
        ],
        "language": {
            "sSearch": "Tìm kiếm:",
            "lengthMenu": "Hiển thị _MENU_ mục",
            "info": "Hiển thị _START_ đến _END_ trong tổng số _TOTAL_ mục",
            "paginate": {
                "first": "<<",
                "last": ">>",
                "next": ">",
                "previous": "<"
            },
            "zeroRecords": "Không tìm thấy kết quả nào",
            "infoEmpty": "Không có mục nào để hiển thị",
            "infoFiltered": "(lọc từ _MAX_ mục)"
        }
    });
});
