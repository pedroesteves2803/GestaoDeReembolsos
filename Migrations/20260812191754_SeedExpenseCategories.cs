using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestaodeReembolsos.Migrations
{
    /// <inheritdoc />
    public partial class SeedExpenseCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "Id", "IsActive", "MonthlyLimit", "Name", "RequiresReceipt" },
                values: new object[,]
                {
                    { new Guid("1d7d9f1c-cba5-488d-a718-5102401ee001"), true, 800m, "Alimentação", true },
                    { new Guid("413b783c-2030-40a8-8a09-a03ea12cd002"), true, 600m, "Transporte", true },
                    { new Guid("59f8b4b9-e72b-4dbd-955b-cb67e940e003"), true, 3000m, "Hospedagem", true }
                });

            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "Id", "IsActive", "MonthlyLimit", "Name" },
                values: new object[] { new Guid("a80f04df-615f-4f0c-8efc-44f39543f005"), true, 1000m, "Quilometragem" });

            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "Id", "IsActive", "MonthlyLimit", "Name", "RequiresReceipt" },
                values: new object[,]
                {
                    { new Guid("bff56b5b-29d8-40f4-8e32-f9a4590d3006"), true, 300m, "Outros", true },
                    { new Guid("f741aa00-84e1-4a02-a5e4-249268fee004"), true, 500m, "Material de escritório", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "Id",
                keyValue: new Guid("1d7d9f1c-cba5-488d-a718-5102401ee001"));

            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "Id",
                keyValue: new Guid("413b783c-2030-40a8-8a09-a03ea12cd002"));

            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "Id",
                keyValue: new Guid("59f8b4b9-e72b-4dbd-955b-cb67e940e003"));

            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "Id",
                keyValue: new Guid("a80f04df-615f-4f0c-8efc-44f39543f005"));

            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "Id",
                keyValue: new Guid("bff56b5b-29d8-40f4-8e32-f9a4590d3006"));

            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "Id",
                keyValue: new Guid("f741aa00-84e1-4a02-a5e4-249268fee004"));
        }
    }
}
