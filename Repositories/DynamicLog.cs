using System.IO;
using System;

namespace WebAppServer.Repositories
{
    public class DynamicLog
    {
        public DynamicLog()
        {

        }

        public void EscreverTextoNoArquivo(string modulo, string arquivo, string mensagem)
        {
            string str_arquivo = arquivo;
            Int16 int1_cont = 0;
            bool bol_move = false;

            if (!Directory.Exists(@".\log\"))
            {
                Directory.CreateDirectory(@".\log\");
            }

            string caminhoArquivoLog = @".\log\" + str_arquivo;

            FileInfo[] ff = new DirectoryInfo(@".\log\").GetFiles();

            if (ff.Length > 0)
            {
                foreach (FileInfo file in ff)
                {
                    bol_move = false;

                    if (file.Name.ToLower() == str_arquivo.ToLower())
                    {
                        if (str_arquivo.IndexOf("conciliacao_log") > -1)
                        {
                            if (Convert.ToDouble((file.Length / 1024.00) / 1024.00) > 10)
                            {
                                bol_move = true;
                            }
                        }
                        else
                        {
                            if (Convert.ToDouble((file.Length / 1024.00) / 1024.00) > 1)
                            {
                                bol_move = true;
                            }
                        }

                        if (bol_move)
                        {
                            if (File.Exists(@".\log\" + str_arquivo.Replace("txt", "bkp")))
                            {
                                File.Delete(@".\log\" + str_arquivo.Replace("txt", "bkp"));
                            }
                            File.Move(@".\log\" + str_arquivo, @".\log\" + str_arquivo.Replace(".txt", DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".bkp"));
                        }
                        break;
                    }
                }
            }

            while (int1_cont < 31)
            {
                try
                {
                    using (StreamWriter streamWriter = new StreamWriter(caminhoArquivoLog, true))
                    {
                        streamWriter.WriteLine(mensagem);
                        streamWriter.Close();
                        int1_cont = 31;
                    }
                }
                catch (Exception ex)
                {
                    if (int1_cont < 31 &&
                        (
                         ex.Message.ToString().Contains("utilizado") ||
                         ex.Message.ToString().Contains("used by") ||
                         ex.Message.ToString().Contains("Could not find file") ||
                         ex.Message.ToString().Contains("Unable to find the specified file")
                        )
                    )
                    {
                        int1_cont++;
                    }
                    /*
                    else
                    {
                        throw ex;
                    }*/

                }
            }
        }

    }
}
