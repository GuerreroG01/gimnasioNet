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

        // POST: api/Usuarios
[HttpPost]
public async Task<ActionResult<Usuarios>> PostUsuarios([FromForm] Usuarios usuario, IFormFile? foto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    if (foto != null)
    {
        var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(foto.FileName);
        var filePath = Path.Combine(_imagePath, newFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await foto.CopyToAsync(stream);
        }

        usuario.Foto = newFileName;
    }
    else
    {
        usuario.Foto = "Default.png";
    }

    _context.Usuarios.Add(usuario);

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al agregar el usuario: " + ex.Message });
    }

    return CreatedAtAction(nameof(GetUsuarios), new { id = usuario.Codigo }, usuario);
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
