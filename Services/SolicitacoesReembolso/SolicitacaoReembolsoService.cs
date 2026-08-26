using GestaodeReembolsos.Data;
using GestaodeReembolsos.Dtos.SolicitacaoReembolso;
using GestaodeReembolsos.Enums;
using GestaodeReembolsos.Exceptions;
using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Services.SolicitacoesReembolso;

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
        var departamento = await context.Departamentos
            .AnyAsync(x => x.Id == departamentoId);

        if (!departamento)
            throw new ExcecaoRegraNegocio(
                "Departamento não encontrado.",
                StatusCodes.Status404NotFound
            );

        var usuario = await context.Usuarios
            .Where(x => x.Id == colaboradorId)
            .Where(x => x.DepartamentoId == departamentoId)
            .FirstOrDefaultAsync();

        if (usuario == null)
            throw new ExcecaoRegraNegocio(
                "O departamento informado não é compatível com o usuário autenticado.",
                StatusCodes.Status409Conflict
            );

        var solicitacao = new SolicitacaoReembolso
        {
            Id = Guid.NewGuid(),
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
                "A solicitação de reembolso não foi encontrada.",
                StatusCodes.Status404NotFound
            );

        if (
            solicitacaoReembolso.Status != StatusSolicitacaoReembolso.Rascunho &&
            solicitacaoReembolso.Status != StatusSolicitacaoReembolso.DevolvidaPeloGestor &&
            solicitacaoReembolso.Status != StatusSolicitacaoReembolso.DevolvidaPeloFinanceiro)
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
                "A categoria de despesa não foi encontrada.",
                StatusCodes.Status404NotFound
            );

        var itemDespesa = new ItemDespesa
        {
            SolicitacaoReembolsoId = idSolicitacaoReembolso,
            Valor = itemDespesaDto.Valor,
            Descricao = itemDespesaDto.Descricao,
            CategoriaDespesaId = itemDespesaDto.CategoriaDespesaId,
            NomeEstabelecimento = itemDespesaDto.NomeEstabelecimento,
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
                "A solicitação de reembolso não foi encontrada.",
                StatusCodes.Status404NotFound
            );

        if (
            solicitacaoReembolso.Status != StatusSolicitacaoReembolso.Rascunho &&
            solicitacaoReembolso.Status != StatusSolicitacaoReembolso.DevolvidaPeloGestor &&
            solicitacaoReembolso.Status != StatusSolicitacaoReembolso.DevolvidaPeloFinanceiro)
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

        var statusAnterior = solicitacaoReembolso.Status;
        solicitacaoReembolso.Status = StatusSolicitacaoReembolso.AguardandoAprovacaoGestor;
        solicitacaoReembolso.EnviadaEmUtc = DateTime.UtcNow;
        solicitacaoReembolso.AtualizadaEmUtc = DateTime.UtcNow;

        var historico = new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacaoReembolso.Id,
            StatusAnterior = statusAnterior,
            NovoStatus = StatusSolicitacaoReembolso.AguardandoAprovacaoGestor,
            AlteradoPorUsuarioId = solicitacaoReembolso.ColaboradorId,
            Motivo = null,
        };

        await context.HistoricosStatusSolicitacao.AddAsync(historico);
        await context.SaveChangesAsync();

        return solicitacaoReembolso;
    }

    public async Task<ResultadoDecisaoGestor> Decisao(
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

        if (solicitacaoDeReembolso.Status != StatusSolicitacaoReembolso.AguardandoAprovacaoGestor)
            throw new ExcecaoRegraNegocio(
                "A solicitação não está aguardando a aprovação do gestor.",
                StatusCodes.Status409Conflict
            );

        if (
            decisaoGestorRequestDto.Decisao != Enums.Decisao.Aprovada &&
            (string.IsNullOrWhiteSpace(decisaoGestorRequestDto.Comentario) ||
             decisaoGestorRequestDto.Comentario.Trim().Length < 10 ||
             decisaoGestorRequestDto.Comentario.Trim().Length > 500))
            throw new ExcecaoRegraNegocio(
                "Para rejeitar ou devolver a solicitação, informe um comentário entre 10 e 500 caracteres.",
                StatusCodes.Status400BadRequest
            );

        var gestor = await context.Usuarios
            .Where(x => x.Id == solicitacaoDeReembolso.ColaboradorId)
            .Where(x => x.GestorId == gestorId)
            .FirstOrDefaultAsync();

        if (gestor == null)
            throw new ExcecaoRegraNegocio(
                "Você não é o gestor responsável por esta solicitação.",
                StatusCodes.Status404NotFound
            );

        solicitacaoDeReembolso.Status = decisaoGestorRequestDto.Decisao switch
        {
            Enums.Decisao.Aprovada => StatusSolicitacaoReembolso.AguardandoValidacaoFinanceira,
            Enums.Decisao.Rejeitada => StatusSolicitacaoReembolso.RejeitadaPeloGestor,
            Enums.Decisao.Devolvida => StatusSolicitacaoReembolso.DevolvidaPeloGestor,
            _ => throw new ExcecaoRegraNegocio(
                "A decisão informada é inválida.",
                StatusCodes.Status400BadRequest)
        };

        solicitacaoDeReembolso.DecididaPeloGestorEmUtc = DateTime.UtcNow;
        solicitacaoDeReembolso.AtualizadaEmUtc = DateTime.UtcNow;

        var decisaoAprovacao = new DecisaoAprovacao
        {
            SolicitacaoReembolsoId = solicitacaoDeReembolso.Id,
            DecididaPorUsuarioId = gestorId,
            Comentario = decisaoGestorRequestDto.Comentario,
            Decisao = decisaoGestorRequestDto.Decisao,
            NivelDecisao = NivelDecisao.Manager,
            CriadaEmUtc = DateTime.UtcNow,
        };

        await context.DecisoesAprovacao.AddAsync(decisaoAprovacao);

        await context.HistoricosStatusSolicitacao.AddAsync(new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacaoDeReembolso.Id,
            StatusAnterior = StatusSolicitacaoReembolso.AguardandoAprovacaoGestor,
            NovoStatus = solicitacaoDeReembolso.Status,
            AlteradoPorUsuarioId = gestorId,
            Motivo = decisaoGestorRequestDto.Comentario
        });

        await context.SaveChangesAsync();

        return new ResultadoDecisaoGestor(solicitacaoDeReembolso, decisaoAprovacao);
    }

    public async Task<IList<ListaSolicitacaoReembolsoResponseDto>> ListarReembolsosPorUsuario(
        Guid colaboradorId)
    {
        return await context.SolicitacoesReembolso
            .AsNoTracking()
            .Where(x => x.ColaboradorId == colaboradorId)
            .Select(x => new ListaSolicitacaoReembolsoResponseDto{
                Id = x.Id,
                NumeroSolicitacao = x.NumeroSolicitacao,
                MesReferencia = x.MesReferencia,
                Status = x.Status,
                ValorTotal = x.ValorTotal,
                CriadaEmUtc = x.CriadaEmUtc,
                EnviadaEmUtc = x.EnviadaEmUtc,
                AtualizadaEmUtc = x.AtualizadaEmUtc,
            })
            .ToListAsync();
    }

    public async Task<DetalheSolicitacaoReembolsoResponseDto?> ObterDetalhe(
        Guid idSolitacaoReembolso,
        Guid colaboradorId)
    {
        var detalhe =  await context.SolicitacoesReembolso
            .AsNoTracking()
            .Where(x => x.Id == idSolitacaoReembolso)
            .Where(x => x.ColaboradorId == colaboradorId)
            .Select(x => new DetalheSolicitacaoReembolsoResponseDto{
                Id = x.Id,
                NumeroSolicitacao = x.NumeroSolicitacao,
                Status = x.Status,
                ValorTotal = x.ValorTotal,
                Itens = x.ItensDespesa.Select(x => new ItemDespesaDetalheResponseDto
                {
                    Id = x.Id,
                    Categoria = x.CategoriaDespesa.Nome,
                    DataDespesa = x.DataDespesa,
                    Descricao = x.Descricao,
                    Valor = x.Valor,
                    NomeEstabelecimento = x.NomeEstabelecimento,
                    NomeArquivoComprovante = ""
                }).ToList(),
                Decisoes = x.DecisoesAprovacao.Select(x => new DecisaoAprovacaoResponseDto
                {
                    NivelDecisao = x.NivelDecisao,
                    Decisao = x.Decisao,
                    Comentario = x.Comentario,
                    DecididaPor = x.DecididaPorUsuario.NomeCompleto,
                    CriadaEmUtc = x.CriadaEmUtc
                }).ToList(),
                Historico = x.HistoricosStatusSolicitacao.Select(x => new HistoricoStatusResponseDto
                {
                    StatusAnterior = x.StatusAnterior,
                    NovoStatus = x.NovoStatus,
                    Motivo = x.Motivo,
                    AlteradoPor = x.AlteradoPorUsuario.NomeCompleto,
                    CriadaEmUtc = x.CriadaEmUtc,
                }).ToList()
            })
            .FirstOrDefaultAsync();
        
        if(detalhe == null)
            throw new ExcecaoRegraNegocio(
                "A solicitação de reembolso não foi encontrada.",
                StatusCodes.Status404NotFound
            );
        
        return detalhe;
    }

    public async Task<ItemDespesa> Editar(
        Guid idSolitacaoReembolso,
        Guid idItem,
        Guid colaboradorId,
        ItemDespesaRequestDto itemDespesaDto)
    {
        var solicitacaoDeReembolso = await context
            .SolicitacoesReembolso
            .Where(x => x.ColaboradorId == colaboradorId)
            .FirstOrDefaultAsync(x => x.Id == idSolitacaoReembolso);

        if (solicitacaoDeReembolso == null)
            throw new ExcecaoRegraNegocio(
                "A solicitação de reembolso não foi encontrada.",
                StatusCodes.Status404NotFound
            );

        if (
            solicitacaoDeReembolso.Status != StatusSolicitacaoReembolso.Rascunho &&
            solicitacaoDeReembolso.Status != StatusSolicitacaoReembolso.DevolvidaPeloGestor &&
            solicitacaoDeReembolso.Status != StatusSolicitacaoReembolso.DevolvidaPeloFinanceiro
        )
            throw new ExcecaoRegraNegocio(
                "O status da solicitação não permite alterar itens.",
                StatusCodes.Status409Conflict
            );
        
        var item = await context
            .ItensDespesa
            .FirstOrDefaultAsync(x =>
                x.Id == idItem &&
                x.SolicitacaoReembolsoId == idSolitacaoReembolso);
        
        if(item == null)
            throw new ExcecaoRegraNegocio(
                "O item de despesa não foi encontrado.",
                StatusCodes.Status404NotFound
            );
        
        
        var dataMinimaPermitida = DateOnly.FromDateTime(solicitacaoDeReembolso.CriadaEmUtc)
            .AddDays(-90);

        if (itemDespesaDto.DataDespesa < dataMinimaPermitida)
            throw new ExcecaoRegraNegocio(
                "A data da despesa não pode ser anterior a 90 dias da criação da solicitação.",
                StatusCodes.Status400BadRequest
            );
        
        if (itemDespesaDto.DataDespesa > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ExcecaoRegraNegocio(
                "A data da despesa não pode ser futura.",
                StatusCodes.Status400BadRequest
            );

        if (
            solicitacaoDeReembolso.MesReferencia.Year != itemDespesaDto.DataDespesa.Year
            || solicitacaoDeReembolso.MesReferencia.Month != itemDespesaDto.DataDespesa.Month)
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
                "A categoria de despesa não foi encontrada.",
                StatusCodes.Status404NotFound
            );
        
        solicitacaoDeReembolso.ValorTotal = solicitacaoDeReembolso.ValorTotal - item.Valor + itemDespesaDto.Valor;
        solicitacaoDeReembolso.AtualizadaEmUtc = DateTime.UtcNow;
        
        item.CategoriaDespesaId = categoria.Id;
        item.DataDespesa = itemDespesaDto.DataDespesa;
        item.Descricao = itemDespesaDto.Descricao;
        item.Valor = itemDespesaDto.Valor;
        item.NomeEstabelecimento = itemDespesaDto.NomeEstabelecimento;
        item.AtualizadaEmUtc = DateTime.UtcNow;
        
        await context.SaveChangesAsync();

        return item;
    }

    public async Task Excluir(
        Guid idSolitacaoReembolso,
        Guid idItem,
        Guid colaboradorId)
    {
        var solicitacaoDeReembolso = await context
            .SolicitacoesReembolso
            .Where(x => x.ColaboradorId == colaboradorId)
            .FirstOrDefaultAsync(x => x.Id == idSolitacaoReembolso);

        if (solicitacaoDeReembolso == null)
            throw new ExcecaoRegraNegocio(
                "A solicitação de reembolso não foi encontrada.",
                StatusCodes.Status404NotFound
            );
        
        if (
            solicitacaoDeReembolso.Status != StatusSolicitacaoReembolso.Rascunho &&
            solicitacaoDeReembolso.Status != StatusSolicitacaoReembolso.DevolvidaPeloGestor &&
            solicitacaoDeReembolso.Status != StatusSolicitacaoReembolso.DevolvidaPeloFinanceiro)
            throw new ExcecaoRegraNegocio(
                "O status da solicitação não permite excluir itens.",
                StatusCodes.Status409Conflict
            );
        
        var item = await context
            .ItensDespesa
            .FirstOrDefaultAsync(x =>
                x.Id == idItem &&
                x.SolicitacaoReembolsoId == idSolitacaoReembolso);
        
        if(item == null)
            throw new ExcecaoRegraNegocio(
                "O item de despesa não foi encontrado.",
                StatusCodes.Status404NotFound
            );
        
        solicitacaoDeReembolso.ValorTotal -= item.Valor;
        solicitacaoDeReembolso.AtualizadaEmUtc = DateTime.UtcNow;
        
        context.ItensDespesa.Remove(item);
        await context.SaveChangesAsync();
    }

    public async Task Cancelar(
        Guid idSolitacaoReembolso,
        Guid colaboradorId,
        CancelarRequestDto cancelarRequestDto)
    {
        var solicitacaoDeReembolso = await context
            .SolicitacoesReembolso
            .FirstOrDefaultAsync(x => x.Id == idSolitacaoReembolso);

        if (solicitacaoDeReembolso == null)
            throw new ExcecaoRegraNegocio(
                "A solicitação de reembolso não foi encontrada.",
                StatusCodes.Status404NotFound
            );

        if (solicitacaoDeReembolso.ColaboradorId != colaboradorId)
            throw new ExcecaoRegraNegocio(
                "A solicitação de reembolso não foi encontrada.",
                StatusCodes.Status404NotFound
            );
        
        if (
            solicitacaoDeReembolso.Status != StatusSolicitacaoReembolso.Rascunho &&
            solicitacaoDeReembolso.Status != StatusSolicitacaoReembolso.DevolvidaPeloGestor &&
            solicitacaoDeReembolso.Status != StatusSolicitacaoReembolso.DevolvidaPeloFinanceiro)
            throw new ExcecaoRegraNegocio(
                "O status da solicitação não permite cancelamento.",
                StatusCodes.Status409Conflict
            );    
        
        var statusAnteriro =  solicitacaoDeReembolso.Status;
        solicitacaoDeReembolso.Status = StatusSolicitacaoReembolso.Cancelada;
        solicitacaoDeReembolso.AtualizadaEmUtc = DateTime.UtcNow;
        
        await context.HistoricosStatusSolicitacao.AddAsync(new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacaoDeReembolso.Id,
            StatusAnterior = statusAnteriro,
            NovoStatus = solicitacaoDeReembolso.Status,
            AlteradoPorUsuarioId = colaboradorId,
            Motivo = cancelarRequestDto.Comentario
        });
        
        await context.SaveChangesAsync();
    }
}
