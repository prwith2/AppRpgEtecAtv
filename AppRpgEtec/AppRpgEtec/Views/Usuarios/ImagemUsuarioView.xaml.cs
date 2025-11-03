using AppRpgEtec.ViewModels.Usuarios;
namespace AppRpgEtec.Views.Usuarios;

public partial class ImagemUsuarioView : ContentPage
{
    ImagemUsuarioViewModel viewModel;
    public ImagemUsuarioView()
	{
		InitializeComponent();
		viewModel = new ImagemUsuarioViewModel();
		Title = "Imagem do Usuário";
		BindingContext = new ImagemUsuarioViewModel();
	}

    private void InitializeComponent()
    {
        throw new NotImplementedException();
    }
}