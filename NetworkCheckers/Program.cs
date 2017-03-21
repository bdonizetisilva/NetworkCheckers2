using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace NetworkCheckers
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal da aplicação.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.ThreadException += ThreadException;
            Application.EnableVisualStyles();
            using (InitialForm form = new InitialForm())
                Application.Run(form);
        }

        /// <summary>
        /// Recupera exceções ocorridas na thread principal e realiza o log delas
        /// </summary>
        private static void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                int ext = 1;
                string filename;

                while (true)
                {
                    filename = "ErrorLog [" + DateTime.Now.ToShortDateString() + "]";
                    filename = filename.Replace('/', '-').Replace('\\', '-');
                    filename += ext != 1
                        ? " (" + ext + ")"
                        : string.Empty;
                    if (!File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\" + filename + ".log"))
                        break;
                    ++ext;
                }

                filename = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + filename + ".log";
                using (StreamWriter fs = File.CreateText(filename))
                {
                    fs.WriteLine(Application.ProductName + " " + Application.ProductVersion);
                    fs.WriteLine(DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString());
                    fs.WriteLine();
                    fs.WriteLine(e.ToString());
                }
            }
            finally
            {
                // Relança exceção fechando a aplicação
                throw e.Exception;
            }
        }
    }
}
