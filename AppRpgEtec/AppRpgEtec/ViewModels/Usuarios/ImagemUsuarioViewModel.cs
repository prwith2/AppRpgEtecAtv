using System.Windows.Input;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Usuarios;
using Azure.Storage.Blobs;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class ImagemUsuarioViewModel : BaseViewModel
    {
        private UsuarioService uService;
        private static string conexaoAzureStorage = "DefaultEndpointsProtocol=https;AccountName=lupastorage;AccountKey=V5OUXopGQhXe8fhI/B1KJVo8wdBW3pQ8W0wdb4qNMUs2QlIVTS2Aic3q3WQO3V87m5SYBn4j0zpk+AStsyaymQ==;EndpointSuffix=core.windows.net";
        private static string container = "arquivos";

        public static string ConexaoAzureStorage => conexaoAzureStorage;
        public static string Container => container;

        public ImagemUsuarioViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService(token);

            FotografarCommand = new Command(Fotografar);
            SalvarImagemCommand = new Command(SalvarImagemAzure);
            AbrirGaleriaCommand = new Command(AbrirGaleria);

            CarregarUsuarioAzure();
        }

        public ICommand FotografarCommand { get; }

        public ICommand SalvarImagemCommand { get; }
        public ICommand AbrirGaleriaCommand { get; }

        private ImageSource fonteImagem;

        public ImageSource FonteImagem
        {
            get { return fonteImagem; }
            set
            {
                fonteImagem = value;
                OnPropertyChanged();
            }
        }

        private byte[] foto;

        public byte[] Foto
        {
            get => foto;
            set
            {
                OnPropertyChanged();
            }
        }

        public async void Fotografar()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        using (Stream sourceStream = await photo.OpenReadAsync())
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                await sourceStream.CopyToAsync(ms); //Conversão do Stream para MemoryStream (arquivo em memoria)

                                //Carregamento do array de bytes a partir do memoria para a propriedade da ViewModel
                                Foto = ms.ToArray();

                                //Carregamento do controle que apresenta a imagem para a ViewModel
                                FonteImagem = ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void SalvarImagemAzure()
        {
            try
            {
                Usuario u = new Usuario();
                u.Foto = foto;
                u.Id = Preferences.Get("UsuarioId", 0);

                string fileName = $"{u.Id}.jpg";

                //define o BLOB no qual a imagem sera armazenada
                var blobClient = new BlobClient(conexaoAzureStorage, container, fileName);

                if (blobClient.Exists())
                    blobClient.Delete();

                using (var stream = new MemoryStream(u.Foto))
                {
                    blobClient.Upload(stream);
                }

                await Application.Current.MainPage.DisplayAlert("Mensagem", "Dados salvos com sucesso!", "Ok");
                await App.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void AbrirGaleria()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult photo = await MediaPicker.Default.PickPhotoAsync();

                    if (photo != null)
                    {
                        using (Stream sourceStream = await photo.OpenReadAsync())
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                await sourceStream.CopyToAsync(ms); //Conversão do Stream para MemoryStream (arquivo em memoria)

                                //Carregamento do array de bytes a partir do memoria para a propriedade da ViewModel
                                Foto = ms.ToArray();

                                //Carregamento do controle que apresenta a imagem para a ViewModel
                                FonteImagem = ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void CarregarUsuarioAzure()
        {
            try
            {
                int usuarioId = Preferences.Get("UsuarioId", 0);
                string filename = $"{usuarioId}.jpg";
                var blobClient = new BlobClient(conexaoAzureStorage, container, filename);

                if (blobClient.Exists())
                {
                    Byte[] fileBytes;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        blobClient.OpenRead().CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    Foto = fileBytes;
                }
            }
            catch (Exception ex) 
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }
    }
}
