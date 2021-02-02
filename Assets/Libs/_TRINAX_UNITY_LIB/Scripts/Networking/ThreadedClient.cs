using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// How I will lose connection = no connection to LAN, server.bat is not opened
// If I only have 1 client connected = show connection lost

namespace TRINAX.Networking {
    public class ThreadedClient : MonoBehaviour {
        #region SINGLETON
        public static ThreadedClient Instance { get; set; }
        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        #endregion
        #region DELEGATES     
        public delegate void OnReceivedMessageCallback(string msg);
        public static event OnReceivedMessageCallback OnReceivedMessage;
        #endregion
        #region private members     
        private TcpClient socketConnection;
        private Thread clientReceiveThread;
        #endregion
        public bool IsReady { get; set; }
        public bool clientReady = false;
        public bool allConnectionsReady = false;
        public bool lostMyConnection = false;
        public ConnectionLostPanel connectionLostPanel;
        public NetworkChecker NetworkChecker;
        [Header("IP & PORT")]
        public string host = "192.168.1.6";
        public int port = 7777;
        public string Message;
        const string PREFIX_DEVICE_NAME = "name:";
        const string DEVICE_NAME = "SCREEN";
        const string HEARTBEAT = "heartbeat";
        const string QUIT_THREAD = "quit";
        const string NOT_ALL_CLIENTS_CONNECTED = "NOT_ALL_CLIENTS_CONNECTED";
        const string ALL_CLIENTS_CONNECTED = "ALL_CLIENTS_CONNECTED";
        //float timer = 0;
        //float timeToSendHeartbeat = 3f;

        async void Start() {
            IsReady = false;
            await new WaitUntil(() => Trinax.Instance.isReady);

            Init();
        }

        async void Init() {
            if (NetworkChecker != null) {
                NetworkChecker.OnNetworkFound += OnNetworkFound;
                NetworkChecker.OnNetworkLost += OnNetworkLost;
            }
            NetworkChecker.StartCheckConnectionToMasterServer();
            await new WaitUntil(() => NetworkChecker.networkConnectionActive);
            ConnectToTcpServer();
        }

        public void PopulateValues(GlobalSettings settings) {
            host = settings.IP;
            //port = settings.port;
        }

        /// <summary>     
        /// Setup socket connection.    
        /// </summary>    
        /// 
        private void ConnectToTcpServer() {
            Debug.Log("Start Client Setup");
            Task _ = Setup();
            IsReady = true;
        }

        async Task Setup() {
            clientReady = false;
            Debug.Log("Connecting...");
            if (NetworkChecker.networkConnectionActive) {
                //try
                //{
                //    await Task.Delay(1000);
                //    socketConnection = new TcpClient();
                //    Debug.Log("Attempting connection...");
                //    Task connecTask = socketConnection.ConnectAsync(host, port);
                //    Task timeoutTask = Task.Delay(10);
                //    if (await Task.WhenAny(connecTask, timeoutTask) == timeoutTask)
                //    {
                //        throw new TimeoutException();
                //    }
                //    Debug.Log("<color=green>SUCCESS</color> " + "Connected to " + host + " at port: " + port);
                //    clientReceiveThread = new Thread(new ThreadStart(ListenForData));
                //    clientReceiveThread.IsBackground = true;
                //    clientReceiveThread.Start();
                //    clientReady = true;
                //    SendMessage(PREFIX_DEVICE_NAME + DEVICE_NAME);
                //}
                //catch (TimeoutException e)
                //{
                //    // reconnect
                //    Debug.Log(e.Message);
                //    Debug.Log("<color=red>FAILED</color> " + " connecting to " + host + " at port: " + port);
                //    Task _ = Reconnect();
                //}
                //try
                //{
                //    var timeOut = TimeSpan.FromSeconds(1);
                //    var cancellationCompletionSource = new TaskCompletionSource<bool>();
                //    try
                //    {
                //        using (var cts = new CancellationTokenSource(timeOut))
                //        {
                //            socketConnection = new TcpClient();
                //            var task = socketConnection.ConnectAsync(host, port);
                //            using (cts.Token.Register(() => cancellationCompletionSource.TrySetResult(true)))
                //            {
                //                if (task != await Task.WhenAny(task, cancellationCompletionSource.Task))
                //                {
                //                    throw new OperationCanceledException(cts.Token);
                //                }
                //                else
                //                {
                //                    Debug.Log("<color=green>SUCCESS</color> " + "Connected to " + host + " at port: " + port);
                //                }
                //            }
                //        }
                //    }
                //catch (OperationCanceledException e)
                //{
                //    Debug.Log(e.Message);
                //    Debug.Log("<color=red>FAILED</color> " + " connecting to " + host + " at port: " + port);
                //    Reconnect();
                //    return;
                //}
                try {
                    socketConnection = new TcpClient();
                    await socketConnection.ConnectAsync(host, port);
                    Debug.Log("<color=green>SUCCESS</color> " + "Connected to " + host + " at port: " + port);
                    clientReceiveThread = new Thread(new ThreadStart(ListenForData));
                    clientReceiveThread.IsBackground = true;
                    clientReceiveThread.Start();
                    clientReady = true;
                    SendMessage(PREFIX_DEVICE_NAME + DEVICE_NAME);
                }
                catch (Exception ex) {
                    Debug.Log("<color=red>FAILED</color> " + " connecting to " + host + " at port: " + port);
                    Debug.Log(ex.Message);
                    Reconnect();
                }
                //}
                //catch (SocketException e)
                //{
                //    // if enter here, usually is because wrong IP or cannot access 
                //    Debug.Log(e.Message);
                //    Reconnect();
                //}
            }
            else {
                Debug.LogWarning("Could not setup socket, no internet connection!");
                // invoke reconnect upon internet connection
                await new WaitUntil(() => NetworkChecker.networkConnectionActive);
                Reconnect();
            }
        }

