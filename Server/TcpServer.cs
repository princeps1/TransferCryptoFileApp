//using System.Net.Sockets;
//using System.Net;
//using System.Text;

//public class TcpServer
//{
//    private TcpListener tcpListener;
//    private Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
//    private Func<string, Task> messageReceivedCallback;

//    public TcpServer(int port, Func<string, Task> messageReceivedCallback)
//    {
//        this.tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
//        this.messageReceivedCallback = messageReceivedCallback;
//        Console.WriteLine("Server initialized.");
//    }

//    public async Task StartServer()
//    {
//        Console.WriteLine("Starting server...");
//        try
//        {
//            this.tcpListener.Start();
//        }
//        catch (Exception e)
//        {
//            Console.WriteLine(e.Message);
//        }

//        Console.WriteLine("Server started.");
//        Console.WriteLine("Listening on port {0}.", ((IPEndPoint)tcpListener.LocalEndpoint).Port);

//        while (true)
//        {
//            TcpClient client = await this.tcpListener.AcceptTcpClientAsync();

//            // Extract the port number from the client's endpoint
//            int port = ((IPEndPoint)client.Client.RemoteEndPoint).Port;

//            clients[port] = client; // Add the connected client to the dictionary

//            _ = HandleClientAsync(client);
//        }
//    }
//    private async Task HandleClientAsync(TcpClient client)
//    {
//        Console.WriteLine("Client connected.");
//        using (NetworkStream stream = client.GetStream())
//        {
//            byte[] buffer = new byte[1024];
//            int bytesRead;

//            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
//            {
//                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
//                await messageReceivedCallback?.Invoke(message);

//                // Extract the port number from the client's endpoint
//                int senderPort = ((IPEndPoint)client.Client.RemoteEndPoint).Port;

//                // Forward the message to the other client
//                foreach (var otherClient in clients.Values)
//                {
//                    int receiverPort = ((IPEndPoint)otherClient.Client.RemoteEndPoint).Port;

//                    if (receiverPort != senderPort)
//                    {
//                        await otherClient.GetStream().WriteAsync(buffer, 0, bytesRead);
//                    }
//                }
//            }
//        }

//        // Remove the disconnected client from the dictionary
//        int disconnectedPort = ((IPEndPoint)client.Client.RemoteEndPoint).Port;
//        clients.Remove(disconnectedPort);

//        client.Close();
//    }
//}
