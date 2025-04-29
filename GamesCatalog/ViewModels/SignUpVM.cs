using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Resps;
using Services;
using System.Text.RegularExpressions;

namespace GamesCatalog.ViewModels
{
    public partial class SignUpVM(IUserService userService) : ViewModelBase
    {
        [ObservableProperty]
        string name, email, password, confirmPassword;

        [ObservableProperty]
        private bool btnCreateUserIsEnabled = true;

        private bool VerifyFileds()
        {
            bool validInformation = true;

            if (string.IsNullOrEmpty(Name))
                validInformation = false;

            if (string.IsNullOrEmpty(Email))
                validInformation = false;
            else if (!ValidateEmail(Email))
            {
                _ = Application.Current.Windows[0].Page.DisplayAlert("Warning", "Invalid Email", null, "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(Password)) validInformation = false;
            else if (Password.Length < 4) validInformation = false;

            if (string.IsNullOrEmpty(ConfirmPassword)) validInformation = false;
            else if (!ConfirmPassword.Equals(Password, StringComparison.CurrentCultureIgnoreCase)) validInformation = false;

            if (!validInformation)
                _ = Application.Current.Windows[0].Page.DisplayAlert("Warning", "Input the email and password correctly", null, "Ok");

            return validInformation;
        }

        public static bool ValidateEmail(string email)
            => Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

        [RelayCommand]
        private async Task SignUp()
        {
            if (!(Connectivity.NetworkAccess == NetworkAccess.Internet))
            {
                _ = await Application.Current.Windows[0].Page.DisplayAlert("Warning", "No Internet Connection.", null, "Ok");
                return;
            }

            if (VerifyFileds())
            {
                BtnCreateUserIsEnabled = false;

                //
                ServiceResp resp = await userService.SignUpAsync(name, email, password);

                if (!resp.Success)
                    await Application.Current.Windows[0].Page.DisplayAlert("Error", "It's not possible create the user.", null, "Ok");
                else
                {
                    if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.Android)
                    {
                        ToastDuration duration = ToastDuration.Short;

                        var toast = Toast.Make("User Created", duration, 15);
                        await toast.Show();
                    }
                    else
                        await Application.Current.Windows[0].Page.DisplayAlert("Success", "User Created", null, "Ok");
                }

                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
