using System;
using System.Collections;
//using System.Runtime.Remoting;
//using System.Runtime.Remoting.Channels;
//using System.Runtime.Remoting.Channels.Ipc;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;

namespace TVRename.Ipc;

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

    private static UI? MainUi;

    /// <summary>
    /// Registers an IPC channel to an existing remote service.
    /// This allows future instances of <see cref="RemoteClient"/> to be created as transparent proxies.
    /// </summary>
    // public static void Proxy()
    // {
    //     ChannelServices.RegisterChannel(new IpcClientChannel(), true);
    //     RemotingConfiguration.RegisterWellKnownClientType(typeof(RemoteClient), $"ipc://{IPC_CHANNEL}/{IPC_SERVICE}");
    // }

    /// <summary>
    /// Binds the specified form and settings to the IPC actions allowing remote invocation.
    /// </summary>
    /// <param name="form">The form to bind actions to.</param>
    /// <param name="settings">The global application settings.</param>
    // public static void Bind(UI form)
    // {
    //     MainUi = form;
    //
    //     Hashtable channelProperties = new() { { "exclusiveAddressUse", false }, { "portName", IPC_CHANNEL } };
    //
    //     IpcServerChannel serverChannel = new(channelProperties, null);
    //     ChannelServices.RegisterChannel(serverChannel, true);
    //
    //     RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteClient), IPC_SERVICE, WellKnownObjectMode.Singleton);
    // }

    /// <summary>
    /// Focuses the window and bring to foreground.
    /// </summary>
    public void FocusWindow()
    {
        if (MainUi?.IsHandleCreated ??false)
        {
            MainUi.BeginInvoke((MethodInvoker)MainUi.FocusWindow);
        }
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Quit()
    {
        MainUi?.BeginInvoke((MethodInvoker)MainUi.Quit);
    }

    public void SendArgs(string[] args)
    {
        MainUi?.BeginInvoke(MainUi.ReceiveArgumentDelegate, new object[] { args });
    }
}
