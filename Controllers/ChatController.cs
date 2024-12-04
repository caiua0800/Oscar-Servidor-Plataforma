using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using DotnetBackend.Models;
using DotnetBackend.Services;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("{clientId}/send")]
        public async Task<IActionResult> SendMessage(string clientId, [FromBody] SendMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Msg))
            {
                return BadRequest("A mensagem não pode estar vazia.");
            }

            // Enviar a mensagem e obter o chat atualizado
            var updatedChat = await _chatService.SendMessage(clientId, request.Msg, request.IsResponse);

            // Retorna o chat atualizado
            return Ok(updatedChat);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Chat>>> GetAllChats()
        {
            var chats = await _chatService.GetAllChats();
            return Ok(chats);
        }

        [HttpGet("{clientId}")]
        public async Task<ActionResult<Chat>> GetChatByClientId(string clientId)
        {
            var chat = await _chatService.GetChatByClientId(clientId);

            if (chat == null)
            {
                return NotFound($"Chat não encontrado para o cliente com ID: {clientId}");
            }

            return Ok(chat);
        }
    }

    public class SendMessageRequest
    {
        public string Msg { get; set; }
        public string ClientName { get; set; }
        public bool IsResponse { get; set; }
        public DateTime DateCreated { get; set; }
    }
}