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
            .FirstOrDefaultAsync();
        
        if(solicitacoesReembolso == null)
            throw new ExcecaoRegraNegocio(
                "A solicitação de reembolso não foi encontrada.",
                StatusCodes.Status404NotFound
            );
        
        if (solicitacoesReembolso.Status != StatusSolicitacaoReembolso.AguardandoValidacaoFinanceira)
            throw new ExcecaoRegraNegocio(
                "A solicitação não está aguardando validação financeira.",
                StatusCodes.Status409Conflict
            );
        
        if (
            decisaoFinanceiraRequestDto.Decisao != Enums.Decisao.Aprovada &&
            (string.IsNullOrWhiteSpace(decisaoFinanceiraRequestDto.Comentario) ||
             decisaoFinanceiraRequestDto.Comentario.Trim().Length < 10 ||
             decisaoFinanceiraRequestDto.Comentario.Trim().Length > 500))
            throw new ExcecaoRegraNegocio(
    "Para rejeitar ou devolver a solicitação, informe um comentário entre 10 e 500 caracteres.",
    StatusCodes.Status400BadRequest
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
            Enums.Decisao.Aprovada => StatusSolicitacaoReembolso.AprovadaParaPagamento,
            Enums.Decisao.Rejeitada => StatusSolicitacaoReembolso.RejeitadaPeloFinanceiro,
            Enums.Decisao.Devolvida => StatusSolicitacaoReembolso.DevolvidaPeloFinanceiro,
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
            StatusAnterior = StatusSolicitacaoReembolso.AguardandoValidacaoFinanceira,
            NovoStatus = solicitacoesReembolso.Status,
            AlteradoPorUsuarioId = idColaborador,
            Motivo = decisaoFinanceiraRequestDto.Comentario
        });
        
        await context.SaveChangesAsync();
        
        return decisaoFinanceira;
    }

    public async Task<Pagamento> Pagamento(
        Guid idSolicitacaoReembolso,
        Guid idColaborador,
        RegistrarPagamentoRequestDto registrarPagamentoRequestDto)
    {
        var solicitacoesReembolso = await context
            .SolicitacoesReembolso
            .Where(x => x.Id == idSolicitacaoReembolso)
            .FirstOrDefaultAsync();
        
        if(solicitacoesReembolso == null)
            throw new ExcecaoRegraNegocio(
                "A solicitação de reembolso não foi encontrada.",
                StatusCodes.Status404NotFound
            );

        if (solicitacoesReembolso.Status != StatusSolicitacaoReembolso.AprovadaParaPagamento)
            throw new ExcecaoRegraNegocio(
                "A solicitação não está aprovada para pagamento.",              StatusCodes.Status409Conflict
            );
        
        if (registrarPagamentoRequestDto.ValorPago != solicitacoesReembolso.ValorTotal)
            throw new ExcecaoRegraNegocio(
                "O valor pago deve ser igual ao valor total da solicitação.",
                StatusCodes.Status400BadRequest
            );
        
        var referenciaExiste = await context.Pagamentos
            .AnyAsync(x => x.ReferenciaPagamento == registrarPagamentoRequestDto.Referencia);

        if (referenciaExiste)
            throw new ExcecaoRegraNegocio(
                "Já existe um pagamento com esta referência.",
                StatusCodes.Status409Conflict
            );

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        if (registrarPagamentoRequestDto.DataPagamento > hoje)
            throw new ExcecaoRegraNegocio(
                "A data do pagamento não pode ser futura.",
                StatusCodes.Status400BadRequest
            );

        var pagamento = new Pagamento
        {
            SolicitacaoReembolsoId = solicitacoesReembolso.Id,
            ReferenciaPagamento = registrarPagamentoRequestDto.Referencia,
            ValorPago =  registrarPagamentoRequestDto.ValorPago,
            DataPagamento = registrarPagamentoRequestDto.DataPagamento,
            ProcessadoPorUsuarioId = idColaborador,
            Observacoes = registrarPagamentoRequestDto.Observacao,
            CriadaEmUtc = DateTime.UtcNow
        };

        await context.Pagamentos.AddAsync(pagamento);

        solicitacoesReembolso.Status = StatusSolicitacaoReembolso.Paga;
        solicitacoesReembolso.PagaEmUtc = DateTime.UtcNow;
        solicitacoesReembolso.AtualizadaEmUtc = DateTime.UtcNow;
        
        context.HistoricosStatusSolicitacao.Add(new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacoesReembolso.Id,
            StatusAnterior = StatusSolicitacaoReembolso.AprovadaParaPagamento,
            NovoStatus = solicitacoesReembolso.Status,
            AlteradoPorUsuarioId = idColaborador,
            Motivo = registrarPagamentoRequestDto.Observacao
        });

        await context.SaveChangesAsync();

        return pagamento;
    }
}
