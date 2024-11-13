﻿var dataTable;
function loadDataTable() {
    dataTable = $('#empoloyees-tbl1').DataTable({
        "ajax": { url: '/employee/ManagerCombo/getall' },
        "columns": [
            {
                data: 'tenCombo',
                "render": function (data) {
                    return `<strong>${data || 'N/A'}</strong>`;
                },
                "title": "Tên Combo",
                "width": "20%"
            },
            {
                data: 'chiTietCombos',
                "render": function (data) {
                    if (!data || data.length === 0) return '<span>Không có sản phẩm</span>';
                    return `
                        <div style="max-height: 200px; overflow-y: auto;">
                            ${data.map(item => `
                                <div>
                                    <p><strong>Sản phẩm:</strong> ${item.sanPham.tenSanPham || 'N/A'}</p>
                                    <p><strong>Mô tả:</strong> ${item.sanPham.moTa || 'N/A'}</p>
                                    <p><strong>Giá:</strong> ${item.sanPham.gia || 'N/A'} VNĐ</p>
                                    <p><strong>Số lượng:</strong> ${item.sanPham.soLuong || 'N/A'}</p>
                                </div>
                            `).join('<hr>')}
                        </div>`;
                },
                "title": "Chi tiết Combo",
                "width": "50%"
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="d-flex justify-content-center gap-2">
                    <a onclick="loadViewUpsert('${data}')" class="btn btn-primary shadow btn-xs sharp me-1" data-bs-toggle="offcanvas" href="#upsertObject"><i class="fas fa-pencil-alt"></i></a>
                    <a onclick="deleteCombo('${data}')" class="btn btn-danger shadow btn-xs sharp"><i class="fas fa-trash-alt"></i></a>
                </div>`;
                },
                "title": "Actions",
                "width": "20%"
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
    loadDataTable();
});

const loadViewUpsert = (id) => {
    $.ajax({
        url: `/Employee/ManagerCombo/Upsert?id=${id}`,
        method: 'GET',
        success: (data) => {
            $('#viewFormUpsert').html(data);
        },
        error: (xhr) => {
            toastr.error("Lỗi tạo form tạo đối tượng người dùng.");
        }
    });
};


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

/*const DeleteDWD = (id) => {
    Swal.fire({
        title: "Bạn có chắc?",
        text: "Bạn sẽ không thể khôi phục lại thông tin bị xóa!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Vâng, xóa đi!"
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: `/Employee/ManagerCombo/Delete?id=${id}`,
                method: 'GET',
                success: (data) => {
                    Swal.fire({
                        title: "Đã xóa!",
                        text: "Thông tin đối được chọn đã bị xóa.",
                        type: "success"
                    });
                    dataTable.ajax.reload();
                },
                error: (xhr) => {
                    toastr.error("Lỗi xóa thông tin đối tượng")
                }
            })
        } else {
            toastr.info("Bạn đã hủy xác nhận xóa");
        }
    });
}
*/
function deleteCombo(id) {
    console.log("Đang xóa combo với ID:", id);
    Swal.fire({
        title: "Bạn có chắc?",
        text: "Bạn sẽ không thể khôi phục lại dữ liệu đã xóa!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Vâng, xóa đi!",
        cancelButtonText: "Hủy!"
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: `/Employee/ManagerCombo/Delete`,
                method: 'POST',
                contentType: "application/json",
                data: JSON.stringify(id),
                success: (data) => {
                    console.log("Server response:", data);
                    if (data.success) {
                        Swal.fire("Đã xóa!", data.message, "success");
                        if (dataTable) {
                            dataTable.ajax.reload();
                        }
                    } else {
                        Swal.fire("Lỗi!", data.message, "error");
                    }
                },
                error: (xhr) => {
                    console.error("Xóa thất bại:", xhr.responseText);
                    Swal.fire("Lỗi!", "Có lỗi xảy ra khi xóa dữ liệu.", "error");
                }
            });
        } else {
            toastr.info("Bạn đã hủy xác nhận xóa");
        }
    });
}




function actionUpsertCombo(event) {
    event.preventDefault();
    var formData = new FormData(document.getElementById('formUpsertCombo'));

    // Thêm số lượng cho từng sản phẩm được chọn
    $('#chiTietComboContainer input[type="number"]').each(function () {
        var productId = $(this).attr("name").match(/\d+/)[0];
        var quantity = $(this).val();
        formData.append(`soLuong[${productId}]`, quantity);
    });

    // Kiểm tra giá trị Id và gán mặc định nếu trống
    if (!formData.get("Id")) {
        formData.set("Id", "0");
    }

    // Loại bỏ các trường không hợp lệ hoặc rỗng, bao gồm cả các trường __Invariant
    for (var pair of formData.entries()) {
        if (!pair[1] || pair[0].includes('__Invariant')) {
            formData.delete(pair[0]);
        } else {
            console.log(pair[0] + ', ' + pair[1]);
        }
    }

    $.ajax({
        url: `/Employee/ManagerCombo/Upsert`,
        data: formData,
        method: 'POST',
        processData: false,
        contentType: false,
        success: (data) => {
            if (data.success) {
                toastr.success(data.message);
                if (dataTable) {
                    dataTable.ajax.reload();
                }
            } else {
                $('#viewFormUpsert').html(data.data);
                toastr.error(data.message);
            }
        },
        error: (xhr) => {
            toastr.error("Có lỗi xảy ra: " + xhr.responseText);
            console.error("Error:", xhr.responseText);
        }
    });
}


