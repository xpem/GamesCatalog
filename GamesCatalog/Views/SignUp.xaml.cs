using GamesCatalog.ViewModels;

namespace GamesCatalog.Views;

public partial class SignUp : ContentPage
{
	public SignUp(SignUpVM signUpVM)
	{
		InitializeComponent();

        BindingContext = signUpVM;
    }
}