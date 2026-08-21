using GestaodeReembolsos.Data;
using GestaodeReembolsos.Dtos.SolicitacaoReembolso;
using GestaodeReembolsos.Enums;
using GestaodeReembolsos.Exceptions;
using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Services;

public class SolicitacaoReembolsoService(
    GestaoDeReembolsoContext context
    )
{
    public async Task<SolicitacaoReembolso> Criar(
        Guid colaboradorId,
        Guid departamentoId,
        DateOnly mesReferencia
        )
    {
        var departamentoExiste = await context.Departamentos
            .AnyAsync(x => x.Id == departamentoId);

        if (!departamentoExiste)
            throw new ExcecaoRegraNegocio(
                "Departamento não encontrado.",
                StatusCodes.Status404NotFound
            );
        
        var solicitacao = new SolicitacaoReembolso
        {
            ColaboradorId = colaboradorId,
            DepartamentoId = departamentoId,
            MesReferencia = mesReferencia,
            NumeroSolicitacao = $"REQ-{Guid.NewGuid():N}"[..20].ToUpperInvariant(),
            Status = StatusSolicitacaoReembolso.Rascunho,
            ValorTotal = 0
        };

        var historico = new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacao.Id,
            NovoStatus = StatusSolicitacaoReembolso.Rascunho,
            AlteradoPorUsuarioId = colaboradorId
        };

        await context.SolicitacoesReembolso.AddAsync(solicitacao);
        await context.HistoricosStatusSolicitacao.AddAsync(historico);

        await context.SaveChangesAsync();

        return solicitacao;
    }

    public async Task<ItemDespesa> AdicionarItem(
        Guid idSolicitacaoReembolso,
        Guid colaboradorId,
        ItemDespesaRequestDto itemDespesaDto
        )
    {
         var solicitacaoReembolso = await context
            .SolicitacoesReembolso
            .Where(x => x.Id == idSolicitacaoReembolso)
            .Where(x => x.ColaboradorId == colaboradorId)
            .FirstOrDefaultAsync();

         if (solicitacaoReembolso == null)
             throw new ExcecaoRegraNegocio(
                 "Esse reembolso informado não existe.",
                 StatusCodes.Status404NotFound
             );

         if (solicitacaoReembolso.Status != StatusSolicitacaoReembolso.Rascunho)
             throw new ExcecaoRegraNegocio(
                 "O status do reembolso não permite adicionar mais itens.",
                 StatusCodes.Status409Conflict
             );
         
         var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

         if (itemDespesaDto.DataDespesa > hoje)
             throw new ExcecaoRegraNegocio(
                 "A data da despesa não pode ser futura.",
                 StatusCodes.Status400BadRequest
             );

         var dataMinimaPermitida = DateOnly.FromDateTime(solicitacaoReembolso.CriadaEmUtc)
             .AddDays(-90);

         if (itemDespesaDto.DataDespesa < dataMinimaPermitida)
             throw new ExcecaoRegraNegocio(
                 "A data da despesa não pode ser anterior a 90 dias da criação da solicitação.",
                 StatusCodes.Status400BadRequest
             );

         if (
             solicitacaoReembolso.MesReferencia.Year != itemDespesaDto.DataDespesa.Year 
             || solicitacaoReembolso.MesReferencia.Month != itemDespesaDto.DataDespesa.Month)
             throw new ExcecaoRegraNegocio(
                 "A data da despesa deve pertencer ao mês de referência.",
                 StatusCodes.Status400BadRequest
             );

         var categoria = await context
             .CategoriasDespesa
             .Where(x => x.Id == itemDespesaDto.CategoriaDespesaId)
             .Where(x => x.Ativo == true)
             .FirstOrDefaultAsync();

         if (categoria == null)
             throw new ExcecaoRegraNegocio(
                 "Categoria não existe",
                 StatusCodes.Status404NotFound
             );
         
         var itemDespesa = new ItemDespesa
         {
             SolicitacaoReembolsoId = idSolicitacaoReembolso,
             Valor = itemDespesaDto.Valor,
             Descricao = itemDespesaDto.Descricao,
             CategoriaDespesaId =  itemDespesaDto.CategoriaDespesaId,
             NomeEstabelecimento =  itemDespesaDto.NomeEstabelecimento,
             DataDespesa = itemDespesaDto.DataDespesa
         };
        
         await context.ItensDespesa.AddAsync(itemDespesa);

         solicitacaoReembolso.ValorTotal += itemDespesaDto.Valor;
        
         await context.SaveChangesAsync();
         
         return itemDespesa;
    }

    public async Task<SolicitacaoReembolso> Enviar(
        Guid idSolicitacaoReembolso,
        Guid colaboradorId
    )
    {
        var solicitacaoReembolso = await context
            .SolicitacoesReembolso
            .Where(x => x.Id == idSolicitacaoReembolso)
            .Where(x => x.ColaboradorId == colaboradorId)
            .FirstOrDefaultAsync();

        if (solicitacaoReembolso == null)
            throw new ExcecaoRegraNegocio(
                "Esse reembolso informado não existe.",
                StatusCodes.Status404NotFound
            );

        if (solicitacaoReembolso.Status != StatusSolicitacaoReembolso.Rascunho)
            throw new ExcecaoRegraNegocio(
                "O status do reembolso não permite enviar para aprovação.",
                StatusCodes.Status409Conflict
            );

        var existemItens = await context
            .ItensDespesa
            .AnyAsync(x => x.SolicitacaoReembolsoId == idSolicitacaoReembolso);
        
        if (!existemItens)
            throw new ExcecaoRegraNegocio(
                "Adicione pelo menos um item antes de enviar a solicitação.",
                StatusCodes.Status409Conflict
            );
        
        solicitacaoReembolso.Status = StatusSolicitacaoReembolso.AguardandoAprovacaoGestor;
        solicitacaoReembolso.EnviadaEmUtc = DateTime.UtcNow;
        solicitacaoReembolso.AtualizadaEmUtc = DateTime.UtcNow;
        
        var historico = new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacaoReembolso.Id,
            StatusAnterior = StatusSolicitacaoReembolso.Rascunho,
            NovoStatus = StatusSolicitacaoReembolso.AguardandoAprovacaoGestor,
            AlteradoPorUsuarioId = solicitacaoReembolso.ColaboradorId,
            Reason = null,
        };

        await context.HistoricosStatusSolicitacao.AddAsync(historico);
        await context.SaveChangesAsync();
        
        return solicitacaoReembolso;
    }

    public async Task<DecisaoAprovacao> Decisao(
        Guid idSolicitacaoReembolso,
        Guid gestorId,
        DecisaoGestorRequestDto decisaoGestorRequestDto)
    {
        if (!Enum.IsDefined(decisaoGestorRequestDto.Decisao))
            throw new ExcecaoRegraNegocio(
                "A decisão informada é inválida.",
                StatusCodes.Status400BadRequest
            );

        var solicitacaoDeReembolso = await context.SolicitacoesReembolso  
            .Where(x => x.Id == idSolicitacaoReembolso)
            .FirstOrDefaultAsync();
        
        if (solicitacaoDeReembolso == null)
            throw new ExcecaoRegraNegocio(
                "A solicitação de reembolso não foi encontrada.",
                StatusCodes.Status404NotFound
            );
        
        if(solicitacaoDeReembolso.Status != StatusSolicitacaoReembolso.AguardandoAprovacaoGestor)
            throw new ExcecaoRegraNegocio(
                "A solicitação não está aguardando a aprovação do gestor.",
                StatusCodes.Status409Conflict
            );

        var gestor = await context.Usuarios
            .Where(x => x.Id == solicitacaoDeReembolso.ColaboradorId)
            .Where(x => x.GestorId == gestorId)
            .FirstOrDefaultAsync();
         
         if(gestor == null)
             throw new ExcecaoRegraNegocio(
                 "Você não é o gestor responsável por esta solicitação.",
                 StatusCodes.Status404NotFound
             );

         if(decisaoGestorRequestDto.Decisao == Enums.Decisao.Aprovada)
             solicitacaoDeReembolso.Status = StatusSolicitacaoReembolso.PendingFinanceValidation;
         
         if(decisaoGestorRequestDto.Decisao == Enums.Decisao.Rejeitada)
             solicitacaoDeReembolso.Status = StatusSolicitacaoReembolso.RejectedByManager;

         if(decisaoGestorRequestDto.Decisao == Enums.Decisao.Devolvida)
             solicitacaoDeReembolso.Status = StatusSolicitacaoReembolso.ReturnedByManager;
         
         solicitacaoDeReembolso.DecididaPeloGestorEmUtc = DateTime.UtcNow;
         solicitacaoDeReembolso.AtualizadaEmUtc =  DateTime.UtcNow;
         
        var decisaoAprovacao = new DecisaoAprovacao
        {
            SolicitacaoReembolsoId = solicitacaoDeReembolso.Id,
            DecididaPorUsuarioId = gestorId,
            Comentario = decisaoGestorRequestDto.Comentario,
            Decisao = decisaoGestorRequestDto.Decisao,
            NivelDecisao = NivelDecisao.Manager,
            CriadaEmUtc =  DateTime.UtcNow,
        };
        
        await context.DecisoesAprovacao.AddAsync(decisaoAprovacao);
        
        await context.HistoricosStatusSolicitacao.AddAsync(new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacaoDeReembolso.Id,
            StatusAnterior = StatusSolicitacaoReembolso.AguardandoAprovacaoGestor,
            NovoStatus = solicitacaoDeReembolso.Status,
            AlteradoPorUsuarioId = gestorId,
            Reason = null,
        });
        
        await context.SaveChangesAsync();
        
        return
        {
            
        };
    }
}