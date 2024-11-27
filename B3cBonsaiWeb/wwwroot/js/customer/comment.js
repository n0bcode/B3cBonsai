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
    });
}

function containsBadWords(comment) {
    const badWords = ["lồn", "cặc", "địt", "chịch", "buồi", "đụ", "đéo", "điếm", "bitch", "fuck", "dick",
        "pussy", "asshole", "motherfucker", "cu", "chó chết", "dâm", "ngu", "vl", "dm",
        "clgt", "vcl", "phò", "đĩ", "khốn nạn", "con mẹ mày", "đéo mẹ", "nứng", "tổ sư",
        "mẹ kiếp", "bố mày", "liếm lồn", "liếm cặc", "óc chó", "cave", "đm", "wtf", "fucking",
        "shit", "cum", "slut", "whore", "tits", "boobs", "rape", "jerk", "suck", "balls",
        "blowjob", "handjob", "faggot", "gay", "lesbian", "dildo", "vagina", "penis", "anus",
        "scum", "bastard", "đĩ mẹ", "đụ mẹ", "đụ cha", "đụ bà", "mẹ cha", "đù", "fuck you",
        "đéo hiểu", "mẹ nó", "fuck off", "cút", "get lost", "piss off", "liếm buồi", "bú cặc",
        "bú lồn", "bố láo", "chó má", "súc vật", "mất dạy", "khốn", "khốn kiếp", "mẹ kiếp",
        "thằng khốn", "con khốn", "dickhead", "cunt", "shithead", "piss", "pissing", "screw you",
        "goddamn", "son of a bitch", "sonofabitch", "dirty", "mothafucka", "jackass", "douchebag",
        "retard", "fuckface", "cock", "shitbag", "fuckwit", "fuckstick", "arsehole", "tosser",
        "bloody hell", "cuntface", "ballsack", "fucker", "dickhead", "bitchface", "ho",
        "cumdumpster", "dickwad", "twat", "shitfaced", "cockface", "gobshite", "bollocks",
        "minger", "arse", "knobhead", "twatwaffle", "dumbfuck", "shitcunt", "cumslut", "wanker",
        "prick", "fucknugget", "fuckhead", "dickweasel", "cockmongler", "dickfucker",
        "shitweasel", "fucksocks", "fucksponge", "fuckbiscuit", "fuckbucket", "cumguzzler",
        "cockjockey", "shitbrick", "cumbucket", "fucktard", "dicknose", "shitstain",
        "craphole", "fuckpile", "shitstick", "fuckbunny", "fuckrag", "fuckknuckle",
        "shitsmear", "cocksucker", "cocksplat"]; // Danh sách từ ngữ thô tục
    for (let word of badWords) {
        if (comment.toLowerCase().includes(word.toLowerCase())) {
            return true;
        }
    }
    return false;
}

function addComment(productId) {
    const commentInput = document.getElementById("comment-input");
    const captchaError = document.getElementById("captcha-error");

    // Kiểm tra nếu bình luận trống
    if (!commentInput.value.trim()) {
        captchaError.style.display = "block";
        return;
    }

    // Kiểm tra từ ngữ thô tục trong bình luận
    if (containsBadWords(commentInput.value.trim())) {
        toastr.info("Bình luận chứa từ ngữ không phù hợp.");
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


function deleteComment(commentId) {
    // Lưu commentId vào một biến toàn cục hoặc đặt vào modal
    $('#confirmDeleteBtn').data('commentId', commentId);

    // Hiển thị modal xác nhận
    $('#deleteModal').modal('show');
}

// Xử lý sự kiện khi người dùng nhấn "Xóa" trên modal
$('#confirmDeleteBtn').click(function () {
    var commentId = $(this).data('commentId');  // Lấy commentId từ data attribute

    // Gửi yêu cầu xóa bình luận qua Ajax
    $.ajax({
        url: '/Customer/Comment/DeleteComment',
        type: 'POST',
        data: { commentId: commentId },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                loadContainerComments(response.comments[0]?.sanPhamId || null);
            } else {
                toastr.info(response.message);
            }

            // Đóng modal
            $('#deleteModal').modal('hide');
        },
        error: function () {
            toastr.info("Đã xảy ra lỗi. Vui lòng thử lại.");
            $('#deleteModal').modal('hide'); // Đóng modal nếu có lỗi
        }
    });
});
