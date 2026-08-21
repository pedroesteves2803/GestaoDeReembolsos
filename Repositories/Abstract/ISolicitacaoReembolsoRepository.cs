using GestaodeReembolsos.Models;

namespace GestaodeReembolsos.Repositories.Abstract;

public interface ISolicitacaoReembolsoRepository
{
    Task<SolicitacaoReembolso> Criar(SolicitacaoReembolso solicitacaoReembolso);
}