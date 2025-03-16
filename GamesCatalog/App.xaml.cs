using Services;

namespace GamesCatalog
{
    public partial class App : Application
    {
        private IBuildDbService BuildDbService { get; set; }

        public App(IBuildDbService buildDbService)
        {
            BuildDbService = buildDbService;

            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            BuildDbService.Init();

            return new Window(new AppShell());
        }
    }
}