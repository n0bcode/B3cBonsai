var dataTable;

// Load DataTable for all orders (getall)
function loadOrderDataTable() {
    dataTable = $('#tableOrder').DataTable({
        "ajax": {
            url: '/Employee/ManagerOrder/getall',
            dataSrc: 'data' // Ensure data is loaded directly if it's an array
        },
        "columns": [
            {
                data: 'id',
                "title": "#",
                "render": function (data, type, row, meta) {
                    return `<span >${meta.row + 1}</span>`;
                },
                "className": "text-center"
            },
            {
                data: 'tenNguoiNhan',
                "title": "Tên người nhận",
                "render": function (data, type, row) {
                    return `<div class="products">
                                <div>
                                    <h6>${decodeURIComponent(data) || 'N/A'}</h6>
                                    <span>INV-${row.id || 'N/A'}</span>
                                </div>
                            </div>`;
                }
            },
            {
                data: 'trangThaiDonHang',
                "title": "Trạng thái đơn hàng",
                "render": function (data, type, row) {
                    return `<select class="default-select status-select form-control" onchange="changeStatusOrder(${row.id}, this.value)">
                                <option value="Pending" class="text-warning" ${data === 'Pending' ? 'selected' : ''}>Đang chờ</option>
                                <option value="Approved" class="text-success" ${data === 'Approved' ? 'selected' : ''}>Đã duyệt</option>
                                <option value="Processing" class="text-info" ${data === 'Processing' ? 'selected' : ''}>Đang xử lý</option>
                                <option value="Shipped" class="text-primary" ${data === 'Shipped' ? 'selected' : ''}>Đã giao hàng</option>
                                <option value="Cancelled" class="text-danger" ${data === 'Cancelled' ? 'selected' : ''}>Đã hủy</option>
                                <option value="Refunded" class="text-secondary" ${data === 'Refunded' ? 'selected' : ''}>Đã hoàn tiền</option>
                            </select>`;
                }
            },
            {
                data: 'ngayDatHang',
                "title": "Ngày đặt hàng",
                "render": function (data) {
                    return `<span>${new Date(data).toLocaleDateString('vi-VN')}</span>`;
                }
            },
            {
                data: 'ngayNhanHang',
                "title": "Ngày nhận hàng",
                "render": function (data) {
                    return `<span>${new Date(data).toLocaleDateString('vi-VN')}</span>`;
                }
            },
            {
                data: 'chiTietDonHangs',
                "title": "Sản phẩm của đơn",
                "render": function (data) {
                    if (Array.isArray(data) && data.length > 0) {
                        return `<div class="avatar-list avatar-list-stacked">` +
                            data.map(sanPham => {
                                // Check if hinhAnhs is defined and is an array, then access the first image
                                const firstImage = Array.isArray(sanPham.sanPham.hinhAnhs) && sanPham.sanPham.hinhAnhs.length > 0 ? sanPham.sanPham.hinhAnhs[0]?.linkAnh : null;
                                return firstImage ?
                                    `<img src="${firstImage}" class="avatar rounded-circle" style="width: 40px; height: 40px;" alt="Product Image">` :
                                    '<span></span>';
                            }).join('') +
                            `</div>`;
                    }
                    return `<span>Không có hình ảnh</span>`;
                }
            },
            {
                data: 'trangThaiThanhToan',
                "title": "Trạng thái thanh toán",
                "render": function (data, type, row) {
                    return `<select class="default-select status-select form-control" onchange="changeStatusPayment(${row.id}, this.value)">
                                <option value="Pending" class="text-warning" ${data === 'Pending' ? 'selected' : ''}>Đang chờ</option>
                                <option value="Approved" class="text-success" ${data === 'Approved' ? 'selected' : ''}>Đã duyệt</option>
                                <option value="ApprovedForDelayedPayment" class="text-info" ${data === 'ApprovedForDelayedPayment' ? 'selected' : ''}>Duyệt thanh toán chậm</option>
                                <option value="Rejected" class="text-danger" ${data === 'Rejected' ? 'selected' : ''}>Từ chối</option>
                            </select>`;
                }
            },
            {
                data: 'tongTienDonHang',
                "title": "Thanh toán",
                "render": function (data) {
                    return `<span>${data.toLocaleString('vi-VN')} VNĐ</span>`;
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
}






$(document).ready(function () {
    loadOrderDataTable();

});
let oldValue = $('select').val(); // Initialize the old value for the select box

$('select').on('focus', function () {
    oldValue = this.value; // Store the old value when the select field is focused
});

const changeStatusOrder = (id, statusOrder) => {

    $.ajax({
        url: `/Employee/ManagerOrder/ChangeStatusOrder?id=${id}&statusOrder=${statusOrder}`,
        method: 'POST',
        success: (data) => {
            if (data.success) {
                Swal.fire({
                    title: "Chỉnh sửa",
                    text: "Bạn đã chỉnh sửa thành công!",
                    type: "success"
                });
                dataTable.ajax.reload();
            } else {
                Swal.fire({
                    title: data.title,
                    text: data.content,
                    type: "info"
                });
                dataTable.ajax.reload();
            }
        }
    });/*
    Swal.fire({
        title: "Chỉnh sửa",
        text: "Bạn có muốn thay đổi tình trạng?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Chỉnh sửa!",
        cancelButtonText: "Hủy"
    }).then((result) => {
        if (result.value) {
        } else {
            toastr.info("Bạn hủy hàng động thay đổi tình trạng.");
            dataTable.ajax.reload();
        }
    });*/
};

const changeStatusPayment = (id, statusPayment) => {
    $.ajax({
        url: `/Employee/ManagerOrder/ChangeStatusPayment?id=${id}&statusPayment=${statusPayment}`,
        method: 'POST',
        data: (id = id, statusPayment = statusPayment),
        success: (data) => {
            if (data.success) {
                Swal.fire({
                    title: "Chỉnh sửa",
                    text: "Bạn đã chỉnh sửa thành công!",
                    type: "success"
                });
                dataTable.ajax.reload();
            } else {
                Swal.fire({
                    title: data.title,
                    text: data.content,
                    type: "info"
                });
                dataTable.ajax.reload();
            }
        }
    });/*
    Swal.fire({
        title: "Chỉnh sửa",
        text: "Bạn có muốn thay đổi tình trạng?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Chỉnh sửa!",
        cancelButtonText: "Hủy"
    }).then((result) => {
        if (result.value) {
        } else {
            toastr.info("Bạn hủy hàng động thay đổi tình trạng.");
            dataTable.ajax.reload();
        }
    });*/
};
