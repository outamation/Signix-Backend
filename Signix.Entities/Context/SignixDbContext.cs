using Microsoft.EntityFrameworkCore;
using Signix.Entities.Entities;
using System.Text.Json;

namespace Signix.Entities.Context;

public class SignixDbContext : DbContext
{
    public SignixDbContext(DbContextOptions<SignixDbContext> options) : base(options) { }

    // Original entities
    public DbSet<Client> Clients { get; set; }
    public DbSet<Designation> Designations { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentStatus> DocumentStatuses { get; set; }
    public DbSet<Signer> Signers { get; set; }
    public DbSet<SigningRoom> SigningRooms { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<SignLog> SignLogs { get; set; }

    // Authentication & Authorization entities
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserModule> UserModules { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure original entity relationships
        ConfigureOriginalEntityRelationships(modelBuilder);

        // Configure authentication entity relationships
        ConfigureAuthenticationEntityRelationships(modelBuilder);

        // Configure PostgreSQL-specific settings
        ConfigurePostgreSQLSettings(modelBuilder);

        // Configure indexes
        ConfigureIndexes(modelBuilder);
    }

    private void ConfigureOriginalEntityRelationships(ModelBuilder modelBuilder)
    {
        // Document relationships
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasOne(d => d.DocumentStatus)
                  .WithMany(ds => ds.Documents)
                  .HasForeignKey(d => d.DocumentStatusId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.SigningRoom)
                  .WithMany(sr => sr.Documents)
                  .HasForeignKey(d => d.SigningRoomId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.DocTags).HasColumnType("jsonb").HasConversion(v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                                  v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, new JsonSerializerOptions { })!
                  );
        });

