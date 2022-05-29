using WebSocketSharp.Server;

namespace Application
{
    public interface IWebSocketFunctions
    {
        public WebSocketServer getWssv();
        public WebSocketServer WebSocketServerCreate(string url,int port);
        public void WebSocketServicesCreate(WebSocketServer wssv);
    }
}
