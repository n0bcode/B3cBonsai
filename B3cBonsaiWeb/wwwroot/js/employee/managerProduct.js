var dataTable;

function loadDataTable() {
    dataTable = $('#products-table').DataTable({
        "ajax": {
            "url": "/Employee/ManagerProduct/GetAll",
            "type": "GET",
            "dataSrc": "data"
        },
        "columns": [
            {data: 'id',"className":"text-center"},
            {
                data: null,
                "render": function (data) {
                    return `<a class="products" role="button" onclick="loadDetailWithDelete('${data.id}')" data-bs-toggle="modal" data-bs-target="#exampleModal1">
                                <img src="${data.hinhAnhs.length > 0 ? data.hinhAnhs[0].linkAnh : '/images/product/default.jpg'}" class="avatar avatar-md" alt="">
                                <div>
                                    <h6>${data.tenSanPham || 'N/A'}</h6>
                                </div>
                            </a>`;
                }
            },
            { data: "tenDanhMuc" },
            { data: "soLuong" },
            { data: "gia", "render": $.fn.dataTable.render.number(',', '.', 0, '', ' VND') },
            { data: "ngayTao", "render": function (data) { return new Date(data).toLocaleDateString(); } },
            { data: "ngaySuaDoi", "render": function (data) { return new Date(data).toLocaleDateString(); } },
            {
                data: null, "render": function (data) {
                    return `<span  onclick="changeStatus(${data.id})" class="badge ${data.trangThai ? 'badge-success' : 'badge-danger'}">${data.trangThai ? "Đang hoạt động" : "Đã bị khóa"}</span>`;
                }
            },
            {
                data: null,
                "render": function (data) {
                    return `
                            <div class="d-flex gap-2">
                                <a onclick="loadViewUpsert('${data.id}')" class="btn btn-primary shadow btn-xs sharp me-1" data-bs-toggle="offcanvas" href="#upsertObject"><i class="fas fa-pencil-alt"></i></a>
                                <button onclick="DeleteDWD('${data.id}')"class="btn btn-danger shadow btn-xs sharp me-1"> <i class="fa-solid fa-trash"></i> </button>
                            </div>
                    `;
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
            "zeroRecords": ` <div style="text-align: center;">Không tìm thấy kết quả nào.</div> `,
            "infoEmpty": "Không có mục nào để hiển thị.",
            "infoFiltered": "(lọc từ _MAX_ mục)"
        }
    });
}

function addProduct() {
    var product = {
        TenSanPham: $('#productName').val(),
        DanhMucId: $('#category').val(),
        SoLuong: $('#quantity').val(),
        Gia: $('#price').val(),
        TrangThai: $('#status').is(':checked'),
        MoTa: $('#description').val()
    };

    $.ajax({
        url: '/Employee/ManagerProduct/Upsert',
        type: 'POST',
        data: JSON.stringify(product),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                Swal.fire("Thành công!", "Sản phẩm đã được thêm.", "success");
                dataTable.ajax.reload();
                var myOffcanvas = bootstrap.Offcanvas.getInstance(document.getElementById('upsertObject'));
                myOffcanvas.hide();
            } else {
                Swal.fire("Lỗi!", response.message || "Không thể thêm sản phẩm.", "error");
            }
        },
        error: function () {
            Swal.fire("Lỗi!", "Có lỗi xảy ra trong quá trình thêm sản phẩm.", "error");
        }
    });
}

$('form').on('submit', function (e) {
    e.preventDefault();
    addProduct();
});

$(document).ready(function () {

    loadDataTable();

    var myOffcanvas = new bootstrap.Offcanvas(document.getElementById('upsertObject'));
    $('[data-bs-toggle="offcanvas"]').on('click', function () {
        myOffcanvas.show();
    });
});

const loadDetailWithDelete = (id) => {
    $.ajax({
        url: `/Employee/ManagerProduct/DetailWithDelete?id=${id}`,
        method: 'GET',
        success: (data) => {
            $('#contentDWD').html(data);
        },
        error: (xhr) => {
            toastr.error("Lỗi tải thông tin đối tượng")
        }
    })
}

function changeVideo(videoUrl) {
    videoPlayer.attr('src', videoUrl);
    videoPlayer[0].load(); // Tải lại video mới
    videoPlayer[0].play(); // Tự động phát video mới
}

const loadViewUpsert = (id) => {
    $.ajax({
        url: `/Employee/ManagerProduct/Upsert?id=${id}`,
        method: 'GET',
        success: (data) => {
            $('#upsertObject').html(data);
        },
        error: (xhr) => {
            toastr.error("Lỗi tạo form tạo đối tượng người dùng.")
        }
    })
}



function changeStatus(id) {
    Swal.fire({
        title: "Bạn có chắc chắn?",
        text: "Thay đổi tình trạng hiển thị của sản phẩm đối với khách hàng!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Thay đổi",
        cancelButtonText: "Hủy"
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: "/Employee/ManagerProduct/ChangeStatus",
                method: "POST",
                data: { id: id },
                success: function (response) {
                    Swal.fire("Thay đổi!", "Tình trạng thay đổi.", "success");
                    dataTable.ajax.reload();
                },
                error: function (xhr, status, error) {
                    Swal.fire("Lỗi!", "Không thể thay đổi tình trạng sản phẩm.", "error");
                }
            });
        }
    });
}


function DeleteDWD(id) {
    Swal.fire({
        title: "Bạn có chắc chắn?",
        text: "Xóa sản phẩm này",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Xóa",
        cancelButtonText: "Hủy"
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: "/Employee/ManagerProduct/Delete",
                method: "POST",
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        Swal.fire("Đã xóa!", "Sản phẩm đã xóa.", "success");
                        dataTable.ajax.reload();
                    } else {
                        Swal.fire("Xóa!", response.message, "info");
                    }
                },
                error: function (xhr, status, error) {
                    Swal.fire("Lỗi!", "Không thể thay đổi tình trạng sản phẩm.", "error");
                }
            });
        }
    });
}