        // SigningRoom relationships
        modelBuilder.Entity<SigningRoom>(entity =>
        {
            entity.HasOne(sr => sr.Notary)
                  .WithMany(u => u.SigningRooms)
                  .HasForeignKey(sr => sr.NotaryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(sr => sr.Client)
                  .WithMany(c => c.SigningRooms)
                  .HasForeignKey(sr => sr.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Signer relationships
        modelBuilder.Entity<Signer>(entity =>
        {
            entity.HasOne(s => s.SigningRoom)
                  .WithMany(sr => sr.Signers)
                  .HasForeignKey(s => s.SigningRoomId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.Designation)
                  .WithMany(d => d.Signers)
                  .HasForeignKey(s => s.DesignationId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint for signing room and email combination
            entity.HasIndex(s => new { s.SigningRoomId, s.Email }).IsUnique();
        });

        // SignLog relationships
        modelBuilder.Entity<SignLog>(entity =>
        {
            entity.HasOne(sl => sl.Document)
                  .WithMany(d => d.SignLogs)
                  .HasForeignKey(sl => sl.DocumentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureAuthenticationEntityRelationships(ModelBuilder modelBuilder)
    {
        // Role relationships with User
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasOne(r => r.CreatedBy)
                  .WithMany(u => u.RoleCreatedBies)
                  .HasForeignKey(r => r.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.ModifiedBy)
                  .WithMany(u => u.RoleModifiedBies)
                  .HasForeignKey(r => r.ModifiedById)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Permission relationships with Module
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasOne(p => p.Module)
                  .WithMany(m => m.Permissions)
                  .HasForeignKey(p => p.ModuleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // UserRole relationships - Composite Key Configuration
        modelBuilder.Entity<UserRole>(entity =>
        {
            // Use standard integer primary key instead of composite key
            entity.HasKey(ur => ur.Id);

            // Add unique constraint for UserId + RoleId combination
            entity.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();

            entity.HasOne(ur => ur.User)
                  .WithMany(u => u.UserRoleUsers)
                  .HasForeignKey(ur => ur.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(ur => ur.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.CreatedBy)
                  .WithMany(u => u.UserRoleCreatedBies)
                  .HasForeignKey(ur => ur.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ur => ur.ModifiedBy)
                  .WithMany(u => u.UserRoleModifiedBies)
                  .HasForeignKey(ur => ur.ModifiedById)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // UserModule relationships
        modelBuilder.Entity<UserModule>(entity =>
        {
            entity.HasOne(um => um.User)
                  .WithMany(u => u.UserModules)
                  .HasForeignKey(um => um.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(um => um.Module)
                  .WithMany(m => m.UserModules)
                  .HasForeignKey(um => um.ModuleId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Add unique constraint for UserId + ModuleId combination
            entity.HasIndex(um => new { um.UserId, um.ModuleId }).IsUnique();
        });

        // RolePermission relationships - Composite Key Configuration
        modelBuilder.Entity<RolePermission>(entity =>
        {
            // Composite primary key
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            entity.HasOne(rp => rp.Role)
                  .WithMany(r => r.RolePermissions)
                  .HasForeignKey(rp => rp.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rp => rp.Permission)
                  .WithMany(p => p.RolePermissions)
                  .HasForeignKey(rp => rp.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigurePostgreSQLSettings(ModelBuilder modelBuilder)
    {
        // Client JSONB configurations
        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(e => e.ClientSecret)
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                      v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, new JsonSerializerOptions { })!
                  );
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });

        // User JSONB configurations
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.MetaData)
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                      v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, new JsonSerializerOptions { })!
                  );
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // SigningRoom JSONB configurations
        modelBuilder.Entity<SigningRoom>(entity =>
        {
            entity.Property(e => e.MetaData)
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                      v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, new JsonSerializerOptions { })!
                  );
            entity.Property(e => e.SignTags)
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                      v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, new JsonSerializerOptions { })!
                  );
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Original entity indexes
        modelBuilder.Entity<SigningRoom>().HasIndex(sr => sr.ClientId);
        modelBuilder.Entity<SigningRoom>().HasIndex(sr => sr.NotaryId);
        modelBuilder.Entity<Document>().HasIndex(d => d.SigningRoomId);
        modelBuilder.Entity<Document>().HasIndex(d => d.DocumentStatusId);
        modelBuilder.Entity<Signer>().HasIndex(s => s.SigningRoomId);
        modelBuilder.Entity<Signer>().HasIndex(s => s.DesignationId);
        modelBuilder.Entity<SignLog>().HasIndex(sl => sl.DocumentId);

        // Authentication entity indexes
        modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(r => r.IsActive);
        modelBuilder.Entity<Role>().HasIndex(r => r.CreatedById);
        modelBuilder.Entity<Role>().HasIndex(r => r.ModifiedById);
        modelBuilder.Entity<Role>().HasIndex(r => r.CreatedDateTime);

        modelBuilder.Entity<Permission>().HasIndex(p => p.Code).IsUnique();
        modelBuilder.Entity<Permission>().HasIndex(p => p.Name).IsUnique();
        modelBuilder.Entity<Permission>().HasIndex(p => p.IsActive);
        modelBuilder.Entity<Permission>().HasIndex(p => p.IsForUser);
        modelBuilder.Entity<Permission>().HasIndex(p => p.IsForServicePrincipal);
        modelBuilder.Entity<Permission>().HasIndex(p => p.ModuleId);

        modelBuilder.Entity<Module>().HasIndex(m => m.Code).IsUnique();
        modelBuilder.Entity<Module>().HasIndex(m => m.Name).IsUnique();
        modelBuilder.Entity<Module>().HasIndex(m => m.IsActive);

        modelBuilder.Entity<UserRole>().HasIndex(ur => ur.UserId);
        modelBuilder.Entity<UserRole>().HasIndex(ur => ur.RoleId);
        modelBuilder.Entity<UserRole>().HasIndex(ur => ur.UniqueId);

        modelBuilder.Entity<UserModule>().HasIndex(um => um.UserId);
        modelBuilder.Entity<UserModule>().HasIndex(um => um.ModuleId);
        modelBuilder.Entity<UserModule>().HasIndex(um => um.ModifiedById);

        modelBuilder.Entity<RolePermission>().HasIndex(rp => rp.RoleId);
        modelBuilder.Entity<RolePermission>().HasIndex(rp => rp.PermissionId);
        modelBuilder.Entity<RolePermission>().HasIndex(rp => rp.ModifiedById);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-update ModifiedAt timestamps for entities that have this property
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity.GetType().GetProperty("ModifiedAt") != null &&
                       (e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            entry.Property("ModifiedAt").CurrentValue = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
