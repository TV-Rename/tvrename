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
        private const string IpcChannel = "TVRename";
        private const string IpcService = "RemoteClient";

        private static UI ui;
        private static TVDoc doc;

        /// <summary>
        /// Registers an IPC channel to an existing remote service.
        /// This allows future instances of <see cref="RemoteClient"/> to be created as transparent proxies.
        /// </summary>
        public static void Proxy()
        {
            ChannelServices.RegisterChannel(new IpcClientChannel(), true);
            RemotingConfiguration.RegisterWellKnownClientType(typeof(RemoteClient), $"ipc://{IpcChannel}/{IpcService}");
        }

        /// <summary>
        /// Binds the specified form and settings to the IPC actions allowing remote invocation.
        /// </summary>
        /// <param name="form">The form to bind actions to.</param>
        /// <param name="settings">The global application settings.</param>
        public static void Bind(UI form, TVDoc settings)
        {
            ui = form;
            doc = settings;

            Hashtable channelProperties = new Hashtable {{"exclusiveAddressUse", false}, {"portName", IpcChannel}};

            IpcServerChannel serverChannel = new IpcServerChannel(channelProperties,null);
            ChannelServices.RegisterChannel(serverChannel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteClient), IpcService, WellKnownObjectMode.Singleton);
        }

        /// <summary>
        /// Gets or sets the action to perform when a missing folder is found.
        /// </summary>
        /// <value>
        /// The missing folder behavior.
        /// </value>
        public CommandLineArgs.MissingFolderBehavior MissingFolderBehavior
        {
            get
            {
                return doc?.Args.MissingFolder ?? CommandLineArgs.MissingFolderBehavior.Ask;
            }
            set
            {
                if (doc != null) doc.Args.MissingFolder = value;
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
            get
            {
                return doc != null && TVSettings.Instance.RenameCheck;
            }
            set
            {
                if (doc != null) TVSettings.Instance.RenameCheck = value;
            }
        }

        /// <summary>
        /// Focuses the window and bring to foreground.
        /// </summary>
        public void FocusWindow()
        {
            ui?.BeginInvoke((MethodInvoker)ui.FocusWindow);
        }

        /// <summary>
        /// Scans all files.
        /// </summary>
        public void Scan()
        {
            ui?.BeginInvoke((MethodInvoker)ui.Scan);
        }

        /// <summary>
        /// Scans all recent files.
        /// </summary>
        public void RecentScan()
        {
            ui?.BeginInvoke((MethodInvoker)ui.RecentScan);
        }

        /// <summary>
        /// Scans all missing recent episodes plus any files in download directory.
        /// </summary>
        public void QuickScan()
        {
            ui?.BeginInvoke((MethodInvoker)ui.QuickScan);
        }
        /// <summary>
         /// Processes all file tasks.
         /// </summary>
        public void ProcessAll()
        {
            ui?.BeginInvoke((MethodInvoker)ui.ProcessAll);
        }

        /// <summary>
        /// Quits the application.
        /// </summary>
        public void Quit()
        {
            ui?.BeginInvoke((MethodInvoker)ui.Quit);
        }
    }
}
