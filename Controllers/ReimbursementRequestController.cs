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
}