using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Windows.Forms;

namespace TVRename.Ipc
{
    /// <summary>
    /// Registers and implements <see cref="IRemoteActions" /> over a named IPC channel for transparent remote proxying.
    /// </summary>
    /// <remarks>
    /// IPC may not work correctly when debugging the process.
    /// </remarks>
    /// <seealso cref="IRemoteActions" />
    internal class RemoteClient : MarshalByRefObject, IRemoteActions
    {
        private const string IPC_CHANNEL = "TVRename";
        private const string IPC_SERVICE = "RemoteClient";

        private static UI MainUi;
        private static TVDoc Engine;

        /// <summary>
        /// Registers an IPC channel to an existing remote service.
        /// This allows future instances of <see cref="RemoteClient"/> to be created as transparent proxies.
        /// </summary>
        public static void Proxy()
        {
            ChannelServices.RegisterChannel(new IpcClientChannel(), true);
            RemotingConfiguration.RegisterWellKnownClientType(typeof(RemoteClient), $"ipc://{IPC_CHANNEL}/{IPC_SERVICE}");
        }

        /// <summary>
        /// Binds the specified form and settings to the IPC actions allowing remote invocation.
        /// </summary>
        /// <param name="form">The form to bind actions to.</param>
        /// <param name="settings">The global application settings.</param>
        public static void Bind(UI form, TVDoc settings)
        {
            MainUi = form;
            Engine = settings;

            Hashtable channelProperties = new Hashtable {{"exclusiveAddressUse", false}, {"portName", IPC_CHANNEL}};

            IpcServerChannel serverChannel = new IpcServerChannel(channelProperties,null);
            ChannelServices.RegisterChannel(serverChannel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteClient), IPC_SERVICE, WellKnownObjectMode.Singleton);
        }

        /// <summary>
        /// Gets or sets the action to perform when a missing folder is found.
        /// </summary>
        /// <value>
        /// The missing folder behavior.
        /// </value>
        public CommandLineArgs.MissingFolderBehavior MissingFolderBehavior
        {
            get => Engine?.Args.MissingFolder ?? CommandLineArgs.MissingFolderBehavior.ask;
            set
            {
                if (Engine != null) Engine.Args.MissingFolder = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to perform a rename check.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a rename check is performed; otherwise, <c>false</c>.
        /// </value>
        public bool RenameBehavior
        {
            get => Engine != null && TVSettings.Instance.RenameCheck;
            set
            {
                if (Engine != null) TVSettings.Instance.RenameCheck = value;
            }
        }

        /// <summary>
        /// Focuses the window and bring to foreground.
        /// </summary>
        public void FocusWindow()
        {
            MainUi?.BeginInvoke((MethodInvoker)MainUi.FocusWindow);
        }

        /// <summary>
        /// Scans all files.
        /// </summary>
        public void Scan()
        {
            MainUi?.BeginInvoke((MethodInvoker)MainUi.Scan);
        }

        /// <summary>
        /// Scans all recent files.
        /// </summary>
        public void RecentScan()
        {
            MainUi?.BeginInvoke((MethodInvoker)MainUi.RecentScan);
        }

        /// <summary>
        /// Scans all missing recent episodes plus any files in download directory.
        /// </summary>
        public void QuickScan()
        {
            MainUi?.BeginInvoke((MethodInvoker)MainUi.QuickScan);
        }
        /// <summary>
         /// Processes all file tasks.
         /// </summary>
        public void ProcessAll()
        {
            MainUi?.BeginInvoke((MethodInvoker)MainUi.ProcessAll);
        }

        /// <summary>
        /// Quits the application.
        /// </summary>
        public void Quit()
        {
            MainUi?.BeginInvoke((MethodInvoker)MainUi.Quit);
        }
    }
}
