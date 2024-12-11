namespace EdsonRodrigoDev.Integracoes.Tecnologias.Domain
{
    public class Produtos
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Estoque { get; set; }
        public decimal Valor { get; set; }
    }
}
