using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dtos;
using MoviesApi.Models;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGeneresServices _generesServices;
        public GenresController(IGeneresServices generesServices)
        {
            _generesServices = generesServices;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genre = await _generesServices.GetAll();
            return Ok(genre);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(GenreDto dto)
        {
            var genre = new Genre { Name = dto.Name };
            await _generesServices.Add(genre);
            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(byte id,[FromBody] GenreDto dto)
        {
            var genre = await _generesServices.GetById(id);
            if (genre == null)
                return NotFound($"No genre was found with ID: {id}");
            genre.Name = dto.Name;
            _generesServices.Update(genre);
            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            var genre = await _generesServices.GetById(id);
            if (genre == null)
                return NotFound($"No genre was found with ID: {id}");
            _generesServices.Delete(genre);
            return Ok(genre);
        }
    }
}
