using GestaodeReembolsos.Models;

namespace GestaodeReembolsos.Services.SolicitacaoReembolso;

public record ResultadoDecisaoGestor(
    Models.SolicitacaoReembolso Solicitacao,
    DecisaoAprovacao Decisao
);