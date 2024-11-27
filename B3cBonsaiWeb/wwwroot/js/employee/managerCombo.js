function loadDataTable() {
    dataTable = $('#empoloyees-tbl1').DataTable({
        "ajax": { url: '/employee/ManagerCombo/getall' },
        "columns": [
            {
                data: null,
                "render": function (data) {
                    return `
                        <div class="d-flex align-items-center gap-3">
                            <img src="${data.linkAnh}" class="rounded" alt="Hình ảnh" style="width: 50px; height: 50px; object-fit: cover;" onerror="this.src='/images/product/default.jpg'">
                            <div>
                                <p class="mb-1"><strong>${data.tenCombo || 'N/A'}</strong></p>
                                <p class="mb-0 text-muted">Tổng giá: ${data.tongGia ? data.tongGia.toLocaleString() : 'N/A'} VNĐ</p>
                            </div>
                        </div>`;
                },
                "title": "Combo",
                "width": "30%"
            },
            {
                data: 'chiTietCombos',
                "render": function (data) {
                    if (!data || data.length === 0) return '<span>Không có sản phẩm</span>';
                    return `
                        <div style="max-height: 150px; overflow-y: auto;">
                            ${data.map(item => `
                                <div>
                                    <p><strong>Sản phẩm:</strong> ${item.sanPham.tenSanPham || 'N/A'}</p>
                                    <p><strong>Mô tả:</strong> ${item.sanPham.moTa || 'N/A'}</p>
                                    <p><strong>Giá:</strong> ${item.sanPham.gia.toLocaleString() || 'N/A'} VNĐ</p>
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
                    return `
                        <div class="d-flex justify-content-center gap-2">
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
            "zeroRecords": `<div style="text-align: center;">Không tìm thấy kết quả nào.</div>`,
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
function deleteCombo(id) {
    console.log("Đang xóa combo với ID:", id);
    Swal.fire({
        title: "Bạn có chắc?",
        text: "Bạn sẽ không thể khôi phục lại dữ liệu đã xóa!",
        type: "warning",
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

    // Thu thập dữ liệu từ form
    var formData = new FormData(document.getElementById('formUpsertCombo'));

    // Thêm số lượng sản phẩm vào formData
    $('#chiTietComboContainer input[type="number"]').each(function () {
        var productId = $(this).attr("name").match(/\d+/)[0]; // Lấy ID sản phẩm từ thuộc tính name
        var quantity = $(this).val(); // Lấy số lượng từ input
        formData.append(`soLuong[${productId}]`, quantity); // Thêm vào formData
    });

    // Kiểm tra và gán giá trị mặc định cho trường Id nếu trống
    if (!formData.get("Id")) {
        formData.set("Id", "0"); // Gán giá trị mặc định là "0"
    }

    // Loại bỏ các trường không hợp lệ hoặc rỗng
    for (var pair of formData.entries()) {
        if (!pair[1] || pair[0].includes('__Invariant')) {
            formData.delete(pair[0]); // Xóa trường không hợp lệ
        } else {
            console.log(pair[0] + ', ' + pair[1]); // Ghi log giá trị hợp lệ
        }
    }

    // Gửi yêu cầu AJAX
    $.ajax({
        url: `/Employee/ManagerCombo/Upsert`, // URL API
        data: formData, // Dữ liệu form
        method: 'POST', // Phương thức HTTP
        processData: false, // Không xử lý dữ liệu form
        contentType: false, // Để content-type mặc định của FormData
        success: (data) => {
            if (data.success) {
                toastr.success(data.message); // Hiển thị thông báo thành công
                if (dataTable) {
                    dataTable.ajax.reload(); // Reload bảng dữ liệu nếu tồn tại
                }
                loadViewUpsert();
            } else {
                $('#viewFormUpsert').html(data.data); // Cập nhật lại form nếu có lỗi
                toastr.error(data.message); // Hiển thị thông báo lỗi
            }
        },
        error: (xhr) => {
            toastr.error("Có lỗi xảy ra: " + xhr.responseText); // Hiển thị lỗi từ server
            console.error("Error:", xhr.responseText); // Log lỗi
        }
    });
}

