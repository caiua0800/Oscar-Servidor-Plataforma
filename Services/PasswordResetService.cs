using DotnetBackend.Models;
namespace DotnetBackend.Services;
public class PasswordResetService
{
    private readonly ClientService _clientService;
    private readonly TokenService _tokenService;
    private readonly EmailService _emailService;

    public PasswordResetService(ClientService clientService, TokenService tokenService, EmailService emailService)
    {
        _clientService = clientService;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    public async Task<string> GeneratePasswordResetToken(string email)
    {
        var user = await _clientService.GetClientByEmailAsync(email);
        if (user == null) return null;

        var token = Guid.NewGuid().ToString();
        var expiry = DateTime.UtcNow.AddMinutes(30);

        await _tokenService.SaveToken(new PasswordResetToken
        {
            UserId = user.Id,
            Token = token,
            CreatedAt = DateTime.UtcNow
        });

        return token;
    }

    public async Task SendPasswordResetEmail(string email)
    {
        var token = await GeneratePasswordResetToken(email);
        if (token == null) return;

        var resetLink = $"https://localhost:3001/reset-password?token={token}";
        var subject = "Redefinição de Senha";
        var plainTextContent = $"Clique aqui para redefinir sua senha: {resetLink}";
        var htmlContent = $"<strong>Clique aqui para redefinir sua senha:</strong> <a href='{resetLink}'>{resetLink}</a>";

        await _emailService.SendEmailAsync(email, subject, plainTextContent, htmlContent);
    }
}