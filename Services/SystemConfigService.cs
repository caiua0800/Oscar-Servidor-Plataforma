using MongoDB.Driver;
using DotnetBackend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DotnetBackend.Services;

public class SystemConfigService
{
    private readonly IMongoCollection<SystemConfig> _systemConfigs;

    public SystemConfigService(MongoDbService mongoDbService)
    {
        _systemConfigs = mongoDbService.GetCollection<SystemConfig>("systemConfigs");
    }

    public async Task<SystemConfig> CreateSystemConfigAsync(SystemConfig systemConfig)
    {
        await _systemConfigs.InsertOneAsync(systemConfig);
        return systemConfig;
    }

    public async Task<List<SystemConfig>> GetAllSystemConfigsAsync()
    {
        return await _systemConfigs.Find(_ => true).ToListAsync();
    }


    public async Task<SystemConfig?> GetSystemConfigByNameAsync(string name)
    {
        var normalizedName = name.Trim();
        return await _systemConfigs.Find(p => p.Name == normalizedName).FirstOrDefaultAsync();
    }
}