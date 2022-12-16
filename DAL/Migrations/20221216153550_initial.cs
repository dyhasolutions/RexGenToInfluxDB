using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class initial : Migration
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
                    Type = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
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
                    Name = table.Column<string>(nullable: true)
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
                    SelectedDataloggerTypeID = table.Column<int>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dataloggers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Dataloggers_DataloggerTypes_SelectedDataloggerTypeID",
                        column: x => x.SelectedDataloggerTypeID,
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
                    SelectedServerCredentialsID = table.Column<int>(nullable: true),
                    SelectedServerTypeID = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IP_URL = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Servers_ServerCredentials_SelectedServerCredentialsID",
                        column: x => x.SelectedServerCredentialsID,
                        principalTable: "ServerCredentials",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Servers_ServerTypes_SelectedServerTypeID",
                        column: x => x.SelectedServerTypeID,
                        principalTable: "ServerTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InstalledDataloggerID = table.Column<int>(nullable: true),
                    VIN = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    Manufacturer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Vehicles_Dataloggers_InstalledDataloggerID",
                        column: x => x.InstalledDataloggerID,
                        principalTable: "Dataloggers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dataloggers_SelectedDataloggerTypeID",
                table: "Dataloggers",
                column: "SelectedDataloggerTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_SelectedServerCredentialsID",
                table: "Servers",
                column: "SelectedServerCredentialsID");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_SelectedServerTypeID",
                table: "Servers",
                column: "SelectedServerTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_InstalledDataloggerID",
                table: "Vehicles",
                column: "InstalledDataloggerID");
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
