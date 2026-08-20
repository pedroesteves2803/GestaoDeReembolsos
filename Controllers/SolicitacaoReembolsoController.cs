using System.Security.Claims;
using GestaodeReembolsos.Data;
using GestaodeReembolsos.Dtos.SolicitacaoReembolso;
using GestaodeReembolsos.Dtos.Shared;
using GestaodeReembolsos.Enums;
using GestaodeReembolsos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Controllers;

[ApiController]
public class ReimbursementRequestController: ControllerBase
{
    [Authorize(Roles = "Employee")]
    [HttpPost("api/v1/solicitacoes-reembolso")]
    public async Task<IActionResult> Create(
        [FromBody] SolicitacaoReembolsoDto dto,
        [FromServices] GestaoDeReembolsoContext context
        )
    {
        var departamento = await context
            .Departamentos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.DepartamentoId);
        
        if (departamento == null)
            return NotFound(new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                false,
                "Departamento não encontrado!"
            ));
        
        var identidadeUsuario = User.Identity as ClaimsIdentity;

        if (identidadeUsuario == null)
            return Unauthorized(
                new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "Token inválido."
                )
            );

        var solicitacao = new SolicitacaoReembolso
        {
            ColaboradorId = Guid.Parse(identidadeUsuario.FindFirst(ClaimTypes.NameIdentifier)!.Value),
            NumeroSolicitacao = $"REQ-{Guid.NewGuid():N}"[..20].ToUpperInvariant(),
            DepartamentoId = dto.DepartamentoId,
            MesReferencia = dto.MesReferencia,
            Status = StatusSolicitacaoReembolso.Rascunho,
            ValorTotal = 0
        };
        
        await context.SolicitacoesReembolso.AddAsync(solicitacao);

        await context.HistoricosStatusSolicitacao.AddAsync(new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacao.Id,
            StatusAnterior = null,
            NovoStatus = StatusSolicitacaoReembolso.Rascunho,
            AlteradoPorUsuarioId = solicitacao.ColaboradorId,
            Reason = null,
        });
        
        await context.SaveChangesAsync();
        
        return StatusCode(201,
            new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                true,
                "Solicitação de reembolso criada!",
                new SolicitacaoReembolsoResponseDto(
                    solicitacao.Id,
                    solicitacao.NumeroSolicitacao
                )
            )
        );
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("api/v1/solicitacoes-reembolso/{idSolicitacao}/itens")]
    public async Task<ActionResult> AddItem(
        [FromRoute] Guid idSolicitacao,
        [FromServices] GestaoDeReembolsoContext context, 
        [FromBody] ItemDespesaRequestDto dto
        )
    {
        if (idSolicitacao == Guid.Empty)
            return BadRequest(new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "O identificador da solicitação é inválido."
                )
            );
        
        var identidadeUsuario = User.Identity as ClaimsIdentity;
        
        var colaboradorId = Guid.Parse(
            identidadeUsuario!.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var request = await context
            .SolicitacoesReembolso
            .Where(x => x.Id == idSolicitacao)
            .Where(x => x.ColaboradorId == colaboradorId)
            .FirstOrDefaultAsync();

        if (request == null)
            return NotFound(
                new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "Esse reembolso informado não existe."
                )
            );

        if (request.Status != StatusSolicitacaoReembolso.Rascunho)
            return Conflict(
                new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "O status do reembolso não permite adicionar mais itens."
                )
            );

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        if (dto.DataDespesa > hoje)
            return BadRequest(
                new ApiResponseDto<ItemDespesaResponseDto>(
                    false,
                    "A data da despesa não pode ser futura."
                )
            );

        var dataMinimaPermitida = DateOnly.FromDateTime(request.CriadaEmUtc)
            .AddDays(-90);

        if (dto.DataDespesa < dataMinimaPermitida)
            return BadRequest(
                new ApiResponseDto<ItemDespesaResponseDto>(
                    false,
                    "A data da despesa não pode ser anterior a 90 dias da criação da solicitação."
                )
            );

        if (request.MesReferencia.Year != dto.DataDespesa.Year ||
            request.MesReferencia.Month != dto.DataDespesa.Month)
            return BadRequest(
                new ApiResponseDto<ItemDespesaResponseDto>(
                    false,
                    "A data da despesa deve pertencer ao mês de referência."
                )
            );

        var categoria = await context.CategoriasDespesa
            .Where(x => x.Id == dto.CategoriaDespesaId)
            .Where(x => x.Ativo == true)
            .FirstOrDefaultAsync();

        if (categoria == null)
            return NotFound(
                new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "Categoria não existe"
                )
            );
        
        var itemDespesa = new ItemDespesa
        {
            SolicitacaoReembolsoId = idSolicitacao,
            Valor = dto.Valor,
            Descricao = dto.Descricao,
            CategoriaDespesaId =  dto.CategoriaDespesaId,
            NomeEstabelecimento =  dto.NomeEstabelecimento,
            DataDespesa = dto.DataDespesa
        };
        
       await context.ItensDespesa.AddAsync(itemDespesa);

        request.ValorTotal += dto.Valor;
        
        await context.SaveChangesAsync();

        return StatusCode(201, new ApiResponseDto<ItemDespesaResponseDto>(
                true,
                "Item adicionado!",
                new ItemDespesaResponseDto(itemDespesa.Id)
            )
        );
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("api/v1/solicitacoes-reembolso/{idSolicitacao}/enviar")]
    public async Task<ActionResult> Submit(
        [FromRoute] Guid idSolicitacao,
        [FromServices] GestaoDeReembolsoContext context
        )
    {
        if (idSolicitacao == Guid.Empty)
            return BadRequest(new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "Id do reembolso inválido"
                )
            );
        
                
        var identidadeUsuario = User.Identity as ClaimsIdentity;
        
        var colaboradorId = Guid.Parse(
            identidadeUsuario!.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );
        
        var solicitacao = await context
            .SolicitacoesReembolso
            .Where(x => x.Id == idSolicitacao)
            .Where(x => x.ColaboradorId == colaboradorId)
            .FirstOrDefaultAsync();

        if (solicitacao == null)
            return NotFound(
                new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "Esse reembolso informado não existe."
                )
            );

        if (solicitacao.Status != StatusSolicitacaoReembolso.Rascunho)
            return Conflict(
                new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "O status do reembolso não permite enviar para aprovação."
                )
            );

        var existemItens = await context
            .ItensDespesa
            .AnyAsync(x => x.SolicitacaoReembolsoId == idSolicitacao);
        
        if (!existemItens)
            return Conflict(
                new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "Adicione pelo menos um item antes de enviar a solicitação."
                )
            );
        
        solicitacao.Status = StatusSolicitacaoReembolso.AguardandoAprovacaoGestor;
        solicitacao.EnviadaEmUtc = DateTime.UtcNow;
        solicitacao.AtualizadaEmUtc = DateTime.UtcNow;

        await context.HistoricosStatusSolicitacao.AddAsync(new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacao.Id,
            StatusAnterior = StatusSolicitacaoReembolso.Rascunho,
            NovoStatus = StatusSolicitacaoReembolso.AguardandoAprovacaoGestor,
            AlteradoPorUsuarioId = solicitacao.ColaboradorId,
            Reason = null,
        });
        
        await context.SaveChangesAsync();

        return Ok(new ApiResponseDto<EnvioSolicitacaoResponseDto>(
            true,
            "Enviado com sucesso!",
            new EnvioSolicitacaoResponseDto(
                solicitacao.Id,
                solicitacao.NumeroSolicitacao,
                solicitacao.Status,
                solicitacao.EnviadaEmUtc
                )
        ));
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("api/v1/solicitacoes-reembolso/{idSolicitacao}/decisao-gestor")]
    public async Task<ActionResult> Decisao(
        [FromRoute] Guid idSolicitacao,
        [FromBody] DecisaoGestorRequestDto dtoDecisao,
        [FromServices] GestaoDeReembolsoContext context
        )
    {
        
        if (idSolicitacao == Guid.Empty)
            return BadRequest(new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "Id do reembolso inválido"
                )
            );
        
        if (!Enum.IsDefined(dtoDecisao.Decisao))
        {
            return BadRequest(
                new ApiResponseDto<DecisaoGestorResponseDto>(
                    false,
                    "A decisão informada é inválida."
                )
            );
        }
        
        var identidadeUsuario = User.Identity as ClaimsIdentity;
        
        var gestorId = Guid.Parse(
            identidadeUsuario!.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var solicitacao = await context.SolicitacoesReembolso  
            .Where(x => x.Id == idSolicitacao)
            .FirstOrDefaultAsync();
        
        if (solicitacao == null)
            return NotFound(
                new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "A solicitação de reembolso não foi encontrada."
                )
            );
        
        if(solicitacao.Status != StatusSolicitacaoReembolso.AguardandoAprovacaoGestor)
            return Conflict(
                new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "A solicitação não está aguardando a aprovação do gestor."
                )
            );

        var usuario = await context.Usuarios
            .Where(x => x.Id == solicitacao.ColaboradorId)
            .Where(x => x.GestorId == gestorId)
            .FirstOrDefaultAsync();
         
         if(usuario == null)
             return NotFound(
                 new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                     false,
                     "Você não é o gestor responsável por esta solicitação."
                 )
             );

         if(dtoDecisao.Decisao == Enums.Decisao.Aprovada)
            solicitacao.Status = StatusSolicitacaoReembolso.PendingFinanceValidation;
         
         if(dtoDecisao.Decisao == Enums.Decisao.Rejeitada)
             solicitacao.Status = StatusSolicitacaoReembolso.RejectedByManager;

         if(dtoDecisao.Decisao == Enums.Decisao.Devolvida)
             solicitacao.Status = StatusSolicitacaoReembolso.ReturnedByManager;
         
         solicitacao.DecididaPeloGestorEmUtc = DateTime.UtcNow;
         solicitacao.AtualizadaEmUtc =  DateTime.UtcNow;
         
        var decisaoAprovacao = new DecisaoAprovacao
        {
            SolicitacaoReembolsoId = solicitacao.Id,
            DecididaPorUsuarioId = gestorId,
            Comentario = dtoDecisao.Comentario,
            Decisao = dtoDecisao.Decisao,
            NivelDecisao = NivelDecisao.Manager,
            CriadaEmUtc =  DateTime.UtcNow,
        };
        
        await context.DecisoesAprovacao.AddAsync(decisaoAprovacao);
        
        await context.HistoricosStatusSolicitacao.AddAsync(new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacao.Id,
            StatusAnterior = StatusSolicitacaoReembolso.AguardandoAprovacaoGestor,
            NovoStatus = solicitacao.Status,
            AlteradoPorUsuarioId = gestorId,
            Reason = null,
        });
        
        await context.SaveChangesAsync();
        
        return Ok(new ApiResponseDto<DecisaoGestorResponseDto>(
            true,
            "Decisão do gestor registrada com sucesso!",
            new DecisaoGestorResponseDto(
                solicitacao.Id,
                solicitacao.NumeroSolicitacao,
                decisaoAprovacao.Decisao,
                solicitacao.Status,
                solicitacao.DecididaPeloGestorEmUtc
            )
        ));
    }
}
