namespace GestaodeReembolsos.Enums;

public enum StatusSolicitacaoReembolso
{
   Draft = 1, 
   PendingManagerApproval = 2, 
   RejectedByManager = 3, 
   ReturnedByManager = 4, 
   PendingFinanceValidation = 5, 
   RejectedByFinance = 6, 
   ReturnedByFinance = 7, 
   ApprovedForPayment = 8, 
   Paid = 9, 
   Cancelled = 10
}