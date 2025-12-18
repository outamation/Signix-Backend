using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Signix.Entities.Migrations
{
    /// <inheritdoc />
    public partial class ssoSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "modules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(900)", maxLength: 900, nullable: false),
                    code = table.Column<string>(type: "character varying(900)", maxLength: 900, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_modules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(256)", unicode: false, maxLength: 256, nullable: false),
                    is_editable = table.Column<bool>(type: "boolean", nullable: false),
                    is_for_service_principal = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by_id = table.Column<int>(type: "integer", nullable: true),
                    modified_by_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                    table.ForeignKey(
                        name: "fk_roles_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_roles_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(900)", maxLength: 900, nullable: false),
                    code = table.Column<string>(type: "character varying(900)", maxLength: 900, nullable: false),
                    module_id = table.Column<int>(type: "integer", nullable: false),
                    is_for_user = table.Column<bool>(type: "boolean", nullable: false),
                    is_for_service_principal = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_for_tenant = table.Column<bool>(type: "boolean", nullable: true),
                    created_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_permissions_modules_module_id",
                        column: x => x.module_id,
                        principalTable: "modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_modules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    module_id = table.Column<int>(type: "integer", nullable: false),
                    modified_by_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_modules", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_modules_modules_module_id",
                        column: x => x.module_id,
                        principalTable: "modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_modules_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    unique_id = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    created_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_id = table.Column<int>(type: "integer", nullable: true),
                    modified_by_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_roles_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_roles_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    permission_id = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    modified_by_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permissions", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "fk_role_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_modules_code",
                table: "modules",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_modules_is_active",
                table: "modules",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_modules_name",
                table: "modules",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_permissions_code",
                table: "permissions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_permissions_is_active",
                table: "permissions",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_is_for_service_principal",
                table: "permissions",
                column: "is_for_service_principal");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_is_for_user",
                table: "permissions",
                column: "is_for_user");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_module_id",
                table: "permissions",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_name",
                table: "permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_modified_by_id",
                table: "role_permissions",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_role_id",
                table: "role_permissions",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_created_by_id",
                table: "roles",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_created_date_time",
                table: "roles",
                column: "created_date_time");

            migrationBuilder.CreateIndex(
                name: "ix_roles_is_active",
                table: "roles",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_roles_modified_by_id",
                table: "roles",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_modules_modified_by_id",
                table: "user_modules",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_modules_module_id",
                table: "user_modules",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_modules_user_id",
                table: "user_modules",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_modules_user_id_module_id",
                table: "user_modules",
                columns: new[] { "user_id", "module_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_created_by_id",
                table: "user_roles",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_modified_by_id",
                table: "user_roles",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_unique_id",
                table: "user_roles",
                column: "unique_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id",
                table: "user_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id_role_id",
                table: "user_roles",
                columns: new[] { "user_id", "role_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "user_modules");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "modules");
        }
    }
}
