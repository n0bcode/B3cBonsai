﻿// Thiết lập các tùy chọn mặc định cho toastr
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-bottom-left",
    "preventDuplicates": true,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};



function loadQuickView(id) {
    $.ajax({
        url: `/Customer/ClientProduct/quickview?id=${id}`,
        method: 'GET',
        success: (data) => {
            $('#quickview').html(data);
        },
        error: (xhr) => {
            toastr.info("Lỗi load dữ liệu xem nhanh.");
        }
    })
}



function QuickViewComBo(id) {
    $.ajax({
        url: '/Customer/ClientProduct/QuickViewComBo',
        type: 'GET',
        data: { id: id },
        success: function (response) {
            $('#quickview .modal-content').html(response); // Chèn nội dung vào modal
            $('#quickview').modal('show'); // Hiển thị modal
        },
        error: function () {
            alert("Không thể tải thông tin sản phẩm.");
        }
    });
}   