        async void Reconnect() {
            Debug.Log("Attempting to reconnect...");
            UnityMainThreadDispatcher.Instance().Enqueue(DispatchConnectionLost());
            clientReady = false;
            allConnectionsReady = false;
            // SUPER RISKY MOVE HERE
            //if (clientReceiveThread != null && clientReceiveThread.IsAlive)
            //    clientReceiveThread.Abort();
            while (!clientReady) {
                await new WaitForSeconds(1f);
                try {
                    Debug.Log("Attempting connection...");
                    //await Task.Delay(1000);
                    socketConnection = new TcpClient();
                    await socketConnection.ConnectAsync(host, port);
                    //Task connecTask = socketConnection.ConnectAsync(host, port);
                    //Task timeoutTask = Task.Delay(100);
                    //if (await Task.WhenAny(connecTask, timeoutTask) == timeoutTask)
                    //{
                    //    throw new TimeoutException();
                    //}
                    Debug.Log("<color=green>SUCCESS</color> " + "Connected to " + host + " at port: " + port);
                    clientReceiveThread = new Thread(new ThreadStart(ListenForData));
                    clientReceiveThread.IsBackground = true;
                    clientReceiveThread.Start();
                    clientReady = true;
                    SendMessage(PREFIX_DEVICE_NAME + DEVICE_NAME);
                    break;
                }
                catch (Exception e) {
                    // reconnect
                    Debug.Log(e.Message);
                    Debug.Log("<color=red>FAILED</color> " + " connecting to " + host + " at port: " + port);
                }
                //var timeOut = TimeSpan.FromSeconds(1);
                //var cancellationCompletionSource = new TaskCompletionSource<bool>();
                //try
                //{
                //    using (var cts = new CancellationTokenSource(timeOut))
                //    {
                //        socketConnection = new TcpClient();
                //        var task = socketConnection.ConnectAsync(host, port);
                //        using (cts.Token.Register(() => cancellationCompletionSource.TrySetResult(true)))
                //        {
                //            if (task != await Task.WhenAny(task, cancellationCompletionSource.Task))
                //            {
                //                throw new OperationCanceledException(cts.Token);
                //            }
                //            else
                //            {
                //                Debug.Log("<color=green>SUCCESS</color> " + "Connected to " + host + " at port: " + port);
                //                break;
                //            }
                //        }
                //    }
                //}
                //catch (OperationCanceledException e)
                //{
                //    Debug.Log(e.Message);
                //    Debug.Log("<color=red>FAILED</color> " + " connecting to " + host + " at port: " + port);
                //}
                //socketConnection = new TcpClient();
                //await socketConnection.ConnectAsync(host, port);
            }
            //clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            //clientReceiveThread.IsBackground = true;
            //clientReceiveThread.Start();
            //clientReady = true;
            //SendMessage(PREFIX_DEVICE_NAME + DEVICE_NAME);
            //UnityMainThreadDispatcher.Instance().Enqueue(DispatchConnectionFound());
        }
        /// <summary>     
        /// Runs in background clientReceiveThread; Listens for incomming data.     
        /// </summary>     
        private void ListenForData() {
            Debug.Log("Listening...");
            try {
                Byte[] bytes = new Byte[1024];
                while (clientReady) {
                    try {
                        // Get a stream object for reading              
                        using (NetworkStream stream = socketConnection.GetStream()) {
                            try {
                                int length;
                                // Read incomming stream into byte arrary.                  
                                while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {
                                    var incommingData = new byte[length];
                                    Array.Copy(bytes, 0, incommingData, 0, length);
                                    // Convert byte array to string message.                        
                                    string serverMessage = Encoding.ASCII.GetString(incommingData).Trim();
                                    Debug.Log("Server: " + serverMessage);
                                    UnityMainThreadDispatcher.Instance().Enqueue(DispatchOnReceived(serverMessage));
                                    Message = serverMessage;
                                    CheckForAllClientsConnected(serverMessage);
                                }
                            }
                            catch (Exception e) {
                                //if (!string.IsNullOrEmpty(e.Message))
                                //{
                                Debug.Log(e.Message);
                                //    Reconnect();
                                //}
                                //else
                                //{
                                //    Debug.Log("UNKNOWN ERROR MESSAGE");
                                //}
                                break;
                            }
                        }
                    }
                    catch (Exception e) {
                        Debug.Log(e.Message);
                        break;
                    }
                }
            }
            catch (SocketException socketException) {
                Debug.Log("Socket exception: " + socketException);
            }
        }
        IEnumerator DispatchOnReceived(string msg) {
            OnReceivedMessage?.Invoke(msg);
            yield return null;
        }
        /// <summary>     
        /// Send message to server using socket connection.     
        /// </summary>    
        public void SendMessage(string msgToSend) {
            if (!clientReady) {
                return;
            }
            try {
                string str = msgToSend + "\n";
                // Get a stream object for writing.             
                NetworkStream stream = socketConnection.GetStream();
                if (stream.CanWrite) {
                    // Convert string message to byte array.                 
                    byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(str);
                    // Write byte array to socketConnection stream.
                    if (clientMessageAsByteArray.Length <= 0) {
                        Debug.LogWarning("Dont send 0 bytes to stream!");
                        return;
                    }
                    stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                    Debug.Log("Send " + str + " of bytes: " + clientMessageAsByteArray.Length);
                }
            }
            catch (Exception e) {
                Debug.Log(e.Message);
            }
        }

