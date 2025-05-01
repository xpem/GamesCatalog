using CommunityToolkit.Mvvm.ComponentModel;

namespace GamesCatalog.ViewModels
{
    public partial class UserStateVM : ObservableObject
    {
        [ObservableProperty]
        private string email;
        [ObservableProperty]
        private string name;
    }
}
