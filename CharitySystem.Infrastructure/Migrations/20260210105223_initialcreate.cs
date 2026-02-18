using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CharitySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CLOTHES",
                columns: table => new
                {
                    ClothID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ClothName = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ClothCode = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    ClothSize = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Category = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ClothPoints = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ImageUrl = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsAvailable = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLOTHES", x => x.ClothID);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Fullname = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Username = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Gender = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Role = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    AccountStatus = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "FAMILIES",
                columns: table => new
                {
                    FamilyID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    FamilyCode = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    FamilyPoints = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FamilyAddress = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FamilyNotes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAMILIES", x => x.FamilyID);
                    table.ForeignKey(
                        name: "FK_FAMILIES_USERS_UserID",
                        column: x => x.UserID,
                        principalTable: "USERS",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FAMILYMEMBERS",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    FullName = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Age = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Gender = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    FamilyID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAMILYMEMBERS", x => x.MemberID);
                    table.ForeignKey(
                        name: "FK_FAMILYMEMBERS_FAMILIES_FamilyID",
                        column: x => x.FamilyID,
                        principalTable: "FAMILIES",
                        principalColumn: "FamilyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDERS",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    OrderCode = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    OrderStatus = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TotalPoints = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    OrderNotes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ShippingAddress = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    FamilyID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDERS", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_ORDERS_FAMILIES_FamilyID",
                        column: x => x.FamilyID,
                        principalTable: "FAMILIES",
                        principalColumn: "FamilyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDERDETAILS",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Quantity = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Points = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    OrderID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ClothID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDERDETAILS", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK_ORDERDETAILS_CLOTHES_ClothID",
                        column: x => x.ClothID,
                        principalTable: "CLOTHES",
                        principalColumn: "ClothID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ORDERDETAILS_ORDERS_OrderID",
                        column: x => x.OrderID,
                        principalTable: "ORDERS",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CLOTHES_ClothCode",
                table: "CLOTHES",
                column: "ClothCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FAMILIES_FamilyCode",
                table: "FAMILIES",
                column: "FamilyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FAMILIES_UserID",
                table: "FAMILIES",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FAMILYMEMBERS_FamilyID",
                table: "FAMILYMEMBERS",
                column: "FamilyID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERDETAILS_ClothID",
                table: "ORDERDETAILS",
                column: "ClothID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERDETAILS_OrderID",
                table: "ORDERDETAILS",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_FamilyID",
                table: "ORDERS",
                column: "FamilyID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_PhoneNumber",
                table: "USERS",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USERS_Username",
                table: "USERS",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAMILYMEMBERS");

            migrationBuilder.DropTable(
                name: "ORDERDETAILS");

            migrationBuilder.DropTable(
                name: "CLOTHES");

            migrationBuilder.DropTable(
                name: "ORDERS");

            migrationBuilder.DropTable(
                name: "FAMILIES");

            migrationBuilder.DropTable(
                name: "USERS");
        }
    }
}
