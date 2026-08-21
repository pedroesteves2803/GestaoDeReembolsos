using GestaodeReembolsos.Data;
using GestaodeReembolsos.Dtos.Financeiro;
using GestaodeReembolsos.Enums;
using GestaodeReembolsos.Exceptions;
using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Services.Financeiro;

public class FinanceiroService(
    GestaoDeReembolsoContext context)
{
    public async Task<DecisaoAprovacao> Decisao(
        Guid idSolicitacaoReembolso,
        Guid idColaborador,
        DecisaoFinanceiraRequestDto decisaoFinanceiraRequestDto
        )
    {
        var solicitacoesReembolso = await context
            .SolicitacoesReembolso
            .Where(x => x.Id == idSolicitacaoReembolso)
            .Where(x => x.Status == StatusSolicitacaoReembolso.PendingFinanceValidation)
            .FirstOrDefaultAsync();
        
        if(solicitacoesReembolso == null)
            throw new ExcecaoRegraNegocio(
                "Esse reembolso informado não existe.",
                StatusCodes.Status404NotFound
            );
        
        var decisaoFinanceira = new DecisaoAprovacao
        {
            NivelDecisao = NivelDecisao.Finance,
            SolicitacaoReembolsoId =  idSolicitacaoReembolso,
            DecididaPorUsuarioId =  idColaborador,
            Decisao = decisaoFinanceiraRequestDto.Decisao,
            Comentario =  decisaoFinanceiraRequestDto.Comentario,
            CriadaEmUtc = DateTime.UtcNow
        };
        
        solicitacoesReembolso.Status = decisaoFinanceiraRequestDto.Decisao switch
        {
            Enums.Decisao.Aprovada => StatusSolicitacaoReembolso.ApprovedForPayment,
            Enums.Decisao.Rejeitada => StatusSolicitacaoReembolso.RejectedByFinance,
            Enums.Decisao.Devolvida => StatusSolicitacaoReembolso.ReturnedByFinance,
            _ => throw new ExcecaoRegraNegocio(
                "A decisão informada é inválida.",
                StatusCodes.Status400BadRequest)
        };

        solicitacoesReembolso.DecididaPeloFinanceiroEmUtc = DateTime.UtcNow;
        solicitacoesReembolso.AtualizadaEmUtc = DateTime.UtcNow;
        
        context.DecisoesAprovacao.Add(decisaoFinanceira);
        
        context.HistoricosStatusSolicitacao.Add(new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacoesReembolso.Id,
            StatusAnterior = StatusSolicitacaoReembolso.PendingFinanceValidation,
            NovoStatus = solicitacoesReembolso.Status,
            AlteradoPorUsuarioId = idColaborador
        });
        
        await context.SaveChangesAsync();
        
        return decisaoFinanceira;
    }
}