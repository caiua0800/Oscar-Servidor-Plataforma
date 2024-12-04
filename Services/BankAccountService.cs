using MongoDB.Driver;
using DotnetBackend.Models;
using System.Threading.Tasks;

namespace DotnetBackend.Services
{
    public class BankAccountService
    {
        private readonly IMongoCollection<BankAccount> _bankAccountCollection;
        private readonly ExtractService _extractService; // Serviço de extrato

        public BankAccountService(MongoDbService mongoDbService, ExtractService extractService)
        {
            _bankAccountCollection = mongoDbService.GetCollection<BankAccount>("BankAccount");
            _extractService = extractService; // Injeta o serviço de extrato
        }

        public async Task<BankAccount> CreateBankAccountAsync(BankAccount bankAccount)
        {
            var existingAccount = await _bankAccountCollection.Find(account => true).FirstOrDefaultAsync();

            if (existingAccount != null)
            {
                throw new InvalidOperationException("Uma conta bancária já existe.");
            }

            await _bankAccountCollection.InsertOneAsync(bankAccount);
            return bankAccount;
        }

        public async Task<BankAccount> GetBankAccountAsync()
        {
            return await _bankAccountCollection.Find(account => true).FirstOrDefaultAsync();
        }

        public async Task<BankAccount> AddToBalanceAsync(string? purchaseId, decimal amount)
        {
            var account = await GetBankAccountAsync();
            if (account == null)
            {
                throw new InvalidOperationException("Conta bancária não encontrada.");
            }
            account.AddToBalance(amount);
            await _bankAccountCollection.ReplaceOneAsync(a => a.Id == account.Id, account);


            if (purchaseId != null)
            {
                var extract = new Extract($"Depósito de R${amount} na conta bancária ref compra #{purchaseId}", amount, account.AccountHolderName);
                await _extractService.CreateExtractAsync(extract);
            }
            else
            {
                var extract = new Extract($"Depósito de R${amount} na conta bancária", amount, account.AccountHolderName);
                await _extractService.CreateExtractAsync(extract);
            }

            return account;
        }

        public async Task<BankAccount> WithdrawFromBalanceAsync(string? withdrawId, decimal amount)
        {
            var account = await GetBankAccountAsync();
            if (account == null)
            {
                throw new InvalidOperationException("Conta bancária não encontrada.");
            }
            account.WithdrawFromBalance(amount);
            await _bankAccountCollection.ReplaceOneAsync(a => a.Id == account.Id, account);

            // Cria extrato para a operação de saque
            if (withdrawId != null)
            {
                var extract = new Extract($"Pagamento de solicitação de saque de {amount} da conta bancária ref saque #{withdrawId}", amount, account.AccountHolderName);
                await _extractService.CreateExtractAsync(extract);
            }
            else
            {
                var extract = new Extract($"Pagamento de solicitação de saque da conta bancária de {amount}", amount, account.AccountHolderName);
                await _extractService.CreateExtractAsync(extract);
            }

            return account;
        }


    }
}