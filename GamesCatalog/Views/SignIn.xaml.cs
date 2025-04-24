using GamesCatalog.ViewModels;

namespace GamesCatalog.Views;

public partial class SignIn : ContentPage
{
	public SignIn(SignInVM signInVM)
	{
		InitializeComponent();

        BindingContext = signInVM;
    }
}