using DotnetBackend.Models;
using DotnetBackend.Services;
using MongoDB.Driver;

public class TokenService
{
    private readonly IMongoCollection<PasswordResetToken> _tokens;

    public TokenService(MongoDbService mongoDbService)
    {
        _tokens = mongoDbService.GetCollection<PasswordResetToken>("PasswordResetTokens"); 
    }

    public async Task SaveToken(PasswordResetToken token)
    {
        await _tokens.InsertOneAsync(token);
    }

    public async Task<PasswordResetToken> GetTokenData(string token)
    {
        return await _tokens.Find(t => t.Token == token).FirstOrDefaultAsync();
    }

    public async Task DeleteToken(string token)
    {
        await _tokens.DeleteOneAsync(t => t.Token == token);
    }
}