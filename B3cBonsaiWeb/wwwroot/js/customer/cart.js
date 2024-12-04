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

    // Thêm sản phẩm vào giỏ hàng
function addToCart(sanPhamId, comboId, soLuong, loaiDoiTuong) {
    /*console.log(sanPhamId);
    console.log(comboId);*/
    $.ajax({
        url: '/Customer/Cart/Add',
        type: 'POST',
        data: {
            sanPhamId: sanPhamId,
            comboId: comboId,
            soLuong: soLuong,
            loaiDoiTuong: loaiDoiTuong
        },
        success: function (response) {
            if (response.success) {
                toastr.success('Thêm vào giỏ hàng thành công!');
            } else {
                toastr.info(response.message);
            }
        },
        error: function () {
            toastr.error('Không thể kết nối đến máy chủ. Vui lòng thử lại.');
        }
    });
}
// Tăng số lượng sản phẩm trong giỏ hàng
function increaseQuantity(cartId) {
    $.ajax({
        url: '/Customer/Cart/Plus',
        type: 'POST',
        data: { cartId: cartId },
        success: function (response) {
            if (response.success) {
                // Cập nhật giá total của sản phẩm
                const itemRow = $(`[data-cart-id="${cartId}"]`).closest('ul.cart-wrap');
                const newTotal = response.total;
                itemRow.find('.full-price').text(`${newTotal.toLocaleString()} đ`);

                const formCart = $(`#shipping-calculator`).closest('.cart-page.section-pt');
                formCart.find('.amount.total-price').text(`${response.totalAll.toLocaleString()} đ`);

                // Tăng số lượng trên giao diện
                /*const quantityInput = itemRow.find('input[name="quantity"]');
                quantityInput.val(parseInt(quantityInput.val()) + 1);*/

                toastr.success('Đã tăng số lượng sản phẩm!');
            } else {
                toastr.info(response.message);
            }
        },
        error: function () {
            toastr.error('Không thể kết nối đến máy chủ. Vui lòng thử lại.');
        }
    });
}

// Giảm số lượng sản phẩm trong giỏ hàng
function decreaseQuantity(cartId) {
    $.ajax({
        url: '/Customer/Cart/Minus',
        type: 'POST',
        data: { cartId: cartId },
        success: function (response) {
            if (response.success) {
                // Cập nhật giá total của sản phẩm
                const itemRow = $(`[data-cart-id="${cartId}"]`).closest('ul.cart-wrap');
                const newTotal = response.total;
                itemRow.find('.full-price').text(`${newTotal.toLocaleString()} đ`);

                const formCart = $(`#shipping-calculator`).closest('.cart-page.section-pt');
                formCart.find('.amount.total-price').text(`${response.totalAll.toLocaleString()} đ`);
                // Giảm số lượng trên giao diện
                /*const quantityInput = itemRow.find('input[name="quantity"]');
                const currentQuantity = parseInt(quantityInput.val());
                if (currentQuantity > 1) {
                    quantityInput.val(currentQuantity - 1);
                }*/

                toastr.success('Đã giảm số lượng sản phẩm!');
            } else {
                toastr.info(response.message);
            }
        },
        error: function () {
            toastr.error('Không thể kết nối đến máy chủ. Vui lòng thử lại.');
        }
    });
}


// Xóa sản phẩm khỏi giỏ hàng
function removeFromCart(cartId) {
    $.ajax({
        url: '/Customer/Cart/Remove',
        type: 'POST',
        data: { cartId: cartId },
        success: function (response) {
            if (response.success) {
                const itemRow = $(`[data-cart-id="${cartId}"]`).closest('ul.cart-wrap');
                if (response.totalAll == 0) {
                    location.reload();
                } else {
                    formCart.find('.amount.total-price').text(`${response.totalAll.toLocaleString()} đ`);
                    itemRow.remove();
                    toastr.success('Đã xóa sản phẩm khỏi giỏ hàng!');
                }
            } else {
                toastr.info(response.message);
            }
        },
        error: function () {
            toastr.error('Không thể kết nối đến máy chủ. Vui lòng thử lại.');
        }
    });
}

function clearCart() {
    $.ajax({
        url: '/Customer/Cart/ClearCart',
        method: 'POST',
        success: (response) => { window.location.reload(); },
        error: (xhr) => { toastr.info("Lỗi xóa dữ liệu toàn bộ giỏ hàng!"); }
    })
}

// Load thanh giỏ hàng xem nhanh --------------------
function loadRightBarCart() {
    $.ajax({
        url: '/Customer/Cart/RightBarCart',
        type: 'GET',
        success: (response) => {
            $('#viewRightBarCart').html(response);
        },
        error: (xhr) => {
            toastr.info("Lỗi tạo giỏ hàng nhanh, vui lòng thử lại sau.");
        }
    })
}

