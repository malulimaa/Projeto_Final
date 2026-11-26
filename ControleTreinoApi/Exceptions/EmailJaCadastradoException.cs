namespace ControleTreinoApi.Exceptions;

public class EmailJaCadastradoException : Exception
{
    public EmailJaCadastradoException() : base("Já existe uma conta com este e-mail.") { }
}