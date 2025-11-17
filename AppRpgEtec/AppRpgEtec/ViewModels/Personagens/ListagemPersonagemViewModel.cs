using AppRpgEtec.Models;
using AppRpgEtec.Services.Personagens;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Personagens
{
    public class ListagemPersonagemViewModel : BaseViewModel
    {
        private PersonagemService pService;
        private Personagem personagemSelecionado;
        public ObservableCollection<Personagem> Personagens { get; set; }
        public ListagemPersonagemViewModel() 
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(token);
            Personagens = new ObservableCollection<Personagem>();

            _ = ObterPersonagens();
            NovoPersonagemCommand = new Command(async () => { await ExibirCadastroPersonagem(); });
            RemoverPersonagemCommand = new Command<Personagem>(async (Personagem p) => { await RemoverPersonagem(p); });
        }
        public ICommand NovoPersonagemCommand { get; }
        public ICommand RemoverPersonagemCommand { get; set; }
        public Personagem PersonagemSelecionado
        {
            get { return personagemSelecionado; }
            set
            {
                if (value != null)
                {
                    personagemSelecionado = value;

                    Shell.Current.GoToAsync($"cadPersonagemView?pId={personagemSelecionado.Id}");
                }
            }
        }

        public async Task ObterPersonagens()
        {
            try // Junto com o catch evitara que os erros fechem o aplicativo
            {
                Personagens = await pService.GetPersonagensAsync();
                OnPropertyChanged(nameof(Personagens)); //Informara a View que houve carregamento
            }
            catch (Exception ex) 
            {
                //Captara o erro para exibir em tela
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async Task ExibirCadastroPersonagem()
        {
            try
            {
                await Shell.Current.GoToAsync("cadPersonagemView");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async Task RemoverPersonagem(Personagem p)
        {
            try
            {
                if(await Application.Current.MainPage.DisplayAlert("Confirmação", $"Confirmar a remoção de {p.Nome}?", "Sim", "Não"))
                {
                    await pService.DeletePersonagemAsync(p.Id);
                    await Application.Current.MainPage.DisplayAlert("Mensagem", "Personagem removido com sucesso!", "Ok");

                    _ = ObterPersonagens();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }
    }
}