// Đưa dữ liệu giỏ hàng xem nhanh vào
// Lấy phần tử giỏ hàng
const cartDrawer = document.getElementById('cart-drawer');

// Hàm kiểm tra class và gọi hàm khi có sự thay đổi
function checkCartDrawer() {
    if (cartDrawer.classList.contains('active')) {
        loadRightBarCart();
    }
}

// Theo dõi sự thay đổi class
const observer = new MutationObserver(checkCartDrawer);

// Bắt đầu theo dõi sự thay đổi class
observer.observe(cartDrawer, { attributes: true });

// Ví dụ về cách thêm class "active" (bạn có thể thay đổi theo cách bạn làm)
document.querySelector('.drawer-close-btn').addEventListener('click', () => {
    cartDrawer.classList.toggle('active');
});


// Tăng số lượng sản phẩm trong giỏ hàng
function increaseQuantityQuick(cartId) {
    $.ajax({
        url: '/Customer/Cart/Plus',
        type: 'POST',
        data: { cartId: cartId },
        success: function (response) {
            if (response.success) {
                // Cập nhật giá total của sản phẩm
                const itemRow = $(`[data-cart-idQuick="${cartId}"]`).closest('li.cart-item');
                const newTotal = response.total;
                itemRow.find('.cart-item-price').text(`${newTotal.toLocaleString()} đ`);

                const formCart = itemRow.closest('.drawer-inner');
                formCart.find('.drawer-totalprice').text(`${response.totalAll.toLocaleString()} đ`);

                // Tăng số lượng trên giao diện
                const quantityInput = itemRow.find('input[name="quantity"]');
                quantityInput.val(parseInt(quantityInput.val()) + 1);

                toastr.success('Đã tăng số lượng sản phẩm!');
            } else {
                toastr.info(response.message);
            }
        },
        error: function () {
            toastr.error('Không thể kết nối đến máy chủ. Vui lòng thử lại.');
        }
    });
}

// Giảm số lượng sản phẩm trong giỏ hàng
function decreaseQuantityQuick(cartId) {
    $.ajax({
        url: '/Customer/Cart/Minus',
        type: 'POST',
        data: { cartId: cartId },
        success: function (response) {
            if (response.success) {
                // Cập nhật giá total của sản phẩm
                const itemRow = $(`[data-cart-idQuick="${cartId}"]`).closest('li.cart-item');
                const newTotal = response.total;
                itemRow.find('.cart-item-price').text(`${newTotal.toLocaleString()} đ`);

                const formCart = itemRow.closest('.drawer-inner');
                formCart.find('.drawer-totalprice').text(`${response.totalAll.toLocaleString()} đ`);

                // Tăng số lượng trên giao diện
                const quantityInput = itemRow.find('input[name="quantity"]');
                quantityInput.val(parseInt(quantityInput.val()) - 1);

                toastr.success('Đã giảm số lượng sản phẩm!');
            } else {
                toastr.info(response.message);
            }
        },
        error: function () {
            toastr.error('Không thể kết nối đến máy chủ. Vui lòng thử lại.');
        }
    });
}


// Xóa sản phẩm khỏi giỏ hàng
function removeFromCartQuick(cartId) {
    $.ajax({
        url: '/Customer/Cart/Remove',
        type: 'POST',
        data: { cartId: cartId },
        success: function (response) {
            if (response.success) {
                const itemRow = $(`[data-cart-idQuick="${cartId}"]`).closest('li.cart-item');
                if (response.totalAll == 0) {
                    loadRightBarCart();
                } else {
                    var formCart = itemRow.closest('.drawer-inner').find('.drawer-totalprice').text(`${response.totalAll.toLocaleString()} đ`);
                    itemRow.remove();
                    toastr.success('Đã xóa sản phẩm khỏi giỏ hàng!');
                }
            } else {
                toastr.info(response.message);
            }
        },
        error: function () {
            toastr.error('Không thể kết nối đến máy chủ. Vui lòng thử lại.');
        }
    });
}


function toggleCheckoutButton() {
    const isChecked = $('#agreeTerms').is(':checked'); // Kiểm tra checkbox
    const hasItems = $('.cart-items').children().length > 0; // Kiểm tra số lượng sản phẩm

    if (isChecked && hasItems) {
        $('#checkoutButton').removeClass('disabled'); // Bật nút
    } else {
        $('#checkoutButton').addClass('disabled'); // Tắt nút
    }
}

function navigateToPayment() {
    // Kiểm tra nếu nút đang bị disabled thì không làm gì cả
    if ($('#checkoutButton').hasClass('disabled')) {
        toastr.error("Vui lòng kiểm tra giỏ hàng hoặc đồng ý với điều khoản.");
        return;
    }

    // Điều hướng tới trang thanh toán
    window.location.href = '/Customer/Payment';
}
