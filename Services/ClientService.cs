using MongoDB.Driver;
using DotnetBackend.Models;
using Microsoft.AspNetCore.Identity; // Para usar PasswordHasher
using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.S3;
using Amazon.S3.Transfer;


namespace DotnetBackend.Services
{
    public class ClientService
    {
        private readonly IMongoCollection<Client> _clients;

        public ClientService(MongoDbService mongoDbService)
        {
            _clients = mongoDbService.GetCollection<Client>("Clients");
        }

        public async Task<Client> CreateClientAsync(Client client, string password)
        {
            Console.WriteLine("criando cliente.");
            if (string.IsNullOrWhiteSpace(client.Id))
            {
                throw new ArgumentException("O CPF (Id) deve ser fornecido.");
            }

            var existingClient = await _clients.Find(c => c.Id == client.Id).FirstOrDefaultAsync();
            if (existingClient != null)
            {
                throw new InvalidOperationException("Já existe um cliente com este CPF.");
            }

            var passwordHasher = new PasswordHasher<Client>();
            client.Password = passwordHasher.HashPassword(client, password);
            client.ClientProfit = 1.5;
            client.DateCreated = DateTime.UtcNow;
            client.Status = 1;
            Console.WriteLine($"Data de Criação antes da inserção: {client.DateCreated}");
            await _clients.InsertOneAsync(client);
            return client;
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            return await _clients.Find(_ => true).ToListAsync();
        }

