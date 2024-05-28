using System.Data.Common;
using backendnet.Data;
using backendnet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backendnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController(DataContext context) : Controller
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            return await context.Categoria.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await context.Categoria.FindAsync(id);
            if (categoria == null) return NotFound();

            return categoria;
        }

        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(CategoriaDTO categoriaDTO)
        {
            var categoria = new Categoria
            {
                Nombre = categoriaDTO.Nombre
            };

            context.Categoria.Add(categoria);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.CategoriaId }, categoria);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, CategoriaDTO categoriaDTO)
        {
            if (id != categoriaDTO.CategoriaId) return BadRequest();

            var categoria = await context.Categoria.FindAsync(id);
            if (categoria == null) return NotFound();

            categoria.Nombre = categoriaDTO.Nombre;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await context.Categoria.FindAsync(id);
            if (categoria == null) return NotFound();

            if (categoria.Protegida) return BadRequest();

            context.Categoria.Remove(categoria);
            await context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
