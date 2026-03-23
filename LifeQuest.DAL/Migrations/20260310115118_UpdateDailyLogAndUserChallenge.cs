using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeQuest.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDailyLogAndUserChallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentProgress",
                table: "UserChallenges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LogDate",
                table: "DailyLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserChallengeId",
                table: "DailyLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_UserChallenges_Id",
                table: "UserChallenges",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DailyLogs_UserChallengeId",
                table: "DailyLogs",
                column: "UserChallengeId");

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
                name: "FK_DailyLogs_UserChallenges_UserChallengeId",
                table: "DailyLogs");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UserChallenges_Id",
                table: "UserChallenges");

            migrationBuilder.DropIndex(
                name: "IX_DailyLogs_UserChallengeId",
                table: "DailyLogs");

            migrationBuilder.DropColumn(
                name: "CurrentProgress",
                table: "UserChallenges");

            migrationBuilder.DropColumn(
                name: "LogDate",
                table: "DailyLogs");

            migrationBuilder.DropColumn(
                name: "UserChallengeId",
                table: "DailyLogs");
        }
    }
}
