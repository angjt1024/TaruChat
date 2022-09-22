using Microsoft.EntityFrameworkCore.Migrations;

namespace TaruChat.Migrations
{
    public partial class AddChatProfilePic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Enrollment_EnrollmentsChatID_EnrollmentsUserID",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_EnrollmentsChatID_EnrollmentsUserID",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "EnrollmentsChatID",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "EnrollmentsUserID",
                table: "Message");

            migrationBuilder.AlterColumn<string>(
                name: "ChatID",
                table: "Message",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePic",
                table: "Chat",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Message_ChatID",
                table: "Message",
                column: "ChatID",
                unique: true,
                filter: "[ChatID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Chat_ChatID",
                table: "Message",
                column: "ChatID",
                principalTable: "Chat",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Chat_ChatID",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_ChatID",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "ProfilePic",
                table: "Chat");

            migrationBuilder.AlterColumn<string>(
                name: "ChatID",
                table: "Message",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnrollmentsChatID",
                table: "Message",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnrollmentsUserID",
                table: "Message",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Message_EnrollmentsChatID_EnrollmentsUserID",
                table: "Message",
                columns: new[] { "EnrollmentsChatID", "EnrollmentsUserID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Enrollment_EnrollmentsChatID_EnrollmentsUserID",
                table: "Message",
                columns: new[] { "EnrollmentsChatID", "EnrollmentsUserID" },
                principalTable: "Enrollment",
                principalColumns: new[] { "ChatID", "UserID" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
