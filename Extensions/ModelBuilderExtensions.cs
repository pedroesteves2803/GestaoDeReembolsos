using GestaodeReembolsos.Enums;
using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Extensions;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder builder)
    {
        const string senha = "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W";

        var technologyId = Guid.Parse("c3a3b4d8-28c3-4d72-a479-55c39a5bd101");
        var commercialId = Guid.Parse("6f468dfa-4cb8-46a9-a6fd-4e333787e202");
        var financeId = Guid.Parse("e154d747-f905-4bea-a32d-13bcc991c303");
        var carlosManagerId = Guid.Parse("9f586795-fc0f-4b35-b251-48cbab902401");
        var marinaManagerId = Guid.Parse("ad9bf52d-1a99-4f03-a64e-65aee12f2502");
        var anaId = Guid.Parse("d62bdf0d-2268-4c5f-b6ef-d7d9f62d3603");
        var brunoId = Guid.Parse("fbb62499-5f39-4e04-98fe-5762a2c15704");
        var fernandaId = Guid.Parse("00bf84cc-22dc-401b-a1ec-a8d523687105");
        var adminId = Guid.Parse("c95ab38b-5c3e-45a4-a087-c930bea18e06");
        var seedCreatedAtUtc = new DateTime(2026, 8, 11, 0, 0, 0, DateTimeKind.Utc);

        builder.Entity<Department>().HasData(
            new Department
            {
                Id = technologyId,
                Name = "Tecnologia",
                CostCenterCode = "TI-100",
                IsActive = true,
                CreatedAtUtc = seedCreatedAtUtc
            },
            new Department
            {
                Id = commercialId,
                Name = "Comercial",
                CostCenterCode = "COM-200",
                IsActive = true,
                CreatedAtUtc = seedCreatedAtUtc
            },
            new Department
            {
                Id = financeId,
                Name = "Financeiro",
                CostCenterCode = "FIN-300",
                IsActive = true,
                CreatedAtUtc = seedCreatedAtUtc
            });

        //maneger tecnologia
        builder.Entity<User>().HasData(new User
        {
            Id = carlosManagerId,
            FullName = "Carlos Lima",
            Email = "carlos.lima@empresa.test",
            Role = Role.Manager,
            DepartmentId = technologyId,
            PasswordHash = senha,
            IsActive = true,
            CreatedAtUtc = seedCreatedAtUtc,
        });

        //maneger comercial
        builder.Entity<User>().HasData(new User
        {
            Id = marinaManagerId,
            FullName = "Marina Costa",
            Email = "marina.costa@empresa.test",
            Role = Role.Manager,
            DepartmentId = commercialId,
            PasswordHash =  senha,
            IsActive = true,
            CreatedAtUtc = seedCreatedAtUtc,
        });

        builder.Entity<User>().HasData(new User
        {
            Id = anaId,
            FullName = "Ana Souza",
            Email = "ana.souza@empresa.test",
            Role = Role.Employee,
            DepartmentId = technologyId,
            ManagerId = carlosManagerId,
            PasswordHash = senha,
            IsActive = true,
            CreatedAtUtc = seedCreatedAtUtc,
        });

        builder.Entity<User>().HasData(new User
        {
            Id = brunoId,
            FullName = "Bruno Alves",
            Email = "bruno.alves@empresa.test",
            Role = Role.Employee,
            DepartmentId = commercialId,
            ManagerId = marinaManagerId,
            PasswordHash = senha,
            IsActive = true,
            CreatedAtUtc = seedCreatedAtUtc,
        });

        builder.Entity<User>().HasData(new User
        {
            Id = fernandaId,
            FullName = "Fernanda Rocha",
            Email = "fernanda.rocha@empresa.test",
            Role = Role.Finance,
            DepartmentId = financeId,
            PasswordHash = senha,
            IsActive = true,
            CreatedAtUtc = seedCreatedAtUtc,
        });

        builder.Entity<User>().HasData(new User
        {
            Id = adminId,
            FullName = "Admin Sistema",
            Email = "admin@empresa.test",
            Role = Role.Admin,
            DepartmentId = financeId,
            PasswordHash = senha,
            IsActive = true,
            CreatedAtUtc = seedCreatedAtUtc
        });
    }
}
