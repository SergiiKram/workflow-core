using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace WorkflowCore.Persistence.PostgreSQL.Migrations
{
    public partial class RandomUUIDs_to_sequential : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "Event",
                schema: "wfc",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v1()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubscriptionId",
                table: "Subscription",
                schema: "wfc",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v1()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "InstanceId",
                table: "Workflow",
                schema: "wfc",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v1()",
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "Event",
                schema: "wfc",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v1()");

            migrationBuilder.AlterColumn<Guid>(
                name: "InstanceId",
                table: "Subscription",
                schema: "wfc",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v1()");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubscriptionId",
                table: "Workflow",
                schema: "wfc",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v1()");

            migrationBuilder.Sql("DROP EXTENSION IF EXISTS \"uuid-ossp\";");
        }
    }
}
