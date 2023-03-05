namespace Sistema.Models
{
    public class CheckInOut
    {
        public int Id { get; set; }
        public DateTime Horario { get; set; }
        public string Responsavel { get; set; } = "";
        public bool Entrada { get; set; } = false;

        public bool ValidaCheckIn()
        {
            if(Responsavel == "Ninguem")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        
    }
}
