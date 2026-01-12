using Astralis.Shared.DTOs;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using System.Text.Json;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public PaymentController(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;

            // Initialisation de la clé Stripe
            var secretKey = _configuration["Stripe:SecretKey"];
            if (!string.IsNullOrEmpty(secretKey))
            {
                StripeConfiguration.ApiKey = secretKey;
            }
        }

        [HttpPost("create-checkout-session")]
        public async Task<ActionResult> CreateCheckoutSession([FromBody] List<CartItemDto> cartItems)
        {
            try
            {
                var domain = _configuration["Stripe:Domain"];
                if (string.IsNullOrEmpty(domain)) domain = "https://localhost:7036";

                var lineItems = new List<SessionLineItemOptions>();

                foreach (var item in cartItems)
                {
                    var productData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.ProductLabel,
                    };

                    if (!string.IsNullOrEmpty(item.ProductPictureUrl))
                    {
                        productData.Images = new List<string> { item.ProductPictureUrl };
                    }

                    var priceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.UnitPrice * 100),
                        Currency = "eur",
                        ProductData = productData,
                    };

                    lineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = priceData,
                        Quantity = item.Quantity,
                    });
                }

                var options = new SessionCreateOptions
                {
                    LineItems = lineItems,
                    Mode = "payment",

                    // --- C'est ici que j'ai adapté l'URL selon ta demande ---
                    SuccessUrl = domain + "/payment-success?session_id={CHECKOUT_SESSION_ID}",

                    // Si l'utilisateur annule, on le renvoie vers le panier
                    CancelUrl = domain + "/cart",

                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
            {
                { "UserId", GetCurrentUserId().ToString() },
                { "OrderType", "CartCheckout" }
            }
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                return Ok(new { url = session.Url });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erreur Checkout : {e.Message}");
                return StatusCode(500, new { error = e.Message });
            }
        }

        // ... Le reste de tes méthodes (Subscribe, Portal, etc.) reste inchangé ...
        // Je les inclus ci-dessous pour que tu aies le fichier complet si besoin

        [HttpPost("subscribe")]
        public async Task<ActionResult> CreateSubscriptionSession([FromBody] JsonElement body)
        {
            try
            {
                string planType = "monthly";
                if (body.TryGetProperty("planType", out JsonElement planProperty))
                {
                    planType = planProperty.GetString() ?? "monthly";
                }

                var domain = _configuration["Stripe:Domain"];
                if (string.IsNullOrEmpty(domain)) domain = "https://localhost:7064";

                string customerId = await GetOrCreateStripeCustomerAsync();

                string priceId = planType == "yearly"
                    ? _configuration["Stripe:YearlyPriceId"]
                    : _configuration["Stripe:MonthlyPriceId"];

                if (string.IsNullOrEmpty(priceId))
                    return StatusCode(500, "Erreur Config : L'ID du prix est vide dans appsettings.json");

                var options = new SessionCreateOptions
                {
                    Customer = customerId,
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions { Price = priceId, Quantity = 1 }
                    },
                    Mode = "subscription",
                    SuccessUrl = domain + "/premium?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = domain + "/premium?canceled=true",
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                return Ok(new { url = session.Url });
            }
            catch (StripeException se)
            {
                return StatusCode(400, $"Stripe Error: {se.StripeError.Message}");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("verify-premium")]
        public async Task<ActionResult> VerifyPremium(string session_id)
        {
            try
            {
                var service = new SessionService();
                Session session = await service.GetAsync(session_id);

                if (session.PaymentStatus == "paid")
                {
                    int userId = GetCurrentUserId();
                    await _userRepository.SetPremiumStatusAsync(userId, true);
                    return Ok(new { status = "Premium Activated" });
                }

                return BadRequest("Le paiement n'a pas été validé.");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("portal")]
        public async Task<ActionResult> CreatePortalSession()
        {
            var domain = _configuration["Stripe:Domain"] ?? "https://localhost:7064";
            int userId = GetCurrentUserId();
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || string.IsNullOrEmpty(user.StripeCustomerId))
                return BadRequest("Aucun historique client trouvé.");

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = user.StripeCustomerId,
                ReturnUrl = domain + "/premium",
            };

            var service = new Stripe.BillingPortal.SessionService();
            var session = await service.CreateAsync(options);

            return Ok(new { url = session.Url });
        }

        private async Task<string> GetOrCreateStripeCustomerAsync()
        {
            int userId = GetCurrentUserId();
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null) throw new Exception("User not found");

            if (!string.IsNullOrEmpty(user.StripeCustomerId))
                return user.StripeCustomerId;

            var options = new CustomerCreateOptions
            {
                Email = user.Email,
                Name = $"{user.FirstName} {user.LastName}",
                Metadata = new Dictionary<string, string> { { "UserId", userId.ToString() } }
            };

            var service = new CustomerService();
            Customer customer = await service.CreateAsync(options);

            await _userRepository.UpdateStripeIdAsync(userId, customer.Id);
            return customer.Id;
        }

        [HttpGet("sync-status")]
        public async Task<ActionResult> SyncSubscriptionStatus()
        {
            try
            {
                int userId = GetCurrentUserId();
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null || string.IsNullOrEmpty(user.StripeCustomerId))
                {
                    if (user != null && user.IsPremium) await _userRepository.SetPremiumStatusAsync(userId, false);
                    return Ok(new { isPremium = false });
                }

                var service = new SubscriptionService();
                var options = new SubscriptionListOptions
                {
                    Customer = user.StripeCustomerId,
                    Status = "all",
                    Limit = 1
                };

                var subscriptions = await service.ListAsync(options);
                var sub = subscriptions.Data.FirstOrDefault();

                bool hasActiveSubscription = false;

                if (sub != null && (sub.Status == "active" || sub.Status == "trialing"))
                {
                    if (sub.CancelAtPeriodEnd)
                    {
                        hasActiveSubscription = false;
                    }
                    else
                    {
                        hasActiveSubscription = true;
                    }
                }

                if (user.IsPremium != hasActiveSubscription)
                {
                    await _userRepository.SetPremiumStatusAsync(userId, hasActiveSubscription);
                }

                return Ok(new { isPremium = hasActiveSubscription });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim)) throw new Exception("Utilisateur non identifié");
            return int.Parse(idClaim);
        }
    }
}