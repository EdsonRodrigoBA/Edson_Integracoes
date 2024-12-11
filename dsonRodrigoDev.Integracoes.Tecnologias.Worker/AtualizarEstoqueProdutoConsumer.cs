using EdsonRodrigoDev.Integracoes.Tecnologias.Domain.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsonRodrigoDev.Integracoes.Tecnologias.Worker
{
    public class AtualizarEstoqueProdutoConsumer : IConsumer<AtualizarEstoqueProdutoEvent>
    {
        public Task Consume(ConsumeContext<AtualizarEstoqueProdutoEvent> context)
        {
            Console.WriteLine($"Atualização de estoque para: {context.Message.Estoque}");
            return Task.CompletedTask;
        }
    }
}
