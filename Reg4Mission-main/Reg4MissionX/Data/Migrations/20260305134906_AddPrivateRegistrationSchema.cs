using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reg4MissionX.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrivateRegistrationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Municipalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScbCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipalities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrivatePersonProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeptLss = table.Column<bool>(type: "bit", nullable: false),
                    DeptSol = table.Column<bool>(type: "bit", nullable: false),
                    DeptSocialtjansten = table.Column<bool>(type: "bit", nullable: false),
                    GdprAccepted = table.Column<bool>(type: "bit", nullable: false),
                    GdprAcceptedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GdprVersion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivatePersonProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivatePersonProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivateProfileMunicipalities",
                columns: table => new
                {
                    PrivatePersonProfileId = table.Column<int>(type: "int", nullable: false),
                    MunicipalityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateProfileMunicipalities", x => new { x.PrivatePersonProfileId, x.MunicipalityId });
                    table.ForeignKey(
                        name: "FK_PrivateProfileMunicipalities_Municipalities_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalTable: "Municipalities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrivateProfileMunicipalities_PrivatePersonProfiles_PrivatePersonProfileId",
                        column: x => x.PrivatePersonProfileId,
                        principalTable: "PrivatePersonProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivatePersonProfiles_UserId",
                table: "PrivatePersonProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateProfileMunicipalities_MunicipalityId",
                table: "PrivateProfileMunicipalities",
                column: "MunicipalityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivateProfileMunicipalities");

            migrationBuilder.DropTable(
                name: "Municipalities");

            migrationBuilder.DropTable(
                name: "PrivatePersonProfiles");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");
        }
    }
}
