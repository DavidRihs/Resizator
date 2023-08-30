using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System;
using System.Globalization;
using System.IO.Compression;
using System.Threading;
using System.Configuration;

namespace Resizator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string batchFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resize.bat");


        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindowClosing;

            Settings.Load();
            if (Settings.INSTANCE.Percent is string percent)
            {
                tbxPercent.Text = percent;
            }
            else
            {
                tbxPercent.Text = App.DEFAULT_PERCENT;
                App.AddOrUpdateSetting(App.KEY_PERCENT, App.DEFAULT_PERCENT);
            }

            if (App.GetSettingValue(App.KEY_PATH) is string path)
            {
                tbxPath.Text = path;
            }
            else
            {
                string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                tbxPath.Text = myDocumentsPath;
                App.AddOrUpdateSetting(App.KEY_PATH, myDocumentsPath);
            }
        }

        private void MainWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            App.AddOrUpdateSetting(App.KEY_PERCENT, tbxPercent.Text);
            App.AddOrUpdateSetting(App.KEY_PATH, tbxPath.Text);
        }

        private void OpenFileDialog(object sender, RoutedEventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyDocuments;
            if(fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                tbxPath.Text = fbd.SelectedPath;
            }
        }

        private void Resize(object sender, RoutedEventArgs e)
        {
            try
            {
                tbxConsole.Clear();
                decimal percent = decimal.Parse(tbxPercent.Text) / 100;

                // Set up the process start info
                ProcessStartInfo processStartInfo = new()
                {
                    CreateNoWindow = true,
                    FileName = $"cmd.exe",
                    Arguments = $" /C \"\"{batchFilePath}\" {percent.ToString("0.00", CultureInfo.InvariantCulture)} \"{tbxPath.Text}\"\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                };

                Debug.WriteLine(processStartInfo.Arguments);

                // Start the process
                using var process = Process.Start(processStartInfo);
                process.EnableRaisingEvents = true;

                process.OutputDataReceived += ProcessOutput;
                process.ErrorDataReceived += ProcessOutput;

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                // Clean up
                process.WaitForExit();
                process.Close();

                Compress();
            }
            catch (Exception ex)
            {
                tbxConsole.Text += $"ERROR: {ex.Message}\n";
            }
        }

        private void Compress()
        {
            string outputFolder = Path.Combine(tbxPath.Text, "resized");
            if (Directory.Exists(outputFolder))
            {


                string zipPath = Path.Combine(tbxPath.Text, "resized.zip");
                WriteToConsole($"Compressing \"{outputFolder}\" into \"{zipPath}\"...");
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
                ZipFile.CreateFromDirectory(outputFolder, zipPath);

                WriteToConsole($"Deleting \"{outputFolder}\"...");
                Directory.Delete(outputFolder, true);
            }
        }

        private void ProcessOutput(object sender, DataReceivedEventArgs e)
        {
            if(e.Data != null)
            {
                WriteToConsole(e.Data);
            }
        }

        private void WriteToConsole(string txt) => tbxConsole.Dispatcher.BeginInvoke(() => { tbxConsole.Text += txt + "\n"; });

        private void ValidatePercent(object sender, TextCompositionEventArgs e)
        {
            var newValue = tbxPercent.Text + e.Text;
            e.Handled = !Regex.IsMatch(newValue, @"^[1-9][0-9]?$");
        }
    }
}
