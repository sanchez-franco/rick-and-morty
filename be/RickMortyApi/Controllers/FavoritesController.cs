using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RickMortyApi.Database;
using RickMortyApi.Models;
using RickMortyApi.WebSockets;

namespace RickMortyApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly RickAndMortyDbContext _dbContext;
        private readonly FavoritesWebSocketHandler _favoritesWebSocketHandler;

        public FavoritesController(RickAndMortyDbContext dbContext, FavoritesWebSocketHandler favoritesWebSocketHandler)
        {
            _dbContext = dbContext;
            _favoritesWebSocketHandler = favoritesWebSocketHandler;
        }

        [HttpGet]
        public ActionResult<IList<FavoriteDto>> Get()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return StatusCode(500, "prueba email");
            }

            var favorites = _dbContext.Favorites
            .Where(f => f.Email == email)
            .Select(f => new FavoriteDto
            {
                Id = f.Id,
                Email = f.Email,
            })
            .ToList();

            return Ok(favorites);
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody] FavoriteDto item)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (item == null || item.Id < 1 || string.IsNullOrEmpty(email))
            {
                return StatusCode(500, "prueba");
            }
            bool exists = _dbContext.Favorites.Any(f => f.Email == email && f.Id == item.Id);
            if (exists)
            {
                return StatusCode(409, "El favorito ya está agregado para este usuario.");
            }
            item.Email = email;
            try
            {
                await _dbContext.Favorites.AddAsync(new FavoriteDb { Email = item.Email, Id = item.Id });
                await _dbContext.SaveChangesAsync();

                await _favoritesWebSocketHandler.BroadcastAsync(email, new WebSocketModel { Favorites = new List<int> { item.Id } });

                return Ok("Favorite agregado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar: {ex.Message} {(ex.InnerException != null ? ex.InnerException.Message : "")}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> Delete(int id)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (id < 1 || string.IsNullOrEmpty(email))
            {
                return StatusCode(500, "item not found");
            }
            var favorite = await _dbContext.Favorites.FirstOrDefaultAsync(f => f.Id == id && f.Email == email);
            if (favorite == null)
            {
                return NotFound("Favorite no encontrado o no pertenece al usuario.");
            }
            try
            {
                _dbContext.Favorites.Remove(favorite);
                await _dbContext.SaveChangesAsync();
                await _favoritesWebSocketHandler.BroadcastAsync(email, new WebSocketModel { IsRemoved = true, Favorites = new List<int> { id } });
                return Ok("Favorite eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar: {ex.Message} {(ex.InnerException != null ? ex.InnerException.Message : "")}");
            }
        }


    }
}