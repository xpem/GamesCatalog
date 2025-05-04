using GamesCatalog.Utils.Sync;
using GamesCatalog.ViewModels;
using Models.DTOs;
using Services;
using Services.Interfaces;

namespace GamesCatalog
{
    public partial class App : Application
    {
        public int? Uid { get; set; }

        private IBuildDbService BuildDbService { get; set; }

        private ISyncService SyncService { get; set; }

        private IUserService UserService { get; set; }

        private UserStateVM UserStateVM { get; set; }

        public App(IBuildDbService buildDbService, IUserService userService, UserStateVM userStateVM, ISyncService syncService)
        {
            BuildDbService = buildDbService;
            UserService = userService;
            UserStateVM = userStateVM;
            SyncService = syncService;

            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var appShellVM = new AppShellVM(SyncService, BuildDbService, UserService, UserStateVM);

            BuildDbService.Init();

            _ = appShellVM.AtualizaUserShowData();

            UserDTO? user = UserService.GetAsync().Result;

            if (user != null)
            {
                Uid = user.Id;
                SyncService.StartThread();

            }

            return new Window(new AppShell(appShellVM, UserStateVM));
        }
    }
}