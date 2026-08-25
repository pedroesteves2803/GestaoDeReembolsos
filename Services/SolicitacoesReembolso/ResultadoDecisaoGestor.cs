using GestaodeReembolsos.Models;

namespace GestaodeReembolsos.Services.SolicitacoesReembolso;

public record ResultadoDecisaoGestor(
    SolicitacaoReembolso Solicitacao,
    DecisaoAprovacao Decisao
);
