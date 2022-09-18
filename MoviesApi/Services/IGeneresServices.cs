using MoviesApi.Models;

namespace MoviesApi.Services
{
    public interface IGeneresServices
    {
        Task<IEnumerable<Genre>> GetAll();
        Task<Genre> GetById(byte id);
        Task<Genre> Add(Genre genre);
        Genre Update(Genre genre);
        Genre Delete(Genre genre);

        Task<bool> IsValidGenre(byte id);
    }
}
