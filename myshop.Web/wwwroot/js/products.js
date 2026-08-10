$(document).ready(function () {

    const table = $("#mytable").DataTable({
        ajax: {
            url: "/Product/GetData",
            type: "GET",
            dataSrc: "data"
        },
        columns: [
            { data: "name" },
            { data: "description" },
            { data: "price" },
            { data: "categoryName" },
            {
                data: "id",
                render: function (data, type, row) {
                    return `
                        <a href="/Product/Edit/${row.id}" class="btn btn-success btn-sm">
                            <i class="fa-solid fa-pen"></i>
                        </a>

                        <a href="#" class="btn btn-danger btn-sm delete-product"
                           data-id="${row.id}"
                           data-name="${row.name || ""}"
                           data-price="${row.price || 0}"
                           data-category="${row.categoryName || ""}">
                            <i class="fa-solid fa-trash"></i>
                        </a>
                    `;
                }
            }
        ],
        autoWidth: false,
        scrollX: true
    });

    $(document).on("click", ".delete-product", function (e) {
        e.preventDefault();

        const id = $(this).data("id");
        const productName = $(this).data("name") || "this product";
        const productPrice = Number($(this).data("price") || 0);
        const productCategory = $(this).data("category") || "Uncategorized";

        Swal.fire({
            title: "Are you sure?",
            html: `
                <div style="text-align: left;">
                    <p><strong>Product:</strong> ${productName}</p>
                    <p><strong>Price:</strong> $${productPrice.toFixed(2)}</p>
                    <p><strong>Category:</strong> ${productCategory}</p>
                    <p>This product will be deleted permanently.</p>
                </div>
            `,
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
                url: "/Product/DeleteAjax",
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

                        table.ajax.reload();
                        return;
                    }

                    Swal.fire("Error", response.message, "error");
                },
                error: function (xhr) {
                    let message = "Something went wrong while deleting the product.";

                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        message = xhr.responseJSON.message;
                    }

                    Swal.fire("Error", message, "error");
                }
            });
        });
    });

});