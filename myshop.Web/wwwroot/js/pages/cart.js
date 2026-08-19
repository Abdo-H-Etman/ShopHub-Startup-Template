$(document).ready(function () {

    // Helper: update empty-state UI if no rows remaining
    function checkCartEmpty(isEmpty) {
        const rowCount = $('#cartTableBody tr:visible').length;
        if (isEmpty || rowCount === 0) {
            $('#cartContentWrapper').addClass('d-none');
            $('#clearCartForm').addClass('d-none');
            $('#emptyCartMessage').removeClass('d-none');
        }
    }

    // Increase Quantity (AJAX - no reload)
    $(document).on('click', '.btn-increase', function (e) {
        e.preventDefault();

        const btn = $(this);
        const form = btn.closest('form');
        const productId = btn.data('product-id');
        const token = form.find('input[name="__RequestVerificationToken"]').val();

        btn.prop('disabled', true);

        $.ajax({
            url: '/Cart/Increase',
            type: 'POST',
            data: {
                productId: productId,
                __RequestVerificationToken: token
            },
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function (response) {
                if (response && response.success) {
                    $(`#quantity-${productId}`).text(response.quantity);
                    $(`#total-${productId}`).text(response.itemTotal);
                    $('#cartOrderTotal').text(response.cartTotal);
                }
            },
            error: function (xhr) {
                console.error('Failed to increase quantity:', xhr);
            },
            complete: function () {
                btn.prop('disabled', false);
            }
        });
    });

    // Decrease Quantity (AJAX - no reload)
    $(document).on('click', '.btn-decrease', function (e) {
        e.preventDefault();

        const btn = $(this);
        const form = btn.closest('form');
        const productId = btn.data('product-id');
        const token = form.find('input[name="__RequestVerificationToken"]').val();

        btn.prop('disabled', true);

        $.ajax({
            url: '/Cart/Decrease',
            type: 'POST',
            data: {
                productId: productId,
                __RequestVerificationToken: token
            },
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function (response) {
                if (response && response.success) {
                    if (response.isRemoved) {
                        $(`#cart-row-${productId}`).fadeOut(300, function () {
                            $(this).remove();
                            checkCartEmpty(response.isEmpty);
                        });
                    } else {
                        $(`#quantity-${productId}`).text(response.quantity);
                        $(`#total-${productId}`).text(response.itemTotal);
                    }
                    $('#cartOrderTotal').text(response.cartTotal);
                }
            },
            error: function (xhr) {
                console.error('Failed to decrease quantity:', xhr);
            },
            complete: function () {
                btn.prop('disabled', false);
            }
        });
    });

    // Remove specific item (AJAX with SweetAlert confirmation)
    $(document).on('click', '.btn-remove-item', function (e) {
        e.preventDefault();

        const btn = $(this);
        const form = btn.closest('form');
        const productId = btn.data('product-id');
        const productName = btn.data('product-name') || 'this item';
        const token = form.find('input[name="__RequestVerificationToken"]').val();

        Swal.fire({
            title: 'Remove Item?',
            text: `Are you sure you want to remove "${productName}" from your cart?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, remove it!',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                btn.prop('disabled', true);

                $.ajax({
                    url: '/Cart/Remove',
                    type: 'POST',
                    data: {
                        productId: productId,
                        __RequestVerificationToken: token
                    },
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    success: function (response) {
                        if (response && response.success) {
                            $(`#cart-row-${productId}`).fadeOut(300, function () {
                                $(this).remove();
                                checkCartEmpty(response.isEmpty);
                            });
                            $('#cartOrderTotal').text(response.cartTotal);

                            Swal.fire({
                                toast: true,
                                position: 'top-end',
                                icon: 'success',
                                title: `"${productName}" removed`,
                                showConfirmButton: false,
                                timer: 2000,
                                timerProgressBar: true
                            });
                        }
                    },
                    error: function (xhr) {
                        console.error('Failed to remove item:', xhr);
                    },
                    complete: function () {
                        btn.prop('disabled', false);
                    }
                });
            }
        });
    });

    // Confirmation for clearing the entire cart
    $('#btnClearCart').on('click', function (e) {
        e.preventDefault();

        Swal.fire({
            title: 'Clear Shopping Cart?',
            text: 'Are you sure you want to remove all items from your cart?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, clear it!',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                $('#clearCartForm').submit();
            }
        });
    });

});