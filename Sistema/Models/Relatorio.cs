namespace Sistema.Models
{
    public enum Tipo 
    {
        Completo,
        Quantidade,
        Nenhum
    }
    public class Relatorio
    {
        public Tipo tipo { get; set; }
        public DateTime DataInicio { get; set; } = DateTime.MinValue;
        public DateTime DataFim { get; set; } = DateTime.MaxValue;
    }
}
