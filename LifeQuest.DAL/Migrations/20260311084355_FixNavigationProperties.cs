using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeQuest.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the FK in DailyLogs that depends on the old AK_UserChallenges_Id
            migrationBuilder.DropForeignKey(
                name: "FK_DailyLogs_UserChallenges_UserChallengeId",
                table: "DailyLogs");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_LevelId",
                table: "UserProfiles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UserChallenges_Id",
                table: "UserChallenges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserChallenges",
                table: "UserChallenges");

            // SQL Server cannot ALTER a column to add IDENTITY — drop and recreate
            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserChallenges");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserChallenges",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Decisions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserChallenges",
                table: "UserChallenges",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_LevelId",
                table: "UserProfiles",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChallenges_UserId_ChallengeId",
                table: "UserChallenges",
                columns: new[] { "UserId", "ChallengeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_UserId",
                table: "Decisions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Decisions_AspNetUsers_UserId",
                table: "Decisions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            // Recreate the FK in DailyLogs pointing to the new PK
            migrationBuilder.AddForeignKey(
                name: "FK_DailyLogs_UserChallenges_UserChallengeId",
                table: "DailyLogs",
                column: "UserChallengeId",
                principalTable: "UserChallenges",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Decisions_AspNetUsers_UserId",
                table: "Decisions");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_LevelId",
                table: "UserProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserChallenges",
                table: "UserChallenges");

            migrationBuilder.DropIndex(
                name: "IX_UserChallenges_UserId_ChallengeId",
                table: "UserChallenges");

            migrationBuilder.DropIndex(
                name: "IX_Decisions_UserId",
                table: "Decisions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Decisions");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserChallenges",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_UserChallenges_Id",
                table: "UserChallenges",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserChallenges",
                table: "UserChallenges",
                columns: new[] { "UserId", "ChallengeId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_LevelId",
                table: "UserProfiles",
                column: "LevelId",
                unique: true);
        }
    }
}
