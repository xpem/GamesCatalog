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

        private IUserService UserService { get; set; }

        private UserStateVM UserStateVM { get; set; }

        public App(IBuildDbService buildDbService, IUserService userService, UserStateVM userStateVM)
        {
            BuildDbService = buildDbService;
            UserService = userService;
            UserStateVM = userStateVM;

            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var appShellVM = new AppShellVM(BuildDbService, UserService, UserStateVM);

            BuildDbService.Init();

            _ = appShellVM.AtualizaUserShowData();

            UserDTO? user = UserService.GetUserAsync().Result;

            if (user != null)
            {
                Uid = user.Id;
            }

            return new Window(new AppShell(appShellVM, UserStateVM));
        }
    }
}