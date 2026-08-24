namespace GestaodeReembolsos.Enums;

public enum StatusSolicitacaoReembolso
{
    Rascunho = 1,
    AguardandoAprovacaoGestor = 2,
    RejeitadaPeloGestor = 3,
    DevolvidaPeloGestor = 4,
    AguardandoValidacaoFinanceira = 5,
    RejeitadaPeloFinanceiro = 6,
    DevolvidaPeloFinanceiro = 7,
    AprovadaParaPagamento = 8,
    Paga = 9,
    Cancelada = 10
}
