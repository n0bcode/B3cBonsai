var dataTable;
$(document).ready(function () {
    dataTable = $('#categoryTable').DataTable({
        "ajax": {
            "url": "/Employee/ManagerCategory/GetAllCategories",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "id", "className": "text-center" },
            { "data": "tenDanhMuc" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="d-flex justify-content-center gap-2">
                                        <a onclick="loadViewUpsert(${data})" class="btn btn-primary shadow btn-xs sharp me-1" data-bs-toggle="offcanvas" href="#upsertObject"><i class="fas fa-pencil-alt"></i></a>
                                        <a onclick="handleCategoryDeletion(${data})" class="btn btn-danger shadow btn-xs sharp"><i class="fas fa-trash-alt"></i></a>
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
            "zeroRecords": ` <div style="text-align: center;">Không tìm thấy kết quả nào.</div> `,
            "infoEmpty": "Không có mục nào để hiển thị",
            "infoFiltered": "(lọc từ _MAX_ mục)"
        }
    });
});

function loadViewUpsert(id = null) {
    let url = `/Employee/ManagerCategory/Upsert/${id ?? ""}`;
    $.ajax({
        url: url,
        method: 'GET',
        success: (data) => {
            $('#viewFormUpsert').html(data);
        },
        error: (xhr) => {
            toastr.info("Lỗi tải form, vui lòng thử lại sau.");
        }
    })
}



function submitUpsertForm(event) {
    event.preventDefault(); // Ngăn không cho form tải lại trang

    var formData = new FormData(document.getElementById('formUpsertCombo'));

    $.ajax({
        url: '/Employee/ManagerCategory/Upsert',
        method: 'POST',
        data: formData,
        contentType: false,  // Gửi đúng định dạng form-data
        processData: false,  // Không xử lý dữ liệu
        success: (data) => {
            if (data.success) {
                toastr.success(data.message || "Cập nhật thông tin thành công"); // Hiển thị thông điệp thành công
                dataTable.ajax.reload()

            } else {
                toastr.error(data.message || "Cập nhật thông tin thất bại"); // Hiển thị thông điệp lỗi
            }
        },
        error: (xhr) => {
            toastr.error(xhr.responseText || "Đã xảy ra lỗi không xác định"); // Hiển thị thông điệp lỗi tổng quát
        }
    });

    return false; // Đảm bảo ngăn submit form thông thường
}

function handleCategoryDeletion(categoryId) {
    $.ajax({
        url: '/employee/managercategory/GetOtherCategory',
        type: 'GET',
        data: { id: categoryId },
        success: function (response) {
            console.log(response);
            if (response.success) {
                if (response.amount.length === 0) {
                    // Không có sản phẩm trong danh mục, chỉ hỏi xác nhận
                    Swal.fire({
                        title: "Bạn có chắc chắn?",
                        text: "Danh mục không chứa sản phẩm, bạn có muốn xóa danh mục này?",
                        type: "warning",
                        showCancelButton: true,
                        confirmButtonColor: "#3085d6",
                        cancelButtonColor: "#d33",
                        confirmButtonText: "Vâng, xóa đi!",
                        cancelButtonText: "Hủy"
                    }).then((result) => {
                        if (result.value) {
                            // Gọi AJAX để xóa danh mục
                            $.ajax({
                                url: `/Employee/ManagerCategory/Delete/${categoryId}`,
                                type: 'POST',
                                success: function (data) {
                                    if (data.success) {
                                        dataTable.ajax.reload(); // Tải lại bảng
                                        toastr.success(data.message); // Hiển thị thông báo thành công
                                    } else {
                                        toastr.error(data.message); // Hiển thị thông báo lỗi
                                    }
                                },
                                error: function () {
                                    toastr.error("Đã xảy ra lỗi khi xóa danh mục.");
                                }
                            });
                        }
                    });
                } else {
                    if (response.data.length < 2) {
                        // Nếu chỉ còn dưới 2 danh mục, hiển thị thông báo không thể xóa
                        Swal.fire({
                            title: "Không thể xóa!",
                            text: "Cần ít nhất 2 danh mục để đảm bảo hệ thống hoạt động.",
                            type: "error"
                        });
                    } else {
                        // Có sản phẩm trong danh mục, yêu cầu chọn danh mục thay thế
                        var options = '';
                        $.each(response.data, function (index, item) {
                            options += '<option value="' + item.value + '">' + item.text + '</option>';
                        });

                        Swal.fire({
                            title: "Có " + response.amount + " sản phẩm mang danh mục này. \n" + "Bạn có chắc chắn?",
                            text: "Số lượng sản phẩm trong danh mục này: " + response.amount +
                                ". Bạn cần chọn danh mục thay thế để chuyển sản phẩm trước khi xóa.",
                            type: "warning",
                            html: '<select id="categorySelect" class="form-control">' + options + '</select>',
                            showCancelButton: true,
                            confirmButtonColor: "#3085d6",
                            cancelButtonColor: "#d33",
                            confirmButtonText: "Vâng, tiếp tục!",
                            cancelButtonText: "Hủy"
                        }).then((result) => {
                            if (result.value) {
                                console.log(result)
                                var selectedValue = $('#categorySelect').val();
                                console.log(selectedValue)
                                // Gọi AJAX để chuyển sản phẩm và xóa danh mục
                                $.ajax({
                                    url: `/employee/managercategory/DeleteAndTransferToOtherCategory`,
                                    type: 'POST',
                                    data: {id: categoryId, idChange: selectedValue},
                                    success: function (data) {
                                        if (data.success) {
                                            dataTable.ajax.reload(); // Tải lại bảng
                                            toastr.success(data.content); // Thông báo thành công
                                        } else {
                                            toastr.error(data.content); // Thông báo lỗi
                                        }
                                    },
                                    error: function () {
                                        toastr.error("Đã xảy ra lỗi khi xóa danh mục.");
                                    }
                                });
                            }
                        });
                    }
                }
            } else {
                Swal.fire({
                    title: "Thất bại!",
                    text: response.content,
                    type: "error"
                });
            }
        },
        error: function () {
            Swal.fire({
                title: "Lỗi!",
                text: "Có lỗi xảy ra khi lấy dữ liệu.",
                type: "error"
            });
        }
    });
}
