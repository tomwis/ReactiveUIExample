using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUIExampleXamarinForms.Services
{
    public interface IMovieService
    {
        Task<IEnumerable<string>> GetMovies(string searchTerm);
    }
}
