using Microsoft.AspNetCore.SignalR;

namespace hub.Hubs
{
    public class IoTEventHub : Hub
    {
        public void NotifyUsers(string message){
            Clients.Client(Context.ConnectionId).SendAsync("notifyusers", $"sending {message}");
        }
        // private void NotifyUsers(string group, string message){
        //     Clients.Group(group).SendAsync("notifyusers", message);
        // }
        public void Echo(string name, string message)
        {
            Clients.Client(Context.ConnectionId).SendAsync("notifyusers", $"sending {message}");
        
            //Clients.Client(Context.ConnectionId).SendAsync("echo", name, message + " (echo from server)");
        }
    }
}