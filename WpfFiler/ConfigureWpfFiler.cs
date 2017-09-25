using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WpfFiler
{
    public class ConfigureWpfFiler : Attributes
    {
        private string path;
        private FileInfo info;
        private FileStream fs;
        //private Attributes attributes;

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
            DefaultConfiguration();
        }

        public ConfigureWpfFiler(string config_path)
        {
            path = config_path;
            info = new FileInfo(path);
            DefaultConfiguration();
        }

        /*!
         * Everything is initialized to the default values.
         */
        public void DefaultConfiguration()
        {
            background = new Background();
            default_icon = new DefaultIcon();
            folder_icon = new FolderIcon();
            hover_background = new HoverBackground();
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
                        string val = GetValueString(chunks[0]);
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
                    output = GetAllAttributes();
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
            for (long line = 0; line < conf.LongLength; line++)
            {
                int commentIndex = conf[line].IndexOf(';');
                string pair;
                if (commentIndex != -1)
                   pair = conf[line].Substring(0, commentIndex);
                else
                    pair = conf[line];

                string[] pairs = pair.Split(':');

                if (pairs.Length < 2)
                    continue;

                if (!SetAttribute( pairs[0].Trim(), pairs[1].Trim() ) )
                    err_msg += "unable to set attribute '" + pairs[0].Trim() + "' from line " + (line + 1) + 
                                " to '" + pairs[1].Trim() + "'\r\n";
            }
            if (err_msg.Length != 0)
                ShowErrorMessage(err_msg);
        }
    }
}
