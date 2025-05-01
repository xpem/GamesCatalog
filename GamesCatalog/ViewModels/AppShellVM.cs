using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamesCatalog.Views;
using Models.DTOs;
using Services;
using Services.Interfaces;

namespace GamesCatalog.ViewModels
{
    public partial class AppShellVM(IBuildDbService buildDbService, IUserService userService, UserStateVM userStateVM) : ObservableObject
    {
        public async Task AtualizaUserShowData()
        {
            UserDTO? user = await userService.GetUserAsync();

            if (user is not null)
            {
                userStateVM.Name = user.Name;
                userStateVM.Email = user.Email;
            }
        }

        [RelayCommand]
        private async Task SignOut()
        {

            (App.Current as App).Uid = 0;

            await buildDbService.CleanLocalDatabase();

            _ = Shell.Current.GoToAsync($"//{nameof(SignIn)}");
        }
    }
}
