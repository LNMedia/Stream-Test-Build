using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class CPHInline
{
    private static ClientWebSocket? ws = null;
    private static readonly Uri serverUri = new Uri("ws://192.168.2.101:3251");

    // Sync-Methode zum Verbinden (extern aufrufbar als Method)
    public bool Connect()
    {
        return ConnectAsync().GetAwaiter().GetResult();
    }

    // Sync-Methode zum Senden der Nachricht (extern aufrufbar als Method)
    public bool Send()
    {
        // Variablen aus Streamer.Bot holen
        string channel = CPH.GetGlobalVar<string>("ws_channel", true);
        string bot = CPH.GetGlobalVar<string>("ws_bot", true);
        string eventType = CPH.GetGlobalVar<string>("ws_type", true);
        string username = CPH.GetGlobalVar<string>("ws_username", true);
        string group = CPH.GetGlobalVar<string>("ws_group", true);
        string action = CPH.GetGlobalVar<string>("ws_action", true);
        string message = CPH.GetGlobalVar<string>("ws_message", true);

        var payload = new
        {
            channel = channel,
            bot = bot,
            type = eventType,
            username = username,
            group = group,
            action = action,
            message = message
        };

        string jsonMessage = JsonSerializer.Serialize(payload);

        return SendMessageAsync(jsonMessage).GetAwaiter().GetResult();
    }

    // Sync-Methode zum Trennen (extern aufrufbar als Method)
    public bool Disconnect()
    {
        DisconnectAsync().GetAwaiter().GetResult();
        return true;
    }

    // Async-Verbindung aufbauen
    private async Task<bool> ConnectAsync()
    {
        if (ws != null && ws.State == WebSocketState.Open)
            return true;

        ws = new ClientWebSocket();
        try
        {
            await ws.ConnectAsync(serverUri, CancellationToken.None);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket-Verbindung fehlgeschlagen: {ex.Message}");
            ws = null;
            return false;
        }
    }

    // Async-Nachricht senden
    private async Task<bool> SendMessageAsync(string jsonMessage)
    {
        if (ws == null || ws.State != WebSocketState.Open)
        {
            bool connected = await ConnectAsync();
            if (!connected)
                return false;
        }

        byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);
        try
        {
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Senden fehlgeschlagen: {ex.Message}");
            return false;
        }
    }

    // Async-Verbindung trennen
    private async Task DisconnectAsync()
    {
        if (ws != null)
        {
            try
            {
                if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Schließen", CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Trennen: {ex.Message}");
            }
            finally
            {
                ws.Dispose();
                ws = null;
            }
        }
    }
}
