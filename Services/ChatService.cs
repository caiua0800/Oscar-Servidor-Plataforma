using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotnetBackend.Models;

namespace DotnetBackend.Services
{
    public class ChatService
    {
        private readonly IMongoCollection<Chat> _chats;
        private readonly ClientService _clientService; // Adicionado ClientService


        public ChatService(MongoDbService mongoDbService, ClientService clientService)
        {
            _chats = mongoDbService.GetCollection<Chat>("Chats");
            _clientService = clientService; // Inicializa ClientService
        }

        private async Task<Chat> CreateChat(string clientId)
        {
            var newChat = new Chat
            {
                ClientId = clientId,
                DateCreated = DateTime.UtcNow,
                Messages = new List<Message>()
            };

            await _chats.InsertOneAsync(newChat);
            return newChat;
        }

        public async Task<Chat> SendMessage(string clientId, string msg, bool isResponse)
        {
            var chat = await _chats.Find(c => c.ClientId == clientId).FirstOrDefaultAsync();

            if (chat == null)
            {
                chat = await CreateChat(clientId);
            }

            var client = await _clientService.GetClientByIdAsync(clientId);
            var clientName = client?.Name ?? "Desconhecido"; // Fallback caso o cliente não seja encontrado

            var message = new Message
            {
                DateCreated = DateTime.UtcNow,
                ClientName = clientName,
                Msg = msg,
                IsResponse = isResponse
            };

            chat.Messages.Add(message);

            var updateDefinition = Builders<Chat>.Update.Set(c => c.Messages, chat.Messages);
            await _chats.UpdateOneAsync(c => c.ClientId == clientId, updateDefinition);

            return chat;
        }

        public async Task<List<Chat>> GetAllChats()
        {
            return await _chats.Find(_ => true).ToListAsync();
        }

        public async Task<Chat> GetChatByClientId(string clientId)
        {
            return await _chats.Find(c => c.ClientId == clientId).FirstOrDefaultAsync();
        }

    }
}