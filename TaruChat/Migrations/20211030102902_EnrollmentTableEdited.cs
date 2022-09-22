using Microsoft.EntityFrameworkCore.Migrations;

namespace TaruChat.Migrations
{
    public partial class EnrollmentTableEdited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_Subject_SubjectID",
                table: "Enrollment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enrollment",
                table: "Enrollment");

            migrationBuilder.DropIndex(
                name: "IX_Enrollment_ChatID",
                table: "Enrollment");

            migrationBuilder.DropIndex(
                name: "IX_Enrollment_SubjectID",
                table: "Enrollment");

            migrationBuilder.DropColumn(
                name: "SubjectID",
                table: "Enrollment");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enrollment",
                table: "Enrollment",
                columns: new[] { "ChatID", "UserID" });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_UserID",
                table: "Enrollment",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Enrollment",
                table: "Enrollment");

            migrationBuilder.DropIndex(
                name: "IX_Enrollment_UserID",
                table: "Enrollment");

            migrationBuilder.AddColumn<string>(
                name: "SubjectID",
                table: "Enrollment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enrollment",
                table: "Enrollment",
                columns: new[] { "UserID", "ChatID" });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_ChatID",
                table: "Enrollment",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_SubjectID",
                table: "Enrollment",
                column: "SubjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_Subject_SubjectID",
                table: "Enrollment",
                column: "SubjectID",
                principalTable: "Subject",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
