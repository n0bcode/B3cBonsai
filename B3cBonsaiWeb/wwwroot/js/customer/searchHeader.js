
// Hàm loại bỏ dấu tiếng Việt
function removeVietnameseTones(str) {
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/Đ/g, "D");
    str = str.replace(/đ/g, "d");

    // Xóa các dấu kết hợp
    str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323|\u0331|\u0340|\u0341|\u0343|\u0344|\u0350|\u0351|\u0352|\u0357|\u0358|\u0359|\u0363|\u0364|\u0365|\u0366|\u0367|\u0368|\u0369|\u036A|\u036B|\u036C|\u036D|\u036E|\u036F/g, "");
    return str;
}

document.getElementById('search-input').addEventListener('input', function () {
    const query = this.value.trim();
    const suggestionsBox = document.getElementById('suggestions');

    // Xóa các gợi ý cũ
    suggestionsBox.innerHTML = '';

    if (query.length > 2) {
        // Loại bỏ dấu từ từ khóa tìm kiếm
        const normalizedQuery = removeVietnameseTones(query.toLowerCase());

        fetch(`/api/products?q=${encodeURIComponent(query)}`)
            .then(response => response.json())
            .then(data => {
                if (data.length === 0) {
                    suggestionsBox.innerHTML = '<div>Không tìm thấy sản phẩm nào.</div>';
                } else {
                    let hasResults = false;
                    console.log(data);
                    // Hiển thị danh sách gợi ý
                    data.forEach(product => {
                        // Loại bỏ dấu từ tên sản phẩm
                        const normalizedProductName = removeVietnameseTones(product.tenSanPham.toLowerCase());

                        // Kiểm tra nếu tên sản phẩm khớp với từ khóa tìm kiếm
                        if (normalizedProductName.includes(normalizedQuery)) {
                            hasResults = true;
                            const suggestionItem = document.createElement('div');
                            suggestionItem.innerHTML = `<a href="/customer/clientproduct/detail?id=${product.id}" class="d-flex" style="width: 30rem;">
            <img src="${product.hinhAnhs.length > 0 ? product.hinhAnhs[0] : '/images/product/default.jpg'}" class="card-img-left" style="width: 110px; height: 110px; object-fit: cover;" alt="${product.tenSanPham}">
                    <div class="card-body d-flex" style="padding-left: 15px;">
            <!-- Cột trái: Tên sản phẩm và giá -->
            <div class="d-flex flex-column me-3">
                        <h5 class="card-title">${product.tenSanPham} (Kho: ${product.soLuong})</h5>
                <p class="card-text">${"Giá: " + product.gia}</p>
            </div>
        </div>

        </a>



                                                            `;
                            suggestionItem.addEventListener('click', () => {
                                // Xử lý khi người dùng chọn sản phẩm
                                document.getElementById('search-input').value = product.tenSanPham;
                                suggestionsBox.innerHTML = ''; // Ẩn gợi ý
                            });
                            suggestionsBox.appendChild(suggestionItem);
                        }
                    });

                    // Nếu không có kết quả nào khớp
                    if (!hasResults) {
                        suggestionsBox.innerHTML = '<div>Không có sản phẩm phù hợp với tìm kiếm.</div>';
                    }
                }
            })
            .catch(error => {
                console.error('Lỗi khi tải gợi ý:', error);
                suggestionsBox.innerHTML = '<div>Lỗi khi tải dữ liệu.</div>';
            });
    }
});