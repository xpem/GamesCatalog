using GamesCatalog.ViewModels;

namespace GamesCatalog
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage(MainPageVM mainPageVM)
        {
            InitializeComponent();
            this.BindingContext = mainPageVM;
        }
    }

}
