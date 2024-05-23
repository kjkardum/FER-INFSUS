using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FER.InfSus.Time.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskItemStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "TaskItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "TaskItems");
        }
    }
}
