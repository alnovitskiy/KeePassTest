using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Core;
using System.Text;
using System;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HelloWorldUWP
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage() {
            this.InitializeComponent();
        }

        private async void button_ClickAsync(object sender, RoutedEventArgs e) {

            var options = new ProcessLauncherOptions();
            var standardOutput = new InMemoryRandomAccessStream();
            var standardError = new InMemoryRandomAccessStream();
            options.StandardOutput = standardOutput;
            options.StandardError = standardError;


            //var options = new ProcessLauncherOptions();

            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {

                const string CommandLineProcesserExe = "c:\\windows\\system32\\cmd.exe";
                //var result = await ProcessLauncher.RunToCompletionAsync(CommandLineProcesserExe, String.Empty, options);
                var result = await ProcessLauncher.RunToCompletionAsync(@"ConsoleApp1.exe", String.Empty, options);

                //await ExecuteCommandLineString(@"C:\Program Files (x86)\KeePass Password Safe 2\KeePass.exe");

            });

            //var result = await ProcessLauncher.RunToCompletionAsync(@"C:\Program Files (x86)\KeePass Password Safe 2\KeePass.exe","\"D:\\KeePass\\KeePass.kdbx\" -pw-stdin", options);


            //reg ADD "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\EmbeddedMode\ProcessLauncher" /v AllowedExecutableFilesList /t REG_MULTI_SZ /d "C:\Program Files (x86)\KeePass Password Safe 2\KeePass.exe\0"
            //var result = await ProcessLauncher.RunToCompletionAsync(@"D:\ConsoleApp1.exe", String.Empty, options);
            //"c:\windows\system32\cmd.exe"

            //REG QUERY "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\EmbeddedMode\ProcessLauncher" /v AllowedExecutableFilesList

            //reg ADD "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\EmbeddedMode\ProcessLauncher" /f /v AllowedExecutableFilesList /t REG_MULTI_SZ /d "c:\windows\system32\cmd.exe\0"

            //reg ADD "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\EmbeddedMode\ProcessLauncher" /f /v AllowedExecutableFilesList /t REG_MULTI_SZ /d "E:\Tests\HelloWorldUWP\HelloWorldUWP\ConsoleApp1.exe\0"

            //reg ADD "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\EmbeddedMode\ProcessLauncher" / f / v AllowedExecutableFilesList / t REG_MULTI_SZ / d "E:\Tests\HelloWorldUWP\HelloWorldUWP\ConsoleApp1.exe\0"

            //IAsyncOperation<ProcessLauncherResult> RunToCompletionAsync(string fileName, string args, ProcessLauncherOptions options)

            /*pProcess.StartInfo.FileName = @"C:\Program Files (x86)\KeePass Password Safe 2\KeePass.exe";
            pProcess.StartInfo.Arguments = "\"D:\\KeePass\\KeePass.kdbx\" -pw-stdin";

            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardInput = true;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;


            pProcess.Start();

            pProcess.StandardInput.WriteLine("q1w2e3r4");
            pProcess.StandardInput.Close();*/
        }

        private async Task<string> ExecuteCommandLineString(string CommandString) {
            const string CommandLineProcesserExe = "c:\\windows\\system32\\cmd.exe";
            const uint CommandStringResponseBufferSize = 8192;
            string currentDirectory = "C:\\";

            StringBuilder textOutput = new StringBuilder((int)CommandStringResponseBufferSize);
            uint bytesLoaded = 0;

            if (string.IsNullOrWhiteSpace(CommandString))
                return ("");

            var commandLineText = CommandString.Trim();

            var standardOutput = new Windows.Storage.Streams.InMemoryRandomAccessStream();
            var standardError = new Windows.Storage.Streams.InMemoryRandomAccessStream();
            var options = new Windows.System.ProcessLauncherOptions {
                StandardOutput = standardOutput,
                StandardError = standardError
            };

            try {
                var args = "/C \"cd \"" + currentDirectory + "\" & " + commandLineText + "\"";
                var result = await Windows.System.ProcessLauncher.RunToCompletionAsync(CommandLineProcesserExe, args, options);

                //First write std out
                using (var outStreamRedirect = standardOutput.GetInputStreamAt(0)) {
                    using (var dataReader = new Windows.Storage.Streams.DataReader(outStreamRedirect)) {
                        while ((bytesLoaded = await dataReader.LoadAsync(CommandStringResponseBufferSize)) > 0)
                            textOutput.Append(dataReader.ReadString(bytesLoaded));

                        new System.Threading.ManualResetEvent(false).WaitOne(10);
                        if ((bytesLoaded = await dataReader.LoadAsync(CommandStringResponseBufferSize)) > 0)
                            textOutput.Append(dataReader.ReadString(bytesLoaded));
                    }
                }

                //Then write std err
                using (var errStreamRedirect = standardError.GetInputStreamAt(0)) {
                    using (var dataReader = new Windows.Storage.Streams.DataReader(errStreamRedirect)) {
                        while ((bytesLoaded = await dataReader.LoadAsync(CommandStringResponseBufferSize)) > 0)
                            textOutput.Append(dataReader.ReadString(bytesLoaded));

                        new System.Threading.ManualResetEvent(false).WaitOne(10);
                        if ((bytesLoaded = await dataReader.LoadAsync(CommandStringResponseBufferSize)) > 0)
                            textOutput.Append(dataReader.ReadString(bytesLoaded));
                    }
                }

                return (textOutput.ToString());
            } catch (UnauthorizedAccessException uex) {
                return ("ERROR - " + uex.Message + "\n\nCmdNotEnabled");
            } catch (Exception ex) {
                return ("ERROR - " + ex.Message + "\n");
            }
        }
    }
}
