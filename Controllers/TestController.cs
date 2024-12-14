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
        [Route("clients")]
        public IActionResult GetClients()
        {
            var collection = _mongoService.GetCollection<Client>("Clients"); 
            var clients = collection.Find(_ => true).ToList(); 

            return Ok(clients); 
        }
    }
}
