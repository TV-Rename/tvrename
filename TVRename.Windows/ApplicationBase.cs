using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Models.Settings;
using TVRename.Core.TVDB;
using TVRename.Windows.Forms;

namespace TVRename.Windows
{
    /// <summary>
    /// Provides the primary form bootstrap including a splash screen.
    /// </summary>
    public class ApplicationBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Form mainForm;
        private SplashScreen splashScreen;

        /// <summary>
        /// The default base path for application data.
        /// </summary>
        public static readonly string BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TV Rename");

        /// <summary>
        /// Sets up and starts the application after displaying a splash screen.
        /// </summary>
        public async Task Run()
        {
            // Start and display splash screen
            this.splashScreen = await RunForm<SplashScreen>();

            UpdateStatus("Loading cache");

            // Run slow operations

            Settings.FilePath = Path.Combine(BasePath, "settings.json");

            // Load TVDB cache
            Logger.Info($"Loading TVDB cache");
            await TVDB.Load(Path.Combine(BasePath, "tvdb.json"));

            // Set TVDB options
            TVDB.Instance.Language = Settings.Instance.Language;
            TVDB.Instance.Threads = Settings.Instance.DownloadThreads;

            UpdateStatus("Starting");

            // Start and display main form
            this.mainForm = await RunForm<Main>();

            // Close splash screen
            this.splashScreen.Invoke(new MethodInvoker(this.splashScreen.Dispose));

            // Focus main form
            this.mainForm.Invoke(new MethodInvoker(() =>
            {
                if (this.mainForm.WindowState == FormWindowState.Minimized) this.mainForm.WindowState = FormWindowState.Normal;

                this.mainForm.Activate();
            }));

            // Wait for main form to close
            AsyncEventHandler formClosed = new AsyncEventHandler();

            this.mainForm.Closed += formClosed.Handler;

            await formClosed.Event;
        }

        /// <summary>
        /// Updates the spash screen status message.
        /// </summary>
        /// <param name="status">The status message.</param>
        private void UpdateStatus(string status)
        {
            this.splashScreen.Invoke(new MethodInvoker(() => this.splashScreen.Status = status));
        }

        /// <summary>
        /// Runs a new instance of a form in its own execution context.
        /// </summary>
        /// <typeparam name="T">Type of form to initiate.</typeparam>
        /// <returns>Form instance.</returns>
        private static async Task<T> RunForm<T>() where T : Form, new()
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();

            Thread thread = new Thread(() =>
            {
                T form = new T();

                form.Shown += (s, a) => tcs.SetResult(form);

                try
                {
                    Application.Run(form);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);

                    new Error(ex).ShowDialog();

                    Environment.Exit(1);
                }
            });

            thread.SetApartmentState(ApartmentState.STA); // Form compontants require STA
            thread.Start();

            return await tcs.Task;
        }
    }

    public class AsyncEventHandler
    {
        private readonly TaskCompletionSource<EventArgs> tcs = new TaskCompletionSource<EventArgs>();

        public EventHandler Handler => (s, a) => this.tcs.SetResult(a);

        public Task<EventArgs> Event => this.tcs.Task;
    }

    public class AsyncEventHandler<TEventArgs>
    {
        private readonly TaskCompletionSource<TEventArgs> tcs = new TaskCompletionSource<TEventArgs>();

        public EventHandler<TEventArgs> Handler => (s, a) => this.tcs.SetResult(a);

        public Task<TEventArgs> Event => this.tcs.Task;
    }
}
