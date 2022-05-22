using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkflowCore.Persistence.PostgreSQL.Migrations
{
    public partial class IdempotentReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Workflow_Reference",
                schema: "wfc",
                table: "Workflow",
                column: "Reference",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workflow_Reference",
                schema: "wfc",
                table: "Workflow");
        }
    }
}
