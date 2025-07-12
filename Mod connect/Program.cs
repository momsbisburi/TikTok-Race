using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class WebSocketServer
{
    private static readonly List<WebSocket> ConnectedClients = new List<WebSocket>();
    private static WebSocket activeWebSocket = null;  // Store a reference to the active client (mod)
    public class globalPlayer
    {
        public int id;
        public string username;
    }
    public static int playerid = 0;
    public static List<globalPlayer> globalPlayerList = new List<globalPlayer>();
    // Handles WebSocket clients
    private static async Task HandleWebSocketClient(WebSocket webSocket)
    {
        byte[] buffer = new byte[1024 * 4];
        Console.WriteLine("WebSocket client connected.");
        ConnectedClients.Add(webSocket);
        activeWebSocket = webSocket;

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message: {message}");

                    // Handle other WebSocket messages if needed
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("WebSocket client disconnected.");
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error with WebSocket client: {ex.Message}");
            await Task.Delay(1000);
            _ = HandleWebSocketClient(webSocket);
        }

    }
    public static( string ,string) GetDefaultQueryString(NameValueCollection query)
    {
       
        string playerName = query["playerName"] ?? "Unknown";
        string tiktokid = query["tiktokid"] ?? "Unknown";
        PlayerJoin(tiktokid);
        return (playerName, tiktokid);
    }

    public static void PlayerJoin(string username)
    {
        var player = globalPlayerList.Where(x=>x.username == username).FirstOrDefault();
        if (player == null) {
            playerid = playerid + 1;
            globalPlayerList.Add(new globalPlayer { username = username,id = playerid });
        }
    }
    // Handles HTTP API requests (for join commands)
    private static async Task HandleHttpRequest(HttpListenerContext context)
    {
        try
        {
            string responseText = "Invalid Request";

            // Handle GET request for /join
            if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/api/app/info")
            {
                responseText = "{\r\n  \"data\":{\r\n    \"author\":\"@zerodytrash\",\r\n    \"name\":\"AwesomeApp\",\r\n    \"version\":\"1.0\"\r\n   }\r\n}";
            }
            else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "api/features/categories")
            {
                responseText = "{\r\n    \"data\": [\r\n        {\r\n            \"categoryId\": \"1\",\r\n            \"categoryName\": \"GTA 5 \"\r\n        },}";
            }
            else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "api/features/actions?categoryId=1")
            {
                responseText = "\r\n{\r\n    \"data\": [\r\n        {\r\n            \"actionId\": \"turn_lights_off\",\r\n            \"actionName\": \"Turn Lights Off\"\r\n        }\r\n]\r\n}";
            }
            else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/join")
            {
                // Extract query string parameters

                var query = context.Request.QueryString;
                (string playerName, string tiktokid) = GetDefaultQueryString(query);                   
                string type = "join";

                Console.WriteLine($"Received join request for player: {playerName}");

                // Send the formatted message to the WebSocket client (mod) using the expected format
                if (activeWebSocket != null)
                {
                    // Create a JSON-like message to simulate what the GTA mod would send
                    string message = CreateJoinMessage(type,playerName, tiktokid);

                    // Send the message to the WebSocket client
                    await SendMessageToClient(message);
                }

                responseText = $"Player '{playerName}' joined the race!";
            }
            else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/vipjoin")
            {
                // Extract query string parameters

                var query = context.Request.QueryString;
                (string playerName, string tiktokid) = GetDefaultQueryString(query);
                string type = "vipjoin";

                Console.WriteLine($"Received join request for player: {playerName}");

                // Send the formatted message to the WebSocket client (mod) using the expected format
                if (activeWebSocket != null)
                {
                    // Create a JSON-like message to simulate what the GTA mod would send
                    string message = CreateJoinMessage(type, playerName, tiktokid);

                    // Send the message to the WebSocket client
                    await SendMessageToClient(message);
                }

                responseText = $"Player '{playerName}' joined the race!";
            }
            else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/rose")
            {
                // Extract query string parameters

                var query = context.Request.QueryString;
                (string playerName, string tiktokid) = GetDefaultQueryString(query);
                string type = "rosespeedboost";

                Console.WriteLine($"Received join request for player: {playerName}");

                // Send the formatted message to the WebSocket client (mod) using the expected format
                if (activeWebSocket != null)
                {
                    // Create a JSON-like message to simulate what the GTA mod would send
                    string message = CreateJoinMessage(type, playerName, tiktokid);

                    // Send the message to the WebSocket client
                    await SendMessageToClient(message);
                }

                responseText = $"Player '{playerName}' joined the race!";
            }
            else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/comment")
            {
                // Extract query string parameters

                var query = context.Request.QueryString;
                (string playerName, string tiktokid) = GetDefaultQueryString(query);
                string type = "comment";

                Console.WriteLine($"Received join request for player: {playerName}");

                // Send the formatted message to the WebSocket client (mod) using the expected format
                if (activeWebSocket != null)
                {
                    // Create a JSON-like message to simulate what the GTA mod would send
                    string message = CreateJoinMessage(type, playerName, tiktokid);

                    // Send the message to the WebSocket client
                    await SendMessageToClient(message);
                }

                responseText = $"Player '{playerName}' joined the race!";
            }
            else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/tap")
            {
                // Extract query string parameters

                var query = context.Request.QueryString;
                (string playerName, string tiktokid) = GetDefaultQueryString(query);
                string type = "tap";

                Console.WriteLine($"Received join request for player: {playerName}");

                // Send the formatted message to the WebSocket client (mod) using the expected format
                if (activeWebSocket != null)
                {
                    // Create a JSON-like message to simulate what the GTA mod would send
                    string message = CreateJoinMessage(type, playerName, tiktokid);

                    // Send the message to the WebSocket client
                    await SendMessageToClient(message);
                }

                responseText = $"Player '{playerName}' joined the race!";
            }

            // Send HTTP response
            byte[] buffer = Encoding.UTF8.GetBytes(responseText);
            byte[] responseData = Encoding.UTF8.GetBytes(responseText);
            HttpListenerResponse response = context.Response;
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "text/html; charset=UTF-8";
            response.Headers.Add("Access-Control-Allow-Origin: *");
            response.Headers.Add("Access-Control-Allow-Methods:*");
            response.Headers.Add("Access-Control-Allow-Headers:*");
            response.OutputStream.Write(responseData, 0, responseData.Length);
            response.OutputStream.Close();

            //context.Response.ContentLength64 = buffer.Length;
            //await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            //context.Response.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error with HTTP API: {ex.Message}");
            context.Response.StatusCode = 500;
            context.Response.Close();
        }
    }


    // Create a message in the format expected by MessageReceived in WSC
    private static string CreateJoinMessage(string type,string playerName,string tiktokid)
    {
        var playerid = globalPlayerList.Where(x => x.username == tiktokid).FirstOrDefault();
        // Create the required data format (replace with actual values for uniqueId, r_nickname, userId)
        var message = new
        {
            type = type,
            comment = "!join " + playerName, // The comment field that will be checked in WSC
            uniqueId = playerid.id.ToString(),      // Replace with actual logic if needed
            r_nickname = tiktokid,    // Replace with actual nickname if needed
            userId = tiktokid,          // Replace with actual user ID if needed
            nickname = playerName            // The player's nickname
        };

        // Convert to JSON string (or you could send a different format if required)
        return Newtonsoft.Json.JsonConvert.SerializeObject(message);
    }

    // Send the message to the WebSocket client (mod)
    private static async Task SendMessageToClient(string message)
    {
        if (activeWebSocket != null && activeWebSocket.State == WebSocketState.Open)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await activeWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public static async Task Start(string wsUri, string httpUri)
    {
        // Start WebSocket listener
        HttpListener wsListener = new HttpListener();
        wsListener.Prefixes.Add(wsUri);
        wsListener.Start();
        Console.WriteLine($"WebSocket server started at {wsUri}");

        // Start HTTP listener
        HttpListener httpListener = new HttpListener();

        httpListener.Prefixes.Add(httpUri);
        httpListener.Start();
        Console.WriteLine($"HTTP API server started at {httpUri}");

        // Handle incoming WebSocket and HTTP requests concurrently
        while (true)
        {
            Task<HttpListenerContext> wsTask = wsListener.GetContextAsync();
            Task<HttpListenerContext> httpTask = httpListener.GetContextAsync();

            var completedTask = await Task.WhenAny(wsTask, httpTask);

            if (completedTask == wsTask)
            {
                // WebSocket request
                HttpListenerContext wsContext = wsTask.Result;
                if (wsContext.Request.IsWebSocketRequest)
                {
                    WebSocketContext wsClient = await wsContext.AcceptWebSocketAsync(null);
                    _ = HandleWebSocketClient(wsClient.WebSocket);
                }
                else
                {
                    wsContext.Response.StatusCode = 400;
                    wsContext.Response.Close();
                }
            }
            else
            {
                // HTTP API request
                HttpListenerContext httpContext = httpTask.Result;
                _ = HandleHttpRequest(httpContext);
            }
        }
    }

    public static async Task Main(string[] args)
    {
        string wsUri = "http://localhost:25654/"; // WebSocket URI
        string httpUri = "http://127.0.0.1:8832/"; // HTTP API URI
        await Start(wsUri, httpUri);
    }
}
