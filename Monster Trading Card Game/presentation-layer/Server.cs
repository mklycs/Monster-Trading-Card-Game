using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace mtcg{
    // Diese Klasse stellt den TCP-Server dar, der auf Anfragen wartet
    public class Server{
        private static RequestHandler? _requestHandler;
        
        public Server(){
            _requestHandler = RequestHandler.getInstance();
        }

        public void Start(){
            TcpListener listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();
            Console.WriteLine("Server started, listening on port 8080...");

            while(true){
                using TcpClient client = listener.AcceptTcpClient();
                using NetworkStream? stream = client.GetStream();

                byte[] buffer = new byte[client.ReceiveBufferSize];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                // Parse die HTTP-Anfrage aus dem eingehenden TCP-Datenstrom
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead); //Konvertieren der bytes aus buffer in string
                string[] requestLines = request.Split("\r\n");      // \r\n Kombination für Zeilenumbruch bei Netzwerkprotokollen (\r setzt Cursor an den Anfang der Zeile und \n springt in die nächste Zeile)              
                string httpMethod = requestLines[0].Split(' ')[0];  // Erhalte die HTTP-Methode (GET, POST, DELETE)
                string requestUrl = requestLines[0].Split(' ')[1];  // Erhalte die URL der Anfrage
                string httpVersion = requestLines[0].Split(' ')[2]; // für die HTTP-Version 

                // Parse Body for POST requests
                string? jsonBody = httpMethod != "GET" ? request.Split("\r\n\r\n")[1] : null;
                _requestHandler.HandleRequest(requestUrl.Substring(1), httpMethod, jsonBody, httpVersion, stream);
            }
        }
    }
}