using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System;

namespace Resizator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string batchFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", "resize.bat");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFileDialog(object sender, RoutedEventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            fbd.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            if(fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                tbxPath.Text = fbd.SelectedPath;
            }
        }

        private void Resize(object sender, RoutedEventArgs e)
        {
            // Set up the process start info
            ProcessStartInfo processStartInfo = new()
            {
                CreateNoWindow= true,
                FileName = $"cmd.exe",
                Arguments = $" /C \"{batchFilePath}\" {tbxMaxWH.Text} \"{tbxPath.Text}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            Debug.WriteLine($" /C \"{batchFilePath}\" {tbxMaxWH.Text} \"{tbxPath.Text}\"");

            // Start the process
            using var process = Process.Start(processStartInfo);

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.OutputDataReceived += ProcessOutput;
            process.ErrorDataReceived += ProcessOutput;

            // Clean up
            process.WaitForExit();
            process.Close();
        }

        private void ProcessOutput(object sender, DataReceivedEventArgs e) => tbxConsole.Dispatcher.BeginInvoke(() => { tbxConsole.Text += e.Data; });

        private void MaxWHPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d$");
        }
    }
}
