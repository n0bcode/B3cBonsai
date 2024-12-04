var dataTable;
$(document).ready(function () {
    dataTable = $('#ClientCommentData').DataTable({
        "ajax": {
            "url": "/Customer/ClientProfile/ClientCommentData", // Updated URL
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                return json.data.map(comment => ({
                    id: comment.id,
                    noiDungBinhLuan: comment.noiDungBinhLuan,
                    nguoiDungId: comment.nguoiDungId,
                    sanPhamId: comment.sanPhamId,
                    ngayBinhLuan: new Date(comment.ngayBinhLuan).toLocaleDateString('vi-VN'),
                    tinhTrang: comment.tinhTrang,
                    tenSanPham: comment.sanPham ? comment.sanPham.tenSanPham : "Không xác định", // Extract product name
                    hinhAnh: comment.sanPham && comment.sanPham.hinhAnhs && comment.sanPham.hinhAnhs.length > 0
                        ? comment.sanPham.hinhAnhs[0].linkAnh
                        : "/images/product/default.jpg" // Extract product image or placeholder
                }));
            }
        },
        "columns": [
            {
                "data": "id", "className": "text-left", "title": "Thông tin sản phẩm",
                "render": function (data, type, row) {
                    return `
                        <a href="/Customer/ClientProduct/Detail/${row.sanPhamId}">
                            <img src="${row.hinhAnh}" alt="${row.tenSanPham}" style="width: 50px; height: 50px; object-fit:cover" />
                            <strong>${row.tenSanPham}</strong>
                        </a>
                    `;
                }
            },
            { "data": "noiDungBinhLuan", "className": "text-left", "title": "Nội dung bình luận" },
            { "data": "ngayBinhLuan", "className": "text-center", "title": "Ngày bình luận" },
            {
                "data": "tinhTrang", "className": "text-center",
                "title": "Tình trạng",
                "render": function (data) {
                    return data ? '<span class="text-success">Được hiển thị</span>' : '<span class="text-danger">Bị khóa</span>';
                }
            },
            {
                "data": "sanPhamId", "className": "text-right",
                "title": "Thao tác",
                "render": function (data) {
                    return `
                        <a href="/Customer/ClientProduct/Detail/${data}" class="btn btn-primary btn-sm me-2" title="Xem chi tiết bình luận">Chi tiết</a>
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
