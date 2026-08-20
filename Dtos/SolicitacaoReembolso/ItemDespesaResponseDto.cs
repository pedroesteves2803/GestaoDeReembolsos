using System.ComponentModel.DataAnnotations;
using GestaodeReembolsos.Validations;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public partial class ItemDespesaResponseDto(
    Guid id
)
{
    public Guid Id { get; set; } = id;
}