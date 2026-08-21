namespace GestaodeReembolsos.Exceptions;

public class ExcecaoRegraNegocio : Exception
{
    public int StatusCode { get; }

    public ExcecaoRegraNegocio(string mensagem, int statusCode)
        : base(mensagem)
    {
        StatusCode = statusCode;
    }
}