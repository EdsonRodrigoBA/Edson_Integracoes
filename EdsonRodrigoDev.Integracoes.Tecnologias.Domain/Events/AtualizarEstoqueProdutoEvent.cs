

namespace EdsonRodrigoDev.Integracoes.Tecnologias.Domain.Events
{
    public interface AtualizarEstoqueProdutoEvent
    {
        public Guid Id { get; set; }
        public decimal Estoque { get; set; }

    }



}
