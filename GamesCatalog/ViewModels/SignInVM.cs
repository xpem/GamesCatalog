using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamesCatalog.Views;
using Models.Resps;
using Services;

namespace GamesCatalog.ViewModels
{
    public partial class SignInVM(IUserService userService) : ViewModelBase
    {
        private string email, password, signInText = "Sign In";

        bool btnSignEnabled = true;
        public string Email { get => email; set { if (email != value) { SetProperty(ref (email), value); } } }

        public string Password { get => password; set { if (password != value) { SetProperty(ref (password), value); } } }

        public string SignInText { get => signInText; set { if (signInText != value) { SetProperty(ref (signInText), value); } } }

        public bool BtnSignEnabled { get => btnSignEnabled; set { if (btnSignEnabled != value) { SetProperty(ref (btnSignEnabled), value); } } }

        [RelayCommand]
        private async Task SignIn()
        {         
            try
            {
                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                {
                    await Application.Current.Windows[0].Page.DisplayAlert("Warning", "Input the email and password", null, "Ok");
                    return;
                }

                if (!(Connectivity.NetworkAccess == NetworkAccess.Internet))
                {
                    await Application.Current.Windows[0].Page.DisplayAlert("Warning", "No Internet Connection", null, "Ok");
                    return;
                }

                if (Password.Length <= 3)
                {
                    await Application.Current.Windows[0].Page.DisplayAlert("Warning", "Invalid password", null, "Ok");
                    return;
                }

                IsBusy = true;
                BtnSignEnabled = false;

                ServiceResp resp = await userService.SignInAsync(Email, Password);


                if (resp.Success)
                {
                    if (resp.Content is not null and int)
                        ((App)App.Current).Uid = (int)resp.Content;

                    _ = Shell.Current.GoToAsync($"//{nameof(Main)}");
                }
                else
                {
                    string errorMessage = "";

                    if (resp.Content is not null and ErrorTypes error)
                    {
                        if (error == ErrorTypes.WrongEmailOrPassword)
                            errorMessage = "Wrong email/password";
                        else if (error == ErrorTypes.ServerUnavaliable)
                            errorMessage = "Server unavailable";
                    }
                    else throw new Exception("Invalid Content");

                    await Application.Current.Windows[0].Page.DisplayAlert("Warnig", errorMessage, null, "Ok");
                }

                BtnSignEnabled = true;
                IsBusy = false;
            }
            catch { throw; }
        }

        [RelayCommand]
        private async Task SignUp() => await Shell.Current.GoToAsync($"{nameof(SignUp)}");

        [RelayCommand]
        private async Task RecoverPassword() => await Shell.Current.GoToAsync($"{nameof(UpdatePassword)}");
    }
}
