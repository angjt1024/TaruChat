using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TaruChat.Migrations
{
    public partial class MessageTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    ID = table.Column<string>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Word = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChatID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrollmentsChatID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EnrollmentsUserID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Message_Enrollment_EnrollmentsChatID_EnrollmentsUserID",
                        columns: x => new { x.EnrollmentsChatID, x.EnrollmentsUserID },
                        principalTable: "Enrollment",
                        principalColumns: new[] { "ChatID", "UserID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_EnrollmentsChatID_EnrollmentsUserID",
                table: "Message",
                columns: new[] { "EnrollmentsChatID", "EnrollmentsUserID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Message");
        }
    }
}
