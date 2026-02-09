var dataTable;

function loadDataTable() {
    dataTable = $('#journal-table').DataTable({
        "ajax": {
            "url": "/Employee/ManagerJournal/GetAll",
            "type": "GET",
            "dataSrc": "data"
        },
        "columns": [
            {data: 'id', "className":"text-center"},
            {
                data: 'hinhAnh',
                "render": function (data) {
                    return `<img src="${data ? data : 'https://placehold.co/100x100?text=No+Img'}" class="avatar avatar-md" alt="">`;
                }
            },
            { data: 'cay' },
            { data: 'nguoiDung' },
            { data: 'ngayTao', "render": function (data) { return new Date(data).toLocaleDateString(); } },
            { data: 'giaiDoanPhatTrien', "render": function(data) { return data ? `<span class="badge badge-info">${data}</span>` : ''; } },
            { data: 'noiDung', "render": function(data) { return data.length > 50 ? data.substring(0, 50) + '...' : data; } },
            {
                data: null,
                "render": function (data) {
                    return `
                            <div class="d-flex gap-2">
                                <button onclick="loadJournalDetails('${data.id}')" data-bs-toggle="modal" data-bs-target="#exampleModalJournal" class="btn btn-primary shadow btn-xs sharp me-1"> <i class="fa-solid fa-eye"></i> </button>
                                <button onclick="deleteJournal('${data.id}')" class="btn btn-danger shadow btn-xs sharp me-1"> <i class="fa-solid fa-trash"></i> </button>
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

const loadJournalDetails = (id) => {
    $.ajax({
        url: `/Employee/ManagerJournal/Details?id=${id}`,
        method: 'GET',
        success: (data) => {
            $('#contentJournalDetail').html(data);
        },
        error: (xhr) => {
            toastr.error("Lỗi tải thông tin nhật ký")
        }
    })
}

function deleteJournal(id) {
    Swal.fire({
        title: "Xóa nhật ký?",
        text: "Hành động này sẽ xóa vĩnh viễn nhật ký này!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Xóa vĩnh viễn"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/Employee/ManagerJournal/Delete",
                method: "POST",
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        Swal.fire("Đã xóa!", "Nhật ký đã được xóa.", "success");
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
