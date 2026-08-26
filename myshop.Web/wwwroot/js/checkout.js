document.addEventListener("DOMContentLoaded", function () {

    // =========================================================
    // STRIPE CONFIGURATION
    // =========================================================

    const stripePublishableKey =
        document.getElementById("stripe-config")?.dataset.publishableKey;

    if (!stripePublishableKey) {
        console.error("Stripe publishable key is missing.");
        return;
    }

    const stripe = Stripe(stripePublishableKey);

    let elements = null;
    let paymentElement = null;
    let paymentIntentId = null;


    // =========================================================
    // DOM ELEMENTS
    // =========================================================

    const paymentForm =
        document.getElementById("payment-form");

    const submitButton =
        document.getElementById("submit-payment");

    const buttonText =
        document.getElementById("button-text");

    const buttonSpinner =
        document.getElementById("button-spinner");

    const paymentErrors =
        document.getElementById("payment-errors");

    const paymentLoading =
        document.getElementById("payment-loading");


    // Make sure the required elements exist.
    if (!paymentForm ||
        !submitButton ||
        !buttonText ||
        !buttonSpinner ||
        !paymentErrors ||
        !paymentLoading) {

        console.error(
            "Required checkout elements were not found."
        );

        return;
    }


    // =========================================================
    // SHOW PAYMENT ERROR
    // =========================================================

    function showPaymentError(message) {

        paymentErrors.textContent = message;

        paymentErrors.classList.add("show");
    }


    // =========================================================
    // CLEAR PAYMENT ERROR
    // =========================================================

    function clearPaymentError() {

        paymentErrors.textContent = "";

        paymentErrors.classList.remove("show");
    }


    // =========================================================
    // BUTTON LOADING STATE
    // =========================================================

    function setButtonLoading(isLoading) {

        if (isLoading) {

            submitButton.disabled = true;

            buttonText.classList.add("d-none");

            buttonSpinner.classList.remove("d-none");

        } else {

            submitButton.disabled = false;

            buttonText.classList.remove("d-none");

            buttonSpinner.classList.add("d-none");
        }
    }


    // =========================================================
    // GET ANTI-FORGERY TOKEN
    // =========================================================

    function getAntiForgeryToken() {

        const token =
            document.querySelector(
                'input[name="__RequestVerificationToken"]'
            );

        return token ? token.value : "";
    }


    // =========================================================
    // INITIALIZE STRIPE PAYMENT
    // =========================================================

    async function initializePayment() {

        try {

            const token =
                getAntiForgeryToken();

            if (!token) {

                throw new Error(
                    "Security token is missing."
                );
            }


            const response =
                await fetch(
                    "/Order/CreatePaymentIntent",
                    {
                        method: "POST",

                        headers: {
                            "RequestVerificationToken": token
                        }
                    }
                );


            const data =
                await response.json();


            if (!response.ok) {

                throw new Error(
                    data.message ||
                    "Unable to initialize payment."
                );
            }


            // Save PaymentIntent ID.
            paymentIntentId =
                data.paymentIntentId;


            // Create Stripe Elements.
            elements =
                stripe.elements({
                    clientSecret: data.clientSecret
                });


            // Create Payment Element.
            paymentElement =
                elements.create("payment");


            // Mount Payment Element.
            paymentElement.mount(
                "#payment-element"
            );


            // Payment is ready.
            paymentLoading.hidden = true;

            submitButton.disabled = false;

        }
        catch (error) {

            console.error(
                "Stripe initialization error:",
                error
            );

            paymentLoading.hidden = true;

            showPaymentError(
                error.message ||
                "Unable to initialize payment."
            );

            submitButton.disabled = true;
        }
    }


    // =========================================================
    // FINALIZE ORDER
    // =========================================================

    async function finalizeOrder() {

        const token =
            getAntiForgeryToken();


        const payload = {

            paymentIntentId:
                paymentIntentId,

            name:
                document.getElementById("Name").value,

            address:
                document.getElementById("Address").value,

            city:
                document.getElementById("City").value,

            postalCode:
                document.getElementById("PostalCode").value,

            phoneNumber:
                document.getElementById("PhoneNumber").value
        };


        const response =
            await fetch(
                "/Order/FinalizePayment",
                {
                    method: "POST",

                    headers: {

                        "Content-Type":
                            "application/json",

                        "RequestVerificationToken":
                            token
                    },

                    body:
                        JSON.stringify(payload)
                }
            );


        const data =
            await response.json();


        if (!response.ok) {

            throw new Error(
                data.message ||
                "Payment succeeded but order finalization failed."
            );
        }


        // Order successfully created.
        window.location.href =
            "/Order/OrderConfirmation?id="
            + data.orderId;
    }


    // =========================================================
    // SUBMIT PAYMENT
    // =========================================================

    paymentForm.addEventListener(
        "submit",
        async function (event) {

            event.preventDefault();

            clearPaymentError();


            // ---------------------------------------------
            // Validate delivery information
            // ---------------------------------------------

            if (!paymentForm.checkValidity()) {

                paymentForm.reportValidity();

                return;
            }


            // ---------------------------------------------
            // Make sure Stripe is initialized
            // ---------------------------------------------

            if (!elements) {

                showPaymentError(
                    "Payment form is not ready yet. Please try again."
                );

                return;
            }


            if (!paymentIntentId) {

                showPaymentError(
                    "Payment could not be initialized. Please refresh the page."
                );

                return;
            }


            setButtonLoading(true);


            try {

                // -----------------------------------------
                // Confirm payment with Stripe
                // -----------------------------------------

                const result =
                    await stripe.confirmPayment({

                        elements: elements,

                        confirmParams: {

                            return_url:
                                window.location.origin +
                                "/Order/PaymentComplete"

                        },

                        redirect: "if_required"
                    });


                // -----------------------------------------
                // Stripe payment error
                // -----------------------------------------

                if (result.error) {

                    throw new Error(
                        result.error.message ||
                        "Payment failed."
                    );
                }


                // -----------------------------------------
                // Payment succeeded
                // -----------------------------------------

                await finalizeOrder();

            }
            catch (error) {

                console.error(
                    "Payment error:",
                    error
                );

                showPaymentError(
                    error.message ||
                    "Payment could not be completed."
                );

                setButtonLoading(false);
            }

        }
    );


    // =========================================================
    // INITIALIZE
    // =========================================================

    initializePayment();

});