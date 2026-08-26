$(document).ready(function () {

    // 1. Active Products DataTable
    const table = $("#mytable").DataTable({
        ajax: {
            url: "/Admin/Product/GetData",
            type: "GET",
            dataSrc: "data"
        },
        columns: [
            { data: "name" },
            { data: "description" },
            { 
                data: "price",
                render: function (data) {
                    return "$" + Number(data).toFixed(2);
                }
            },
            { data: "categoryName" },
            {
                data: "id",
                render: function (data, type, row) {
                    return `
                        <a href="/Admin/Product/Edit/${row.id}" class="btn btn-success btn-sm" title="Edit">
                            <i class="fa-solid fa-pen"></i>
                        </a>

                        <a href="#" class="btn btn-danger btn-sm delete-product"
                           data-id="${row.id}"
                           data-name="${row.name || ""}"
                           data-price="${row.price || 0}"
                           data-category="${row.categoryName || ""}"
                           title="Archive (Soft Delete)">
                            <i class="fa-solid fa-box-archive"></i>
                        </a>
                    `;
                }
            }
        ],
        autoWidth: false,
        scrollX: true
    });

    // 2. Archived Products DataTable
    let archivedTable = null;

    // Bootstrap 4 (AdminLTE) fires tab events via jQuery, so .on('shown.bs.tab') works here.
    $('#archived-tab').on('shown.bs.tab', function () {
        if (!archivedTable) {
            archivedTable = $("#archivedTable").DataTable({
                ajax: {
                    url: "/Admin/Product/GetArchivedData",
                    type: "GET",
                    dataSrc: "data"
                },
                columns: [
                    { data: "name" },
                    { data: "description" },
                    { 
                        data: "price",
                        render: function (data) {
                            return "$" + Number(data).toFixed(2);
                        }
                    },
                    { data: "categoryName" },
                    {
                        data: "id",
                        render: function (data, type, row) {
                            return `
                                <button class="btn btn-primary btn-sm restore-product"
                                   data-id="${row.id}"
                                   data-name="${row.name || ""}"
                                   title="Restore Product">
                                    <i class="fa-solid fa-rotate-left me-1"></i> Restore
                                </button>
                            `;
                        }
                    }
                ],
                autoWidth: false,
                scrollX: true
            });
        } else {
            archivedTable.ajax.reload();
        }
    });

    // Handle Soft Delete / Archive
    $(document).on("click", ".delete-product", function (e) {
        e.preventDefault();

        const id = $(this).data("id");
        const productName = $(this).data("name") || "this product";
        const productPrice = Number($(this).data("price") || 0);
        const productCategory = $(this).data("category") || "Uncategorized";

        Swal.fire({
            title: "Archive Product?",
            html: `
                <div style="text-align: left;">
                    <p><strong>Product:</strong> ${productName}</p>
                    <p><strong>Price:</strong> $${productPrice.toFixed(2)}</p>
                    <p><strong>Category:</strong> ${productCategory}</p>
                    <p class="text-muted">This product will be archived and hidden from the storefront. You can restore it anytime from the Archived tab.</p>
                </div>
            `,
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Yes, archive it!",
            cancelButtonText: "Cancel"
        }).then((result) => {
            if (!result.isConfirmed) {
                return;
            }

            $.ajax({
                url: "/Admin/Product/DeleteAjax",
                type: "DELETE",
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: "Archived!",
                            text: response.message,
                            icon: "success",
                            timer: 1500,
                            showConfirmButton: false
                        });

                        table.ajax.reload();
                        if (archivedTable) {
                            archivedTable.ajax.reload();
                        }
                        return;
                    }

                    Swal.fire("Error", response.message, "error");
                },
                error: function (xhr) {
                    let message = "Something went wrong while archiving the product.";
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        message = xhr.responseJSON.message;
                    }
                    Swal.fire("Error", message, "error");
                }
            });
        });
    });

    // Handle Restore Archived Product
    $(document).on("click", ".restore-product", function (e) {
        e.preventDefault();

        const id = $(this).data("id");
        const productName = $(this).data("name") || "this product";

        Swal.fire({
            title: "Restore Product?",
            text: `Are you sure you want to restore "${productName}" back to the store?`,
            icon: "question",
            showCancelButton: true,
            confirmButtonColor: "#2563eb",
            cancelButtonColor: "#6c757d",
            confirmButtonText: "Yes, restore it!",
            cancelButtonText: "Cancel"
        }).then((result) => {
            if (!result.isConfirmed) {
                return;
            }

            $.ajax({
                url: "/Admin/Product/RestoreAjax",
                type: "POST",
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: "Restored!",
                            text: response.message,
                            icon: "success",
                            timer: 1500,
                            showConfirmButton: false
                        });

                        table.ajax.reload();
                        if (archivedTable) {
                            archivedTable.ajax.reload();
                        }
                        return;
                    }

                    Swal.fire("Error", response.message, "error");
                },
                error: function (xhr) {
                    let message = "Something went wrong while restoring the product.";
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        message = xhr.responseJSON.message;
                    }
                    Swal.fire("Error", message, "error");
                }
            });
        });
    });

});