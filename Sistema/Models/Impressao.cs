using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.Versioning;

namespace Sistema.Models
{
    [SupportedOSPlatform("windows")]
    public class Impressao
    {

        public static List<string?> MostrarImpressoras()
        {
            List<string?> lista = new List<string?>();
            foreach (var impressora in PrinterSettings.InstalledPrinters)
            {
                lista.Add(impressora.ToString());
            }

            return lista;
        }

        public static void Imprimir()
        {
            using(PrintDocument pd = new PrintDocument())
            {
                pd.PrintPage += Pd_PrintPage;
                pd.Print();
            }
        }

        private static void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            using (Font font = new Font("Lucida Console", 36))
            {
                using (Brush brush = new SolidBrush(Color.Red))
                {
                    e.Graphics.DrawString("Um alo do Macoratti...,\n para todos", font, brush, new Point(50,50));
                }
            }
        }
    }
}
