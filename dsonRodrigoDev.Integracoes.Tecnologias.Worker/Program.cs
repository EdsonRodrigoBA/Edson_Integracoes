using dsonRodrigoDev.Integracoes.Tecnologias.Worker;
using MassTransit;

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.ReceiveEndpoint("atualizar-estoque-produto", e =>
    {
        e.PrefetchCount = 10;
        e.UseMessageRetry(p => p.Interval(3, 100));
        e.Consumer<AtualizarEstoqueProdutoConsumer>();
    });
});
busControl.Start();
while (true) ;