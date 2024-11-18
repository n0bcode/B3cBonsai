$(document).ready(function () {
    // Khởi chạy kiểm tra ban đầu cho các phần tử đã có sẵn
    applyLikeAction();

    // Sử dụng sự kiện on cho các phần tử động
    $(document).on('click', '.wishlist', function (e) {
        e.preventDefault(); // Ngăn điều hướng mặc định
        const $this = $(this);
        const objectId = $this.data('id');
        const objectType = $this.data('type');

        // Gọi API để thêm hoặc xóa thích
        likeOrNot(objectId, objectType, function () {
            const icon = $this.find('.likeAction i');
            icon.toggleClass('feather-heart fas fa-heart');

            // Cập nhật tooltip nếu cần
            const tooltip = $this.find('.tooltip-text');
            if (icon.hasClass('fas fa-heart')) {
                tooltip.text('Bỏ yêu thích');
            } else {
                tooltip.text('Yêu thích');
            }
        });
    });
});

// Hàm để áp dụng kiểm tra trạng thái thích
function applyLikeAction() {
    $('.wishlist').each(function () {
        const $this = $(this);
        const objectId = $this.data('id');
        const objectType = $this.data('type');

        // Gọi API kiểm tra trạng thái
        checkIfLiked(objectId, objectType, function (isLiked) {
            const icon = $this.find('.likeAction i');
            if (isLiked) {
                icon.removeClass('feather-heart').addClass('fas fa-heart');
            }
        });
    });
}

// Hàm kiểm tra trạng thái thích
function checkIfLiked(idObject, loaiDoiTuong, callback) {
    $.ajax({
        url: '/customer/wishlist/isliked',
        type: 'GET',
        data: { idObject: idObject, loaiDoiTuong: loaiDoiTuong },
        success: function (response) {
            if (response.success) {
                callback(response.isLiked); // Sử dụng isLiked từ phản hồi API
            } else {
                toastr.info(response.message || "Đã xảy ra lỗi khi kiểm tra trạng thái thích.");
            }
        },
        error: function () {
            toastr.error("Không thể kết nối đến máy chủ. Vui lòng thử lại.");
        }
    });
}


// Hàm thêm hoặc xóa thích
function likeOrNot(idObject, loaiDoiTuong, callback) {
    $.ajax({
        url: '/customer/wishlist/likeornot',
        type: 'POST',
        data: { objectId: idObject, loaiDoiTuong: loaiDoiTuong },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message ?? "Đã yêu thích thành công.");
                callback();
            } else {
                toastr.info(response.message || "Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        },
        error: function () {
            toastr.error("Không thể kết nối đến máy chủ. Vui lòng thử lại.");
        }
    });
}
