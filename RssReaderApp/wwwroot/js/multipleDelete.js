$(document).ready(function () {
    $("#selectAll").click(function () {
        $(".feedCheckbox").prop('checked', $(this).prop('checked'));
    });

    $("#deleteSelected").click(function () {
        var selectedIds = [];
        $(".feedCheckbox:checked").each(function () {
            selectedIds.push($(this).val());
        });

        if (selectedIds.length > 0) {
            if (confirm('Are you sure you want to delete ' + selectedIds.length + ' selected feed(s)?')) {
                $.ajax({
                    url: '/Feed/DeleteMultiple',
                    type: 'POST',
                    data: { ids: selectedIds },
                    success: function (result) {
                        location.reload();
                    },
                    error: function () {
                        alert('An error occurred while deleting feeds.');
                    }
                });
            }
        } else {
            alert('Please select at least one feed to delete.');
        }
    });
});
