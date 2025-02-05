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

    public async Task DeleteSystemConfigByNameAsync(string name)
    {
        var normalizedName = name.Trim();
        var configToDelete = await _systemConfigs.Find(p => p.Name == normalizedName).FirstOrDefaultAsync();

        if (configToDelete != null)
        {
            await _systemConfigs.DeleteOneAsync(p => p.Name == normalizedName);
        }
    }
    public async Task<SystemConfig?> EditSystemConfigByName(string name, string newValue)
    {
        var normalizedName = name.Trim();

        var existingConfig = await _systemConfigs.Find(p => p.Name == normalizedName).FirstOrDefaultAsync();

        if (existingConfig == null)
        {
            return null;
        }else{
            SystemConfig? created = await CreateSystemConfigAsync(new SystemConfig(name, newValue));
            existingConfig = created;
        }

        existingConfig.LastValue = existingConfig.Value;
        existingConfig.Value = newValue;
        existingConfig.LastUpdate = DateTime.UtcNow;

        var updateDefinition = Builders<SystemConfig>.Update
            .Set(config => config.Value, newValue)
            .Set(config => config.LastValue, existingConfig.LastValue)
            .Set(config => config.LastUpdate, existingConfig.LastUpdate);

        await _systemConfigs.UpdateOneAsync(config => config.Name == normalizedName, updateDefinition);

        return existingConfig;
    }

    public async Task<SystemConfig?> GetSystemConfigByNameAsync(string name)
    {
        var normalizedName = name.Trim();
        return await _systemConfigs.Find(p => p.Name == normalizedName).FirstOrDefaultAsync();
    }
}