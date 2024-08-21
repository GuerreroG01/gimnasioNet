using System;
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

    [Column(TypeName = "DATE")]
    public DateTime? FechaPago { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaPagoA { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaVencimiento { get; set; }

    public Usuarios Usuario { get; set; }
}
