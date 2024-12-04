var dataTable;
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/employee/Managercomment/getall',
            type: "GET",
            datatype: "json",
            dataSrc: function (json) {
                return json.data.map(comment => ({
                    id: comment.id,
                    noiDungBinhLuan: comment.noiDungBinhLuan,
                    nguoiDungId: comment.nguoiDungId,
                    sanPhamId: comment.sanPhamId,
                    ngayBinhLuan: new Date(comment.ngayBinhLuan).toLocaleDateString('vi-VN'),
                    tinhTrang: comment.tinhTrang,
                    tenSanPham: comment.sanPham ? comment.sanPham.tenSanPham : "Không xác định",
                    hinhAnh: comment.sanPham?.hinhAnhs?.[0]?.linkAnh || "/images/product/default.jpg"
                }));
            }
        },
        "columns": [
            {
                "data": "id",
                "className": "text-center",
                "render": function (data) {
                    return `
                        <div class="form-check custom-checkbox">
                            <input type="checkbox" class="form-check-input rowCheckbox" id="check_${data}" value="${data}" />
                            <label class="form-check-label" for="check_${data}"></label>
                        </div>
                    `;
                }
            },
            {
                "data": "id",
                "className": "text-left",
                "title": "Thông tin sản phẩm",
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
                "data": "tinhTrang",
                "className": "text-center",
                "title": "Tình trạng",
                "render": function (data) {
                    return data ? '<span class="text-success">Được hiển thị</span>' : '<span class="text-danger">Bị khóa</span>';
                }
            },
            {
                "data": "id",
                "className": "text-right",
                "title": "Thao tác",
                "render": function (data) {
                    return `
                        <button class="btn btn-primary btn-sm me-2" onclick="deleteOneComment(${data})" title="Xóa bình luận">Xóa</button>
                        <button class="btn btn-warning btn-sm" onclick="changeStatusOneComment(${data})" title="Thay đổi trạng thái">Sửa trạng thái</button>
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
            "zeroRecords": "<div style='text-align: center;'>Không tìm thấy kết quả nào.</div>",
            "infoEmpty": "Không có mục nào để hiển thị",
            "infoFiltered": "(lọc từ _MAX_ mục)"
        }
    });

    // Xử lý checkbox "Chọn tất cả"
    $('#checkAllComments').on('change', function () {
        const isChecked = $(this).is(':checked');
        $('.rowCheckbox').prop('checked', isChecked);
    });
}


$(document).ready(function () {
    loadDataTable();
})

function takeAllCommentsSelect() {
    const listComments = document.querySelectorAll('.rowCheckbox');
    const listCheckedComments = Array.from(listComments).filter(cmt => cmt.checked).map(cmt => cmt.value);
    return listCheckedComments;
}

function deleteOneComment(id) {
    const isQuestion = document.getElementById('deleteNotQuest').checked;

    if (!isQuestion) {
        Swal.fire({
            title: 'Xác nhận',
            text: 'Bạn có chắc chắn muốn xóa bình luận này không?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Có',
            cancelButtonText: 'Hủy',
        }).then((result) => {
            if (result.value) {
                proceedDeleteOneComment(id);
            }
        });
    } else {
        proceedDeleteOneComment(id);
    }
}

function proceedDeleteOneComment(id) {
    $.ajax({
        url: '/Employee/ManagerComment/DeleteOne',
        method: 'POST',
        data: { id },
        success: (response) => {
            if (response.success) {
                toastr.success(response.message);
                dataTable.ajax.reload();
            } else {
                toastr.info(response.message ?? "Bạn không thể xóa bình luận này!");
            }
        },
        error: () => {
            toastr.error("Lỗi kết nối với hệ thống!");
        }
    });
}

function deleteManyComment(ids) {
    const isQuestion = document.getElementById('deleteNotQuest').checked;

    if (!isQuestion) {
        Swal.fire({
            title: 'Xác nhận',
            text: 'Bạn có chắc chắn muốn xóa các bình luận này không?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Có',
            cancelButtonText: 'Hủy',
        }).then((result) => {
            if (result.value) {
                proceedDeleteManyComment(ids);
            }
        });
    } else {
        proceedDeleteManyComment(ids);
    }
}

function proceedDeleteManyComment(ids) {
    $.ajax({
        url: '/Employee/ManagerComment/DeleteMany',
        method: 'POST',
        data: { ids },
        success: (response) => {
            if (response.success) {
                toastr.success(response.message);
                dataTable.ajax.reload();
            } else {
                toastr.info(response.message ?? "Bạn không thể xóa các bình luận này!");
            }
        },
        error: () => {
            toastr.error("Lỗi kết nối với hệ thống!");
        }
    });
}

function changeStatusOneComment(id) {
    const isQuestion = document.getElementById('changeNotQuest').checked;

    if (!isQuestion) {
        Swal.fire({
            title: 'Xác nhận',
            text: 'Bạn có chắc chắn muốn thay đổi trạng thái bình luận này không?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Có',
            cancelButtonText: 'Hủy',
        }).then((result) => {
            if (result.value) {
                proceedChangeStatusOneComment(id);
            }
        });
    } else {
        proceedChangeStatusOneComment(id);
    }
}

function proceedChangeStatusOneComment(id) {
    $.ajax({
        url: '/Employee/ManagerComment/ChangeOneStatus',
        method: 'POST',
        data: { id },
        success: (response) => {
            if (response.success) {
                toastr.success(response.message);
                dataTable.ajax.reload();
            } else {
                toastr.info(response.message ?? "Bạn không thể thay đổi trạng thái bình luận này!");
            }
        },
        error: () => {
            toastr.error("Lỗi kết nối với hệ thống!");
        }
    });
}

function changeManyStatus(ids) {
    const isQuestion = document.getElementById('changeNotQuest').checked;

    if (!isQuestion) {
        Swal.fire({
            title: 'Xác nhận',
            text: 'Bạn có chắc chắn muốn thay đổi trạng thái các bình luận này không?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Có',
            cancelButtonText: 'Hủy',
        }).then((result) => {
            if (result.value) {
                proceedChangeManyStatus(ids);
            }
        });
    } else {
        proceedChangeManyStatus(ids);
    }
}

function proceedChangeManyStatus(ids) {
    $.ajax({
        url: '/Employee/ManagerComment/ChangeManyStatus',
        method: 'POST',
        data: { ids },
        success: (response) => {
            if (response.success) {
                toastr.success(response.message);
                dataTable.ajax.reload();
            } else {
                toastr.info(response.message ?? "Bạn không thể thay đổi trạng thái các bình luận này!");
            }
        },
        error: () => {
            toastr.error("Lỗi kết nối với hệ thống!");
        }
    });
}
