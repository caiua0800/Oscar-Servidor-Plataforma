using DotnetBackend.Models;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetBackend.Services
{
    public class ConnectionIpService
    {
        private readonly IMongoCollection<ConnectionIp> _connectionIps;

        public ConnectionIpService(MongoDbService mongoDbService)
        {
            _connectionIps = mongoDbService.GetCollection<ConnectionIp>("ConnectionIps");
        }

        public async Task RegisterIpAsync(string ip, string userId = null)
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            var lastAccess = await _connectionIps
                .Find(ipRecord => ipRecord.Ip == ip && ipRecord.Timestamp >= oneWeekAgo)
                .SortByDescending(ipRecord => ipRecord.Timestamp)
                .FirstOrDefaultAsync();

            if (lastAccess == null)
            {
                var connectionIp = new ConnectionIp
                {
                    Ip = ip,
                    Timestamp = DateTime.UtcNow,
                    UserId = userId
                };

                await _connectionIps.InsertOneAsync(connectionIp);
                Console.WriteLine($"Novo IP registrado: {ip}");
            }
            else
            {
                Console.WriteLine($"IP {ip} já registrado na última semana. Não será registrado novamente.");
            }
        }

        public async Task<int> CountUniqueIpsThisMonthAsync()
        {
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var uniqueIps = await _connectionIps
                .Find(ip => ip.Timestamp >= startOfMonth && ip.Timestamp <= endOfMonth)
                .ToListAsync();

            var distinctIps = uniqueIps.Select(ip => ip.Ip).Distinct().Count();
            return distinctIps;
        }

        public async Task<int> CountTotalAccessesThisMonthAsync()
        {
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var totalAccesses = await _connectionIps
                .CountDocumentsAsync(ip => ip.Timestamp >= startOfMonth && ip.Timestamp <= endOfMonth);

            return (int)totalAccesses;
        }

        public async Task<int> CountTotalAccessesLastWeekAsync()
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);

            var totalAccessesLastWeek = await _connectionIps
                .CountDocumentsAsync(ip => ip.Timestamp >= oneWeekAgo);

            return (int)totalAccessesLastWeek;
        }
    }
}