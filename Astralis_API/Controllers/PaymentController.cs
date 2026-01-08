using Astralis.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession([FromBody] List<CartItemDto> cartItems)
        {
            try
            {
                // 1. Vérification de la clé Stripe
                var secretKey = _configuration["Stripe:SecretKey"];
                if (string.IsNullOrEmpty(secretKey))
                {
                    return StatusCode(500, "La clé Stripe (SecretKey) est introuvable dans la configuration serveur.");
                }
                StripeConfiguration.ApiKey = secretKey;

                // 2. Vérification du panier
                if (cartItems == null || !cartItems.Any())
                {
                    return BadRequest("Le panier est vide ou nul.");
                }

                // 3. Configuration de l'URL de base (Front-end)
                // Regardez l'URL dans votre navigateur quand vous êtes sur le site.
                var domain = "https://localhost:7036";

                var lineItems = new List<SessionLineItemOptions>();

                foreach (var item in cartItems)
                {
                    // Protection contre les images vides pour éviter que Stripe râle
                    var images = !string.IsNullOrEmpty(item.ProductPictureUrl)
                        ? new List<string> { item.ProductPictureUrl }
                        : new List<string>();

                    lineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmountDecimal = item.UnitPrice * 100, // Stripe est en centimes
                            Currency = "eur",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductLabel ?? "Produit sans nom", // Protection nom vide
                                Images = images
                            },
                        },
                        Quantity = item.Quantity,
                    });
                }

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    // IMPORTANT : {CHECKOUT_SESSION_ID} est remplacé automatiquement par Stripe
                    SuccessUrl = domain + "/payment-success?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = domain + "/panier",
                };

                var service = new SessionService();
                Session session = service.Create(options);

                return Ok(new { url = session.Url });
            }
            catch (StripeException e)
            {
                // Erreur venant directement de Stripe (ex: montant négatif)
                return BadRequest($"Erreur Stripe : {e.StripeError.Message}");
            }
            catch (Exception e)
            {
                // Erreur générale (ex: serveur mal configuré)
                return StatusCode(500, $"Erreur Serveur Interne : {e.Message} \n {e.StackTrace}");
            }
        }
    }
}