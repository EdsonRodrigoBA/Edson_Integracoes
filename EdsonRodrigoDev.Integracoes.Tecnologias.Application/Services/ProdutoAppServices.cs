using EdsonRodrigoDev.Integracoes.Tecnologias.Domain;
using EdsonRodrigoDev.Integracoes.Tecnologias.Domain.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdsonRodrigoDev.Integracoes.Tecnologias.Application.Services
{
    public class ProdutoAppServices
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public ProdutoAppServices(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task<bool> CriarProduto(Produtos model)
        {
            //Cria produto


            //Cria evento para processar estoque
            await _publishEndpoint.Publish<AtualizarEstoqueProdutoEvent>(new
            {
                Id = model.Id,
                Estoque = model.Estoque
            });

            return true;
        }
    }
}
