using GamesCatalog.ViewModels;

namespace GamesCatalog
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellVM appShellVM, UserStateVM userStateVM)
        {
            InitializeComponent();

            BindingContext = appShellVM;

            if (this.FlyoutHeader is BindableObject flyoutHeader)
            {
                flyoutHeader.BindingContext = userStateVM;
            }
        }
    }
}
