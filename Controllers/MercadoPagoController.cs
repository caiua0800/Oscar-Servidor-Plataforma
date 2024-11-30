using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MercadoPagoController : ControllerBase
    {
        private readonly string _accessToken;

        public MercadoPagoController(IConfiguration configuration)
        {
            _accessToken = configuration["ACCESS_TOKEN"];
        }

        [HttpPost("pix")]
        public async Task<IActionResult> CriarPix([FromBody] PixRequest request)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                    // Obter o horário UTC, adicionando 1 hora
                    var expirationDate = DateTime.UtcNow.AddHours(1);
                    var formattedExpirationDate = expirationDate.ToString("yyyy-MM-ddTHH:mm:ssZ"); // Formato correto

                    // Verificando a data formatada
                    Console.WriteLine($"Formatted expiration date: {formattedExpirationDate}"); // Para depuração

                    var body = new
                    {
                        transaction_amount = request.TransactionAmount,
                        description = request.Description,
                        payment_method_id = request.PaymentMethodId,
                        payer = new
                        {
                            email = request.PayerEmail,
                            identification = new
                            {
                                type = request.IdentificationType,
                                number = request.IdentificationNumber
                            }
                        },
                        date_of_expiration = formattedExpirationDate, // Usando a data formatada corretamente
                        notification_url = request.NotificationUrl
                    };

                    // Serializar o corpo em JSON
                    var json = JsonConvert.SerializeObject(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Enviar a requisição
                    var response = await client.PostAsync("https://api.mercadopago.com/v1/payments", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return Ok(JsonConvert.DeserializeObject(responseContent));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return BadRequest($"Erro ao fazer a requisição: {response.ReasonPhrase}, Detalhes: {errorContent}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Erro ao fazer a requisição: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }
    }

    public class PixRequest
    {
        public decimal TransactionAmount { get; set; }
        public string Description { get; set; }
        public string PaymentMethodId { get; set; }
        public string PayerEmail { get; set; }
        public string IdentificationType { get; set; }
        public string IdentificationNumber { get; set; }
        public string NotificationUrl { get; set; }
    }
}