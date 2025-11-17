using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel; // Adicione isso
using System.Runtime.CompilerServices; // Adicione isso
using Map = Microsoft.Maui.Controls.Maps.Map;
using AppRpgEtec.Services.Usuarios;
using System.Collections.ObjectModel;
using AppRpgEtec.Models;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class LocalizacaoViewModel : INotifyPropertyChanged
    {
        private Map meuMapa;
        private UsuarioService uService;
        public LocalizacaoViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService(token);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public Map MeuMapa
        {
            get => meuMapa;
            set
            {
                if (value != null)
                {
                    meuMapa = value;
                    OnPropertyChanged();
                }
            }
        }

        // Método para disparar o evento PropertyChanged
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void InicializarMapa()
        {
            try
            {
                //Coordenadas geograficas da escola
                Location location = new Location(-23.5200241d, -46.596498d);
                Pin pinEtec = new Pin()
                {
                    Type = PinType.Place,
                    Label = "Etec Horácio",
                    Address = "Rua alcântara, 113, Vila Guilherme",
                    Location = location
                };

                Map map = new Map();
                MapSpan mapSpan = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(5));
                map.Pins.Add(pinEtec);
                map.MoveToRegion(mapSpan);

                meuMapa = map;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        public async void ExibirUsuariosNoMapa()
        {
            try
            {
                ObservableCollection<Usuario> ocUsuarios = await uService.GetUsuariosAsync();
                List<Usuario> listaUsuarios = new List<Usuario>(ocUsuarios);
                Map map = new Map();

                foreach (Usuario u in listaUsuarios)
                {
                    if (u.Latitude != null && u.Longitude != null)
                    {
                        double latitude = (double)u.Latitude;
                        double longitude = (double)u.Longitude;
                        Location location = new Location(latitude, longitude);

                        Pin pinAtual = new Pin()
                        {
                            Type = PinType.Place,
                            Label = u.Username,
                            Address = $"E-mail: {u.Email}",
                            Location = location
                        };
                        map.Pins.Add(pinAtual);
                    }
                }
                MeuMapa = map;
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", ex.Message, "OK");
            }
        }
    }
}
