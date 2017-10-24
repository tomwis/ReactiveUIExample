using ReactiveUI;
using ReactiveUIExampleXamarinForms.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUIExampleXamarinForms.ViewModels
{
#if !DEBUG2
    public class MainViewModel : ReactiveObject
    {
        IMovieService _movieService;

        public MainViewModel(IMovieService movieService)
        {
            _movieService = movieService;

            Results = new ReactiveList<string>();

            var canSearch = this.WhenAny(x => x.SearchTerm, x => !string.IsNullOrWhiteSpace(x.Value) && x.Value.Length >= 3);
            SearchCmd = ReactiveCommand.CreateFromTask<string, IEnumerable<string>>(term => _movieService.GetMovies(term), canSearch);
            SearchCmd.Subscribe(results =>
            {
                Results.Clear();
                Results.AddRange(results);
            });

            _isSearching = SearchCmd.IsExecuting.ToProperty(this, vm => vm.IsSearching);

            this.WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .InvokeCommand(SearchCmd);
        }

        public ReactiveCommand<string, IEnumerable<string>> SearchCmd { get; set; }
        public ReactiveList<string> Results { get; set; }

        string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set { this.RaiseAndSetIfChanged(ref _searchTerm, value); }
        }
        
        readonly ObservableAsPropertyHelper<bool> _isSearching;
        public bool IsSearching => _isSearching.Value;      
    }
#else
    public class MainViewModel : ReactiveObject
    {
        IMovieService _movieService;

        public MainViewModel(IMovieService movieService)
        {
            _movieService = movieService;

            Results = new ReactiveList<string>();
            
            var canSearch = this.WhenAnyValue(vm => vm.SearchTerm)
                .Where(x => !string.IsNullOrWhiteSpace(x) && x.Length >= 3)
                .Publish()
                .RefCount();
            
            SearchCmd = ReactiveCommand.CreateFromObservable<string, IEnumerable<string>>(term => Observable.StartAsync(_ => _movieService.GetMovies(term)).TakeUntil(canSearch));

            SearchCmd.Subscribe(results =>
            {
                Results.Clear();
                Results.AddRange(results);
            });

            _isSearching = SearchCmd.IsExecuting.ToProperty(this, vm => vm.IsSearching);

            canSearch
                .Throttle(TimeSpan.FromMilliseconds(500))
                .InvokeCommand(SearchCmd);
        }
        
        public ReactiveCommand<string, IEnumerable<string>> SearchCmd { get; set; }
        public ReactiveList<string> Results { get; set; }

        string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set { this.RaiseAndSetIfChanged(ref _searchTerm, value); }
        }

        readonly ObservableAsPropertyHelper<bool> _isSearching;
        public bool IsSearching => _isSearching.Value;
    }
#endif
}
