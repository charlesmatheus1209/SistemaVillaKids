namespace Sistema.Models
{
    public enum Sala
    {
        NaoClassificado, 
        Sala1,
        Sala2,
        Sala3,
        Sala4,
        Sala5
    }
    public class Crianca
    {
        public int Id { get; set; } = 0;
        public string Nome { get; set; } = "";
        public DateTime DataDeNascimento { get; set; }
        public string Responsaveis { get; set; } = "";
        public bool CkeckinRealizado { get; set; } = false;

        public string ResponsavelCheckinRealizado { get; set; } = "";


        public Crianca() { }
        public Crianca(string nome, DateTime dataDeNascimento, string responsaveis)
        {
            Nome = nome;
            DataDeNascimento = dataDeNascimento;
            Responsaveis = responsaveis;
        }

        public static double GetIdade(DateTime DataNascimento)
        {
            DateTime DataAtual = DateTime.Now;

            double ano = DataAtual.Year - DataNascimento.Year;

            double idade = ano + (DataAtual.DayOfYear - DataNascimento.DayOfYear) / 365.0;
            return idade;
        }

        public  double GetIdade()
        {
            DateTime DataAtual = DateTime.Now;

            double ano = DataAtual.Year - DataDeNascimento.Year;

            double idade = ano + (DataAtual.DayOfYear - DataDeNascimento.DayOfYear) / 365.0;
            return idade;
        }

        public static Sala GetSala(DateTime DataDeNascimento)
        {
            Sala sala = new Sala();
            double idade = GetIdade(DataDeNascimento);
            if(idade < 1.5 || idade > 15)
            {
                sala = Sala.NaoClassificado;
            }
            else if (idade < 2.5)
            {
                sala = Sala.Sala1;
            }
            else if (idade < 4)
            {
                sala = Sala.Sala2;
            }
            else if (idade < 6)
            {
                sala = Sala.Sala3;
            }
            else if (idade < 8)
            {
                sala = Sala.Sala4;
            }
            else if (idade < 15)
            {
                sala = Sala.Sala4;
            }

            return sala;
        }


    }
}
