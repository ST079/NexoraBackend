using MediatR;
using Microsoft.Extensions.Options;
using NexoraBackend.Application.Commands.Payments;
using NexoraBackend.Config;
using Stripe;

namespace NexoraBackend.API.Endpoints;

/// <summary>
/// Stripe calls this endpoint server-to-server after a payment event.
/// The stripe-signature header is verified with the webhook secret before
/// processing — this guarantees the request actually came from Stripe.
/// </summary>
public static class StripeWebhookEndpoint
{
    public static void MapStripeWebhook(this WebApplication app)
    {
        app.MapPost("/webhooks/stripe", async (
            HttpRequest request,
            IMediator mediator,
            IOptions<StripeSettings> stripeOptions,
            ILogger<Program> logger) =>
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            var stripeSignature = request.Headers["Stripe-Signature"].ToString();

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json, stripeSignature, stripeOptions.Value.WebhookSecret);
            }
            catch (StripeException ex)
            {
                logger.LogWarning(ex, "Invalid Stripe webhook signature.");
                return Results.BadRequest("Invalid stripe-signature.");
            }

            // Only handle the payment_intent.succeeded event
            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var intent = stripeEvent.Data.Object as PaymentIntent;
                if (intent is null) return Results.Ok();

                var chargeId = intent.LatestChargeId;
                var result = await mediator.Send(
                    new ConfirmPaymentCommand(intent.Id, chargeId));

                if (!result.IsSuccess)
                    logger.LogError("Payment confirmation failed for intent {IntentId}: {Error}",
                        intent.Id, result.Error?.Message);
            }

            return Results.Ok();
        });
    }
}