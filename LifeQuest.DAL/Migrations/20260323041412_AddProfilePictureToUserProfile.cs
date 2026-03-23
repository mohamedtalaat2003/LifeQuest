using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeQuest.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureToUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskLevel",
                table: "Decisions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "RiskLevel",
                table: "Decisions");
        }
    }
}
