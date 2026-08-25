using System.Data.Common;
using System.Security.Claims;
using GestaodeReembolsos.Dtos.SolicitacaoReembolso;
using GestaodeReembolsos.Dtos.Shared;
using GestaodeReembolsos.Exceptions;
using GestaodeReembolsos.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestaodeReembolsos.Services.SolicitacoesReembolso;
namespace GestaodeReembolsos.Controllers;

[ApiController]
public class SolicitacaoReembolsoController(
    SolicitacaoReembolsoService solicitacaoReembolsoService
    ): ControllerBase
{
    [Authorize(Roles = "Employee")]
    [HttpPost("api/v1/solicitacoes-reembolso")]
    public async Task<IActionResult> Criar(
        [FromBody] SolicitacaoReembolsoDto dto)
    {
        var identidadeUsuario = User.Identity as ClaimsIdentity;

        if (identidadeUsuario == null)
        {
            return Unauthorized(
                new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "Token inválido."
                )
            );
        }

        var colaboradorId = User.ObterIdUsuario();

        try
        {
            var solicitacao = await solicitacaoReembolsoService.Criar(
                colaboradorId,
                dto.DepartamentoId,
                dto.MesReferencia
            );

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    true,
                    "Solicitação de reembolso criada.",
                    new SolicitacaoReembolsoResponseDto(
                        solicitacao.Id,
                        solicitacao.NumeroSolicitacao
                    )
                )
            );
        }
        catch (ExcecaoRegraNegocio excecao)
        {
            return StatusCode(
                excecao.StatusCode,
                new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
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

    [Authorize(Roles = "Employee")]
    [HttpPost("api/v1/solicitacoes-reembolso/{idSolicitacao}/itens")]
    public async Task<ActionResult> AddItem(
        [FromRoute] Guid idSolicitacao,
        [FromBody] ItemDespesaRequestDto dto
        )
    {
        if (idSolicitacao == Guid.Empty)
            return BadRequest(new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "O identificador da solicitação é inválido."
                )
            );

        try
        {
            var itemDespesa = await solicitacaoReembolsoService.AdicionarItem(
                idSolicitacao,
                User.ObterIdUsuario(),
                dto
            );
            
            return StatusCode(201, new ApiResponseDto<ItemDespesaResponseDto>(
                    true,
                    "Item de despesa adicionado com sucesso.",
                    new ItemDespesaResponseDto(itemDespesa.Id)
                )
            );

        }
        catch (ExcecaoRegraNegocio excecao)
        {
            return StatusCode(
                excecao.StatusCode,
                new ApiResponseDto<ItemDespesaResponseDto>(
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

    [Authorize(Roles = "Employee")]
    [HttpPost("api/v1/solicitacoes-reembolso/{idSolicitacao}/enviar")]
    public async Task<ActionResult> Submit(
        [FromRoute] Guid idSolicitacao
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

        try
        {
            var solicitacaoReembolso = await solicitacaoReembolsoService.Enviar(idSolicitacao, colaboradorId);
            
            return Ok(new ApiResponseDto<EnvioSolicitacaoResponseDto>(
                true,
                "Solicitação enviada para aprovação do gestor com sucesso.",
                new EnvioSolicitacaoResponseDto(
                    solicitacaoReembolso.Id,
                    solicitacaoReembolso.NumeroSolicitacao,
                    solicitacaoReembolso.Status,
                    solicitacaoReembolso.EnviadaEmUtc)
            ));
        }
        catch (ExcecaoRegraNegocio excecao)
        {
            return StatusCode(
                excecao.StatusCode,
                new ApiResponseDto<EnvioSolicitacaoResponseDto>(
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

    [Authorize(Roles = "Manager")]
    [HttpPost("api/v1/solicitacoes-reembolso/{idSolicitacao}/decisao-gestor")]
    public async Task<ActionResult> Decisao(
        [FromRoute] Guid idSolicitacao,
        [FromBody] DecisaoGestorRequestDto dtoDecisao
        )
    {
        if (idSolicitacao == Guid.Empty)
            return BadRequest(new ApiResponseDto<SolicitacaoReembolsoResponseDto>(
                    false,
                    "O identificador da solicitação é inválido."
                )
            );
        
        var gestorId = User.ObterIdUsuario();

        try
        {
            var resultadoDecisaoGestor = await solicitacaoReembolsoService.Decisao(
                idSolicitacao,
                gestorId,
                dtoDecisao
                );
            
            return Ok(new ApiResponseDto<DecisaoGestorResponseDto>(
                true,
                "Decisão do gestor registrada com sucesso!",
                new DecisaoGestorResponseDto(
                    resultadoDecisaoGestor.Solicitacao.Id,
                    resultadoDecisaoGestor.Solicitacao.NumeroSolicitacao,
                    resultadoDecisaoGestor.Decisao.Decisao,
                    resultadoDecisaoGestor.Solicitacao.Status,
                    resultadoDecisaoGestor.Solicitacao.DecididaPeloGestorEmUtc
                )
            ));
        }
        catch (ExcecaoRegraNegocio excecao)
        {
            return StatusCode(
                excecao.StatusCode,
                new ApiResponseDto<DecisaoGestorResponseDto>(
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

    [Authorize]
    [HttpGet("/api/v1/solicitacoes-reembolso")]
    public async Task<ActionResult> Listar()
    {
         var solitacoesDeReembolso = await solicitacaoReembolsoService
             .ListarReembolsosPorUsuario(User.ObterIdUsuario());
         
         return Ok(new ApiResponseDto<IList<ListaSolicitacaoReembolsoResponseDto>>(
             true,
             "Solicitações encontradas com sucesso.",
             solitacoesDeReembolso
         ));
    }

    [Authorize]
    [HttpGet("/api/v1/solicitacoes-reembolso/{idSolicitacao}")]
    public async Task<ActionResult> ObterDetalhe(
        [FromRoute] Guid idSolicitacao
    ){
        try
        {
            var detalhe = await solicitacaoReembolsoService
                .ObterDetalhe(idSolicitacao, User.ObterIdUsuario());

            return Ok(new ApiResponseDto<DetalheSolicitacaoReembolsoResponseDto>(
                true,
                "Solicitação encontrada com sucesso.",
                detalhe
            ));
        }
        catch (ExcecaoRegraNegocio excecao)
        {
            return StatusCode(
                excecao.StatusCode,
                new ApiResponseDto<DetalheSolicitacaoReembolsoResponseDto>(
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
