using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services;
using System.Text.RegularExpressions;

namespace GamesCatalog.ViewModels;

public partial class UpdatePasswordVM(IUserService userService) : ViewModelBase
{
    [ObservableProperty]
    string email;

    [RelayCommand]
    private async Task UpdatePassword()
    {
        if (!(Connectivity.NetworkAccess == NetworkAccess.Internet))
        {
            await Application.Current.Windows[0].Page.DisplayAlert("Warning", "No internet connection", null, "Ok");
            return;
        }

        if (string.IsNullOrEmpty(Email) || (!ValidateEmail(Email)))
        {
            await Application.Current.Windows[0].Page.DisplayAlert("Warning", "Enter a valid email", null, "Ok");
            return;
        }
        else
        {
            _ = userService.RecoverPasswordAsync(Email);

            await Application.Current.Windows[0].Page.DisplayAlert("Warning", "Password reset email sent!", null, "Ok");

            await Shell.Current.GoToAsync("..");
        }
    }

    public static bool ValidateEmail(string email)
          => Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

}
