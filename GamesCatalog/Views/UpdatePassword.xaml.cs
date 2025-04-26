using GamesCatalog.ViewModels;

namespace GamesCatalog.Views;

public partial class UpdatePassword : ContentPage
{
    public UpdatePassword(UpdatePasswordVM updatePasswordVM)
    {
        InitializeComponent();

        BindingContext = updatePasswordVM;
    }
}