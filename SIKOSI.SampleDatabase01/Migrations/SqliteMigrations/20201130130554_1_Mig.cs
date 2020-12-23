using Microsoft.EntityFrameworkCore.Migrations;

namespace SIKOSI.SampleDatabase01.Migrations.SqliteMigrations
{
    public partial class _1_Mig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lastname",
                table: "User",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Firstname",
                table: "User",
                newName: "FirstName");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "User",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "User",
                newName: "Lastname");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "User",
                newName: "Firstname");
        }
    }
}
