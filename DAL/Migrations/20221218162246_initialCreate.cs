using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class initialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataloggerTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CanChannels = table.Column<int>(nullable: true),
                    MemoryStorage = table.Column<int>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataloggerTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServerCredentials",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Token = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Login = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerCredentials", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServerTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Dataloggers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataloggerID = table.Column<int>(nullable: true),
                    DataloggerTypeID = table.Column<int>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dataloggers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Dataloggers_DataloggerTypes_DataloggerTypeID",
                        column: x => x.DataloggerTypeID,
                        principalTable: "DataloggerTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerCredentialsID = table.Column<int>(nullable: false),
                    ServerTypeID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    IP_URL = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Servers_ServerCredentials_ServerCredentialsID",
                        column: x => x.ServerCredentialsID,
                        principalTable: "ServerCredentials",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Servers_ServerTypes_ServerTypeID",
                        column: x => x.ServerTypeID,
                        principalTable: "ServerTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataloggerID = table.Column<int>(nullable: true),
                    VIN = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Model = table.Column<string>(nullable: false),
                    Manufacturer = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Vehicles_Dataloggers_DataloggerID",
                        column: x => x.DataloggerID,
                        principalTable: "Dataloggers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dataloggers_DataloggerTypeID",
                table: "Dataloggers",
                column: "DataloggerTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_ServerCredentialsID",
                table: "Servers",
                column: "ServerCredentialsID");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_ServerTypeID",
                table: "Servers",
                column: "ServerTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_DataloggerID",
                table: "Vehicles",
                column: "DataloggerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "ServerCredentials");

            migrationBuilder.DropTable(
                name: "ServerTypes");

            migrationBuilder.DropTable(
                name: "Dataloggers");

            migrationBuilder.DropTable(
                name: "DataloggerTypes");
        }
    }
}
