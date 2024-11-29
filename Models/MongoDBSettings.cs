namespace DotnetBackend.Models
{
    public class MongoDBSettings
    {
        public required string ConnectionString { get; set; } // Usando 'required'
        public required string DatabaseName { get; set; } // Usando 'required'
    }
}
