using backendnet.Data;
using backendnet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backendnet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeliculasController(DataContext context) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pelicula>>> GetPeliculas(string? s)
    {
        if (string.IsNullOrEmpty(s)) 
            return await context.Pelicula.Include(i => i.Categorias).AsNoTracking().ToListAsync();

        return await context.Pelicula.Include(i => i.Categorias).Where(p => p.Titulo.Contains(s)).AsNoTracking().ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Pelicula>> GetPelicula(int id)
    {
        var pelicula = await context.Pelicula.Include(i => i.Categorias).AsNoTracking().FirstOrDefaultAsync(p => p.PeliculaId == id);
        if (pelicula == null) return NotFound();

        return pelicula;
    }

    [HttpPost]
    public async Task<ActionResult<Pelicula>> PostPelicula(PeliculaDTO peliculaDTO)
    {
        var pelicula = new Pelicula
        {
            Titulo = peliculaDTO.Titulo,
            Sinopsis = peliculaDTO.Sinopsis,
            Anio = peliculaDTO.Anio,
            Poster = peliculaDTO.Poster,
            Categorias = []
        };

        context.Pelicula.Add(pelicula);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPelicula), new { id = pelicula.PeliculaId }, pelicula);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPelicula(int id, PeliculaDTO peliculaDTO)
    {
        if (id != peliculaDTO.PeliculaId) return BadRequest();

        var pelicula = await context.Pelicula.FirstOrDefaultAsync(p => p.PeliculaId == id);
        if (pelicula == null) return NotFound();

        pelicula.Titulo = peliculaDTO.Titulo;
        pelicula.Sinopsis = peliculaDTO.Sinopsis;
        pelicula.Anio = peliculaDTO.Anio;
        pelicula.Poster = peliculaDTO.Poster;
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePelicula(int id)
    {
        var pelicula = await context.Pelicula.FindAsync(id);
        if (pelicula == null) return NotFound();

        context.Pelicula.Remove(pelicula);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/categoria")]
    public async Task<ActionResult> PostCategoriaPelicula(int id, AsignaCategoriaDTO itemToAdd)
    {
        Categoria? categoria = await context.Categoria.FindAsync(itemToAdd.CategoriaId);
        if (categoria == null) return NotFound();

        var pelicula = await context.Pelicula.Include(i => i.Categorias).FirstOrDefaultAsync(p => p.PeliculaId == id);
        if (pelicula == null) return NotFound();

        if (pelicula?.Categorias?.FirstOrDefault(categoria) != null) 
        {
            pelicula.Categorias.Add(categoria);
            await context.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpDelete("{id}/categoria/{categoriaid}")]
    public async Task<ActionResult> DeleteCategoriaPelicula(int id, int categoriaid) {
        Categoria? categoria = await context.Categoria.FindAsync(categoriaid);
        if (categoria == null) return NotFound();

        var pelicula = await context.Pelicula.Include(i => i.Categorias).FirstOrDefaultAsync(p => p.PeliculaId == id);
        if (pelicula == null) return NotFound();

        if (pelicula?.Categorias?.FirstOrDefault(categoria) != null) 
        {
            pelicula.Categorias.Remove(categoria);
            await context.SaveChangesAsync();
        }

        return NoContent();
    }
}
