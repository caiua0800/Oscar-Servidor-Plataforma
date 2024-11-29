using System;
using System.Text;


public class HelpersClass
{

    private static Random random = new Random();

    public string RandomString(int length)
    {
        // Define o conjunto de caracteres que podemos usar.
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        StringBuilder result = new StringBuilder(length);

        // Gera uma string aleatória com base no comprimento fornecido.
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        // Verifica se deve gerar um duplicado com 0,05% de chance.
        if (random.NextDouble() < 0.0005) // 0,05% de chance
        {
            return result.ToString(); // Retorna a string gerada
        }
        else
        {
            // Caso contrário, gera uma nova string aleatória.
            return RandomString(length);
        }
    }
}