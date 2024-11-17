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
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error('Đã xảy ra lỗi, vui lòng thử lại.');
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
                const pricePerItem = parseInt(itemRow.find('.item-price .amount').text().replace(/\D/g, ''));
                const newTotal = response.total;
                itemRow.find('.full-price').text(`${newTotal.toLocaleString()} VNĐ`);

                // Tăng số lượng trên giao diện
                /*const quantityInput = itemRow.find('input[name="quantity"]');
                quantityInput.val(parseInt(quantityInput.val()) + 1);*/

                toastr.success('Đã tăng số lượng sản phẩm!');
            } else {
                toastr.error(response.message);
            }
        },
        error: function () {
            toastr.error('Đã xảy ra lỗi, vui lòng thử lại.');
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
                const pricePerItem = parseInt(itemRow.find('.item-price .amount').text().replace(/\D/g, ''));
                const newTotal = response.total;
                itemRow.find('.full-price').text(`${newTotal.toLocaleString()} VNĐ`);

                // Giảm số lượng trên giao diện
                /*const quantityInput = itemRow.find('input[name="quantity"]');
                const currentQuantity = parseInt(quantityInput.val());
                if (currentQuantity > 1) {
                    quantityInput.val(currentQuantity - 1);
                }*/

                toastr.success('Đã giảm số lượng sản phẩm!');
            } else {
                toastr.error(response.message);
            }
        },
        error: function () {
            toastr.error('Đã xảy ra lỗi, vui lòng thử lại.');
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
                    toastr.success('Đã xóa sản phẩm khỏi giỏ hàng!');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error('Đã xảy ra lỗi, vui lòng thử lại.');
            }
        });
    }

    // Ví dụ gọi các hàm này
    // addToCart(1, null, 1, 'sanPham');
    // increaseQuantity(1);
    // decreaseQuantity(1);
    // removeFromCart(1);
