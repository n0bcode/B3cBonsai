var dataTable;
var dataTableProduct;
let currentPage = 1;
let pageSize = 5;

function loadDataTable() {
    dataTable = $('#empoloyeesDatatale').DataTable({
        "ajax": { url: '/employee/ManagerUser/getall' },
        "columns": [
            {
                data: null,
                "render": function (data) {
                    return `<a onclick="loadDetailWithDelete('${data.id}')" data-bs-toggle="modal" data-bs-target="#exampleModal1" class="products">
                                <img src="${data.linkAnh || '/images/user/default.png'}" class="avatar avatar-md" alt="">
                                <div>
                                    <h6>${data.hoTen || 'N/A'}</h6>
                                    <span>${data.userName || 'N/A'}</span>
                                </div>
                            </a>`;
                }
            },
            { data: 'email', "width": "15%", "className": "text-primary" },
            {
                data: null, "width": "5%", "render": function (data) {
                    return data.gioiTinh ? 'Nam' : 'Nữ';
                }
            },
            { data: 'soDienThoai', "width": "15%" },
            {
                data: null,
                "render": function (data) {
                    return `<div class="d-flex">
                                <select onchange="updateUserRole('${data.id}', this.value)" class="form-control">
                                    <option value="Admin" ${data.vaiTro.includes('Admin') ? 'selected' : ''}>Admin</option>
                                    <option value="Customer" ${data.vaiTro.includes('Customer') ? 'selected' : ''}>Khách hàng</option>
                                    <option value="Staff" ${data.vaiTro.includes('Staff') ? 'selected' : ''}>Nhân viên</option>
                                </select>
                            </div>`;
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
    dataTableOrder = $('#orderDashboard').DataTable({
        "ajax": {
            url: '/Employee/Dashboard/DonHangList',
            dataSrc: 'data'
        },
        "columns": [
            {
                data: 'ngayDatHang',
                "width": "20%",
                "render": function (data) {
                    return new Date(data).toLocaleDateString();
                }
            },
            {
                data: null,
                "render": function (data) {
                    switch (data.trangThaiDonHang) {
                        case 'SD.StatusInProcess':
                            return 'Đang xử lý';
                        case 'SD.StatusPending':
                            return 'Đang chờ';
                        case 'SD.StatusCancelled':
                            return 'Đã hủy';
                        case 'SD.StatusShipped':
                            return 'Đã giao';
                        case 'SD.StatusApproved':
                            return 'Đã duyệt';
                        default:
                            return 'Không xác định';
                    }
                },
                "width": "20%"
            },
            {
                data: 'soLuong',
                "width": "20%"
            },
            {
                data: null,
                "render": function (data) {
                    return new Intl.NumberFormat().format(data.tongTien) + ' VND';
                },
                "width": "20%"
            },
        ],
    });

    dataTableProduct = $('#productDashboard').DataTable({
        "ajax": {
            url: '/Employee/Dashboard/SanPhamList',
            dataSrc: 'data'
        },
        "columns": [
            {
                data: null,
                "render": function (data) {
                    return `<div>
                            <h6>${data.tenSanPham || 'N/A'}</h6>
                            <span>${data.moTa || 'N/A'}</span>
                        </div>`;
                }
            },
            { data: 'soLuong', "width": "10%" },
            { data: 'gia', "width": "15%", "render": function (data) { return new Intl.NumberFormat().format(data) + ' VND'; } },
            { data: 'ngayTao', "width": "15%", "render": function (data) { return new Date(data).toLocaleDateString(); } },
            {
                data: null,
                "render": function (data) {
                    return data.trangThai ? 'Đang bán' : 'Ngừng bán';
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
    dataTableDonHang = $('#donhangDatatable').DataTable({
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
                "render": function (data) {
                    let statusClass = '';
                    let statusText = '';

                    switch (data) {
                        case 'Pending':
                            statusClass = 'text-warning';
                            statusText = 'Đang chờ';
                            break;
                        case 'Approved':
                            statusClass = 'text-success';
                            statusText = 'Đã duyệt';
                            break;
                        case 'Processing':
                            statusClass = 'text-info';
                            statusText = 'Đang xử lý';
                            break;
                        case 'Shipped':
                            statusClass = 'text-primary';
                            statusText = 'Đã giao hàng';
                            break;
                        case 'Cancelled':
                            statusClass = 'text-danger';
                            statusText = 'Đã hủy';
                            break;
                        case 'Refunded':
                            statusClass = 'text-secondary';
                            statusText = 'Đã hoàn tiền';
                            break;
                        default:
                            statusClass = 'text-muted';
                            statusText = 'Không xác định';
                    }

                    return `<span class="${statusClass}">${statusText}</span>`;
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
                "render": function (data) {
                    let statusClass = '';
                    let statusText = '';

                    switch (data) {
                        case 'Pending':
                            statusClass = 'text-warning';
                            statusText = 'Đang chờ';
                            break;
                        case 'Approved':
                            statusClass = 'text-success';
                            statusText = 'Đã duyệt';
                            break;
                        case 'ApprovedForDelayedPayment':
                            statusClass = 'text-info';
                            statusText = 'Duyệt thanh toán chậm';
                            break;
                        case 'Rejected':
                            statusClass = 'text-danger';
                            statusText = 'Từ chối';
                            break;
                        default:
                            statusClass = 'text-muted';
                            statusText = 'Không xác định';
                    }

                    return `<span class="${statusClass}">${statusText}</span>`;
                }
            },
            {
                data: 'tongTienDonHang',
                "title": "Thanh toán",
                "render": function (data) {
                    return `<span>${data.toLocaleString('vi-VN')} đ</span>`;
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
    $.ajax({
        url: '/Employee/Dashboard/GetOrderStatusData?timeRange=day',
        success: function (data) {
            console.log(data);
            var ctx = document.getElementById('projectChart').getContext('2d');
            var projectChart = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: data.labels,
                    datasets: [{
                        data: data.data,
                        backgroundColor: [
                            '#FF9F00',
                            '#FF5E5E',
                            '#3AC977',
                            '#FFB8C6',
                            '#007bff'
                        ],
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top',
                        },
                        tooltip: {
                            callbacks: {
                                label: function (tooltipItem) {
                                    var value = tooltipItem.raw;
                                    return value + ' orders';
                                }
                            }
                        }
                    }
                }
            });
        },
        error: function (error) {
            console.error("Error loading order status data:", error);
        }
    });
    loadDataTable();
});

function updateUserRole(userId, newRole) {
    $.ajax({
        url: '/Employee/ManagerUser/UpdateUserRole',
        type: 'POST',
        data: {
            userId: userId,
            newRole: newRole
        },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message, 'Thành công');
                dataTable.ajax.reload();
            } else {
                toastr.error(response.message, 'Lỗi');
            }
        },
        error: function () {
            toastr.error('Đã xảy ra lỗi khi cập nhật vai trò người dùng.', 'Lỗi');
        }
    });
}

function lockOrUnlockUser(userId, isLock) {
    $.ajax({
        url: '/Employee/ManagerUser/LockOrUnlockUser',
        type: 'POST',
        data: {
            userId: userId,
            isLock: isLock
        },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message, 'Thành công');
                dataTable.ajax.reload();
            } else {
                toastr.error(response.message, 'Lỗi');
            }
        },
        error: function () {
            toastr.error('Đã xảy ra lỗi khi cập nhật trạng thái tài khoản.', 'Lỗi');
        }
    });
}
