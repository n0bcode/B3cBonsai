function loadContainerComments(productId) {
    $.ajax({
        url: '/customer/comment/index',
        data: { productId: productId },
        success: (response) => {
            $('#container-comments').html(response);
        },
        error: (xhr) => {
            toastr.info("Lỗi kết nối với hệ thống, vui lòng thử lại sau.");
        }
    })
}
function addComment(productId) {
    const commentInput = document.getElementById("comment-input");
    const captchaError = document.getElementById("captcha-error");

    if (!commentInput.value.trim()) {
        captchaError.style.display = "block";
        return;
    }

    captchaError.style.display = "none"; // Ẩn thông báo lỗi


    // Gửi bình luận qua Ajax
    $.ajax({
        url: '/Customer/Comment/AddComment',
        type: 'POST',
        data: {
            productId: productId,
            commentContent: commentInput.value.trim()
        },
        success: function (response) {
            if (response.success) {
                loadContainerComments(productId);

                // Xóa nội dung bình luận đã nhập
                commentInput.value = '';
                toastr.success("Bình luận thành công.");
            } else {
                toastr.info(response.message); // Hiển thị thông báo lỗi
            }
        },
        error: function () {
            toastr.info("Đã xảy ra lỗi. Vui lòng thử lại.");
        }
    });
}