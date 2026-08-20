using System.Security.Claims;
using GestaodeReembolsos.Data;
using GestaodeReembolsos.Dtos.ReimbursementRequest;
using GestaodeReembolsos.Dtos.Shared;
using GestaodeReembolsos.Enums;
using GestaodeReembolsos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Controllers;

[ApiController]
public class ReimbursementRequestController: ControllerBase
{
    [Authorize(Roles = "Employee")]
    [HttpPost("api/v1/reimbursement-requests")]
    public async Task<IActionResult> Create(
        [FromBody] ReimbursementRequestDto requestDto,
        [FromServices] GestaoDeReembolsoContext context
        )
    {
        var department = await context
            .Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == requestDto.DepartmentId);
        
        if (department == null)
            return NotFound(new ApiResponseDto<ReimbursementRequestResponseDto>(
                false,
                "Departamento não encontrado!"
            ));
        
        var userIdentity = User.Identity as ClaimsIdentity;

        if (userIdentity == null)
            return Unauthorized(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "Token inválido."
                )
            );

        var reimbursementRequest = new ReimbursementRequest
        {
            EmployeeId = Guid.Parse(userIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value),
            RequestNumber = $"REQ-{Guid.NewGuid():N}"[..20].ToUpperInvariant(),
            DepartmentId = requestDto.DepartmentId,
            ReferenceMonth = requestDto.ReferenceMonth,
            Status = ReimbursementRequestEnum.Draft,
            TotalAmount = 0
        };
        
        await context.ReimbursementRequests.AddAsync(reimbursementRequest);

        await context.RequestStatusHistories.AddAsync(new RequestStatusHistory
        {
            ReimbursementRequestId = reimbursementRequest.Id,
            PreviousStatus = null,
            NewStatus = ReimbursementRequestEnum.Draft,
            ChangedByUserId = reimbursementRequest.EmployeeId,
            Reason = null,
        });
        
        await context.SaveChangesAsync();
        
        return StatusCode(201,
            new ApiResponseDto<ReimbursementRequestResponseDto>(
                true,
                "Solicitação de reembolso criada!",
                new ReimbursementRequestResponseDto(
                    reimbursementRequest.Id,
                    reimbursementRequest.RequestNumber
                )
            )
        );
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("/api/v1/reimbursement-requests/{requestId}/items")]
    public async Task<ActionResult> AddItem(
        [FromRoute] Guid requestId,
        [FromServices] GestaoDeReembolsoContext context, 
        [FromBody] ExpenseItemRequestDto requestDto
        )
    {
        if (requestId == Guid.Empty)
            return BadRequest(new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "O identificador da solicitação é inválido."
                )
            );
        
        var userIdentity = User.Identity as ClaimsIdentity;
        
        var employeeId = Guid.Parse(
            userIdentity!.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var request = await context
            .ReimbursementRequests
            .Where(x => x.Id == requestId)
            .Where(x => x.EmployeeId == employeeId)
            .FirstOrDefaultAsync();

        if (request == null)
            return NotFound(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "Esse reembolso informado não existe."
                )
            );

        if (request.Status != ReimbursementRequestEnum.Draft)
            return Conflict(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "O status do reembolso não permite adicionar mais itens."
                )
            );

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (requestDto.ExpenseDate > today)
            return BadRequest(
                new ApiResponseDto<ExpenseItemResponseDto>(
                    false,
                    "A data da despesa não pode ser futura."
                )
            );

        var oldestAllowedDate = DateOnly.FromDateTime(request.CreatedAtUtc)
            .AddDays(-90);

        if (requestDto.ExpenseDate < oldestAllowedDate)
            return BadRequest(
                new ApiResponseDto<ExpenseItemResponseDto>(
                    false,
                    "A data da despesa não pode ser anterior a 90 dias da criação da solicitação."
                )
            );

        if (request.ReferenceMonth.Year != requestDto.ExpenseDate.Year ||
            request.ReferenceMonth.Month != requestDto.ExpenseDate.Month)
            return BadRequest(
                new ApiResponseDto<ExpenseItemResponseDto>(
                    false,
                    "A data da despesa deve pertencer ao mês de referência."
                )
            );

        var category = await context.ExpenseCategories
            .Where(x => x.Id == requestDto.ExpenseCategoryId)
            .Where(x => x.IsActive == true)
            .FirstOrDefaultAsync();

        if (category == null)
            return NotFound(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "Categoria não existe"
                )
            );
        
        var expenseItem = new ExpenseItem
        {
            ReimbursementRequestId = requestId,
            Amount = requestDto.Amount,
            Description = requestDto.Description,
            ExpenseCategoryId =  requestDto.ExpenseCategoryId,
            MerchantName =  requestDto.MerchantName,
            ExpenseDate = requestDto.ExpenseDate
        };
        
       await context.ExpenseItems.AddAsync(expenseItem);

        request.TotalAmount += requestDto.Amount;
        
        await context.SaveChangesAsync();

        return StatusCode(201, new ApiResponseDto<ExpenseItemResponseDto>(
                true,
                "Item adicionado!",
                new ExpenseItemResponseDto(expenseItem.Id)
            )
        );
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("/api/v1/reimbursement-requests/{requestId}/submit")]
    public async Task<ActionResult> Submit(
        [FromRoute] Guid requestId,
        [FromServices] GestaoDeReembolsoContext context
        )
    {
        if (requestId == Guid.Empty)
            return BadRequest(new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "Id do reembolso inválido"
                )
            );
        
                
        var userIdentity = User.Identity as ClaimsIdentity;
        
        var employeeId = Guid.Parse(
            userIdentity!.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );
        
        var reimbursementRequest = await context
            .ReimbursementRequests
            .Where(x => x.Id == requestId)
            .Where(x => x.EmployeeId == employeeId)
            .FirstOrDefaultAsync();

        if (reimbursementRequest == null)
            return NotFound(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "Esse reembolso informado não existe."
                )
            );

        if (reimbursementRequest.Status != ReimbursementRequestEnum.Draft)
            return Conflict(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "O status do reembolso não permite enviar para aprovação."
                )
            );

        var existItems = await context
            .ExpenseItems
            .AnyAsync(x => x.ReimbursementRequestId == requestId);
        
        if (!existItems)
            return Conflict(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "Adicione pelo menos um item antes de enviar a solicitação."
                )
            );
        
        reimbursementRequest.Status = ReimbursementRequestEnum.PendingManagerApproval;
        reimbursementRequest.SubmittedAtUtc = DateTime.UtcNow;
        reimbursementRequest.UpdatedAtUtc = DateTime.UtcNow;

        await context.RequestStatusHistories.AddAsync(new RequestStatusHistory
        {
            ReimbursementRequestId = reimbursementRequest.Id,
            PreviousStatus = ReimbursementRequestEnum.Draft,
            NewStatus = ReimbursementRequestEnum.PendingManagerApproval,
            ChangedByUserId = reimbursementRequest.EmployeeId,
            Reason = null,
        });
        
        await context.SaveChangesAsync();

        return Ok(new ApiResponseDto<SubmitResponseDto>(
            true,
            "Enviado com sucesso!",
            new SubmitResponseDto(
                reimbursementRequest.Id,
                reimbursementRequest.RequestNumber,
                reimbursementRequest.Status,
                reimbursementRequest.SubmittedAtUtc
                )
        ));
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("/api/v1/reimbursement-requests/{requestId}/manager-decision")]
    public async Task<ActionResult> Decision(
        [FromRoute] Guid requestId,
        [FromBody] DecisionManagerRequestDto reimbursementRequestDto,
        [FromServices] GestaoDeReembolsoContext context
        )
    {
        
        if (requestId == Guid.Empty)
            return BadRequest(new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "Id do reembolso inválido"
                )
            );
        
        if (!Enum.IsDefined(reimbursementRequestDto.Decision))
        {
            return BadRequest(
                new ApiResponseDto<DecisionManagerResponseDto>(
                    false,
                    "A decisão informada é inválida."
                )
            );
        }
        
        var userIdentity = User.Identity as ClaimsIdentity;
        
        var managerId = Guid.Parse(
            userIdentity!.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var reimbursementRequest = await context.ReimbursementRequests  
            .Where(x => x.Id == requestId)
            .FirstOrDefaultAsync();
        
        if (reimbursementRequest == null)
            return NotFound(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "A solicitação de reembolso não foi encontrada."
                )
            );
        
        if(reimbursementRequest.Status != ReimbursementRequestEnum.PendingManagerApproval)
            return Conflict(
                new ApiResponseDto<ReimbursementRequestResponseDto>(
                    false,
                    "A solicitação não está aguardando a aprovação do gestor."
                )
            );

        var user = await context.Users
            .Where(x => x.Id == reimbursementRequest.EmployeeId)
            .Where(x => x.ManagerId == managerId)
            .FirstOrDefaultAsync();
         
         if(user == null)
             return NotFound(
                 new ApiResponseDto<ReimbursementRequestResponseDto>(
                     false,
                     "Você não é o gestor responsável por esta solicitação."
                 )
             );

         if(reimbursementRequestDto.Decision == Enums.Decision.Approved)
            reimbursementRequest.Status = ReimbursementRequestEnum.PendingFinanceValidation;
         
         if(reimbursementRequestDto.Decision == Enums.Decision.Rejected)
             reimbursementRequest.Status = ReimbursementRequestEnum.RejectedByManager;

         if(reimbursementRequestDto.Decision == Enums.Decision.Returned)
             reimbursementRequest.Status = ReimbursementRequestEnum.ReturnedByManager;
         
         reimbursementRequest.ManagerDecisionAtUtc = DateTime.UtcNow;
         reimbursementRequest.UpdatedAtUtc =  DateTime.UtcNow;
         
        var approvalDecision = new ApprovalDecision
        {
            ReimbursementRequestId = reimbursementRequest.Id,
            DecidedByUserId = managerId,
            Comment =  reimbursementRequestDto.Comment,
            Decision =  reimbursementRequestDto.Decision,
            DecisionLevel = DecisionLevel.Manager,
            CreatedAtUtc =  DateTime.UtcNow,
        };
        
        await context.ApprovalDecisions.AddAsync(approvalDecision);
        
        await context.RequestStatusHistories.AddAsync(new RequestStatusHistory
        {
            ReimbursementRequestId = reimbursementRequest.Id,
            PreviousStatus = ReimbursementRequestEnum.PendingManagerApproval,
            NewStatus = reimbursementRequest.Status,
            ChangedByUserId = managerId,
            Reason = null,
        });
        
        await context.SaveChangesAsync();
        
        return Ok(new ApiResponseDto<DecisionManagerResponseDto>(
            true,
            "Decisão do gestor registrada com sucesso!",
            new DecisionManagerResponseDto(
                reimbursementRequest.Id,
                reimbursementRequest.RequestNumber,
                approvalDecision.Decision,
                reimbursementRequest.Status,
                reimbursementRequest.ManagerDecisionAtUtc
            )
        ));
    }
}
