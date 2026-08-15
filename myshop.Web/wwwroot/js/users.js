$(document).ready(function () {

    const table = $("#mytable").DataTable({
        ajax: {
            url: "/User/GetData",
            type: "GET",
            dataSrc: "data"
        },

        columns: [
            {
                data: "name"
            },

            {
                data: "email"
            },

            {
                data: "role",
                render: function (data) {

                    if (data === "Admin") {
                        return `
                            <span class="role-badge role-admin">
                                <i class="fa-solid fa-user-shield me-1"></i>
                                Admin
                            </span>
                        `;
                    }

                    return `
                        <span class="role-badge role-customer">
                            <i class="fa-solid fa-user me-1"></i>
                            Customer
                        </span>
                    `;
                }
            },

            {
                data: "isLockedOut",
                render: function (data) {

                    if (data) {
                        return `
                            <span class="lock-badge lock-locked">
                                <i class="fa-solid fa-lock me-1"></i>
                                Locked
                            </span>
                        `;
                    }

                    return `
                        <span class="lock-badge lock-active">
                            <i class="fa-solid fa-lock-open me-1"></i>
                            Active
                        </span>
                    `;
                }
            },

            {
                data: null,
                className: "text-end", // Right-aligns column header & content
                render: function (data, type, row) {

                    let buttons = `<div class="action-container">`;

                    // Promote / Demote
                    if (row.role === "Customer") {
                        buttons += `
                <a href="#"
                   class="btn btn-admin action-btn change-role"
                   data-id="${row.id}"
                   data-role="Admin"
                   data-name="${row.name}">
                    <i class="fa-solid fa-user-shield"></i>
                    Make Admin
                </a>
            `;
                    } else {
                        buttons += `
                <a href="#"
                   class="btn btn-customer action-btn change-role"
                   data-id="${row.id}"
                   data-role="Customer"
                   data-name="${row.name}">
                    <i class="fa-solid fa-user"></i>
                    Make Customer
                </a>
            `;
                    }

                    // Lock / Unlock
                    if (row.isLockedOut) {
                        buttons += `
                <a href="#"
                   class="btn btn-unlock action-btn unlock-user"
                   data-id="${row.id}"
                   data-name="${row.name}">
                    <i class="fa-solid fa-lock-open"></i>
                    Unlock
                </a>
            `;
                    } else {
                        buttons += `
                <a href="#"
                   class="btn btn-lock action-btn lock-user"
                   data-id="${row.id}"
                   data-name="${row.name}">
                    <i class="fa-solid fa-lock"></i>
                    Lock
                </a>
            `;
                    }

                    // Delete
                    buttons += `
            <a href="#"
               class="btn btn-delete action-btn delete-user"
               data-id="${row.id}"
               data-name="${row.name}">
                <i class="fa-solid fa-trash"></i>
            </a>
        `;

                    buttons += `</div>`;

                    return buttons;
                }
            }
        ],

        autoWidth: false,
        scrollX: true
    });


    // =========================
    // Change Role
    // =========================

    $(document).on("click", ".change-role", function (e) {

        e.preventDefault();

        const id = $(this).data("id");
        const role = $(this).data("role");
        const name = $(this).data("name");

        Swal.fire({
            title: "Change User Role?",
            html: `
                <div style="text-align: left;">
                    <p><strong>User:</strong> ${name}</p>
                    <p><strong>New Role:</strong> ${role}</p>
                </div>
            `,
            icon: "question",
            showCancelButton: true,
            confirmButtonColor: "#2563eb",
            cancelButtonColor: "#6b7280",
            confirmButtonText: "Yes, change role!",
            cancelButtonText: "Cancel"
        }).then((result) => {

            if (!result.isConfirmed) {
                return;
            }

            $.ajax({
                url: "/User/ChangeRole",
                type: "POST",
                data: {
                    userId: id,
                    role: role
                },

                success: function (response) {

                    if (response.success) {

                        Swal.fire({
                            title: "Updated!",
                            text: response.message,
                            icon: "success",
                            timer: 1500,
                            showConfirmButton: false
                        });

                        table.ajax.reload(null, false);

                        return;
                    }

                    Swal.fire(
                        "Error",
                        response.message,
                        "error"
                    );
                },

                error: function () {

                    Swal.fire(
                        "Error",
                        "Something went wrong while changing the user role.",
                        "error"
                    );
                }
            });
        });
    });


    // =========================
    // Lock User
    // =========================

    $(document).on("click", ".lock-user", function (e) {

        e.preventDefault();

        const id = $(this).data("id");
        const name = $(this).data("name");

        Swal.fire({
            title: "Lock Account?",
            html: `
                <p>Are you sure you want to lock:</p>
                <strong>${name}</strong>
            `,
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#dc2626",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Yes, lock it!",
            cancelButtonText: "Cancel"
        }).then((result) => {

            if (!result.isConfirmed) {
                return;
            }

            $.ajax({
                url: "/User/Lock",
                type: "POST",
                data: {
                    userId: id
                },

                success: function (response) {

                    if (response.success) {

                        Swal.fire({
                            title: "Locked!",
                            text: response.message,
                            icon: "success",
                            timer: 1500,
                            showConfirmButton: false
                        });

                        table.ajax.reload(null, false);

                        return;
                    }

                    Swal.fire(
                        "Error",
                        response.message,
                        "error"
                    );
                },

                error: function () {

                    Swal.fire(
                        "Error",
                        "Something went wrong while locking the user.",
                        "error"
                    );
                }
            });
        });
    });


    // =========================
    // Unlock User
    // =========================

    $(document).on("click", ".unlock-user", function (e) {

        e.preventDefault();

        const id = $(this).data("id");
        const name = $(this).data("name");

        Swal.fire({
            title: "Unlock Account?",
            html: `
                <p>Are you sure you want to unlock:</p>
                <strong>${name}</strong>
            `,
            icon: "question",
            showCancelButton: true,
            confirmButtonColor: "#16a34a",
            cancelButtonColor: "#6b7280",
            confirmButtonText: "Yes, unlock it!",
            cancelButtonText: "Cancel"
        }).then((result) => {

            if (!result.isConfirmed) {
                return;
            }

            $.ajax({
                url: "/User/Unlock",
                type: "POST",
                data: {
                    userId: id
                },

                success: function (response) {

                    if (response.success) {

                        Swal.fire({
                            title: "Unlocked!",
                            text: response.message,
                            icon: "success",
                            timer: 1500,
                            showConfirmButton: false
                        });

                        table.ajax.reload(null, false);

                        return;
                    }

                    Swal.fire(
                        "Error",
                        response.message,
                        "error"
                    );
                },

                error: function () {

                    Swal.fire(
                        "Error",
                        "Something went wrong while unlocking the user.",
                        "error"
                    );
                }
            });
        });
    });


    // =========================
    // Delete User
    // =========================

    $(document).on("click", ".delete-user", function (e) {

        e.preventDefault();

        const id = $(this).data("id");
        const name = $(this).data("name");

        Swal.fire({
            title: "Are you sure?",
            html: `
                <div style="text-align: left;">
                    <p><strong>User:</strong> ${name}</p>
                    <p>This user will be deleted permanently.</p>
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
                url: "/User/DeleteAjax",
                type: "DELETE",
                data: {
                    id: id
                },

                success: function (response) {

                    if (response.success) {

                        Swal.fire({
                            title: "Deleted!",
                            text: response.message,
                            icon: "success",
                            timer: 1500,
                            showConfirmButton: false
                        });

                        table.ajax.reload(null, false);

                        return;
                    }

                    Swal.fire(
                        "Error",
                        response.message,
                        "error"
                    );
                },

                error: function (xhr) {

                    let message =
                        "Something went wrong while deleting the user.";

                    if (xhr.responseJSON &&
                        xhr.responseJSON.message) {

                        message = xhr.responseJSON.message;
                    }

                    Swal.fire(
                        "Error",
                        message,
                        "error"
                    );
                }
            });
        });
    });

});