using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gimnasioNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gimnasioNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Fechas_UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Fechas_UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Fechas_Usuario
        [HttpGet]
        public async Task<ActionResult<List<object>>> GetFechasUsuarios([FromQuery] int? usuarioId)
        {
            try
            {
                var query = _context.Fechas_Usuarios.AsQueryable();

                if (usuarioId.HasValue)
                {
                    query = query.Where(f => f.UsuarioId == usuarioId.Value);
                }

                var fechasUsuarios = await query
                    .Include(f => f.Usuario)
                    .ToListAsync();

                var fechasUsuariosDto = fechasUsuarios.Select(f => new
                {
                    f.Id,
                    f.UsuarioId,
                    f.FechaPago,
                    f.FechaPagoA,
                    f.FechaVencimiento,
                    Usuario = new
                    {
                        f.Usuario.Codigo,
                        f.Usuario.Nombres,
                        f.Usuario.Apellidos,
                        f.Usuario.Telefono,
                        f.Usuario.Foto,
                        f.Usuario.FechaIngreso,
                        f.Usuario.Activo,
                        f.Usuario.Observaciones
                    }
                }).ToList();

                return Ok(fechasUsuariosDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Fechas_Usuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetFechasUsuario(int id)
        {
            var fechasUsuario = await _context.Fechas_Usuarios
                .Include(f => f.Usuario)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fechasUsuario == null)
            {
                return NotFound();
            }

            var fechasUsuarioDto = new
            {
                fechasUsuario.Id,
                fechasUsuario.UsuarioId,
                fechasUsuario.FechaPago,
                fechasUsuario.FechaPagoA,
                fechasUsuario.FechaVencimiento,
                Usuario = new
                {
                    fechasUsuario.Usuario.Codigo,
                    fechasUsuario.Usuario.Nombres,
                    fechasUsuario.Usuario.Apellidos,
                    fechasUsuario.Usuario.Telefono,
                    fechasUsuario.Usuario.Foto,
                    fechasUsuario.Usuario.FechaIngreso,
                    fechasUsuario.Usuario.Activo,
                    fechasUsuario.Usuario.Observaciones
                }
            };

            return Ok(fechasUsuarioDto);
        }

        // PUT: api/Fechas_Usuario/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFechasUsuario(int id, [FromBody] Fechas_Usuario fechasUsuario)
        {
            if (id != fechasUsuario.Id)
            {
                return BadRequest("El ID proporcionado no coincide con el ID del recurso.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica que el usuario exista
            var usuario = await _context.Usuarios.FindAsync(fechasUsuario.UsuarioId);
            if (usuario == null)
            {
                ModelState.AddModelError("UsuarioId", "UsuarioId es requerido y debe ser un usuario existente.");
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
                // Asocia el usuario y actualiza la entrada
                fechasUsuario.Usuario = usuario; // Relacionar con el usuario encontrado
                _context.Entry(fechasUsuario).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FechasUsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }

            return NoContent();
        }

        // POST: api/Fechas_Usuario
        // POST: api/Fechas_Usuario
// Fechas_UsuarioController.cs

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



        // DELETE: api/Fechas_Usuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFechasUsuario(int id)
        {
            var fechasUsuario = await _context.Fechas_Usuarios.FindAsync(id);
            if (fechasUsuario == null)
            {
                return NotFound();
            }

            _context.Fechas_Usuarios.Remove(fechasUsuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FechasUsuarioExists(int id)
        {
            return _context.Fechas_Usuarios.Any(e => e.Id == id);
        }
    }
}
