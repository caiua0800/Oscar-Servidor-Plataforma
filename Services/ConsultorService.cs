using MongoDB.Driver;
using DotnetBackend.Models;
using Microsoft.AspNetCore.Identity; // Para usar PasswordHasher
using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualBasic;


namespace DotnetBackend.Services
{
    public class ConsultorService
    {
        private readonly IMongoCollection<Consultor> _consultores;
        private readonly ClientService _clientService;

        public ConsultorService(MongoDbService mongoDbService, ClientService clientService)
        {
            _consultores = mongoDbService.GetCollection<Consultor>("Consultores");
            _clientService = clientService;
        }

        public async Task<Consultor> CreateConsultorAsync(Consultor consultor)
        {
            if (string.IsNullOrWhiteSpace(consultor.Id))
            {
                throw new ArgumentException("O Id deve ser fornecido.");
            }

            var existingConsultor = await _consultores.Find(c => c.Id == consultor.Id).FirstOrDefaultAsync();
            if (existingConsultor != null)
            {
                throw new InvalidOperationException("Já existe um consultor com este CPF.");
            }

            consultor.DateCreated = DateTime.UtcNow;
            consultor.Status = 1;
            await _consultores.InsertOneAsync(consultor);

            return consultor;
        }

        public async Task<List<Consultor>> GetAllConsultoresAsync()
        {
            var consultores = await _consultores.Find(_ => true).ToListAsync();

            foreach (var consultor in consultores)
            {
                var clients = await _clientService.GetAllClientsByConsultorIdAsync(consultor.Id);
                consultor.ClientsQtt = clients.Count;
            }

            return consultores;
        }

        public async Task<List<Consultor?>> GetAllConsultoresWithCreationDateFilterAsync(int dateFilter)
        {
            if (dateFilter < 0)
            {
                throw new ArgumentException("O filtro de data deve ser um número positivo representando a quantidade de dias.");
            }

            var cutoffDate = DateTime.UtcNow.AddDays(-dateFilter);

            // Obter todos os consultores
            var consultores = await _consultores.Find(_ => true).ToListAsync();

            // Atualizar a contagem de clientes para cada consultor
            foreach (var consultor in consultores)
            {
                var clients = await _clientService.GetAllClientsByConsultorIdAsync(consultor.Id);
                consultor.ClientsQtt = clients.Count;
            }

            // Filtrar consultores pela data de criação
            var filteredConsultores = consultores
                .Where(consultor => consultor.DateCreated >= cutoffDate)
                .ToList();

            return filteredConsultores;
        }

        public async Task<bool> DeleteConsultorAsync(string id)
        {
            var deleteResult = await _consultores.DeleteOneAsync(c => c.Id == id);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<Consultor?> GetConsultorByIdAsync(string id)
        {
            var normalizedId = id.Trim();
            return await _consultores.Find(c => c.Id == normalizedId).FirstOrDefaultAsync();
        }

        public async Task<Consultor?> GetConsultorByEmailAsync(string email)
        {
            var normalizedEmail = email.Trim();
            return await _consultores.Find(c => c.Email == normalizedEmail).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateConsultorAsync(string id, Consultor updatedConsultor)
        {
            if (updatedConsultor.Id != id)
            {
                throw new ArgumentException("O ID do consultor atualizado deve coincidir com o ID do cliente que está sendo atualizado.");
            }

            var currentConsultor = await _consultores.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (currentConsultor == null)
            {
                throw new Exception("Consultor não encontrado.");
            }

            var updateDefinition = Builders<Consultor>.Update;
            var updateFields = new List<UpdateDefinition<Consultor>>();

            if (updatedConsultor.Name != currentConsultor.Name)
            {
                updateFields.Add(updateDefinition.Set(c => c.Name, updatedConsultor.Name));
            }
            if (updatedConsultor.Email != currentConsultor.Email)
            {
                updateFields.Add(updateDefinition.Set(c => c.Email, updatedConsultor.Email));
            }
            if (updatedConsultor.Phone != currentConsultor.Phone)
            {
                updateFields.Add(updateDefinition.Set(c => c.Phone, updatedConsultor.Phone));
            }

            if (updatedConsultor.DateCreated != currentConsultor.DateCreated)
            {
                updateFields.Add(updateDefinition.Set(c => c.DateCreated, updatedConsultor.DateCreated));
            }

            if (updateFields.Count > 0)
            {
                var updateResult = await _consultores.UpdateOneAsync(c => c.Id == id, Builders<Consultor>.Update.Combine(updateFields));
                return updateResult.ModifiedCount > 0;
            }

            return false;
        }

        public async Task<bool> ExcludeAccount(string consultorId)
        {
            var consultor = await GetConsultorByIdAsync(consultorId);
            if (consultor == null)
            {
                throw new InvalidOperationException("Consultor não encontrado.");
            }


            consultor.Status = 2;

            var updateDefinition = Builders<Consultor>.Update.Set(c => c.Status, consultor.Status);

            var result = await _consultores.UpdateOneAsync(c => c.Id == consultorId, updateDefinition);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateConsultorName(string consultorId, string newValue)
        {
            if (consultorId == null)
            {
                throw new Exception("Id Nullo.");
            }

            var currentConsultor = await _consultores.Find(c => c.Id == consultorId).FirstOrDefaultAsync();
            if (currentConsultor == null)
            {
                throw new Exception("Consultor não encontrado.");
            }

            currentConsultor.Name = newValue;
            var updateDefinition = Builders<Consultor>.Update.Set(c => c.Name, newValue);

            var result = await _consultores.UpdateOneAsync(c => c.Id == consultorId, updateDefinition);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateConsultorEmail(string consultorId, string newValue)
        {
            if (string.IsNullOrEmpty(consultorId))
            {
                throw new ArgumentException("O ID do consultor não pode ser nulo ou vazio.");
            }

            var currentConsultor = await _consultores.Find(c => c.Id == consultorId).FirstOrDefaultAsync();
            if (currentConsultor == null)
            {
                throw new Exception("Consultor não encontrado.");
            }

            var responseIfExists = await _consultores.Find(c => c.Email == newValue).FirstOrDefaultAsync();

            if (responseIfExists != null)
            {
                throw new Exception("Email já existente.");
            }

            var updateDefinition = Builders<Consultor>.Update.Set(c => c.Email, newValue);
            var result = await _consultores.UpdateOneAsync(c => c.Id == consultorId, updateDefinition);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateConsultorPhone(string consultorId, string newValue)
        {
            if (string.IsNullOrEmpty(consultorId))
            {
                throw new ArgumentException("O ID do consultor não pode ser nulo ou vazio.");
            }

            var currentConsultor = await _consultores.Find(c => c.Id == consultorId).FirstOrDefaultAsync();
            if (currentConsultor == null)
            {
                throw new Exception("Consultor não encontrado.");
            }

            var updateDefinition = Builders<Consultor>.Update.Set(c => c.Phone, newValue);
            var result = await _consultores.UpdateOneAsync(c => c.Id == consultorId, updateDefinition);
            return result.ModifiedCount > 0;
        }
    }
}
