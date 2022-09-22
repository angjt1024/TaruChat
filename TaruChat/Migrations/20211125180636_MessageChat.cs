using Microsoft.EntityFrameworkCore.Migrations;

namespace TaruChat.Migrations
{
    public partial class MessageChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Message_ChatID",
                table: "Message");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ChatID",
                table: "Message",
                column: "ChatID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Message_ChatID",
                table: "Message");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ChatID",
                table: "Message",
                column: "ChatID",
                unique: true,
                filter: "[ChatID] IS NOT NULL");
        }
    }
}
