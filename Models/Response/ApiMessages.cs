namespace IsekaiFantasyBE.Models.Response;

public class ApiMessages
{
    public const string UserRetrieved = "Usuários encontrados com sucesso.";
    public const string UserNotFound = "Nenhum usuário encontrado.";
    public const string UserCreated = "Usuário cadastrado com sucesso.";
    public const string EmailInvalid = "Email é inválido.";
    public const string EmailAlreadyExists = "Email já registrado.";
    public const string PasswordInvalidLength = "Senha deve conter pelo menos 8 caracteres.";
    public const string PasswordInvalidUpper = "Senha deve conter pelo menos uma letra maiúscula.";
    public const string PasswordInvalidLower = "Senha deve conter pelo menos uma letra minúscula.";
    public const string PasswordInvalidDigit = "Senha deve conter pelo menos um número.";
    public const string PasswordInvalidSpecial = "Senha deve conter pelo menos um caractere especial.";
    public const string EmptyCredentials = "Identificador (usuário ou email) e senha não podem estar vazios.";
    public const string WrongPassword = "Senha incorreta.";
    public const string LoginSuccess = "Login realizado.";
    public const string NotAuthenticated = "Não autorizado. Você deve estar logado para continuar.";
    public const string InsufficientPermissions = "Não autorizado. Permissões insuficientes.";
    public const string UserUpdated = "Usuário atualizado com sucesso.";
    public const string AlreadyRegistered = "Usuário já cadastrado.";
    public const string InRegisterProgress = "Usuário já em processo de cadastro. Cheque a caixa de email.";
    public const string NotInPreRegister = "Usuário não está em processo de cadastro.";
    public const string UserBanned = "Usuário banido.";
    public const string PropertiesNotFound = "Propriedades do usuário não existem.";
}