using GestaodeReembolsos.Models;

namespace GestaodeReembolsos.Repositories;

public interface IHistoricosStatusSolicitacaoRepository
{
    Task<HistoricoStatusSolicitacao> Criar(HistoricoStatusSolicitacao historicoStatusSolicitacao);
}