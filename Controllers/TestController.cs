using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using MongoDB.Driver; // Certifique-se de que este namespace está incluído

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly MongoDbService _mongoService;

        public TestController(MongoDbService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet]
        [Route("clients")] // Adicionar rota específica para clientes
        public IActionResult GetClients()
        {
            // Acessar a coleção de clientes especificamente
            var collection = _mongoService.GetCollection<Client>("Clients"); // Certifique-se que o nome "Clients" está correto no MongoDB
            var clients = collection.Find(_ => true).ToList(); // Busca todos os documentos

            return Ok(clients); // Retorna os clientes como resposta
        }
    }
}
