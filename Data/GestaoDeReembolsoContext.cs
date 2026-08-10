using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Data;

public class GestaoDeReembolsoContext(DbContextOptions<GestaoDeReembolsoContext> options) 
    : DbContext(options)
{
    
}