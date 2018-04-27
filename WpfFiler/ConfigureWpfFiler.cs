using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WpfFiler
{
    public class ConfigureWpfFiler
    {
        private string path;
        private FileInfo info;
        private Attributes attributes;

        private MessageBoxResult ShowErrorMessage(string message)
        {
            return MessageBox.Show(     message,
                                        "WpfFiler",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Exclamation
                                   );
        }

        // Properties to make access easier
        public SolidColorBrush BackgroundBrush
        {
            get { return attributes.background.Value; }
        }
        public System.Drawing.Icon DefaultFileIcon
        {
            get { return attributes.default_icon.Value; }
        }
        public System.Drawing.Icon DefaultFolderIcon
        {
            get { return attributes.folder_icon.Value; }
        }
        public SolidColorBrush HoverBackgroundBrush
        {
            get { return attributes.hover_background.Value; }
        }
        public uint IconSize
        {
            get { return attributes.default_icon.IconSize; }
            set
            {
                attributes.default_icon.IconSize = value;
                attributes.folder_icon.IconSize = value;
            }
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
        
        public void DefaultConfiguration()
        {
            attributes = new Attributes();
        }

        public void SaveConfiguration()
        {
            try
            {
                Queue<string> output = new Queue<string>();
                if (info.Exists)
                {
                    StreamReader reader = new StreamReader(
                            new FileStream(path, FileMode.Open, FileAccess.Read));
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string comment = "";

                        int semi_colon = line.IndexOf(';');
                        if (semi_colon != -1)
                        {
                            comment = line.Substring(semi_colon);
                            line = line.Substring(0, semi_colon);
                        }

                        int colon = line.IndexOf(':');
                        if (colon == -1)
                            output.Enqueue(line + comment);
                        else
                        {
                            string attribute = line.Substring(0, colon);

                            // preserve whitespace before the beginning of a comment
                            int idx = line.Length - 1;
                            while (idx > 0 && (line[idx] == '\t' || line[idx] == ' '))
                                idx--;
                            idx++;
                            string whitespace = line.Substring(idx);

                            // TODO : save whitespace here too
                            string val = attributes.GetValueString(attribute.Trim());
                            if (val.Length == 0)
                                output.Enqueue(line + comment);
                            else
                                output.Enqueue(attribute + ": " + val + whitespace + comment);
                        }
                    }
                    reader.Close();
                }
                else
                {
                    output.Enqueue("; Icons are specified with the following syntax:");
                    output.Enqueue("; Attribute: path-to-icon-file index-of-icon");
                    output.Enqueue(";");
                    output.Enqueue("; Comments begin with ';', the rest of the line will be ignored");
                    foreach (string s in attributes.GetAllAttributes())
                        output.Enqueue(s);
                }
                StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write));
                while (output.Count > 0)
                {
                    string line = output.Dequeue();
                    writer.WriteLine(line);
                }
                writer.Close();
            }
            catch (Exception e)
            {
                ShowErrorMessage("There was an error when trying to save the configuration:  "
                                 + e.Source + ": " + e.TargetSite + ": " + e.InnerException
                                 + ": " + e.StackTrace + ": " + e.Message);
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
            FileStream fs = new FileStream(path, FileMode.Open);
            fs.Read(tmp, 0, (int)info.Length);
            fs.Close();

            UTF8Encoding encoder = new UTF8Encoding(true);
            string[] conf = encoder.GetString(tmp).Split(
                    new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

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

                if (pairs.Length > 1 && !attributes.SetAttribute(pairs[0].Trim(), pairs[1].Trim()))
                    err_msg += "unable to set attribute '" + pairs[0].Trim() + "' from line " + (line + 1) + 
                                " to '" + pairs[1].Trim() + "'\r\n";
            }

            if (err_msg.Length != 0)
                ShowErrorMessage(err_msg);
        }
    }
}
