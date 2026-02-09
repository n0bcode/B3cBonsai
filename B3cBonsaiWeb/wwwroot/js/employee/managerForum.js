var dataTable;

function loadDataTable() {
    dataTable = $('#forum-table').DataTable({
        "ajax": {
            "url": "/Employee/ManagerForum/GetAll",
            "type": "GET",
            "dataSrc": "data"
        },
        "columns": [
            {data: 'id', "className":"text-center"},
            {
                data: null,
                "render": function (data) {
                    return `<a class="forum-thread" role="button" onclick="loadForumDetails('${data.id}')" data-bs-toggle="modal" data-bs-target="#exampleModalForum">
                                <div>
                                    <h6>${data.tieuDe} ${data.hasImage ? '<i class="fa fa-image text-muted ms-1" title="Có hình ảnh"></i>' : ''}</h6>
                                </div>
                            </a>`;
                }
            },
            { data: "danhMuc" },
            { data: "nguoiTao" },
            { data: "ngayTao", "render": function (data) { return new Date(data).toLocaleDateString(); } },
            { data: "luotXem" },
            {
                data: null, "render": function (data) {
                    return `<span onclick="changeStatus(${data.id})" class="badge ${data.trangThai ? 'badge-success' : 'badge-danger'}" style="cursor:pointer;">${data.trangThai ? "Mở" : "Đóng"}</span>`;
                }
            },
            {
                data: null,
                "render": function (data) {
                    return `
                            <div class="d-flex gap-2">
                                <button onclick="deleteThread('${data.id}')" class="btn btn-danger shadow btn-xs sharp me-1"> <i class="fa-solid fa-trash"></i> </button>
                            </div>
                    `;
                }
            }
        ],
        "language": {
            "sSearch": "Tìm kiếm:",
            "lengthMenu": "Hiển thị _MENU_ mục",
            "info": "Hiển thị _START_ đến _END_ trong tổng số _TOTAL_ mục",
            "paginate": { "first": "<<", "last": ">>", "next": ">", "previous": "<" },
            "zeroRecords": "Không tìm thấy kết quả nào.",
            "infoEmpty": "Không có mục nào để hiển thị."
        }
    });
}

const loadForumDetails = (id) => {
    $.ajax({
        url: `/Employee/ManagerForum/Details?id=${id}`,
        method: 'GET',
        success: (data) => {
            $('#contentForumDetail').html(data);
        },
        error: (xhr) => {
            toastr.error("Lỗi tải thông tin bài viết")
        }
    })
}

function changeStatus(id) {
    Swal.fire({
        title: "Bạn có chức chắn?",
        text: "Thay đổi trạng thái (Mở/Đóng) của chủ đề này?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Thay đổi"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/Employee/ManagerForum/ChangeStatus",
                method: "POST",
                data: { id: id },
                success: function (response) {
                    if(response.success){
                        toastr.success("Thay đổi trạng thái thành công");
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(response.message);
                    }
                }
            });
        }
    });
}

function deleteThread(id) {
    Swal.fire({
        title: "Xóa chủ đề?",
        text: "Hành động này sẽ xóa vĩnh viễn chủ đề và các bình luận liên quan!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Xóa vĩnh viễn"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/Employee/ManagerForum/Delete",
                method: "POST",
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        Swal.fire("Đã xóa!", "Chủ đề đã được xóa.", "success");
                        dataTable.ajax.reload();
                    } else {
                        Swal.fire("Lỗi!", response.message, "error");
                    }
                }
            });
        }
    });
}

$(document).ready(function () {
    loadDataTable();
});
