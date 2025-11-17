using AppRpgEtec.Models;
using AppRpgEtec.Services.Usuarios;
using AppRpgEtec.Views.Personagens;
using AppRpgEtec.Views.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class UsuarioViewModel : BaseViewModel
    {
        private UsuarioService uService;
        public ICommand AutenticarCommand { get; set; }
        public ICommand RegistrarCommand { get; set; }
        public ICommand DirecionarCadastroCommand { get; set; }

        public UsuarioViewModel()
        {
            uService = new UsuarioService();
            InicializarCommands();
        }

        public void InicializarCommands()
        {
            AutenticarCommand = new Command(async () => await AutenticarUsuario());
            RegistrarCommand = new Command(async () => await RegistrarUsuario());
            DirecionarCadastroCommand = new Command(async () => await DirecionarParaCadastro());
        }

        #region AtributosPropriedades
        //As propriedades serão chamadas na View futuramente

        private string login = string.Empty;
        public string Login
        {
            get { return login; }
            set 
            { 
                login = value;
                OnPropertyChanged();
            }
        }

        private string senha = string.Empty;
        public string Senha
        {
            get { return senha; }
            set 
            {
                senha = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;

        public async Task AutenticarUsuario()//Método para autenticar um usuário
        {
            try
            {
                Usuario u = new Usuario();
                u.Username = login; 
                u.PasswordString = senha;

                Usuario uAutenticado = await uService.PostAutenticarUsuarioAsync(u);

                if (!string.IsNullOrEmpty(uAutenticado.Token))
                {
                    string mensagem = $"Bem-vindo(a) {uAutenticado.Username}";

                    //Guardando dados do usuário para uso futuro
                    Preferences.Set("UsuarioId", uAutenticado.Id);
                    Preferences.Set("UsuarioUsername", uAutenticado.Username);
                    Preferences.Set("UsuarioPerfil", uAutenticado.Perfil);
                    Preferences.Set("UsuarioToken", uAutenticado.Token);

                    //Inicio da coleta de Geolocalização atual para Atualização na API
                    _isCheckingLocation = true;
                    _cancelTokenSource = new CancellationTokenSource();
                    GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                    Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                    Usuario uLoc = new Usuario();
                    uLoc.Id = uAutenticado.Id;
                    uLoc.Latitude = location.Latitude;
                    uLoc.Longitude = location.Longitude;

                    UsuarioService uServiceLoc = new UsuarioService(uAutenticado.Token);
                    await uServiceLoc.PutAtualizarLocalizacaoAsync(uLoc);
                    //Fim da coleta de Geolocalização atual para Atualização na API

                    await Application.Current.MainPage
                        .DisplayAlert("Informação", mensagem, "Ok");

                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await Application.Current.MainPage
                        .DisplayAlert("Informação", "Dados incorretos :(", "Ok");
                }
            }
            catch (Exception ex) 
            {
                await Application.Current.MainPage
                    .DisplayAlert("Informação", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }

        #region Métodos
        public async Task RegistrarUsuario()//Método para registrar um usuário
        {
            try
            {
                Usuario u = new Usuario();
                u.Username = login;
                u.PasswordString = senha;

                Usuario uRegistrado = await uService.PostRegistrarUsuarioAsync(u);

                if (uRegistrado.Id != 0)
                {
                    string mensagem = $"Usuário Id {uRegistrado.Id} registrado com sucesso.";
                    await Application.Current.MainPage.DisplayAlert("Informação", mensagem, "Ok");

                    await Application.Current.MainPage.Navigation.PopAsync();//Remove a pagina da pilha de navegação
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Informação", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }
        #endregion

        public async Task DirecionarParaCadastro()//Método para exibição da View de cadastro
        {
            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new CadastroView());
            }
            catch (Exception ex) 
            {
                await Application.Current.MainPage.DisplayAlert("Informação", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }
    }
}