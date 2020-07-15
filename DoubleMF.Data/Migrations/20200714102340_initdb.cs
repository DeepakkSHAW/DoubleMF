using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoubleMF.Data.Migrations
{
    public partial class initdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_AssetManagtCO",
                columns: table => new
                {
                    AMCId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AMCName = table.Column<string>(maxLength: 100, nullable: false),
                    inDate = table.Column<DateTime>(nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_AssetManagtCO", x => x.AMCId);
                });

            migrationBuilder.CreateTable(
                name: "T_MutualFund",
                columns: table => new
                {
                    MutualFundId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MutualFundName = table.Column<string>(maxLength: 100, nullable: false),
                    MutualFundCode = table.Column<int>(nullable: false),
                    DowbloadEnabled = table.Column<bool>(nullable: false),
                    AMCId = table.Column<int>(nullable: true),
                    inDate = table.Column<DateTime>(nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_MutualFund", x => x.MutualFundId);
                    table.ForeignKey(
                        name: "FK_T_MutualFund_T_AssetManagtCO_AMCId",
                        column: x => x.AMCId,
                        principalTable: "T_AssetManagtCO",
                        principalColumn: "AMCId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T_NetAssetValue",
                columns: table => new
                {
                    NetAssetValueId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Price = table.Column<double>(nullable: false),
                    OnDate = table.Column<DateTime>(nullable: false),
                    MFMutualFundId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_NetAssetValue", x => x.NetAssetValueId);
                    table.ForeignKey(
                        name: "FK_T_NetAssetValue_T_MutualFund_MFMutualFundId",
                        column: x => x.MFMutualFundId,
                        principalTable: "T_MutualFund",
                        principalColumn: "MutualFundId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_MutualFund_AMCId",
                table: "T_MutualFund",
                column: "AMCId");

            migrationBuilder.CreateIndex(
                name: "IX_T_NetAssetValue_MFMutualFundId",
                table: "T_NetAssetValue",
                column: "MFMutualFundId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_NetAssetValue");

            migrationBuilder.DropTable(
                name: "T_MutualFund");

            migrationBuilder.DropTable(
                name: "T_AssetManagtCO");
        }
    }
}
