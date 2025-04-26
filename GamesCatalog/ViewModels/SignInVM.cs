using CommunityToolkit.Mvvm.Input;
using GamesCatalog.Views;

namespace GamesCatalog.ViewModels
{
    public partial class SignInVM : ViewModelBase
    {
        private string email, password, signInText = "Sign In";

        bool btnSignEnabled = true;

        public string Email { get => email; set { if (email != value) { SetProperty(ref (email), value); } } }

        public string Password { get => password; set { if (password != value) { SetProperty(ref (password), value); } } }

        public string SignInText { get => signInText; set { if (signInText != value) { SetProperty(ref (signInText), value); } } }

        public bool BtnSignEnabled { get => btnSignEnabled; set { if (btnSignEnabled != value) { SetProperty(ref (btnSignEnabled), value); } } }

        [RelayCommand]
        private async Task SignIn() { }

        [RelayCommand]
        private async Task SignUp() => await Shell.Current.GoToAsync($"{nameof(SignUp)}");

        [RelayCommand]
        private async Task RecoverPassword() => await Shell.Current.GoToAsync($"{nameof(UpdatePassword)}");
    }
}
