using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI_Code_First.Migrations
{
    public partial class FileupladProfileImageadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCurrentProfileImage",
                table: "FileUploads",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCurrentProfileImage",
                table: "FileUploads");
        }
    }
}
