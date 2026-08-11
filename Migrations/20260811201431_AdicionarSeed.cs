using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestaodeReembolsos.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {   
            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "CostCenterCode", "CreatedAtUtc", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("6f468dfa-4cb8-46a9-a6fd-4e333787e202"), "COM-200", new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, "Comercial" },
                    { new Guid("c3a3b4d8-28c3-4d72-a479-55c39a5bd101"), "TI-100", new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, "Tecnologia" },
                    { new Guid("e154d747-f905-4bea-a32d-13bcc991c303"), "FIN-300", new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, "Financeiro" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAtUtc", "DepartmentId", "Email", "FullName", "IsActive", "ManagerId", "PasswordHash", "Role", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("00bf84cc-22dc-401b-a1ec-a8d523687105"), new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("e154d747-f905-4bea-a32d-13bcc991c303"), "fernanda.rocha@empresa.test", "Fernanda Rocha", true, null, "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W", "Finance", null },
                    { new Guid("9f586795-fc0f-4b35-b251-48cbab902401"), new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("c3a3b4d8-28c3-4d72-a479-55c39a5bd101"), "carlos.lima@empresa.test", "Carlos Lima", true, null, "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W", "Manager", null },
                    { new Guid("ad9bf52d-1a99-4f03-a64e-65aee12f2502"), new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("6f468dfa-4cb8-46a9-a6fd-4e333787e202"), "marina.costa@empresa.test", "Marina Costa", true, null, "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W", "Manager", null },
                    { new Guid("c95ab38b-5c3e-45a4-a087-c930bea18e06"), new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("e154d747-f905-4bea-a32d-13bcc991c303"), "admin@empresa.test", "Admin Sistema", true, null, "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W", "Admin", null },
                    { new Guid("d62bdf0d-2268-4c5f-b6ef-d7d9f62d3603"), new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("c3a3b4d8-28c3-4d72-a479-55c39a5bd101"), "ana.souza@empresa.test", "Ana Souza", true, new Guid("9f586795-fc0f-4b35-b251-48cbab902401"), "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W", "Employee", null },
                    { new Guid("fbb62499-5f39-4e04-98fe-5762a2c15704"), new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("6f468dfa-4cb8-46a9-a6fd-4e333787e202"), "bruno.alves@empresa.test", "Bruno Alves", true, new Guid("ad9bf52d-1a99-4f03-a64e-65aee12f2502"), "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W", "Employee", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00bf84cc-22dc-401b-a1ec-a8d523687105"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c95ab38b-5c3e-45a4-a087-c930bea18e06"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d62bdf0d-2268-4c5f-b6ef-d7d9f62d3603"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("fbb62499-5f39-4e04-98fe-5762a2c15704"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("e154d747-f905-4bea-a32d-13bcc991c303"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("9f586795-fc0f-4b35-b251-48cbab902401"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ad9bf52d-1a99-4f03-a64e-65aee12f2502"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("6f468dfa-4cb8-46a9-a6fd-4e333787e202"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("c3a3b4d8-28c3-4d72-a479-55c39a5bd101"));
        }
    }
}
