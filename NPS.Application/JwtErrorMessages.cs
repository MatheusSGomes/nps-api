namespace NPS.Application;

public class JwtErrorMessages
{
    public static string InvalidSecret = "Chave secreta inválida ou vazia";
    public static string InvalidUsername = "Username deve ser informato";
    public static string InvalidExpiration = "Tempo de expiração não pode ser negativo";
    public static string InvalidSecretEncode = "Chave secreta menor que 256 bits (32 bytes/caracteres)";
}
