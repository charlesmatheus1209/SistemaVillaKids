using Microsoft.AspNetCore.Mvc;
using Sistema.Models;
using System.Diagnostics;
using System.Runtime.Versioning;


namespace Sistema.Controllers
{
    public class HomeController : Controller
    {
        [SupportedOSPlatform("windows")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Cadastro(int? id)
        {
            ViewBag.Edicao = true;
            return View(BancoDeDados.GetCrianca(id));
        }

        [HttpPost]
        public IActionResult CadastroRealizado(Crianca? crianca)
        {
            BancoDeDados.CadastrarCrianca(crianca);
            return View(crianca);
        }

        [HttpGet]
        public IActionResult ListaDeCadastros()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ListaDeCadastros(string? Pesquisa)
        {
            if (String.IsNullOrEmpty(Pesquisa))
            {
                ViewBag.Pesquisa = "";
            }
            else
            {
                ViewBag.Pesquisa = Pesquisa;
            }
            
            return View();
        }

        [HttpGet]
        public IActionResult CheckIn(string? Pesquisa)
        {
            List<Crianca> cr = BancoDeDados.GetTodasCriancas(Pesquisa);
            ViewBag.TodasCriancas = cr;
            ViewBag.Pesquisa = Pesquisa;
            Console.WriteLine(cr[0].CkeckinRealizado);
            Console.WriteLine(cr[0].ResponsavelCheckinRealizado);
            return View();
        }

        [HttpPost]
        public IActionResult CheckIn(CheckInOut check)
        {
            ViewBag.boolMensagem = false;

            ViewBag.Mensagem = "Ação executada";
            if (check.ValidaCheckIn())
            {

                ViewBag.boolMensagem = true;
                ViewBag.Mensagem = "Ação executada";
                if (check.Entrada)
                {
                    BancoDeDados.RealizarCheckIn(check);
                }
                else
                {
                    ViewBag.Mensagem = "Ação executada";
                    if (BancoDeDados.RealizarCheckOut(check) is null)
                    {
                        ViewBag.Mensagem = "Problema interno ao servidor";
                        ViewBag.boolMensagem = false;
                    }
                }

            }
            else
            {
                ViewBag.boolMensagem = false;
                ViewBag.Mensagem = "Responsável não selecionado";
            }

            ViewBag.TodasCriancas = BancoDeDados.GetTodasCriancas();

            return View();
        }

        [HttpGet]
        public IActionResult Exclusao(int? id)
        {
            Console.WriteLine("1");
            return View(id);
        }
        [HttpGet]
        public IActionResult ExclusaoReal(double id)
        {
            Console.WriteLine($"2 - {id}");
            BancoDeDados.ExcluirCrianca((int)id);
            return View("ListaDeCadastros");
        }

        [HttpPost]
        public IActionResult Exclusao(int id)
        {
            Console.WriteLine("3");
            return View(id);
        }

        [HttpGet]
        public IActionResult Relatorio()
        {
            ViewBag.Tipo = Tipo.Nenhum;
            ViewBag.Hoje = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            ViewBag.Ontem = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss");
            return View();
        }

        [HttpPost]
        public IActionResult Relatorio(Relatorio relatorio)
        {
            ViewBag.Hoje = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            ViewBag.Ontem = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss");
            ViewBag.Tipo = relatorio.tipo;

            if (relatorio.tipo == Tipo.Completo)
            {
                List<string> relatorio_string = BancoDeDados.GerarRelatorioCompleto(relatorio);
                foreach(var item in relatorio_string)
                {
                    Console.WriteLine(item);
                }
                ViewBag.Relatorio = relatorio_string;
            }
            else if(relatorio.tipo == Tipo.Quantidade)
            {
                Console.WriteLine("Entrei");
                Dictionary<Sala, (int, int, int)> Salas_alunos = new Dictionary<Sala, (int, int, int)>() { 
                        { Sala.NaoClassificado, (0, 0, 0)}, 
                        { Sala.Sala1,  (0, 0, 0)},
                        { Sala.Sala2,  (0, 0, 0)},
                        { Sala.Sala3,  (0, 0, 0)},
                        { Sala.Sala4,  (0, 0, 0)},
                        { Sala.Sala5,  (0, 0, 0)}
                };
                List<string> relatorio_string = BancoDeDados.GerarRelatorioQuantidade(relatorio);
                foreach(string item in relatorio_string)
                {
                    List<string> dados = item.Split(";").ToList();
                    Sala sala = Crianca.GetSala(Convert.ToDateTime(dados[0]));
                   
                    if (Salas_alunos.ContainsKey(sala))
                    {
                        (int, int, int) aux = Salas_alunos[sala];
                        aux.Item1++;
                        Salas_alunos[sala] = aux;
                    }
                    if (!String.IsNullOrEmpty(dados[2]))
                    {
                        (int, int, int) aux = Salas_alunos[sala];
                        aux.Item2++;
                        Salas_alunos[sala] = aux;
                    }
                
                    
                }
                foreach(var item in Salas_alunos)
                {
                    (int, int, int) aux = Salas_alunos[item.Key];
                    aux.Item3 = aux.Item1 - aux.Item2;
                    Salas_alunos[item.Key] = aux;
                }
                ViewBag.RelatorioQuantidade = Salas_alunos;
            }

            

            return View();
        }



    }
}