using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastVocab.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TargetAudience = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DifficultyLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsHiding = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VnText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsHiding = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Meaning = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Definition = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Level = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Example1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Example2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Example3 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WordLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordLists_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TakedWords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WordId = table.Column<int>(type: "int", nullable: false),
                    RepetitionCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    EasinessFactor = table.Column<double>(type: "float(3)", precision: 3, scale: 2, nullable: false, defaultValue: 2.5),
                    LastReviewed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReview = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastGrade = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TakedWords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TakedWords_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TakedWords_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WordTopics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WordId = table.Column<int>(type: "int", nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordTopics_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracticeSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListId = table.Column<int>(type: "int", nullable: false),
                    RepetitionCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastReviewed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReview = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PracticeSessions_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PracticeSessions_WordLists_ListId",
                        column: x => x.ListId,
                        principalTable: "WordLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WordListDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WordListId = table.Column<int>(type: "int", nullable: false),
                    WordId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordListDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordListDetails_WordLists_WordListId",
                        column: x => x.WordListId,
                        principalTable: "WordLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordListDetails_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_AccountId",
                table: "AppUsers",
                column: "AccountId",
                unique: true,
                filter: "[AccountId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_IsDeleted",
                table: "AppUsers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_SessionId",
                table: "AppUsers",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_DifficultyLevel",
                table: "Collections",
                column: "DifficultyLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_IsDeleted",
                table: "Collections",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_IsHiding",
                table: "Collections",
                column: "IsHiding");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_Name",
                table: "Collections",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSessions_IsDeleted",
                table: "PracticeSessions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSessions_ListId",
                table: "PracticeSessions",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSessions_NextReview",
                table: "PracticeSessions",
                column: "NextReview");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSessions_UserId",
                table: "PracticeSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSessions_UserId_ListId",
                table: "PracticeSessions",
                columns: new[] { "UserId", "ListId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSessions_UserId_NextReview",
                table: "PracticeSessions",
                columns: new[] { "UserId", "NextReview" });

            migrationBuilder.CreateIndex(
                name: "IX_TakedWords_LastGrade",
                table: "TakedWords",
                column: "LastGrade");

            migrationBuilder.CreateIndex(
                name: "IX_TakedWords_NextReview",
                table: "TakedWords",
                column: "NextReview");

            migrationBuilder.CreateIndex(
                name: "IX_TakedWords_UserId",
                table: "TakedWords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TakedWords_UserId_LastGrade",
                table: "TakedWords",
                columns: new[] { "UserId", "LastGrade" });

            migrationBuilder.CreateIndex(
                name: "IX_TakedWords_UserId_NextReview",
                table: "TakedWords",
                columns: new[] { "UserId", "NextReview" });

            migrationBuilder.CreateIndex(
                name: "IX_TakedWords_UserId_WordId",
                table: "TakedWords",
                columns: new[] { "UserId", "WordId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TakedWords_WordId",
                table: "TakedWords",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_IsDeleted",
                table: "Topics",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_IsHiding",
                table: "Topics",
                column: "IsHiding");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Name",
                table: "Topics",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WordListDetails_WordId",
                table: "WordListDetails",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_WordListDetails_WordListId",
                table: "WordListDetails",
                column: "WordListId");

            migrationBuilder.CreateIndex(
                name: "IX_WordListDetails_WordListId_WordId",
                table: "WordListDetails",
                columns: new[] { "WordListId", "WordId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WordLists_CollectionId",
                table: "WordLists",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_WordLists_IsDeleted",
                table: "WordLists",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_WordLists_Name_CollectionId",
                table: "WordLists",
                columns: new[] { "Name", "CollectionId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Words_IsDeleted",
                table: "Words",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Words_Level",
                table: "Words",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Words_Text",
                table: "Words",
                column: "Text",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Words_Type",
                table: "Words",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Words_Type_Level",
                table: "Words",
                columns: new[] { "Type", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_WordTopics_TopicId",
                table: "WordTopics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_WordTopics_WordId",
                table: "WordTopics",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_WordTopics_WordId_TopicId",
                table: "WordTopics",
                columns: new[] { "WordId", "TopicId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PracticeSessions");

            migrationBuilder.DropTable(
                name: "TakedWords");

            migrationBuilder.DropTable(
                name: "WordListDetails");

            migrationBuilder.DropTable(
                name: "WordTopics");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "WordLists");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.DropTable(
                name: "Collections");
        }
    }
}
