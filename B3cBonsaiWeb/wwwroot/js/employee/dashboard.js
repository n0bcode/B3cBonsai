var dataTable;
var dataTableProduct;
let currentPage = 1;
let pageSize = 5;
$.ajax({
    url: '/employee/ManagerUser/getall',
    success: function (data) {
        console.log(data); // Check if data is being returned correctly
    }
});
function loadDataTable() {
    dataTable = $('#empoloyees-tbl').DataTable({
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
        ]
    });
    dataTableProduct = $('#product-tbl').DataTable({
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
            },
            {
                data: null,
                "render": function (data) {
                    return `<a href="javascript:void(0)" onclick="viewProductDetail(${data.id})">Chi tiết</a>`;
                }
            }
        ]
    });
    dataTableDonHang = $('#donhang-tbl').DataTable({
        "ajax": {
            url: '/Employee/Dashboard/DonHangList?top=10',
            dataSrc: 'data'
        },
        "columns": [
            { data: 'id', "visible": false },
            { data: 'tenNguoiNhan', "width": "20%" },
            { data: 'soDienThoai', "width": "15%" },
            { data: 'duong', "width": "20%", "render": function (data) { return data || 'N/A'; } },
            { data: 'thanhPho', "width": "15%", "render": function (data) { return data || 'N/A'; } },
            {
                data: 'tongTienDonHang',
                "width": "15%",
                "render": function (data) {
                    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(data);
                }
            },
            { data: 'trangThaiDonHang', "width": "10%", "render": function (data) { return data || 'N/A'; } },
            {
                data: 'ngayDatHang',
                "width": "15%",
                "render": function (data) {
                    return new Date(data).toLocaleDateString('vi-VN');
                }
            }
        ]
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
