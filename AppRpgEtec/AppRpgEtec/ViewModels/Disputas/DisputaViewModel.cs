using AppRpgEtec.Models;
using AppRpgEtec.Services.Personagens;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Disputas
{
    // Matheus Santos | Luca Kenzo
    public class DisputaViewModel : BaseViewModel
    {
        private PersonagemService pService;
        public ObservableCollection<Personagem> PersonagensEncontrados { get; set; }
        public Personagem Atacante { get; set; }
        public Personagem Oponente { get; set; }
        public DisputaViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(token);

            Atacante = new Personagem();
            Oponente = new Personagem();

            PersonagensEncontrados = new ObservableCollection<Personagem>();

            PesquisarPersonagemCommand = new Command<string>(async (string pesquisa) => { await PesquisarPersonagens(pesquisa); });
        }

        public ICommand PesquisarPersonagemCommand { get; set; }

        public async Task PesquisarPersonagens(string textoPesquisaPersonagem)
        {
            try
            {
                PersonagensEncontrados = await pService.GetByNomeAproximadoAsync(textoPesquisaPersonagem);
                OnPropertyChanged(nameof(PersonagensEncontrados));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public string DescricaoPersonagemAtacante { get => Atacante.Nome; }
        public string DescricaoPersonagemOponente { get => Oponente.Nome; }

        public async void SelecionarPersonagem(Personagem p)
        {
            try
            {
                string tipoCombatente = await Application.Current.MainPage.DisplayActionSheet("Atacante ou Oponente?", "Cancelar", "", "Atacante", "Oponente");

                if (tipoCombatente == "Atacante")
                {
                    Atacante = p;
                    OnPropertyChanged(nameof(DescricaoPersonagemAtacante));
                }
                else if (tipoCombatente == "Oponente")
                {
                    Oponente = p;
                    OnPropertyChanged(nameof(DescricaoPersonagemOponente));
                }
            }
            catch (Exception ex) 
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        private Personagem personagemSelecionado;
        public Personagem PersonagemSelecionado
        {
            set
            {
                if(value != null)
                {
                    personagemSelecionado = value;
                    SelecionarPersonagem(personagemSelecionado);
                    OnPropertyChanged();
                    PersonagensEncontrados.Clear();
                }
            }
        }

        private string textoBuscaDigitado = string.Empty;
        public string TextoBuscaDigitado
        {
            get { return textoBuscaDigitado; }
            set
            {
                //Verifique se não é nulo, se não é vazio e se o tamanho do texto é maior que zero.
                if (value != null && !string.IsNullOrEmpty(value) && value.Length > 0)
                {
                    textoBuscaDigitado = value;
                    _ = PesquisarPersonagens(textoBuscaDigitado);
                }
                else
                {
                    //Limpa o list view que exibe o resultado da pesquisa
                    PersonagensEncontrados.Clear();
                }
            }
        }
    }
}
