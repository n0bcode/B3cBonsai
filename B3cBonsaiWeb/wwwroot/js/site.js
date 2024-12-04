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
            initQuantityButtons();
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



//Show/hide password
document.addEventListener("DOMContentLoaded", function () {
    const togglePassword = document.getElementById("toggle-password");
    const passwordField = document.getElementById("password-field");
    const iconEye = document.getElementById("icon-eye");
    if (togglePassword != null) {
        togglePassword.addEventListener("click", function () {
            // Kiểm tra loại input hiện tại
            const isPassword = passwordField.type === "password";
            // Thay đổi loại input
            passwordField.type = isPassword ? "text" : "password";
            // Thay đổi biểu tượng mắt
            iconEye.classList.toggle("fa-eye", isPassword);
            iconEye.classList.toggle("fa-eye-slash", !isPassword);
        });
    }
});


$(document).ready(function () {
    // Lấy ngày hiện tại
    const today = new Date();
    const todayString = today.toISOString().split('T')[0]; // Định dạng yyyy-mm-dd

    // Ngày min là năm 1950
    const minDate = new Date(1950, 0, 1).toISOString().split('T')[0]; // Định dạng yyyy-mm-dd

    // Thiết lập cho tất cả các input type="date" và type="datetime-local"
    $('input[type="date"], input[type="datetime-local"]').each(function () {
        // Kiểm tra và thiết lập min nếu chưa có
        if (!$(this).attr('min')) {
            $(this).attr('min', minDate); // Thiết lập giá trị min nếu chưa có
        }

        // Kiểm tra và thiết lập max nếu chưa có
        if (!$(this).attr('max')) {
            $(this).attr('max', todayString); // Thiết lập giá trị max nếu chưa có
        }
    });
});


//region
function initQuantityButtons() {
    const $quantityInput = $('#quantityValue');
    const $minusBtn = $('#minusBtn');
    const $plusBtn = $('#plusBtn');
    const maxQuantity = parseInt($quantityInput.attr('max'), 10);

    // Khởi động trạng thái ban đầu cho các nút
    updateButtonStates($quantityInput, $minusBtn, $plusBtn, maxQuantity);

    // Thêm sự kiện click cho nút giảm
    $minusBtn.on('click', function () {
        changeQuantity('decrease', $quantityInput, $minusBtn, $plusBtn, maxQuantity);
    });

    // Thêm sự kiện click cho nút tăng
    $plusBtn.on('click', function () {
        changeQuantity('increase', $quantityInput, $minusBtn, $plusBtn, maxQuantity);
    });
}

function updateButtonStates($quantityInput, $minusBtn, $plusBtn, maxQuantity) {
    const currentQuantity = parseInt($quantityInput.val(), 10);
    $minusBtn.prop('disabled', currentQuantity <= 1);
    $plusBtn.prop('disabled', currentQuantity >= maxQuantity);
}

function changeQuantity(action, $quantityInput, $minusBtn, $plusBtn, maxQuantity) {
    let currentQuantity = parseInt($quantityInput.val(), 10);

    if (action === 'decrease' && currentQuantity > 1) {
        currentQuantity--;
    } else if (action === 'increase' && currentQuantity < maxQuantity) {
        currentQuantity++;
    }

    $quantityInput.val(currentQuantity);
    updateButtonStates($quantityInput, $minusBtn, $plusBtn, maxQuantity);
}
//endregion