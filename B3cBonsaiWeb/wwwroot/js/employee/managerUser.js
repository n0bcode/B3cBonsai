var dataTable;
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/employee/ManagerUser/getall' },
        "columns": [
            {
                data: null,
                "render": function (data) {
                    return `<a onclick="loadDetailWithDelete('${data.id}')" data-bs-toggle="modal" data-bs-target="#exampleModal1" class="products">
                                <img src="${data.linkAnh || '/images/user/default.png'}" class="avatar avatar-md" alt="" onerror="this.src='/images/user/default.png'">
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
            { data: 'diaChi', "width": "15%" },
            {
                data: null,
                "render": function (data) {
                    const isLocked = data.lockoutEnabled && data.lockoutEnd && new Date() < new Date(data.lockoutEnd);
                    return isLocked ?
                        `<span onclick="lockOrUnlockUser('${data.id}', ${!isLocked})" class="badge badge-danger cursor-pointer">Đã bị khóa</span>` :
                        `<span onclick="lockOrUnlockUser('${data.id}', ${!isLocked})" class="badge badge-success cursor-pointer">Đang hoạt động</span>`;
                }
            },
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
            },
            {
                data: null,
                "render": function (data) {
                    return `<div class="d-flex">
                                <a onclick="loadViewUpsert('${data.id}')" class="btn btn-primary shadow btn-xs sharp me-1" data-bs-toggle="offcanvas" href="#upsertObject"><i class="fas fa-pencil-alt"></i></a>
                            </div>`;
                }
            }
        ],
        "language": {
            "sSearch": "Tìm kiếm:", // Tên trường tìm kiếm
            "lengthMenu": "Hiển thị _MENU_ mục", // Text phân trang
            "info": "Hiển thị _START_ đến _END_ trong tổng số _TOTAL_ mục", // Thông tin
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
    loadDataTable();
});

const loadViewUpsert = (id) => {
    $.ajax({
        url: `/Employee/ManagerUser/Upsert?id=${id}`,
        method: 'GET',
        success: (data) => {
            $('#upsertObject').html(data);
        },
        error: (xhr) => {
            toastr.error("Lỗi tạo form tạo đối tượng người dùng.")
        }
    })
}


const loadDetailWithDelete = (id) => {
    $.ajax({
        url: `/Employee/ManagerUser/DetailWithDelete?id=${id}`,
        method: 'GET',
        success: (data) => {
            $('#contentDWD').html(data)
        },
        error: (xhr) => {
            toastr.error("Lỗi tải thông tin đối tượng")
        }
    })
}

const DeleteDWD = (id) => {
    Swal.fire({
        title: "Bạn có chắc chắn không?",
        text: "Bạn sẽ không thể khôi phục lại thông tin đã xóa!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Vâng, xóa đi!"
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: `/Employee/ManagerUser/Delete?id=${id}`,
                method: 'DELETE',
                success: (data) => {
                    if (data.success) {
                        Swal.fire({
                            title: "Đã xóa!",
                            text: data.message,
                            type: "success"
                        });
                        dataTable.ajax.reload(); // Tải lại dữ liệu bảng
                    } else {
                        Swal.fire({
                            title: "Lỗi!",
                            text: data.message,
                            type: "error"
                        });
                    }
                },
                error: (xhr) => {
                    let message = "Lỗi xóa thông tin đối tượng. Vui lòng thử lại sau.";
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        message = xhr.responseJSON.message; // Đưa ra thông báo cụ thể nếu có
                    }
                    toastr.error(message);
                }
            });
        }
    });
}

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
