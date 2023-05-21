using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TVRename.App;

internal class SingleInstanceService
{
    private const string LOCAL_HOST = "127.0.0.1";
    private const int LOCAL_PORT = 19191;
    private readonly Action<string[]> onArgumentsReceived;
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
    // ReSharper disable once NotAccessedField.Local
    private Semaphore? semaphore;
    private readonly string semaphoreName = $"Global\\{Environment.MachineName}-myAppName{Assembly.GetExecutingAssembly().GetName().Version}-sid{Process.GetCurrentProcess().SessionId}";

    public SingleInstanceService(Action<string[]> onArgumentsReceived)
    {
        this.onArgumentsReceived = onArgumentsReceived;
    }

    internal bool IsFirstInstance()
    {
        if (Semaphore.TryOpenExisting(semaphoreName, out semaphore))
        {
            return false;
        }
        else
        {
            semaphore = new Semaphore(0, 1, semaphoreName);
            Task.Run(ListenForArguments);
            return true;
        }
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
