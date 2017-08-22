using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WpfFiler
{
    public class ConfigureWpfFiler
    {
        private string path;
        private FileInfo info;
        private FileStream fs;
        private Attributes attributes;

        private MessageBoxResult ShowErrorMessage(string message)
        {
            return MessageBox.Show(     message,
                                        "WpfFiler",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Exclamation
                                   );
        }

        public ConfigureWpfFiler()
        {
            path = "./WpfFiler.conf";
            info = new FileInfo(path);
            attributes = new Attributes();
        }
        public ConfigureWpfFiler(string config_path)
        {
            path = config_path;
            info = new FileInfo(path);
            attributes = new Attributes();
        }
        public void DefaultConfiguration()
        {
            attributes = new Attributes();
        }

        public void SaveConfiguration()
        {
            try
            {
                string output = "";
                if (info.Exists)
                {
                    // Read the file into an array with each element being one line, blank lines
                    // are excluded.
                    char[] seperators = { '\r', '\n' };
                    fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    string[] lines = new StreamReader(fs).ReadToEnd().Split(seperators, 
                                                                        StringSplitOptions.RemoveEmptyEntries);
                    fs.Close();
                    foreach (string line in lines)
                    {
                        // Split the current line into at most 3 parts: key, value, and comment
                        char[] seperators2 = { ':', ';' };
                        string[] chunks = line.Split(seperators2, StringSplitOptions.RemoveEmptyEntries);
                        // If there's only one chunk, it's likely a comment, therefore, add it to output
                        // and move to the next line.
                        if (chunks.Length == 1)
                        {
                            output += line + "\r\n";
                            continue;
                        }
                        chunks[0] = chunks[0].Trim();
                        string val = attributes.GetValueString(chunks[0]);
                        if (val == null)
                            output += chunks[0] + ": " + chunks[1];
                        else
                            output += chunks[0] + ": " + val;
                        if (chunks.Length == 3)
                            output += "\t; " + chunks[2];
                        output += "\r\n";
                    }
                }
                else
                    output = attributes.GetAllAttributes();
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                UTF8Encoding encoder = new UTF8Encoding(true);
                fs.Write(encoder.GetBytes(output), 0, output.Length);
                fs.Close();
            }
            catch(Exception e)
            {
                ShowErrorMessage("There was an error when trying to save the configuration:  " +
                                    e.Source + ": " + e.TargetSite + ": " + e.InnerException +
                                    ": " + e.StackTrace + ": " + e.Message);
            }
        }

        public void LoadConfiguration()
        {
            if (!info.Exists)
            {
                DefaultConfiguration();
                return;
            }
            byte[] tmp = new byte[info.Length];
            fs = new FileStream(path, FileMode.Open);
            fs.Read(tmp, 0, (int)info.Length);
            fs.Close();
            char[] seperators = { '\r', '\n' };
            UTF8Encoding encoder = new UTF8Encoding(true);
            string[] conf = encoder.GetString(tmp).Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            string err_msg = "";
            int err_msg_old_len;
            for (long line = 0; line < conf.LongLength; line++)
            {
                string[] separators2 = { ":", ";" };
                string[] pairs = conf[line].Split(separators2, StringSplitOptions.RemoveEmptyEntries);

                if (pairs.Length < 2)
                    continue;
                err_msg_old_len = err_msg.Length;
                err_msg += SetAttribute(pairs[0].Trim(), pairs[1].Trim());
                if (err_msg_old_len != err_msg.Length)
                    err_msg += " : From line " + (line + 1) + "\r\n";
            }
            if (err_msg.Length != 0)
                ShowErrorMessage(err_msg);
        }
    }
}
