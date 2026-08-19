$(document).ready(function () {
    $(document).on("click", ".delete-category", function (e) {
        e.preventDefault();

        const id = $(this).data("id");
        const name = $(this).data("name") || "this category";

        Swal.fire({
            title: "Are you sure?",
            html: `<p>You are about to delete <strong>${name}</strong>.</p><p>This action cannot be undone.</p>`,
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Yes, delete it!",
            cancelButtonText: "Cancel"
        }).then((result) => {
            if (!result.isConfirmed) {
                return;
            }

            $.ajax({
                url: "/Admin/Category/DeleteAjax",
                type: "DELETE",
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: "Deleted!",
                            text: response.message,
                            icon: "success",
                            timer: 1500,
                            showConfirmButton: false
                        });

                        $(`.delete-category[data-id='${id}']`).closest("tr").remove();
                        return;
                    }

                    Swal.fire("Error", response.message, "error");
                },
                error: function (xhr) {
                    let message = "Something went wrong while deleting the category.";

                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        message = xhr.responseJSON.message;
                    }

                    Swal.fire("Error", message, "error");
                }
            });
        });
    });
});