using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ReactiveUIExampleXamarinForms.ViewModels;
using ReactiveUIExampleXamarinForms.Services;

namespace ReactiveUIExampleXamarinForms
{
    public partial class MainPage : ContentPage, IViewFor<MainViewModel>
    {
        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainViewModel(new MovieService());
            this.Bind(ViewModel, vm => vm.SearchTerm, v => v.entryField.Text);
            this.OneWayBind(ViewModel, vm => vm.Results, v => v.resultList.ItemsSource);
            this.OneWayBind(ViewModel, vm => vm.IsSearching, v => v.loadingBar.IsVisible);
        }

        public MainViewModel ViewModel { get; set; }
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainViewModel)value;
        }
    }
}
