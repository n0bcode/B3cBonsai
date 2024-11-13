var dataTable;

function loadDataTable() {
    dataTable = $('#products-table').DataTable({
        "ajax": {
            "url": "/Employee/ManagerProduct/GetAll",
            "type": "GET",
            "dataSrc": "data"
        },
        "columns": [
            {
                data: null,
                "render": function (data) {
                    return `<a class="products" role="button" onclick="loadDetailWithDelete('${data.id}')" data-bs-toggle="modal" data-bs-target="#exampleModal">
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
                data: "trangThai", "render": function (data) {
                    return `<span class="badge ${data ? 'badge-success' : 'badge-danger'}">${data ? "Đang hoạt động" : "Đã bị khóa"}</span>`;
                }
            },
            {
                data: null,
                "render": function (data) {
                    return `
                            <div class="d-flex gap-2">
                                <a onclick="loadViewUpsert('${data.id}')" class="btn btn-primary shadow btn-xs sharp me-1" data-bs-toggle="offcanvas" href="#upsertObject"><i class="fas fa-pencil-alt"></i></a>
                                <button class="btn btn-danger shadow btn-xs sharp me-1" onclick="deleteProduct(${data.id})"> <i class="fa-solid fa-trash"></i> </button>
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
            "zeroRecords": "Không tìm thấy kết quả nào",
            "infoEmpty": "Không có mục nào để hiển thị",
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
            renderDataInView(data.data);

        },
        error: (xhr) => {
            toastr.error("Lỗi tải thông tin đối tượng")
        }
    })
}

// render dữ liệu ra view
const renderDataInView = (data) => {
    // Đảm bảo data không null
    console.log(data);

    if (!data) {
        console.error('No data received');
        return;
    }

    // Thêm modal- prefix cho các ID để khớp với modal
    $('#modal-productName').text(data.tenSanPham || 'N/A');
    $('#modal-productCategory').text(data.tenDanhMuc || 'N/A');
    $('#modal-productPrice').text(
        (data.gia || 0).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })
    );
    $('#modal-productQuantity').text(data.soLuong || '0');
    $('#modal-productDescription').text(data.moTa || 'Không có mô tả');

    // Xóa nội dung cũ trong carousel-inner
    const carouselInner = $('#productImageCarousel .carousel-inner');
    carouselInner.empty();

    // Lặp qua danh sách hình ảnh và thêm các slide
    data.hinhAnhs.forEach((url, index) => {
        const isActive = index === 0 ? 'active' : '';
        const slide = `
        <div class="carousel-item ${isActive}">
            <img src="${url.linkAnh}" class="d-block w-100" alt="Product Image ${index + 1}" style="height: 20rem; object-fit: cover;">
        </div>`;
        carouselInner.append(slide);
    });


    // Xóa nội dung cũ trong video player
    const videoPlayer = $('#productVideo');
    videoPlayer.empty();

    // Lấy URL video đầu tiên và thêm vào source của video player
    const videoSource = `<source src="${data.videos[0].linkVideo}" type="video/mp4">`;
    videoPlayer.append(videoSource);

    // Nếu có nhiều video, hiển thị danh sách nút hoặc thumbnail để người dùng chọn video
    if (data.videos.length > 1) {
        const thumbnailContainer = $('#thumbnailContainer');
        thumbnailContainer.empty(); // Xóa nội dung cũ nếu có

        data.videos.forEach((videoUrl, index) => {
            const thumbnail = `
            <button class="btn btn-outline-primary me-2" onclick="changeVideo('${videoUrl}')">
                Video ${index + 1}
            </button>`;
            thumbnailContainer.append(thumbnail);
        });
    }
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



function deleteProduct(id) {
    Swal.fire({
        title: "Bạn có chắc chắn?",
        text: "Bạn sẽ không thể khôi phục lại sản phẩm này!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Vâng, xóa đi!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/Employee/ManagerProduct/Delete",
                method: "POST",
                data: { id: id },
                success: function (response) {
                    Swal.fire("Đã xóa!", "Sản phẩm đã bị xóa.", "success");
                    dataTable.ajax.reload();
                },
                error: function (xhr, status, error) {
                    Swal.fire("Lỗi!", "Không thể xóa sản phẩm.", "error");
                }
            });
        }
    });
}
