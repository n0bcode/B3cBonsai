var dataTable;

// Load DataTable for all orders (getall)
function loadOrderDataTable() {
    dataTable = $('#empoloyeestbl2').DataTable({
        "ajax": {
            url: '/employee/ManagerOrder/getall',
            dataSrc: 'data'
        },
        "columns": [
            {
                data: null,
                "render": function (data) {
                    return `<div class="form-check custom-checkbox">
                                <input type="checkbox" class="form-check-input" id="customCheckBox${data.id}" required>
                                <label class="form-check-label" for="customCheckBox${data.id}"></label>
                            </div>`;
                }
            },
            {
                data: 'soTheoDoi',
                "render": function (data, type, row, meta) {
                    return `<span>${meta.row + 1}</span>`;
                }
            },
            {
                data: null,
                "render": function (data) {
                    return `<div class="products">
                    <div>
                      <h6>${decodeURIComponent(data.tenNguoiNhan) || 'N/A'}</h6>
                      <span>${data.id || 'N/A'}</span>
                    </div>
                  </div>`;
                }

            },
            {
                data: 'trangThaiDonHang',
                "render": function (data) {
                    return `<select class="default-select status-select">
                                <option value="Complete" ${data === 'Complete' ? 'selected' : ''}>Complete</option>
                                <option value="Pending" ${data === 'Pending' ? 'selected' : ''}>Pending</option>
                                <option value="Testing" ${data === 'Testing' ? 'selected' : ''}>Testing</option>
                                <option value="Processing" ${data === 'In Progress' ? 'selected' : ''}>In Progress</option>
                                <option value="Cancelled" ${data === 'Cancelled' ? 'selected' : ''}>Cancelled</option>
                            </select>`;
                }
            },
            {
                data: 'ngayDatHang',
                "render": function (data) {
                    return `<span>${new Date(data).toLocaleDateString()}</span>`;
                }
            },
            {
                data: 'ngayNhanHang',
                "render": function (data) {
                    return `<span>${new Date(data).toLocaleDateString()}</span>`;
                }
            },
            {
                data: 'hinhAnhSanPham',
                "render": function (data) {
                    if (Array.isArray(data) && data.length > 0) {
                        return data.map(img => `<img src="${img.linkAnh}" class="product-image" style="width: 40px; height: 40px; border-radius: 5px; margin-right: 5px;" alt="Product Image">`).join('');
                    }
                    return `<span>No images</span>`;
                }
            },
            {
                data: 'trangThaiThanhToan',
                "render": function (data) {
                    return `<span class="badge badge-primary light border-0 me-1">${data}</span>`;
                }
            },
            {
                data: 'tongTienDonHang',
                "render": function (data) {
                    return `${data.toLocaleString('vi-VN')} đ`;
                }
            }
        ]
    });
}


// Load DataTable for order details (ctiet)
function loadOrderDetailDataTable() {
    // Initialize the DataTable
    var dataTable = $('#ctiet').DataTable({
        "ajax": {
            url: '/employee/ManagerOrder/ctiet',  // Endpoint to fetch the order details
            dataSrc: 'data'  // The key containing the data in the JSON response
        },
        "columns": [
            {
                data: null,
                "render": function (data) {
                    return `<div class="form-check custom-checkbox">
                                <input type="checkbox" class="form-check-input" id="customCheckBox${data.id}" required>
                                <label class="form-check-label" for="customCheckBox${data.id}"></label>
                            </div>`;
                }
            },
            {
                data: 'soTheoDoi',
                "render": function (data, type, row, meta) {
                    return `<span>${meta.row + 1}</span>`;
                }
            },
            {
                data: null,
                "render": function (data) {
                    return `<div class="products">
                    <div>
                      <h6>${decodeURIComponent(data.tenNguoiNhan) || 'N/A'}</h6>
                      <span>${data.id || 'N/A'}</span>
                    </div>
                  </div>`;
                }

            },
            {
                data: 'trangThaiDonHang',
                "render": function (data) {
                    return `<select class="default-select status-select">
                                <option value="Complete" ${data === 'Complete' ? 'selected' : ''}>Complete</option>
                                <option value="Pending" ${data === 'Pending' ? 'selected' : ''}>Pending</option>
                                <option value="Testing" ${data === 'Testing' ? 'selected' : ''}>Testing</option>
                                <option value="Processing" ${data === 'In Progress' ? 'selected' : ''}>In Progress</option>
                                <option value="Cancelled" ${data === 'Cancelled' ? 'selected' : ''}>Cancelled</option>
                            </select>`;
                }
            },
            {
                data: 'ngayDatHang',
                "render": function (data) {
                    return `<span>${new Date(data).toLocaleDateString()}</span>`;
                }
            },
            {
                data: 'ngayNhanHang',
                "render": function (data) {
                    return `<span>${new Date(data).toLocaleDateString()}</span>`;
                }
            },
            {
                data: null,
                "render": function (data) {
                    return `<div class="avatar-list avatar-list-stacked">
                                <img src="images/contacts/pic666.jpg" class="avatar rounded-circle" alt="">
                                <img src="images/contacts/pic555.jpg" class="avatar rounded-circle" alt="">
                                <img src="images/contacts/pic666.jpg" class="avatar rounded-circle" alt="">
                            </div>`;

                }
            },
            {
                data: 'trangThaiThanhToan',
                "render": function (data) {
                    return `<span class="badge badge-primary light border-0 me-1">${data}</span>`;
                }
            },
            {
                data: 'tongTienDonHang',
                "render": function (data) {
                    return `${data.toLocaleString('vi-VN')} đ`;
                }
            }
        ]
    });
}

$(document).ready(function () {
    // Initialize DataTable for the first endpoint (orders)
    loadOrderDataTable();
    // Optionally, you can switch to the second DataTable if needed
    loadOrderDetailDataTable();
});

let oldValue = $('select').val(); // Initialize the old value for the select box

$('select').on('focus', function () {
    oldValue = this.value; // Store the old value when the select field is focused
});

$('select').on('change', function (e) {
    const selectElement = $(this); // Store a reference to the select element

    Swal.fire({
        title: "Chỉnh sửa",
        text: "Bạn có muốn thay đổi tình trạng?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Chỉnh!"
    }).then((result) => {
        if (result.value) {
            // If confirmed, show a success message
            toastr.success("Chỉnh sửa thành công", "Điều chỉnh tình trạng", {
                timeOut: 5000,
                closeButton: true,
                positionClass: "toast-top-right"
            });
            oldValue = this.value;  // Update the old value after confirmation
        } else {
            this.value = oldValue;  // Revert to the old value if not confirmed
        }
    });
});



