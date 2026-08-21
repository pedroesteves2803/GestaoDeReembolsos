using GestaodeReembolsos.Models;

namespace GestaodeReembolsos.Repositories.Abstract;

public interface IDepartamentoRepository
{
    Task<Departamento?> ObterPorId(Guid id);
}