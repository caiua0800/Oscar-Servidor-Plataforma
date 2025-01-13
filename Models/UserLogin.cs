
namespace DotnetBackend.Models;

public class UserLogin
{
    public required string Id { get; set; } // CPF do cliente ou ID do administrador
    public required string Password { get; set; }
}