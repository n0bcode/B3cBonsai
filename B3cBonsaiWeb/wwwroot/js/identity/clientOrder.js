var dataTable;
$(document).ready(function () {
    dataTable = $('#clientOrderTable').DataTable({
        "ajax": {
            "url": "/Customer/ClientProfile/clientOrderTable",
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
            { "data": "trangThai", "className": "text-center" },
            { "data": "tongTien", "className": "text-right" },
            {
                "data": "maDonHang", "className": "text-right",
                "render": function (data) {
                    return `<a href="/Customer/Payment/OrderComplete?orderId=${data}">Chi tiết</a>`
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
