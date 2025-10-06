// notification.js - Handles notification dropdown and mark as read functionality

$(document).ready(function () {
    // Handle mark as read button clicks
    $(document).on('click', '.mark-read-btn', function () {
        var notificationId = $(this).data('notification-id');
        var $notificationItem = $(this).closest('.notification-item');
        var $badge = $('.notification-badge');

        // Send AJAX request to mark as read
        $.ajax({
            type: 'POST',
            url: '/Customer/Notification/MarkAsRead',
            data: { id: notificationId },
            success: function (response) {
                if (response.success) {
                    // Remove the notification from the dropdown
                    $notificationItem.fadeOut(300, function () {
                        $(this).remove();

                        // Update badge count
                        var currentCount = parseInt($badge.text()) || 0;
                        var newCount = Math.max(0, currentCount - 1);

                        if (newCount > 0) {
                            $badge.text(newCount);
                        } else {
                            $badge.hide();
                            // Show "no notifications" message if list is empty
                            if ($('.notification-list .notification-item').length === 0) {
                                $('.notification-list').html('<div class="notification-item text-center"><p class="no-notification">Không có thông báo nào</p></div>');
                            }
                        }
                    });
                } else {
                    // Show error message
                    toastr.error('Không thể đánh dấu đã đọc');
                }
            },
            error: function () {
                toastr.error('Có lỗi xảy ra khi cập nhật thông báo');
            }
        });
    });

    // Close dropdown when clicking outside
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.notification-wrap').length) {
            $('.notification-dropdown').removeClass('show');
        }
    });

    // Refresh notifications every 30 seconds for real-time updates
    setInterval(function () {
        // Only update if the dropdown is not currently open to avoid interrupting user interaction
        if (!$('.notification-dropdown').hasClass('show')) {
            // Here you could make an AJAX call to update the notification list
            // For now, we'll just refresh the component
            location.reload(); // Simple refresh - can be optimized later
        }
    }, 30000); // 30 seconds
});
