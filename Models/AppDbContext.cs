using Microsoft.EntityFrameworkCore;

namespace gimnasioNet.Models 
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Usuarios>()
                .HasMany(u => u.FechasUsuario)
                .WithOne(f => f.Usuario)
                .HasForeignKey(f => f.UsuarioId);
        }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Fechas_Usuario> Fechas_Usuarios { get; set; }
    }
}
