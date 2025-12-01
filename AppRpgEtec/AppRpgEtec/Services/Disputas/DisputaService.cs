using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static UIKit.UIGestureRecognizer;
using AppRpgEtec.Models;
using System.Threading.Tasks;

namespace AppRpgEtec.Services.Disputas
{
    public class DisputaService : Request
    {
        private readonly Request _request;
        private string _token;

        private const string _apiUrlBase = "https://xyz/Disputas";

        public DisputaService(string token)
        {
            _request = new Request();
            _token = token;
        }
        public async Task<Disputa> PostDisputaComArmaAsync(Disputa d)
        {
            string urlComplementar = "/Arma";
            return await _request.PostAsync(_apiUrlBase + urlComplementar, d, _ token);
        }
        public async Task<Disputa> PostDisputaComHabilidadesAsync(Disputa d)
        {
            string urlComplementar = "/Habilidade";
            return await_request.PostAsync(_apiUrlBase + urlComplementar, d, _token);
        }
        public async Task<Disputa> PostDisputaGeralAsync(Disputa d)
        {
            string urlComplementar = "/DisputaEmGrupo";
            return await _request.PostAsync(_apiUrlBase + urlComplementar, d, token);
        }
    }

}
