using GestaodeReembolsos.Data;
using GestaodeReembolsos.Models;
using GestaodeReembolsos.Repositories.Abstract;

namespace GestaodeReembolsos.Repositories;

public class HistoricoDeReembolsoRepository(GestaoDeReembolsoContext context) : IHistoricosStatusSolicitacaoRepository
{
    public async Task<HistoricoStatusSolicitacao> Criar(HistoricoStatusSolicitacao historicoStatusSolicitacao)
    {
        await context.HistoricosStatusSolicitacao.AddAsync(historicoStatusSolicitacao);
        await context.SaveChangesAsync();
        
        return historicoStatusSolicitacao;
    }
}