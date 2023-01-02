using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlatRenting.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentAnnoucementRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AnnoucementId",
                table: "Comments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AnnoucementId",
                table: "Comments",
                column: "AnnoucementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Annoucements_AnnoucementId",
                table: "Comments",
                column: "AnnoucementId",
                principalTable: "Annoucements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Annoucements_AnnoucementId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_AnnoucementId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AnnoucementId",
                table: "Comments");
        }
    }
}
