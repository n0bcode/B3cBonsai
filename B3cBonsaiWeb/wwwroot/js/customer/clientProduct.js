
const loadGridViewProduct = () => {
    $.ajax({
        url: '/customer/clientproduct/ListPagedProduct',
        method: 'GET',
        data: {},
        success: function (data) {
            $('#gridViewListProduct').html(data);
            applyLikeAction();
        },
        error: (xhr) => {
            console.log(xhr)
        }
    });
}
$(document).ready(function () {
    loadGridViewProduct();
    $(document).on('click', '.pagination-container .pagination-page-box a', function (event) {
        event.preventDefault();
        $.ajax({
            url: $(this).attr('href'),
            method: 'GET',
            success: (data) => {
                $('#gridViewListProduct').html(data);
                applyLikeAction();
            },
            error: (xhr) => {
                createAlert({ title: "Thông báo", content: "Lỗi load dữ liệu, vui lòng thử lại sau", type: 'warning', timeout: 5000 });
            }
        });
    });

    $(document).on('submit', '#filterProductForm', function (event) {
        event.preventDefault();

        // Lấy dữ liệu từ form
        var formData = $(this).serialize();
        $.ajax({
            url: `/customer/clientproduct/ListPagedProduct`,
            method: "GET",
            data: formData,
            success: (data) => {
                $('#gridViewListProduct').html(data);
                applyLikeAction();
            },
            error: (xhr) => {
                console.log(xhr)
            }
        });
    });
});
