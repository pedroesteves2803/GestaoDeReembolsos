using GestaodeReembolsos.Models;

namespace GestaodeReembolsos.Repositories.Abstract;

public interface ISolicitacaoReembolsoRepository
{
    Task<Departamento?>  ObterPorId(int id);
}