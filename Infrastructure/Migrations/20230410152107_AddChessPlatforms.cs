using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChessPlatforms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false, comment: "Имя/логин игрока")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PositionEvaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Depth = table.Column<int>(type: "INTEGER", nullable: false, comment: "Глубина на которой была посчитана оценка"),
                    Cp = table.Column<double>(type: "REAL", nullable: false, comment: "Оценка позиции"),
                    OneMove = table.Column<string>(type: "TEXT", nullable: false, comment: "Сильнейший ход на глубине Depth"),
                    Fen = table.Column<string>(type: "TEXT", nullable: false, comment: "Позиция в стандарте FEN")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionEvaluations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Pgn = table.Column<string>(type: "TEXT", nullable: false, comment: "Pgn данные партии"),
                    WhiteGamer = table.Column<string>(type: "TEXT", nullable: false, comment: "Логин игрока с белыми фигурами"),
                    BlackGamer = table.Column<string>(type: "TEXT", nullable: false, comment: "Логин игрока с черными фигурами"),
                    Result = table.Column<string>(type: "TEXT", nullable: false, comment: "Строка результата из Pgn"),
                    DateGame = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "Дата, когда была сыграна партия"),
                    Platform = table.Column<byte>(type: "INTEGER", nullable: true, comment: "Платформа, на которой была сыграна игра"),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fen = table.Column<string>(type: "TEXT", nullable: false, comment: "Позиция в стандарте FEN"),
                    YourMove = table.Column<string>(type: "TEXT", nullable: false, comment: "Сделанный в партии ход"),
                    WhoseMove = table.Column<byte>(type: "INTEGER", nullable: false, comment: "Чей ход в позиции"),
                    PositionEvaluationId = table.Column<int>(type: "INTEGER", nullable: true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: true, comment: "Идентификатор игры"),
                    IsMistake = table.Column<bool>(type: "INTEGER", nullable: false, comment: "Является ли ход в партии ошибкой")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Positions_PositionEvaluations_PositionEvaluationId",
                        column: x => x.PositionEvaluationId,
                        principalTable: "PositionEvaluations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_BlackGamer",
                table: "Games",
                column: "BlackGamer");

            migrationBuilder.CreateIndex(
                name: "IX_Games_PlayerId",
                table: "Games",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_WhiteGamer",
                table: "Games",
                column: "WhiteGamer");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_GameId",
                table: "Positions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PositionEvaluationId",
                table: "Positions",
                column: "PositionEvaluationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "PositionEvaluations");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
