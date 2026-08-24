using System.ComponentModel.DataAnnotations;
using GestaodeReembolsos.Validations;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class SolicitacaoReembolsoDto
{
    [NotEmptyGuid]
    [Required(ErrorMessage = "Informe o departamento")]
    public Guid DepartamentoId { get; set; }

    [Required(ErrorMessage = "Informe o mês de referência.")]
    public DateOnly MesReferencia { get; set; }
}
