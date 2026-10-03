using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;

namespace TVRename.App;

internal class SingleInstanceService(Action<string[]> onArgumentsReceived)
{
    private const string LOCAL_HOST = "127.0.0.1";
    private const int LOCAL_PORT = 19191;
    private readonly Action<string[]> onArgumentsReceived = onArgumentsReceived;
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
    // ReSharper disable once NotAccessedField.Local
    private Semaphore? semaphore;
    private readonly string semaphoreName = $"Global\\{Environment.MachineName}-TVRename-{Assembly.GetExecutingAssembly().GetName().Version}.SingleInstanceLimiter";

    internal bool IsFirstInstance()
    {
        try
        {
            if (Semaphore.TryOpenExisting(semaphoreName, out semaphore))
            {
                return false;
            }
            else
            {
                semaphore = CreateSemaphore();
                Task.Run(ListenForArguments);
                return true;
            }
        }
        catch (IOException ex)
        {
            Log.Error(ex);
            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            Log.Error(ex);
            return false;
        }
        catch (ArgumentException ex)
        {
            Log.Error(ex);
            return false;
        }
        catch (WaitHandleCannotBeOpenedException ex)
        {
            Log.Error(ex);
            return false;
        }
    }

    private Semaphore CreateSemaphore()
    {
        // 1. Create a security descriptor
        var semaphoreSecurity = new SemaphoreSecurity();

        // 2. Define a rule that allows "Everyone" (WorldSid) to enter and release the semaphore
        var everyoneSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
        var accessRule = new SemaphoreAccessRule(
            everyoneSid,
            SemaphoreRights.Synchronize | SemaphoreRights.Modify, // Rights to enter/release
            AccessControlType.Allow);

        semaphoreSecurity.AddAccessRule(accessRule);

        // 3. Create the semaphore safely with the attached Access Control List (ACL)
        return SemaphoreAcl.Create(
            initialCount: 0,
            maximumCount: 1,
            name: semaphoreName,
            createdNew: out bool createdNew,
            semaphoreSecurity: semaphoreSecurity);
    }

    public static void SendArgumentsToExistingInstance() => Task.Run(SendArguments);

    private void ListenForArguments()
    {
        TcpListener tcpListener = new(IPAddress.Parse(LOCAL_HOST), LOCAL_PORT);
        try
        {
            tcpListener.Start();
            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                Task.Run(() => ReceivedMessage(client));
            }
        }
        catch (SocketException ex)
        {
            Log.Error(ex);
            tcpListener.Stop();
        }
    }

    private void ReceivedMessage(TcpClient tcpClient)
    {
        try
        {
            using NetworkStream networkStream = tcpClient.GetStream();
            string data = string.Empty;
            byte[] bytes = new byte[256];
            int bytesCount;
            while ((bytesCount = networkStream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data += Encoding.UTF8.GetString(bytes, 0, bytesCount);
            }
            onArgumentsReceived(data.Split(' '));
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    private static void SendArguments()
    {
        try
        {
            using TcpClient tcpClient = new(LOCAL_HOST, LOCAL_PORT);
            using NetworkStream networkStream = tcpClient.GetStream();
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            string message = commandLineArgs.Length > 1
                ? string.Join(" ", commandLineArgs)
                : "/focus";
            byte[] data = Encoding.UTF8.GetBytes(message);
            networkStream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
