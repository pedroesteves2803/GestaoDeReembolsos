using GestaodeReembolsos.Models;

namespace GestaodeReembolsos.Repositories.Abstract;

public interface IHistoricosStatusSolicitacaoRepository
{
    Task<HistoricoStatusSolicitacao> Criar(HistoricoStatusSolicitacao historicoStatusSolicitacao);
}