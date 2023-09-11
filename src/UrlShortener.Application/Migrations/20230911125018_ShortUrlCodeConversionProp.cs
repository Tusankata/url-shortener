using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Application.Migrations
{
    /// <inheritdoc />
    public partial class ShortUrlCodeConversionProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code_Value",
                table: "ShortUrlCodes");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ShortUrlCodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "ShortUrlCodes");

            migrationBuilder.AddColumn<string>(
                name: "Code_Value",
                table: "ShortUrlCodes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
