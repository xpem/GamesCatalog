using GamesCatalog.Utils.Sync;
using GamesCatalog.Views;
using Models.DTOs;
using Services;
using Services.Interfaces;

namespace GamesCatalog.ViewModels
{
    public partial class FirstSyncProcessVM : ViewModelBase
    {
        private decimal progress;

        public decimal Progress { get => progress; set { if (progress != value) { SetProperty(ref (progress), value); } } }

        private readonly IUserService UserService;
        private readonly IGameApiService GameApiService;
        private readonly IGameService GameService;
        private readonly ISyncService SyncService;
        private readonly IApiOperationService ApiOperationService;
        private readonly AppShellVM AppShellVM;

        public FirstSyncProcessVM(IUserService userService, IGameApiService gameApiService, IGameService gameService, ISyncService syncService, IApiOperationService apiOperationService, AppShellVM appShellVM)
        {
            UserService = userService;
            GameApiService = gameApiService;
            GameService = gameService;
            SyncService = syncService;
            ApiOperationService = apiOperationService;
            AppShellVM = appShellVM;
            _ = SynchronizingProcess();
        }


        private async Task SynchronizingProcess()
        {
            try
            {
                UserDTO user = await UserService.GetAsync();

                if (user != null)
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        Progress = 0.25M;

                        await GameSyncService.ApiToLocalAsync(GameApiService, GameService, user.Id, user.LastUpdate);

                        Progress = 0.5M;

                        await UserService.UpdateLastUpdate(user.Id);

                        Progress = 1;

                        _ = Task.Run(() => { Task.Delay(5000); SyncService.StartThread(); });

                        _ = AppShellVM.AtualizaUserShowData();

                        _ = Shell.Current.GoToAsync($"//{nameof(Main)}");

                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
