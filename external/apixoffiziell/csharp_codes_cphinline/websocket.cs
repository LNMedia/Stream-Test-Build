using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

public class CPHInline
{
    private static readonly Uri serverUri = new("ws://192.168.2.101:3251");
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly SemaphoreSlim connectionLock = new(1, 1);
    private static ClientWebSocket ws = null!;
    private static CancellationTokenSource? cts;

    private string GetVar(string key, bool persist = false) => CPH.GetGlobalVar<string>(key, persist);
    private void Log(string message) => Console.WriteLine($"[WebSocket] {DateTime.Now:HH:mm:ss} | {message}");

    public bool Send() => SendAsync().GetAwaiter().GetResult();

    private async Task<bool> EnsureConnectedAsync()
    {
        if (ws is { State: WebSocketState.Open }) return true;

        await connectionLock.WaitAsync();
        try
        {
            if (ws is { State: WebSocketState.Open }) return true;

            await DisconnectInternalAsync();

            ws?.Dispose();
            ws = new ClientWebSocket();

            cts?.Dispose();
            cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            Log("🔌 Versuche Verbindungsaufbau...");
            await ws.ConnectAsync(serverUri, cts.Token);

            if (ws.State == WebSocketState.Open)
            {
                Log("✅ Verbindung erfolgreich aufgebaut.");
                return true;
            }

            Log("❌ Verbindung fehlgeschlagen.");
            return false;
        }
        catch (OperationCanceledException)
        {
            Log("❌ Timeout beim Verbindungsaufbau.");
            return false;
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler beim Verbinden: {ex.Message}");
            return false;
        }
        finally
        {
            connectionLock.Release();
        }
    }

    private async Task<bool> SendAsync()
    {
        try
        {
            if (!await EnsureConnectedAsync())
                return false;

            var payload = BuildPayload();
            var json = JsonSerializer.Serialize(payload, jsonOptions);
            var buffer = Encoding.UTF8.GetBytes(json);
            var segment = new ArraySegment<byte>(buffer);

            await ws.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            Log("📤 Nachricht gesendet.");
            return true;
        }
        catch (WebSocketException ex)
        {
            Log($"❌ WebSocket-Fehler: {ex.Message}");
            await DisconnectInternalAsync();
            return false;
        }
        catch (Exception ex)
        {
            Log($"❌ Allgemeiner Fehler: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> DisconnectInternalAsync()
    {
        try
        {
            if (ws != null && (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived))
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client Disconnect", CancellationToken.None);
                Log("🔌 Verbindung getrennt.");
            }

            return true;
        }
        catch (Exception ex)
        {
            Log($"⚠️ Fehler beim Trennen: {ex.Message}");
            return false;
        }
    }

    private object BuildPayload() => new
    {
        channelData = new
        {
            streamerChannel = GetVar("ws_streamerChannel", true),
            gameName = GetVar("ws_gameName", true),
            gameImage = GetVar("ws_gameImage", true)
        },
        mainData = new
        {
            timeStamp = GetVar("ws_timeStamp"),
            sourceType = GetVar("ws_sourceType"),
            eventType = GetVar("ws_eventType"),
            actionType = GetVar("ws_actionType"),
            message = GetVar("ws_message")
        },
        userData = new
        {
            userColor = GetVar("ws_userColor"),
            userName = GetVar("ws_userName"),
            userGroups = GetVar("ws_userGroups"),
            userType = GetVar("ws_userType"),
            userPicture = GetVar("ws_userPicture"),
            userCreatedAt = GetVar("ws_userCreatedAt")
        },
        permissionData = new
        {
            isFollowing = GetVar("ws_isFollowing"),
            isSubscriber = GetVar("ws_isSubscriber"),
            isVip = GetVar("ws_isVip"),
            isModerator = GetVar("ws_isModerator"),
            isBot = GetVar("ws_isBot")
        },
        followData = new
        {
            followAgeLong = GetVar("ws_followAgeLong"),
            followDate = GetVar("ws_followDate")
        },
        subscriptionData = new
        {
            userSubMonth = GetVar("ws_userSubMonth"),
            userSubMonthStreak = GetVar("ws_userSubMonthStreak"),
            userSubTier = GetVar("ws_userSubTier"),
            userSubsGiftedMonths = GetVar("ws_userSubsGiftedMonths"),
            userSubsGiftedNow = GetVar("ws_userSubsGiftedNow"),
            userSubsGiftedTotal = GetVar("ws_userSubsGiftedTotal"),
            userSubRecipients = GetVar("ws_userSubRecipients")
        },
        raidData = new
        {
            raidViewers = GetVar("ws_raidViewers"),
            raidToUser = GetVar("ws_raidToUser"),
            raidToPicture = GetVar("ws_raidToPicture")
        },
        cheerData = new
        {
            cheerValue = GetVar("ws_cheerValue")
        },
        donationData = new
        {
            donationUser = GetVar("ws_donationUser"),
            donationPicture = GetVar("ws_donationPicture"),
            donationAmount = GetVar("ws_donationAmount"),
            donationCurrency = GetVar("ws_donationCurrency"),
            donationMessage = GetVar("ws_donationMessage")
        },
        latestData = new
        {
            latestFollower = GetVar("ws_latestFollower", true),
            latestSubscriber = GetVar("ws_latestSubscriber", true),
            latestCheerer = GetVar("ws_latestCheerer", true),
            latestDonator = GetVar("ws_latestDonator", true)
        },
        counterData = new
        {
            viewerCounter = GetVar("ws_viewerCounter"),
            followCounter = GetVar("ws_followerCounter", true),
            subscriberCounter = GetVar("ws_subscriberCounter", true)
        },
        clipData = new
        {
            clipSuccess = GetVar("ws_clipSuccess"),
            clipCreated = GetVar("ws_clipCreated"),
            clipUrl = GetVar("ws_clipUrl")
        }
    };
}
