using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("usuarios")]
public class Usuarios
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Codigo { get; set; }

    [Required]
    [StringLength(50)]
    public string Nombres { get; set; }

    [Required]
    [StringLength(50)]
    public string Apellidos { get; set; }

    [Required]
    [StringLength(15)]
    public string Telefono { get; set; }

    [Required]
    public string Foto { get; set; }

    [Required]
    public bool Activo { get; set; }

    [Required]
    [StringLength(150)]
    public string Observaciones { get; set; }

    public ICollection<Fechas_Usuario> FechasUsuario { get; set; }
}
