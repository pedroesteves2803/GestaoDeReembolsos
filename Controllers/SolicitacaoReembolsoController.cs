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
    [HttpPost("api/v1/reimbursement-requests")]
    public async Task<IActionResult> Create(
        [FromBody] ReimbursementRequestDto dto,
        [FromServices] GestaoDeReembolsoContext context
        )
    {
        var departamento = await context
            .Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.DepartamentoId);
        
        if (departamento == null)
            return NotFound(new ApiResponseDto<ReimbursementRequestResponseDto>(
                false,
                "Departamento não encontrado!"
            ));
        
        var identidadeUsuario = User.Identity as ClaimsIdentity;

        if (identidadeUsuario == null)
            return Unauthorized(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
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
            Status = StatusSolicitacaoReembolso.Draft,
            ValorTotal = 0
        };
        
        await context.ReimbursementRequests.AddAsync(solicitacao);

        await context.RequestStatusHistories.AddAsync(new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacao.Id,
            StatusAnterior = null,
            NovoStatus = StatusSolicitacaoReembolso.Draft,
            AlteradoPorUsuarioId = solicitacao.ColaboradorId,
            Reason = null,
        });
        
        await context.SaveChangesAsync();
        
        return StatusCode(201,
            new ApiResponseDto<ReimbursementRequestResponseDto>(
                true,
                "Solicitação de reembolso criada!",
                new ReimbursementRequestResponseDto(
                    solicitacao.Id,
                    solicitacao.NumeroSolicitacao
                )
            )
        );
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("/api/v1/reimbursement-requests/{idSolicitacao}/items")]
    public async Task<ActionResult> AddItem(
        [FromRoute] Guid idSolicitacao,
        [FromServices] GestaoDeReembolsoContext context, 
        [FromBody] ExpenseItemRequestDto dto
        )
    {
        if (idSolicitacao == Guid.Empty)
            return BadRequest(new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "O identificador da solicitação é inválido."
                )
            );
        
        var identidadeUsuario = User.Identity as ClaimsIdentity;
        
        var colaboradorId = Guid.Parse(
            identidadeUsuario!.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var request = await context
            .ReimbursementRequests
            .Where(x => x.Id == idSolicitacao)
            .Where(x => x.ColaboradorId == colaboradorId)
            .FirstOrDefaultAsync();

        if (request == null)
            return NotFound(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "Esse reembolso informado não existe."
                )
            );

        if (request.Status != StatusSolicitacaoReembolso.Draft)
            return Conflict(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "O status do reembolso não permite adicionar mais itens."
                )
            );

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        if (dto.DataDespesa > hoje)
            return BadRequest(
                new ApiResponseDto<ExpenseItemResponseDto>(
                    false,
                    "A data da despesa não pode ser futura."
                )
            );

        var dataMinimaPermitida = DateOnly.FromDateTime(request.CriadaEmUtc)
            .AddDays(-90);

        if (dto.DataDespesa < dataMinimaPermitida)
            return BadRequest(
                new ApiResponseDto<ExpenseItemResponseDto>(
                    false,
                    "A data da despesa não pode ser anterior a 90 dias da criação da solicitação."
                )
            );

        if (request.MesReferencia.Year != dto.DataDespesa.Year ||
            request.MesReferencia.Month != dto.DataDespesa.Month)
            return BadRequest(
                new ApiResponseDto<ExpenseItemResponseDto>(
                    false,
                    "A data da despesa deve pertencer ao mês de referência."
                )
            );

        var categoria = await context.ExpenseCategories
            .Where(x => x.Id == dto.CategoriaDespesaId)
            .Where(x => x.Ativo == true)
            .FirstOrDefaultAsync();

        if (categoria == null)
            return NotFound(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
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
        
       await context.ExpenseItems.AddAsync(itemDespesa);

        request.ValorTotal += dto.Valor;
        
        await context.SaveChangesAsync();

        return StatusCode(201, new ApiResponseDto<ExpenseItemResponseDto>(
                true,
                "Item adicionado!",
                new ExpenseItemResponseDto(itemDespesa.Id)
            )
        );
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("/api/v1/reimbursement-requests/{idSolicitacao}/submit")]
    public async Task<ActionResult> Submit(
        [FromRoute] Guid idSolicitacao,
        [FromServices] GestaoDeReembolsoContext context
        )
    {
        if (idSolicitacao == Guid.Empty)
            return BadRequest(new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "Id do reembolso inválido"
                )
            );
        
                
        var identidadeUsuario = User.Identity as ClaimsIdentity;
        
        var colaboradorId = Guid.Parse(
            identidadeUsuario!.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );
        
        var solicitacao = await context
            .ReimbursementRequests
            .Where(x => x.Id == idSolicitacao)
            .Where(x => x.ColaboradorId == colaboradorId)
            .FirstOrDefaultAsync();

        if (solicitacao == null)
            return NotFound(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "Esse reembolso informado não existe."
                )
            );

        if (solicitacao.Status != StatusSolicitacaoReembolso.Draft)
            return Conflict(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "O status do reembolso não permite enviar para aprovação."
                )
            );

        var existemItens = await context
            .ExpenseItems
            .AnyAsync(x => x.SolicitacaoReembolsoId == idSolicitacao);
        
        if (!existemItens)
            return Conflict(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "Adicione pelo menos um item antes de enviar a solicitação."
                )
            );
        
        solicitacao.Status = StatusSolicitacaoReembolso.PendingManagerApproval;
        solicitacao.EnviadaEmUtc = DateTime.UtcNow;
        solicitacao.AtualizadaEmUtc = DateTime.UtcNow;

        await context.RequestStatusHistories.AddAsync(new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacao.Id,
            StatusAnterior = StatusSolicitacaoReembolso.Draft,
            NovoStatus = StatusSolicitacaoReembolso.PendingManagerApproval,
            AlteradoPorUsuarioId = solicitacao.ColaboradorId,
            Reason = null,
        });
        
        await context.SaveChangesAsync();

        return Ok(new ApiResponseDto<SubmitResponseDto>(
            true,
            "Enviado com sucesso!",
            new SubmitResponseDto(
                solicitacao.Id,
                solicitacao.NumeroSolicitacao,
                solicitacao.Status,
                solicitacao.EnviadaEmUtc
                )
        ));
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("/api/v1/reimbursement-requests/{idSolicitacao}/manager-decision")]
    public async Task<ActionResult> Decisao(
        [FromRoute] Guid idSolicitacao,
        [FromBody] DecisionManagerRequestDto reimbursementRequestDto,
        [FromServices] GestaoDeReembolsoContext context
        )
    {
        
        if (idSolicitacao == Guid.Empty)
            return BadRequest(new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "Id do reembolso inválido"
                )
            );
        
        if (!Enum.IsDefined(reimbursementRequestDto.Decisao))
        {
            return BadRequest(
                new ApiResponseDto<DecisionManagerResponseDto>(
                    false,
                    "A decisão informada é inválida."
                )
            );
        }
        
        var identidadeUsuario = User.Identity as ClaimsIdentity;
        
        var gestorId = Guid.Parse(
            identidadeUsuario!.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var solicitacao = await context.ReimbursementRequests  
            .Where(x => x.Id == idSolicitacao)
            .FirstOrDefaultAsync();
        
        if (solicitacao == null)
            return NotFound(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "A solicitação de reembolso não foi encontrada."
                )
            );
        
        if(solicitacao.Status != StatusSolicitacaoReembolso.PendingManagerApproval)
            return Conflict(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "A solicitação não está aguardando a aprovação do gestor."
                )
            );

        var usuario = await context.Users
            .Where(x => x.Id == solicitacao.ColaboradorId)
            .Where(x => x.GestorId == gestorId)
            .FirstOrDefaultAsync();
         
         if(usuario == null)
             return NotFound(
                 new ApiResponseDto<ReimbursementRequestResponseDto>(
                     false,
                     "Você não é o gestor responsável por esta solicitação."
                 )
             );

         if(reimbursementRequestDto.Decisao == Enums.Decisao.Approved)
            solicitacao.Status = StatusSolicitacaoReembolso.PendingFinanceValidation;
         
         if(reimbursementRequestDto.Decisao == Enums.Decisao.Rejected)
             solicitacao.Status = StatusSolicitacaoReembolso.RejectedByManager;

         if(reimbursementRequestDto.Decisao == Enums.Decisao.Returned)
             solicitacao.Status = StatusSolicitacaoReembolso.ReturnedByManager;
         
         solicitacao.DecididaPeloGestorEmUtc = DateTime.UtcNow;
         solicitacao.AtualizadaEmUtc =  DateTime.UtcNow;
         
        var decisaoAprovacao = new DecisaoAprovacao
        {
            SolicitacaoReembolsoId = solicitacao.Id,
            DecididaPorUsuarioId = gestorId,
            Comentario =  reimbursementRequestDto.Comentario,
            Decisao =  reimbursementRequestDto.Decisao,
            NivelDecisao = NivelDecisao.Manager,
            CriadaEmUtc =  DateTime.UtcNow,
        };
        
        await context.ApprovalDecisions.AddAsync(decisaoAprovacao);
        
        await context.RequestStatusHistories.AddAsync(new HistoricoStatusSolicitacao
        {
            SolicitacaoReembolsoId = solicitacao.Id,
            StatusAnterior = StatusSolicitacaoReembolso.PendingManagerApproval,
            NovoStatus = solicitacao.Status,
            AlteradoPorUsuarioId = gestorId,
            Reason = null,
        });
        
        await context.SaveChangesAsync();
        
        return Ok(new ApiResponseDto<DecisionManagerResponseDto>(
            true,
            "Decisão do gestor registrada com sucesso!",
            new DecisionManagerResponseDto(
                solicitacao.Id,
                solicitacao.NumeroSolicitacao,
                decisaoAprovacao.Decisao,
                solicitacao.Status,
                solicitacao.DecididaPeloGestorEmUtc
            )
        ));
    }
}
