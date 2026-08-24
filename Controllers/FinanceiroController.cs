using System.Data.Common;
using GestaodeReembolsos.Dtos.Financeiro;
using GestaodeReembolsos.Dtos.Shared;
using GestaodeReembolsos.Exceptions;
using GestaodeReembolsos.Extensions;
using GestaodeReembolsos.Services.Financeiro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaodeReembolsos.Controllers;

[ApiController]
public class FinanceiroController(
    FinanceiroService financeiroService
    ) : ControllerBase
{
    [Authorize(Roles = "Finance,Admin")]
    [HttpPost("api/v1/solicitacoes-reembolso/{idSolicitacao}/decisao-financeiro")]
    public async Task<ActionResult> Decisao(
        Guid idSolicitacao,
        [FromBody] DecisaoFinanceiraRequestDto dto
        )
    {
        if (idSolicitacao == Guid.Empty)
            return BadRequest(new ApiResponseDto<DecisaoFinanceiroResponseDto>(
                    false,
                    "O identificador da solicitação é inválido."
                )
            );

        try
        {
            var decisao = await financeiroService.Decisao(
                idSolicitacao,
                User.ObterIdUsuario(),
                dto
            );
            
            return StatusCode(200, new ApiResponseDto<DecisaoFinanceiroResponseDto>(
                    true,
                    "Decisão financeira registrada com sucesso!",
                    new DecisaoFinanceiroResponseDto(decisao.Id)
                )
            );
            
        }
        catch (ExcecaoRegraNegocio excecao)
        {
            return StatusCode(
                excecao.StatusCode,
                new ApiResponseDto<DecisaoFinanceiroResponseDto>(
                    false,
                    excecao.Message
                )
            );
        }
        catch (DbException exception)
        {
            return StatusCode(
                StatusCodes.Status400BadRequest,
                exception.Message
            );
        }

    }

    [Authorize(Roles = "Finance,Admin")]
    [HttpPost("api/v1/solicitacoes-reembolso/{idSolicitacao}/pagamento")]
    public async Task<ActionResult> Pagamento(
        Guid idSolicitacao,
        [FromBody] RegistrarPagamentoRequestDto dto
        )
    {
        if (idSolicitacao == Guid.Empty)
            return BadRequest(new ApiResponseDto<DecisaoFinanceiroResponseDto>(
                    false,
                    "O identificador da solicitação é inválido."
                )
            );

        try
        {
            var pagamento = await financeiroService.Pagamento(
                idSolicitacao,
                User.ObterIdUsuario(),
                dto
                );
            
            return StatusCode(200, new ApiResponseDto<PagamentoResponseDto>(
                    true,
                    "Pagamento realizado com sucesso!",
                    new PagamentoResponseDto(pagamento.Id)
                )
            );        
        }
        catch (ExcecaoRegraNegocio excecao)
        {
            return StatusCode(
                excecao.StatusCode,
                new ApiResponseDto<PagamentoResponseDto>(
                    false,
                    excecao.Message
                )
            );
        }
        catch (DbException exception)
        {
            return StatusCode(
                StatusCodes.Status400BadRequest,
                exception.Message
            );
        }
    }
    
}