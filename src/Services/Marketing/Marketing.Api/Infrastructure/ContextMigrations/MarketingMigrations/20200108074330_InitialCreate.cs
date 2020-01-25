using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenCodeCamp.Services.Marketing.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "marketing");

            migrationBuilder.CreateSequence(
                name: "newslettersubscriptionseq",
                schema: "marketing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "newslettersubscriptiontokenseq",
                schema: "marketing",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "newslettersubscriptionstatus",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_newslettersubscriptionstatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "newslettersubscriptiontokentypes",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_newslettersubscriptiontokentypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "requests",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "newslettersubscriptions",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    EmailAddress = table.Column<string>(maxLength: 80, nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    Cancelled = table.Column<DateTime>(maxLength: 25, nullable: true),
                    Confirmed = table.Column<DateTime>(maxLength: 25, nullable: true),
                    Inserted = table.Column<DateTime>(maxLength: 25, nullable: false, defaultValue: new DateTime(2020, 1, 8, 8, 43, 30, 562, DateTimeKind.Local).AddTicks(5775)),
                    Language = table.Column<string>(maxLength: 5, nullable: false),
                    LastUpdated = table.Column<DateTime>(maxLength: 25, nullable: false, defaultValue: new DateTime(2020, 1, 8, 8, 43, 30, 573, DateTimeKind.Local).AddTicks(1253))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_newslettersubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_newslettersubscriptions_newslettersubscriptionstatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "marketing",
                        principalTable: "newslettersubscriptionstatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "newslettersubscriptiontokens",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    TokenTypeId = table.Column<int>(nullable: false),
                    Inserted = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2020, 1, 8, 8, 43, 30, 599, DateTimeKind.Local).AddTicks(7754)),
                    NewsletterSubscriptionId = table.Column<int>(nullable: false),
                    Token = table.Column<string>(maxLength: 32, nullable: false),
                    Used = table.Column<DateTime>(maxLength: 25, nullable: true),
                    Expiration = table.Column<DateTime>(maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_newslettersubscriptiontokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_newslettersubscriptiontokens_newslettersubscriptions_NewsletterSubscriptionId",
                        column: x => x.NewsletterSubscriptionId,
                        principalSchema: "marketing",
                        principalTable: "newslettersubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_newslettersubscriptiontokens_newslettersubscriptiontokentypes_TokenTypeId",
                        column: x => x.TokenTypeId,
                        principalSchema: "marketing",
                        principalTable: "newslettersubscriptiontokentypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_newslettersubscriptions_StatusId",
                schema: "marketing",
                table: "newslettersubscriptions",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_newslettersubscriptiontokens_NewsletterSubscriptionId",
                schema: "marketing",
                table: "newslettersubscriptiontokens",
                column: "NewsletterSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_newslettersubscriptiontokens_TokenTypeId",
                schema: "marketing",
                table: "newslettersubscriptiontokens",
                column: "TokenTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "newslettersubscriptiontokens",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "requests",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "newslettersubscriptions",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "newslettersubscriptiontokentypes",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "newslettersubscriptionstatus",
                schema: "marketing");

            migrationBuilder.DropSequence(
                name: "newslettersubscriptionseq",
                schema: "marketing");

            migrationBuilder.DropSequence(
                name: "newslettersubscriptiontokenseq",
                schema: "marketing");
        }
    }
}
