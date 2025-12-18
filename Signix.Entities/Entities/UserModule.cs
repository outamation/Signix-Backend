#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Signix.Entities.Entities;

public partial class UserModule
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ModuleId { get; set; }

    public int? ModifiedById { get; set; }

    [ForeignKey("ModuleId")]
    [InverseProperty("UserModules")]
    public virtual Module Module { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserModules")]
    public virtual User User { get; set; } = null!;
}