        IEnumerator DispatchConnectionLost() {
            connectionLostPanel.Show();
            //if (Trinax.Instance.scene != SCENE.MAIN) {
            //    //TrinaxHelperMethods.ChangeLevel(SCENE.MAIN, null);
            //}
            yield return null;
        }

        IEnumerator DispatchConnectionFound() {
            connectionLostPanel.Hide();
            yield return null;
        }

        void OnNetworkFound() {
            if (lostMyConnection) {
                lostMyConnection = false;
                Reconnect();
            }
        }

        void OnNetworkLost() {
            lostMyConnection = true;
            clientReady = false;
            UnityMainThreadDispatcher.Instance().Enqueue(DispatchConnectionLost());
        }

        void CheckForAllClientsConnected(string msg) {
            //Debug.Log("Checking for all clients connected....");
            if (msg == NOT_ALL_CLIENTS_CONNECTED) {
                //Debug.Log("Not all clients connected");
                allConnectionsReady = false;
                UnityMainThreadDispatcher.Instance().Enqueue(DispatchConnectionLost());
            }
            else if (msg == ALL_CLIENTS_CONNECTED) {
                //Debug.Log("All clients connected");
                allConnectionsReady = true;
                UnityMainThreadDispatcher.Instance().Enqueue(DispatchConnectionFound());
            }
        }
        private void OnApplicationQuit() {
            SendMessage(QUIT_THREAD);
            clientReady = false;
            // SUPER RISKY MOVE HERE
            //if (clientReceiveThread != null && clientReceiveThread.IsAlive)
            //    clientReceiveThread.Abort();
        }
    }
}
