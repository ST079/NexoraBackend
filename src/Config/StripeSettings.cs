namespace NexoraBackend.Config;
public class StripeSettings
{
    public const string SectionName = "Stripe";
    public string SecretKey { get; set; } = null!; //Server-side payment processing, Sensitive
    public string PublishableKey { get; set; } = null!;//Frontend payment usage, Safe to expose publicly
    public string WebhookSecret { get; set; } = null!; //Used to verify Stripe callbacks, Very important for payment security.
    public string Currency { get; set; } = "usd";
}