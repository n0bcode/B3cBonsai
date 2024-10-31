let oldValue = $('select').val(); // Khởi tạo giá trị cũ ngay từ đầu

$('select').on('focus', function () {
    console.log(this.value)
    oldValue = this.value; // Lưu giá trị cũ khi trường được focus
});

$('select').on('change', function (e) {
    const selectElement = $(this); // Lưu tham chiếu đến phần tử select

    Swal.fire({
        title: "Chỉnh sửa",
        text: "Bạn có muốn thay đổi tình trạng?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Chỉnh!"
    }).then((result) => {
        console.log(result)
        if (result.value) {
            // Nếu người dùng xác nhận, hiển thị thông báo thành công
            toastr.success("Chỉnh sửa thành công", "Điều chỉnh tình trạng", {
                timeOut: 5000,
                closeButton: true,
                debug: false,
                newestOnTop: true,
                progressBar: true,
                positionClass: "toast-top-right",
                preventDuplicates: true,
                onclick: null,
                showDuration: "300",
                hideDuration: "1000",
                extendedTimeOut: "1000",
                showEasing: "swing",
                hideEasing: "linear",
                showMethod: "fadeIn",
                hideMethod: "fadeOut",
                tapToDismiss: false
            });
            oldValue = this.value;
        } else {
            console.log(selectElement);
            console.log(oldValue);
            this.value = oldValue;
        }
    });
});
