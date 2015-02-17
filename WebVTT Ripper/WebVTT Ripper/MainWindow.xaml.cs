using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Button = System.Windows.Controls.Button;
using IWin32Window = System.Windows.Forms.IWin32Window;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace WebVTT_Ripper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IWin32Window _windowHandle;
        public MainWindow()
        {
            Loaded += delegate {
                _windowHandle = new HandleWrapper(new WindowInteropHelper(this).Handle);
            };

            InitializeComponent();
        }

        private void StartBtn_OnClick(object sender, RoutedEventArgs ev)
        {
            // Clear the message box
            MessageBox.Text = "";

            var vttFile = Path.GetFullPath(WebVttTextBox.Text);
            var outputFile = Path.GetFullPath(OutputTextBox.Text);

            // I/O validation
            if (!File.Exists(vttFile))
            {
                MessageBox.Text = "WebVTT File does not exist.";
                return;
            }
            if (string.IsNullOrEmpty(outputFile))
            {
                MessageBox.Text = "Output File is empty.";
                return;
            }

            // Parse
            var allText = File.ReadAllText(vttFile);

            var regex = new Regex(@"-->[0-9:\. ]+\r?\n(.*?)\r?\n\s*$", RegexOptions.Multiline | RegexOptions.Singleline);

            var matches = regex.Matches(allText);

            var outputText = "";

            // matches newlines for replacement
            var newlineRegex = new Regex(@"\r?\n");
            foreach (Match match in matches)
            {
                if (match.Groups.Count < 2)
                {
                    MessageBox.Text += "There was a problem with this match:" + Environment.NewLine + match.Value;
                }

                var text = match.Groups[1].Value;
                // Replace all newlines with spaces
                text = newlineRegex.Replace(text, " ");
                // Scrub the ends
                text = text.Trim();
                outputText += text + Environment.NewLine + Environment.NewLine;
            }

            // Write
            try
            {
                File.WriteAllText(outputFile, outputText);
            }
            catch (Exception e)
            {
                MessageBox.Text = e.ToString();
                return;
            }

            MessageBox.Text = "Done!";

            // Unfortunately this enhanced message box uses WinForms :(
            var result = MessageBoxEx.Show(_windowHandle, "Would you like to open the output file?", "Open Output File?",
                MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                Process.Start(outputFile);
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
            {
                MessageBox.Text = "Something crazy just happened";
                return;
            }

            switch (button.Name)
            {
                case "WebVttBtn":
                    var openDialog = new OpenFileDialog
                    {
                        Filter = "WebVTT Files (.vtt)|*.vtt|All Files (*.*)|*.*",
                        Multiselect = false
                    };

                    bool? open = openDialog.ShowDialog();

                    if (open == true)
                    {
                        WebVttTextBox.Text = System.IO.Path.GetFullPath(openDialog.FileName);
                    }
                    break;
                case "OutputBtn":
                    var saveDialog = new SaveFileDialog
                    {
                        Filter = "Text File (.txt)|*.txt"
                    };

                    bool? save = saveDialog.ShowDialog();

                    if (save == true)
                    {
                        OutputTextBox.Text = System.IO.Path.GetFullPath(saveDialog.FileName);
                    }
                    break;
            }
        }
    }
}