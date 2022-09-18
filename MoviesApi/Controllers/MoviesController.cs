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

    public class MoviesController : ControllerBase
    {
        private readonly IMoviesServices _moviesServices;
        private readonly IGeneresServices _generesServices;

        public MoviesController(IMoviesServices moviesServices, IGeneresServices generesServices)
        {
            _moviesServices = moviesServices;
            _generesServices = generesServices;
        }

        private new List<string> _allowedExtensions = new List<string> { ".jpg",".png"};
        private long _maxAllowedPosterSize = 1048576;
        

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movie = await _moviesServices.GetAll();
            return Ok(movie);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _moviesServices.GetById(id);
            if (movie == null)
                return NotFound();

            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm]MovieDto dto)
        {
            if (dto.Poster == null)
                return BadRequest("Poster is required!");

            if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only .jpg and .png are allowed!");

            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("Max allowed size of poster is 1MB!");

            var isValidGenre = await _generesServices.IsValidGenre(dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid genre Id");

            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);
            var movie = new Movie
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Poster = dataStream.ToArray(),
                Rate = dto.Rate,
                Storyline = dto.Storyline,
                Year = dto.Year,
            };
            _moviesServices.Add(movie);
            return Ok(movie);

        }

        [HttpPut ("{id}")]

        public async Task<IActionResult> UpdateAsync (int id, [FromForm] MovieDto dto)
        {
                      
            var movie = await _moviesServices.GetById(id);
            if (movie == null)
                return NotFound($"No movie was found with ID: {id}");

            var isValidGenre = await _generesServices.IsValidGenre(dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid genre Id");

            if (dto.Poster != null)
            {
                if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .jpg and .png are allowed!");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Max allowed size of poster is 1MB!");

                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();
            }


            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.Year = dto.Year;
            movie.Rate = dto.Rate;
            movie.Storyline = dto.Storyline;

            _moviesServices.Update(movie);  
            return Ok(movie);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _moviesServices.GetById(id);
            if (movie == null)
                return NotFound($"No movie was found with ID: {id}");
            _moviesServices.Delete(movie);
            return Ok(movie);
        }
    }
}
