using MongoDB.Driver;
using DotnetBackend.Models;
using Microsoft.AspNetCore.Identity; // Para usar PasswordHasher
using System.Threading.Tasks;

namespace DotnetBackend.Services
{
    public class AdminService
    {
        private readonly IMongoCollection<Admin> _admins;
        private readonly ClientService _clientService;
        private readonly PurchaseService _purchaseService;
        private readonly WithdrawalService _withdrawalService;

        public AdminService(MongoDbService mongoDbService, ClientService clientService, PurchaseService purchaseService, WithdrawalService withdrawalService)
        {
            _admins = mongoDbService.GetCollection<Admin>("Admins");
            _clientService = clientService;
            _purchaseService = purchaseService;
            _withdrawalService = withdrawalService;
        }

        public async Task<List<Admin>> GetAllAdminsAsync()
        {
            return await _admins.Find(_ => true).ToListAsync();
        }

        public async Task<bool> DeleteAdminAsync(string id)
        {
            var deleteResult = await _admins.DeleteOneAsync(c => c.Id == id);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<Admin?> GetAdminByIdAsync(string id)
        {
            var normalizedId = id.Trim();
            return await _admins.Find(c => c.Id == normalizedId).FirstOrDefaultAsync();
        }

        public async Task<Admin> CreateAdminAsync(Admin admin, string password)
        {
            if (string.IsNullOrWhiteSpace(admin.Id))
            {
                throw new ArgumentException("O ID do administrador deve ser fornecido.");
            }

            var existingAdmin = await _admins.Find(a => a.Id == admin.Id).FirstOrDefaultAsync();
            if (existingAdmin != null)
            {
                throw new InvalidOperationException("Já existe um administrador com este ID.");
            }

            var passwordHasher = new PasswordHasher<Admin>();
            admin.Password = passwordHasher.HashPassword(admin, password);

            await _admins.InsertOneAsync(admin);
            return admin;
        }

        public async Task<bool> VerifyPasswordAsync(string adminId, string password)
        {
            var admin = await GetAdminByIdAsync(adminId);
            if (admin == null || string.IsNullOrEmpty(admin.Password))
            {
                Console.WriteLine($"Admin não encontrado ou senha não definida para ID: {adminId}");
                return false;
            }

            Console.WriteLine($"Tentando verificar a senha para Admin ID: {adminId}");
            Console.WriteLine($"Hash armazenado: {admin.Password}");

            var passwordHasher = new PasswordHasher<Admin>();
            var result = passwordHasher.VerifyHashedPassword(admin, admin.Password, password);

            Console.WriteLine($"Resultado da verificação: {result}");
            Console.WriteLine($"Senha de entrada: {password}");

            return result == PasswordVerificationResult.Success;
        }


    }
}
