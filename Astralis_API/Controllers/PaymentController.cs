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

            var secretKey = _configuration["Stripe:SecretKey"];
            if (!string.IsNullOrEmpty(secretKey))
            {
                StripeConfiguration.ApiKey = secretKey;
            }
        }

        [HttpPost("create-checkout-session")]
        public async Task<ActionResult> CreateCheckoutSession([FromBody] List<CartItemDto> cartItems)
        {
            return BadRequest("Code panier existant à conserver");
        }

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
                Console.WriteLine($"ERREUR STRIPE : {se.StripeError.Message}");
                return StatusCode(400, $"Stripe Error: {se.StripeError.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERREUR INTERNE : {e.Message}");
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
            var domain = _configuration["Stripe:Domain"] ?? "https://localhost:7036";
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
                        Console.WriteLine($"[STRICT] User {userId} a annulé (fin de période). Accès coupé immédiatement.");
                    }
                    else
                    {
                        hasActiveSubscription = true;
                    }
                }

                if (user.IsPremium != hasActiveSubscription)
                {
                    await _userRepository.SetPremiumStatusAsync(userId, hasActiveSubscription);
                    Console.WriteLine($"[SYNC] Statut mis à jour pour User {userId} : {hasActiveSubscription}");
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