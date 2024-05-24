using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FER.InfSus.Time.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixHistoryLogForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItemHistoryLogs_TaskItems_TaskItemId",
                table: "TaskItemHistoryLogs");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "TaskItemHistoryLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskItemId",
                table: "TaskItemHistoryLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemHistoryLogs_TaskItems_TaskItemId",
                table: "TaskItemHistoryLogs",
                column: "TaskItemId",
                principalTable: "TaskItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItemHistoryLogs_TaskItems_TaskItemId",
                table: "TaskItemHistoryLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskItemId",
                table: "TaskItemHistoryLogs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                table: "TaskItemHistoryLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemHistoryLogs_TaskItems_TaskItemId",
                table: "TaskItemHistoryLogs",
                column: "TaskItemId",
                principalTable: "TaskItems",
                principalColumn: "Id");
        }
    }
}
