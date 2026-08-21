using GestaodeReembolsos.Data;
using GestaodeReembolsos.Models;
using GestaodeReembolsos.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Repositories;

public class DepartamentoRepository(GestaoDeReembolsoContext context) : IDepartamentoRepository
{
    public async Task<Departamento?> ObterPorId(Guid id)
    {
        return await context
            .Departamentos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);    
    }
}