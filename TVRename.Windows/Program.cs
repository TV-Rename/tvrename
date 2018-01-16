using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TVRename.Windows
{
    /// <summary>
    /// Application entry point.
    /// </summary>
    public static class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        [STAThread]
        public static async Task Main(string[] args)
        {
            Logger.Info($"TV Rename started with args: {string.Join(" ", args)}");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Logger.Info("Starting new instance");

            await new ApplicationBase().Run();

            Logger.Info("Application exiting");
        }
    }
}
