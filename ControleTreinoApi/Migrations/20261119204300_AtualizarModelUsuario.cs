using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleTreinoApi.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarModelUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosSerieExecutadas_Exercicios_ExercicioId",
                table: "RegistrosSerieExecutadas");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessoes_Treinos_TreinoId",
                table: "Sessoes");

            migrationBuilder.DropForeignKey(
                name: "FK_TreinoItems_Exercicios_ExercicioId",
                table: "TreinoItems");

            migrationBuilder.DropIndex(
                name: "IX_TreinoItems_TreinoId",
                table: "TreinoItems");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreinoItems_TreinoId_Ordem",
                table: "TreinoItems",
                columns: new[] { "TreinoId", "Ordem" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosSerieExecutadas_Exercicios_ExercicioId",
                table: "RegistrosSerieExecutadas",
                column: "ExercicioId",
                principalTable: "Exercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessoes_Treinos_TreinoId",
                table: "Sessoes",
                column: "TreinoId",
                principalTable: "Treinos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TreinoItems_Exercicios_ExercicioId",
                table: "TreinoItems",
                column: "ExercicioId",
                principalTable: "Exercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosSerieExecutadas_Exercicios_ExercicioId",
                table: "RegistrosSerieExecutadas");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessoes_Treinos_TreinoId",
                table: "Sessoes");

            migrationBuilder.DropForeignKey(
                name: "FK_TreinoItems_Exercicios_ExercicioId",
                table: "TreinoItems");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_TreinoItems_TreinoId_Ordem",
                table: "TreinoItems");

            migrationBuilder.CreateIndex(
                name: "IX_TreinoItems_TreinoId",
                table: "TreinoItems",
                column: "TreinoId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosSerieExecutadas_Exercicios_ExercicioId",
                table: "RegistrosSerieExecutadas",
                column: "ExercicioId",
                principalTable: "Exercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessoes_Treinos_TreinoId",
                table: "Sessoes",
                column: "TreinoId",
                principalTable: "Treinos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TreinoItems_Exercicios_ExercicioId",
                table: "TreinoItems",
                column: "ExercicioId",
                principalTable: "Exercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
