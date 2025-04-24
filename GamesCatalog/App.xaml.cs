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

        public App(IBuildDbService buildDbService,IUserService userService)
        {
            BuildDbService = buildDbService;
            UserService = userService;

            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            BuildDbService.Init();

            UserDTO? user = UserService.GetUserAsync().Result;

            if (user != null)
            {
                Uid = user.Id;
            }

            return new Window(new AppShell());
        }
    }
}