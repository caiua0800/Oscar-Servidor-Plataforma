using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Services;
using DotnetBackend.Models;
using Microsoft.Extensions.Configuration; // Para usar IConfiguration

namespace DotnetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration; // Adicione esta linha

        public EmailController(EmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _configuration = configuration; // Inicializa a configuração
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] Client user)
        {
            await _emailService.SendEmailAsync(user.Email,
                "Cadastro realizado com sucesso",
                "Bem-vindo ao nosso serviço!",
                "<strong>Bem-vindo ao nosso serviço!</strong>");

            return Ok();
        }

        // Rota para verificar se a chave da API está configurada
        [HttpGet("check-api-key")]
        public IActionResult CheckApiKey()
        {
            var apiKey = _configuration["EMAIL_API_KEY"]; // Tenta acessar a chave API

            if (string.IsNullOrEmpty(apiKey))
            {
                return NotFound("Chave API do SendGrid não encontrada."); // Retorna 404 se não estiver configurada
            }

            return Ok("Chave API do SendGrid está configurada."); // Retorna 200 se estiver configurada
        }
    }
}