        public async Task<bool> DeleteClientAsync(string id)
        {
            var deleteResult = await _clients.DeleteOneAsync(c => c.Id == id);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<Client?> GetClientByIdAsync(string id)
        {
            var normalizedId = id.Trim();
            return await _clients.Find(c => c.Id == normalizedId).FirstOrDefaultAsync();
        }

        public async Task<string> UploadProfilePictureAsync(IFormFile file)
        {
            var bucketName = "oscar-plataforma";
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            try
            {
                using (var client = new AmazonS3Client(Amazon.RegionEndpoint.SAEast1)) // Escolha a região apropriada
                {
                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(file.OpenReadStream(), bucketName, fileName);
                }

                return $"https://{bucketName}.s3.amazonaws.com/{fileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar a imagem para o S3: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> AddToBalanceAsync(string clientId, decimal amount)
        {
            var client = await GetClientByIdAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException("Cliente não encontrado.");
            }

            client.AddToBalance(amount);
            var updateDefinition = Builders<Client>.Update.Set(c => c.Balance, client.Balance);
            var result = await _clients.UpdateOneAsync(c => c.Id == clientId, updateDefinition);
            Console.WriteLine("Valor adicionado ao Balance do cliente");
            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddToExtraBalanceAsync(string clientId, decimal amount)
        {
            var client = await GetClientByIdAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException("Cliente não encontrado.");
            }

            client.AddToExtraBalance(amount);
            var updateDefinition = Builders<Client>.Update.Set(c => c.ExtraBalance, client.ExtraBalance);
            var result = await _clients.UpdateOneAsync(c => c.Id == clientId, updateDefinition);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddToBlockedBalanceAsync(string clientId, decimal amount)
        {
            var client = await GetClientByIdAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException("Cliente não encontrado.");
            }

            client.AddToBlockedBalance(amount);
            Console.WriteLine($"Chamando função no Service para adicionar valor ao BlockedBalance");
            var updateDefinition = Builders<Client>.Update.Set(c => c.BlockedBalance, client.BlockedBalance);
            var result = await _clients.UpdateOneAsync(c => c.Id == clientId, updateDefinition);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> WithdrawFromBalanceAsync(string clientId, decimal amount)
        {
            var client = await GetClientByIdAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException("Cliente não encontrado.");
            }

            client.WithdrawFromBalance(amount);

            var updateDefinition = Builders<Client>.Update.Set(c => c.Balance, client.Balance);
            var result = await _clients.UpdateOneAsync(c => c.Id == clientId, updateDefinition);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> WithdrawFromExtraBalanceAsync(string clientId, decimal amount)
        {
            var client = await GetClientByIdAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException("Cliente não encontrado.");
            }

            client.WithdrawFromExtraBalance(amount);

            var updateDefinition = Builders<Client>.Update.Set(c => c.ExtraBalance, client.ExtraBalance);
            var result = await _clients.UpdateOneAsync(c => c.Id == clientId, updateDefinition);
            return result.ModifiedCount > 0;
        }


        public async Task<bool> WithdrawFromBlockedBalanceAsync(string clientId, decimal amount)
        {
            var client = await GetClientByIdAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException("Cliente não encontrado.");
            }

            client.WithdrawFromBlockedBalance(amount);

            var updateDefinition = Builders<Client>.Update.Set(c => c.BlockedBalance, client.BlockedBalance);
            var result = await _clients.UpdateOneAsync(c => c.Id == clientId, updateDefinition);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateClientAsync(string id, Client updatedClient)
        {
            if (updatedClient.Id != id)
            {
                throw new ArgumentException("O ID do cliente atualizado deve coincidir com o ID do cliente que está sendo atualizado.");
            }

            var currentClient = await _clients.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (currentClient == null)
            {
                throw new Exception("Cliente não encontrado.");
            }

            var updateDefinition = Builders<Client>.Update;
            var updateFields = new List<UpdateDefinition<Client>>();

            if (updatedClient.Name != currentClient.Name)
            {
                updateFields.Add(updateDefinition.Set(c => c.Name, updatedClient.Name));
            }
            if (updatedClient.Email != currentClient.Email)
            {
                updateFields.Add(updateDefinition.Set(c => c.Email, updatedClient.Email));
            }
            if (updatedClient.Phone != currentClient.Phone)
            {
                updateFields.Add(updateDefinition.Set(c => c.Phone, updatedClient.Phone));
            }
            if (updatedClient.SponsorId != currentClient.SponsorId)
            {
                updateFields.Add(updateDefinition.Set(c => c.SponsorId, updatedClient.SponsorId));
            }
            if (updatedClient.ClientProfit != currentClient.ClientProfit)
            {
                updateFields.Add(updateDefinition.Set(c => c.ClientProfit, updatedClient.ClientProfit));
            }

            if (updatedClient.Address != null)
            {
                if (updatedClient.Address.Street != currentClient.Address.Street)
                {
                    updateFields.Add(updateDefinition.Set(c => c.Address.Street, updatedClient.Address.Street));
                }
                if (updatedClient.Address.Number != currentClient.Address.Number)
                {
                    updateFields.Add(updateDefinition.Set(c => c.Address.Number, updatedClient.Address.Number));
                }
                if (updatedClient.Address.Zipcode != currentClient.Address.Zipcode)
                {
                    updateFields.Add(updateDefinition.Set(c => c.Address.Zipcode, updatedClient.Address.Zipcode));
                }
                if (updatedClient.Address.Neighborhood != currentClient.Address.Neighborhood)
                {
                    updateFields.Add(updateDefinition.Set(c => c.Address.Neighborhood, updatedClient.Address.Neighborhood));
                }
                if (updatedClient.Address.City != currentClient.Address.City)
                {
                    updateFields.Add(updateDefinition.Set(c => c.Address.City, updatedClient.Address.City));
                }
                if (updatedClient.Address.State != currentClient.Address.State)
                {
                    updateFields.Add(updateDefinition.Set(c => c.Address.State, updatedClient.Address.State));
                }
                if (updatedClient.Address.Country != currentClient.Address.Country)
                {
                    updateFields.Add(updateDefinition.Set(c => c.Address.Country, updatedClient.Address.Country));
                }
            }

            if (updatedClient.PlatformId != currentClient.PlatformId)
            {
                updateFields.Add(updateDefinition.Set(c => c.PlatformId, updatedClient.PlatformId));
            }
            if (updatedClient.Balance != currentClient.Balance)
            {
                updateFields.Add(updateDefinition.Set(c => c.Balance, updatedClient.Balance));
            }
            if (updatedClient.DateCreated != currentClient.DateCreated)
            {
                updateFields.Add(updateDefinition.Set(c => c.DateCreated, updatedClient.DateCreated));
            }
            if (updatedClient.Password != currentClient.Password)
            {
                updateFields.Add(updateDefinition.Set(c => c.Password, updatedClient.Password));
            }

            if (updateFields.Count > 0)
            {
                var updateResult = await _clients.UpdateOneAsync(c => c.Id == id, Builders<Client>.Update.Combine(updateFields));
                return updateResult.ModifiedCount > 0;
            }

            return false;
        }

        public async Task<bool> AddPurchaseAsync(string clientId, string purchaseId)
        {
            var client = await GetClientByIdAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException("Cliente não encontrado.");
            }

            if (client.WalletExtract == null)
            {
                client.WalletExtract = new WalletExtract();
            }

            client.WalletExtract.Purchases.Add(purchaseId);

            var updateDefinition = Builders<Client>.Update.Set(c => c.WalletExtract.Purchases, client.WalletExtract.Purchases);

            var result = await _clients.UpdateOneAsync(c => c.Id == clientId, updateDefinition);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddWithdrawalAsync(string clientId, string withdrawalId)
        {
            var client = await GetClientByIdAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException("Cliente não encontrado.");
            }

            if (client.WalletExtract == null)
            {
                client.WalletExtract = new WalletExtract();
            }

            client.WalletExtract.Withdrawals.Add(withdrawalId);

            var updateDefinition = Builders<Client>.Update.Set(c => c.WalletExtract.Withdrawals, client.WalletExtract.Withdrawals);

            var result = await _clients.UpdateOneAsync(c => c.Id == clientId, updateDefinition);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ExcludeAccount(string clientId)
        {
            var client = await GetClientByIdAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException("Cliente não encontrado.");
            }


            client.Status = 2;

            var updateDefinition = Builders<Client>.Update.Set(c => c.Status, client.Status);

            var result = await _clients.UpdateOneAsync(c => c.Id == clientId, updateDefinition);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemovePurchaseAsync(string clientId, string purchaseId)
        {
            var client = await GetClientByIdAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException("Cliente não encontrado.");
            }

            if (client.WalletExtract.Purchases.Contains(purchaseId))
            {
                client.WalletExtract.Purchases.Remove(purchaseId);
            }

            var updateDefinition = Builders<Client>.Update.Set(c => c.WalletExtract.Purchases, client.WalletExtract.Purchases);

            var result = await _clients.UpdateOneAsync(c => c.Id == clientId, updateDefinition);
            return result.ModifiedCount > 0;
        }


        public async Task<bool> VerifyPasswordAsync(string clientId, string password)
        {
            var client = await GetClientByIdAsync(clientId);
            if (client == null || string.IsNullOrEmpty(client.Password))
            {
                Console.WriteLine($"Cliente não encontrado ou senha não definida para ID: {clientId}");
                return false;
            }

            var passwordHasher = new PasswordHasher<Client>();
            var result = passwordHasher.VerifyHashedPassword(client, client.Password, password);

            Console.WriteLine($"Hash Armazenado: {client.Password}");
            Console.WriteLine($"Resultado da verificação: {result}");

            return result == PasswordVerificationResult.Success;
        }

    }
}
