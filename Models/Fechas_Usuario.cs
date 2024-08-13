using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("fechas_usuario")]
public class Fechas_Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("Usuarios")]
    public int UsuarioId { get; set; }

    [Required]
    public DateTime FechaIngreso { get; set; }

    [Required]
    public DateTime FechaPago { get; set; }

    [Required]
    public DateTime FechaPagoA { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public Usuarios Usuarios { get; set; }
}
