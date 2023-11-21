using Microsoft.AspNetCore.SignalR;

namespace NotesApp_Postgre.Service

{
    public class NotificationHub : Hub
    {

        // Метод для отправки уведомлений
        public async Task SendNotification(string title, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", title, message);
        }

    }
}
