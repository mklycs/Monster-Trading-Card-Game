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

        public string handleResponseStatus(string statusCode){
            if(statusCode == "100") return "Continue";
            else if(statusCode == "101") return "Switching Protocols";
            else if(statusCode == "200") return "OK";
            else if(statusCode == "201") return "Created";
            else if(statusCode == "202") return "Accepted";
            else if(statusCode == "204") return "No Content";
            else if(statusCode == "301") return "Moved Permanently";
            else if(statusCode == "302") return "Found";
            else if(statusCode == "304") return "No Modified";
            else if(statusCode == "400") return "Bad Request";
            else if(statusCode == "401") return "Unauthorized";
            else if(statusCode == "403") return "Forbidden";
            else if(statusCode == "404") return "Not Found";
            else if(statusCode == "500") return "Internal Server Error";
            else if(statusCode == "502") return "Bad Gateway";
            else if(statusCode == "503") return "Service Unavailable";
            else return "Unknown Request";
        }

        public void Start(){
            // Starte den TCP-Listener auf Port 8080, der eingehende Verbindungen akzeptiert
            TcpListener listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();
            Console.WriteLine("Server started, listening on port 8080...");

            while(true){
                // Akzeptiere eingehende TCP-Verbindungen
                using TcpClient client = listener.AcceptTcpClient();
                using NetworkStream? stream = client.GetStream();

                // Erstellt einen Puffer (ein Array von Bytes), um die eingehenden Daten zu speichern.
                // Die Größe des Puffers wird basierend auf der maximalen Empfangsgröße des Clients festgelegt.
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
                string response = _requestHandler.HandleRequest(requestUrl.Substring(1), httpMethod, jsonBody);

                // Erstelle eine HTTP-Antwort und sende diese zurück
                byte[] responseBuffer = Encoding.UTF8.GetBytes($"{httpVersion} {response.Split(':')[0]} {handleResponseStatus(response.Split(':')[0])}\r\nContent-Length: {response.Length}\r\n\r\n{response.Split(':')[1]}");
                stream.Write(responseBuffer, 0, responseBuffer.Length); // Startet bei der Position 0 des Buffers und sendet die volle Länge des Buffers.
            }
        }
    }
}