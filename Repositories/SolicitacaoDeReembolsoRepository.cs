using GestaodeReembolsos.Data;
using GestaodeReembolsos.Models;
using GestaodeReembolsos.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Repositories;

public class SolicitacaoDeReembolsoRepository(GestaoDeReembolsoContext context) : ISolicitacaoReembolsoRepository
{
    public async Task<SolicitacaoReembolso> Criar(SolicitacaoReembolso solicitacaoReembolso)
    {
        await context.SolicitacoesReembolso.AddAsync(solicitacaoReembolso);
        await context.SaveChangesAsync();

        return solicitacaoReembolso;
    }
}