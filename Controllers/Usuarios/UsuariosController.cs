using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gimnasioNet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace gimnasioNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagePath;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUsuarios(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuarios(int id, [FromForm] Usuarios usuarios, IFormFile? foto)
        {
            if (id != usuarios.Codigo)
            {
                return BadRequest("El ID del usuario no coincide.");
            }

            var existingUser = await _context.Usuarios.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            if (foto != null)
            {
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(foto.FileName);
                var filePath = Path.Combine(_imagePath, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                // Elimina la foto antigua si es necesario
                if (!string.IsNullOrEmpty(existingUser.Foto) && existingUser.Foto != "Default.png")
                {
                    var oldFilePath = Path.Combine(_imagePath, existingUser.Foto);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                usuarios.Foto = newFileName;
            }
            else
            {
                usuarios.Foto = existingUser.Foto;
            }

            _context.Entry(existingUser).CurrentValues.SetValues(usuarios);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuariosExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(500, "Error al actualizar el usuario.");
                }
            }

            return NoContent();
        }

        // POST: api/Fechas_Usuario
        [HttpPost("Fechas_Usuario")]
        public async Task<ActionResult<Fechas_Usuario>> PostFechasUsuario(Fechas_Usuario fechasUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica que UsuarioId esté presente
            if (fechasUsuario.UsuarioId <= 0)
            {
                ModelState.AddModelError("UsuarioId", "UsuarioId es requerido y debe ser un número positivo.");
                return BadRequest(ModelState);
            }

            // Verifica si el UsuarioId existe en la tabla Usuarios
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Codigo == fechasUsuario.UsuarioId);

            if (usuarioExistente == null)
            {
                ModelState.AddModelError("UsuarioId", "UsuarioId no existe en la tabla de Usuarios.");
                return BadRequest(ModelState);
            }

            // Verifica fechas
            if (fechasUsuario.FechaPago > fechasUsuario.FechaVencimiento)
            {
                ModelState.AddModelError("FechaPago", "La fecha de pago no puede ser mayor que la fecha de vencimiento.");
                return BadRequest(ModelState);
            }

            try
            {
                _context.Fechas_Usuarios.Add(fechasUsuario);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetFechasUsuario), new { id = fechasUsuario.Id }, fechasUsuario);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarios(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_imagePath, usuario.Foto);
            if (System.IO.File.Exists(filePath) && usuario.Foto != "Default.png")
            {
                System.IO.File.Delete(filePath);
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuariosExists(int id)
        {
            return _context.Usuarios.Any(e => e.Codigo == id);
        }

        // GET: api/Fechas_Usuario/5
        [HttpGet("Fechas_Usuario/{id}")]
        public async Task<ActionResult<Fechas_Usuario>> GetFechasUsuario(int id)
        {
            var fechasUsuario = await _context.Fechas_Usuarios.FindAsync(id);

            if (fechasUsuario == null)
            {
                return NotFound();
            }

            return fechasUsuario;
        }
    }
}
