using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text.Json;
using static Mysqlx.Expect.Open.Types;

namespace Sistema.Models
{
    class DadosBancoDeDados
    {
        public string? ServerName { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
        public string? DatabaseName { get; set; }
        public int? Port { get; set; }
    }
    public static class BancoDeDados
    {
        public static int acessos { get; set; }

        public static string createStringConnection()
        {
            acessos++;
            string credenciais;
            string StringConnection = "";

            using (StreamReader r = new StreamReader("BancoDeDadosCredenciais.json"))
            {
                credenciais = r.ReadToEnd();
            }
            try
            {
                DadosBancoDeDados? dbd = JsonSerializer.Deserialize<DadosBancoDeDados>(credenciais);
                StringConnection = "server=" + dbd.ServerName +
                                   ";user=" + dbd.User +
                                   ";database=" + dbd.DatabaseName +
                                   ";port=" + dbd.Port +
                                   ";password=" + dbd.Password;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return StringConnection;
        }

        public static Crianca StringParaCrianca(string txt)
        {
            Crianca crianca = new Crianca();
            crianca.Id = Convert.ToInt32(txt.Split(";")[0]);
            crianca.Nome = txt.Split(";")[1];
            crianca.DataDeNascimento = Convert.ToDateTime(txt.Split(";")[2]);
            crianca.Responsaveis = txt.Split(";")[3];
            return crianca;
        }

        public static List<Crianca> GetTodasCriancas(string Pesquisa)
        {
            if (String.IsNullOrEmpty(Pesquisa))
                return GetTodasCriancas();

            List<Crianca> lista = new List<Crianca>();
            MySqlConnection connection = new MySqlConnection(createStringConnection());

            try
            {
                connection.Open();
                string stringCommand = $"SELECT * FROM criancas WHERE Nome LIKE '%{Pesquisa}%'";
                MySqlCommand command = new MySqlCommand(stringCommand, connection);
                MySqlDataReader reader = command.ExecuteReader();
                string linha = "";

                while (reader.Read())
                {
                    linha = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i != 0)
                        {
                            linha += ";" + reader[i].ToString();
                        }
                        else
                        {
                            linha += reader[i].ToString();
                        }
                    }
                    lista.Add(StringParaCrianca(linha));
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return InsereCheckInRealizado(lista);
        }

        public static List<Crianca> GetTodasCriancas() 
        {
            List<Crianca> lista = new List<Crianca>();
            MySqlConnection connection = new MySqlConnection(createStringConnection());

            try
            {
                connection.Open();
                string stringCommand = $"SELECT * FROM criancas";
                MySqlCommand command = new MySqlCommand(stringCommand, connection);
                MySqlDataReader reader = command.ExecuteReader();
                string linha = "";

                while (reader.Read())
                {
                    linha = "";
                    for(int i = 0; i < reader.FieldCount; i++)
                    {
                        if(i != 0)
                        {
                            linha += ";" + reader[i].ToString();
                        }
                        else
                        {
                            linha += reader[i].ToString();
                        }
                    }
                    lista.Add(StringParaCrianca(linha));
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return InsereCheckInRealizado(lista);
        }

        public static List<Crianca> InsereCheckInRealizado(List<Crianca> criancas) {
            
            for(int i = 0; i < criancas.Count(); i++)
            {
                string nome = VerificaCheckinRealizado(criancas[i].Id.ToString());
                Console.WriteLine(nome);
                criancas[i].ResponsavelCheckinRealizado = nome;
                if (String.IsNullOrEmpty(criancas[i].ResponsavelCheckinRealizado))
                {
                    criancas[i].CkeckinRealizado = false;
                }
                else
                {
                    criancas[i].CkeckinRealizado = true;
                }
            }

            return criancas;
        }

        public static Crianca? GetCrianca(int? id)
        {

            if (id is null)
                return null;


            Crianca crianca = new Crianca();

            MySqlConnection connection = new MySqlConnection(createStringConnection());

            try
            {
                connection.Open();
                string stringCommand = $"SELECT * FROM criancas WHERE Id = {id}";
                MySqlCommand command = new MySqlCommand(stringCommand, connection);
                MySqlDataReader reader = command.ExecuteReader();
                string linha = "";
                while (reader.Read())
                {
                    linha = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i != 0)
                        {
                            linha += ";" + reader[i].ToString();
                        }
                        else
                        {
                            linha += reader[i].ToString();
                        }
                    }
                    crianca =  StringParaCrianca(linha);
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return crianca;
        }

        public static bool CadastrarCrianca(Crianca crianca)
        {
            MySqlConnection connection = new MySqlConnection(createStringConnection());
            if (crianca.Id == 0)
            {
                try
                {
                    connection.Open();
                    string stringCommand = $"insert into criancas (Nome, DataDeNascimento, Responsaveis) values ('{crianca.Nome}', '{crianca.DataDeNascimento.ToString("yyyy-MM-ddTHH:mm:ss")}', '{crianca.Responsaveis}');";
                    MySqlCommand command = new MySqlCommand(stringCommand, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                try
                {
                    connection.Open();
                    string stringCommand = $"UPDATE criancas SET Nome = '{crianca.Nome}', DataDeNascimento = '{crianca.DataDeNascimento.ToString("yyyy-MM-ddTHH:mm:ss")}', Responsaveis = '{crianca.Responsaveis}' WHERE Id = {crianca.Id}";
                    Console.WriteLine(stringCommand);
                    MySqlCommand command = new MySqlCommand(stringCommand, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }

            }
            

            

            connection.Close();
            return true;
        }

        public static List<Crianca> GetCriancasCheckIn()
        {
            List<Crianca> lista = new List<Crianca>();
            MySqlConnection connection = new MySqlConnection(createStringConnection());

            try
            {
                connection.Open();
                string stringCommand = $"SELECT Id, Nome FROM criancas";
                MySqlCommand command = new MySqlCommand(stringCommand, connection);
                MySqlDataReader reader = command.ExecuteReader();
                string linha = "";

                while (reader.Read())
                {
                    linha = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i != 0)
                        {
                            linha += ";" + reader[i].ToString();
                        }
                        else
                        {
                            linha += reader[i].ToString();
                        }
                    }
                    lista.Add(StringParaCrianca(linha));
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return lista;
        }

        public static bool RealizarCheckIn(CheckInOut check)
        {
            MySqlConnection connection = new MySqlConnection(createStringConnection());

            try
            {
                connection.Open();
                string stringCommand = $"insert into checkin (Id, HorarioEntrada, ResponsavelEntrada) values ('{check.Id}', '{check.Horario.ToString("yyyy-MM-ddTHH:mm:ss")}', '{check.Responsavel}');";
                MySqlCommand command = new MySqlCommand(stringCommand, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            connection.Close();
            return true;
        }

        public static DateTime GetUltimoHorarioEntrada(int id)
        {
            List<string> selectReturn = new List<string>();
            MySqlConnection connection = new MySqlConnection(createStringConnection());

            try
            {
                connection.Open();
                string stringCommand = $"SELECT HorarioEntrada FROM checkin WHERE Id = {id} AND HorarioSaida IS NULL";
                //Console.WriteLine(stringCommand);
                MySqlCommand command = new MySqlCommand(stringCommand, connection);
                MySqlDataReader reader = command.ExecuteReader();
                string? linha = "";

                while (reader.Read())
                {
                    linha = "";
                    
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i != 0)
                            linha += ";" + reader[i].ToString();
                        else
                            linha += reader[i].ToString();
                    }
                    
                    selectReturn.Add(linha);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            connection.Close();
            if(selectReturn.Count > 0)
            {
                return Convert.ToDateTime(selectReturn[0]);
            }
            else
            {
                return DateTime.MinValue;
            }
            
        }

        public static object RealizarCheckOut(CheckInOut check)
        {
            MySqlConnection connection = new MySqlConnection(createStringConnection());
            DateTime dt = GetUltimoHorarioEntrada(check.Id);
            string horario = "";
            if (dt == DateTime.MinValue)
            {
                return null;
            }
            else
            {
               horario = dt.ToString("yyyy-MM-ddTHH:mm:ss");
            }

            try
            {
                connection.Open();
                string stringCommand = $"UPDATE checkin SET HorarioSaida = '{check.Horario.ToString("yyyy-MM-ddTHH:mm:ss")}', ResponsavelSaida = '{check.Responsavel}' WHERE HorarioEntrada = '{horario.Replace(" ", "T").Replace("/", "-")}' AND Id={check.Id}";
                MySqlCommand command = new MySqlCommand(stringCommand, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            connection.Close();
            Console.WriteLine("Dado atualizado");
            return true;
        }

        public static string VerificaCheckinRealizado(string _id)
        {

            MySqlConnection connection = new MySqlConnection(createStringConnection());

            try
            {
                connection.Open();
                string stringCommand = $"SELECT * FROM checkin WHERE Id = {_id} AND HorarioSaida IS NULL";

                MySqlCommand command = new MySqlCommand(stringCommand, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int i = reader.GetOrdinal("HorarioSaida");
                    if (String.IsNullOrEmpty(Convert.ToString(reader[i])))
                    {
                        return Convert.ToString(reader[reader.GetOrdinal("ResponsavelEntrada")]);
                    }
                    else
                    {
                        return "";
                    }
                }
                connection.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return "";
        }

        public static bool ExcluirCrianca(int id)
        {
            MySqlConnection connection = new MySqlConnection(createStringConnection());

            try
            {
                connection.Open();
                string stringCommand = $"DELETE FROM criancas WHERE Id = {id}";
                MySqlCommand command = new MySqlCommand(stringCommand, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            connection.Close();
            return true;
        }

        public static List<string> GerarRelatorioCompleto(Relatorio rlt)
        {
            List<string> lista = new List<string>();

            MySqlConnection connection = new MySqlConnection(createStringConnection());

            try
            {
                connection.Open();
                string stringCommand = $"select * from criancas c1 NATURAL JOIN checkin c2 where HorarioEntrada > '{rlt.DataInicio.ToString("yyyy-MM-ddTHH:mm:ss")}' and (HorarioSaida < '{rlt.DataFim.ToString("yyyy-MM-ddTHH:mm:ss")}' or HorarioSaida is null) order by HorarioSaida DESC";
                MySqlCommand command = new MySqlCommand(stringCommand, connection);
                MySqlDataReader reader = command.ExecuteReader();
                string linha = "";

                while (reader.Read())
                {
                    linha = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i != 0)
                        {
                            linha += ";" + reader[i].ToString();
                        }
                        else
                        {
                            linha += reader[i].ToString();
                        }
                    }
                    lista.Add(linha);
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return lista;
        }

        public static List<string> GerarRelatorioQuantidade(Relatorio rlt)
        {
            List<string> lista = new List<string>();

            MySqlConnection connection = new MySqlConnection(createStringConnection());

            try
            {
                connection.Open();
                string stringCommand = $"select DataDeNascimento, HorarioEntrada, HorarioSaida from criancas c1 join checkin c2 on c1.id = c2.id where HorarioEntrada >= '{rlt.DataInicio.ToString("yyyy-MM-ddTHH:mm:ss")}' and HorarioSaida <= '{rlt.DataFim.ToString("yyyy-MM-ddTHH:mm:ss")}' or HorarioSaida is null;";
                MySqlCommand command = new MySqlCommand(stringCommand, connection);
                MySqlDataReader reader = command.ExecuteReader();
                string linha = "";

                while (reader.Read())
                {
                    linha = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i != 0)
                        {
                            linha += ";" + reader[i].ToString();
                        }
                        else
                        {
                            linha += reader[i].ToString();
                        }
                    }
                    lista.Add(linha);
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return lista;
        }

    }
}
