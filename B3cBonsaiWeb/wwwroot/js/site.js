// Thiết lập các tùy chọn mặc định cho toastr
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

$('input[type="number"]').on("change", function () {
    const afterChange = Number($(this).val()); // Chuyển đổi giá trị đầu vào thành số
    const maxValue = Number($(this).attr('max')); // Chuyển đổi maxValue thành số
    const minValue = Number($(this).attr('min')); // Chuyển đổi minValue thành số

    // Kiểm tra nếu maxValue không được chỉ định
    if (maxValue == null || isNaN(maxValue)) {
        toastr.info(`Bạn không thể tăng số lượng mua đối với sản phẩm này!`);
        $(this).val(minValue); // Đặt lại giá trị về minValue
        return; // Thoát nếu không có maxValue
    }

    // Kiểm tra nếu giá trị sau khi thay đổi lớn hơn maxValue
    if (afterChange > maxValue) {
        toastr.info(`Bạn không thể tăng số lượng sản phẩm này quá ${maxValue}!`);
        $(this).val(maxValue); // Đặt lại giá trị về maxValue
    }

    // Kiểm tra nếu giá trị sau khi thay đổi nhỏ hơn minValue
    if (afterChange < minValue) {
        toastr.info(`Bạn không thể đặt số lượng sản phẩm thấp hơn ${minValue}!`);
        $(this).val(minValue); // Đặt lại giá trị về minValue
    }
});


$('input[type="text"][name="quantity"]').on("change", function () {
    const afterChange = Number($(this).val()); // Chuyển đổi giá trị đầu vào thành số
    const maxValue = Number($(this).attr('max')); // Chuyển đổi maxValue thành số
    const minValue = Number($(this).attr('min')); // Chuyển đổi minValue thành số

    console.log(afterChange)

    // Kiểm tra nếu giá trị sau khi thay đổi không phải là số
    if (isNaN(afterChange)) {
        toastr.info(`Giá trị nhập vào không hợp lệ!`);
        $(this).val(minValue); // Đặt lại giá trị về minValue
        return; // Thoát
    }

    // Kiểm tra nếu giá trị maxValue hoặc minValue chưa được xác định
    if (isNaN(maxValue) || isNaN(minValue)) {
        toastr.info(`Giá trị max hoặc min không hợp lệ!`);
        return; // Thoát nếu không có giá trị hợp lệ
    }

    // Kiểm tra nếu giá trị sau khi thay đổi lớn hơn maxValue
    if (afterChange > maxValue) {
        toastr.info(`Bạn không thể tăng số lượng sản phẩm này quá ${maxValue}!`);
        $(this).val(maxValue); // Đặt lại giá trị về maxValue
        return;
    }

    // Kiểm tra nếu giá trị sau khi thay đổi nhỏ hơn minValue
    if (afterChange < minValue) {
        toastr.info(`Bạn không thể đặt số lượng sản phẩm thấp hơn ${minValue}!`);
        $(this).val(minValue); // Đặt lại giá trị về minValue
        return;
    }
});