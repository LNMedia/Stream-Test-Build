// main handler
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class CPHInline
{
    public bool Execute()
    {
        string rawInput = args["rawInput"]?.ToString() ?? "";
        string[] inputParts = rawInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (inputParts.Length == 0)
        {
            CPH.SendMessage("❌ Kein Befehl angegeben.", true);
            return false;
        }

        string command = inputParts[0];

        // Liste der Befehle, die die vollständige Befehlsliste ausgeben
        var listTriggers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "!commands", "!befehle"
        };

        // Lade alle Commands
        List<CommandData> commandList = CPH.GetCommands();
        if (commandList == null || commandList.Count == 0)
        {
            CPH.SendMessage("⚠️ Keine Befehle gefunden.", true);
            return false;
        }

        // Wenn ein Befehl aus der Liste für Befehlsübersicht kommt
        if (listTriggers.Contains(command))
        {
            var sortedCommands = commandList
                .Where(cmd => cmd.Commands != null && cmd.Commands.Count > 0)
                .Select(cmd => new
                {
                    Original = cmd,
                    Prefix = ExtractPrefix(cmd.Name),
                    Suffix = ExtractSuffix(cmd.Name),
                    SuffixRank = GetSuffixRank(ExtractSuffix(cmd.Name))
                })
                .OrderBy(x => x.SuffixRank)
                .ThenBy(x => x.Prefix, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var entry in sortedCommands)
            {
                var cmd = entry.Original;
                List<string> commands = cmd.Commands
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (commands.Count == 0)
                    continue;

                string header = $"🔹 {cmd.Name}:\n";
                List<string> chunks = ChunkCommandList(commands, 450 - header.Length);

                for (int i = 0; i < chunks.Count; i++)
                {
                    string partPrefix = chunks.Count > 1 ? $"(Teil {i + 1}/{chunks.Count}) " : "";
                    CPH.SendMessage($"{header}{partPrefix}{chunks[i]}", true);
                }
            }

            return false;
        }

        // Prüfen, ob der eingegebene Befehl in einem der Commands existiert
        var allCommands = commandList
            .SelectMany(c => c.Commands)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
		
        if (allCommands.Contains(command))
		{
			CPH.RunAction("## COMMAND EXECUTER ##", true);
			return false; // <-- Command ausführen zulassen!
		}
		else
		{
			CPH.RunAction("-- Command not found --", true);
			return false;
		}

        return false;
    }

    private List<string> ChunkCommandList(List<string> commands, int maxLength)
    {
        List<string> result = new List<string>();
        StringBuilder current = new StringBuilder();

        foreach (var cmd in commands)
        {
            if (current.Length + cmd.Length + 2 > maxLength)
            {
                result.Add(current.ToString().TrimEnd(',', ' ', '\n'));
                current.Clear();
            }

            current.Append(cmd + ", ");
        }

        if (current.Length > 0)
            result.Add(current.ToString().TrimEnd(',', ' ', '\n'));

        return result;
    }

    private string ExtractPrefix(string name)
    {
        int index = name.LastIndexOf(" (");
        return index >= 0 ? name.Substring(0, index).Trim() : name;
    }

    private string ExtractSuffix(string name)
    {
        Match match = Regex.Match(name, @"\((.*?)\)");
        return match.Success ? match.Groups[1].Value.ToLowerInvariant() : "jeder";
    }

    private int GetSuffixRank(string suffix)
    {
        return suffix switch
        {
            "jeder" => 0,
            "follower+" => 1,
            "sub+" => 2,
            "vip+" => 3,
            "mod" => 4,
            _ => 5 // unbekannte Suffixe kommen ganz hinten
        };
    }
}


// textout only
using System;
using System.Collections.Generic;

public class CPHInline {
	public bool Execute() {
		string rawInput = args["rawInput"].ToString();
        string command = rawInput.Split(' ')[0].ToLowerInvariant();
        string userName = args["userName"].ToString();
        string user = args["userName"].ToString();
        string broadcaster = args["broadcastUser"].ToString();
        
        bool isFollower = (bool)args["isFollowing"];
        bool isFollowerPlus = (bool)args["isFollowing"] || (bool)args["isSubscribed"] || (bool)args["isVip"] || (bool)args["isModerator"];
        bool isSubPlus = (bool)args["isSubscribed"] || (bool)args["isVip"] || (bool)args["isModerator"];
        bool isVipPlus = (bool)args["isVip"] || (bool)args["isModerator"];
        bool isMod = (bool)args["isModerator"];
        bool isEveryone = true;
        
        if (isEveryone) {
        	if (command.Equals("!category", StringComparison.OrdinalIgnoreCase)) {
				string gameName = CPH.GetGlobalVar<string>("ws_gameName").ToString();
				string gameImage = CPH.GetGlobalVar<string>("ws_gameImage").ToString();
				CPH.SetGlobalVar("ws_eventType", "command", false);
				CPH.SetGlobalVar("ws_actionType", "showGame", false);
				CPH.SendMessage($"Wir befinden uns aktuell in der Kategorie: {gameName}", true);
				CPH.ExecuteMethod("Websocket", "Send");
				CPH.ClearNonPersistedGlobals();
				return false;
			}
        	else if (command.Equals("!streamplan", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Hier im Kalender findest du unsere Streamingzeiten: 🐺 https://pixelundpils.de/calendar 🐺 (WIP)", true);
				return false;
			}
			// SOCIAL MEDIA
        	else if (command.Equals("!discord", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Hier geht es zu unserem epischen Discord: 🐺 https://discord.gg/vjg7kKyH2m 🐺 (Hier gibt es Kekse ^^)", true);
				return false;
			}
        	else if (command.Equals("!instagram", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Mein Instagram – Wo es mehr Bilder von zufälligen Gegenständen gibt als von mir: 🐺 https://instagram.com/einfachandi98 🐺", true);
				return false;
			}
        	else if (command.Equals("!tiktok", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Ich bin zwar selten dort, aber du hast so lieb gefragt. 🐺 Gaming: https://tiktok.com/@apixoffiziell 🐺 Musik: https://tiktok.com/@andifenrirofficial 🐺 Podcast: https://tiktok.com/@pixelundpils 🐺", true);
				return false;
			}
        	else if (command.Equals("!youtube", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Na gut, überredet. 🐺 Gaming: https://youtube.com/@apixoffiziell 🐺 Musik: https://youtube.com/@andifenrirofficial 🐺 Podcast: https://youtube.com/@pixelundpils 🐺", true);
				return false;
			}
        	else if (command.Equals("!steam", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Du willst mal mit mir zocken? Na gut, aber sei lieb (bestenfalls auch über 18, ist aber keine Pflicht, solange du dich benimmst) 🐺 https://steamcommunity.com/id/apixoffiziell 🐺", true);
				return false;
			}
			else if (command.Equals("!pixelundpils", StringComparison.OrdinalIgnoreCase) ||
					 command.Equals("!podcast", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("4 Bekloppte reden über irgendwelchen Schwachsinn. Viel Spaß dabei :) 🐺 https://pixelundpils.de 🐺 https://twitch.tv/pixelundpils 🐺 https://youtube.com/@pixelundpils 🐺 https://tiktok.com/@pixelundpils 🐺", true);
				return false;
			}
			else if (command.Equals("!spotify", StringComparison.OrdinalIgnoreCase) ||
					 command.Equals("!andifenrir", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Nun ja, noch ist hier nicht wirklich viel zu sehen. Ich werde aber bald damit anfangen :)", true);
				return false;
			}
			else if (command.Equals("!playlist", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Hier hast du meine Playlist: 🐺 https://open.spotify.com/playlist/5C2IK5kGKxpYcDqTeDfNqy?si=2eb01854c7594520 🐺", true);
				return false;
			}
        }
        if (isFollowerPlus) {
        	// ALLGEMEIN
        	if (command.Equals("!afk", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage($"{userName} ist kurz AFK! ☕️ Vielleicht auf geheimer Mission, um Kaffee zu holen 🚀, gegen einen leeren Kühlschrank zu kämpfen 🍕🥊 oder in einen unerwarteten Kuschelmarathon mit der Katze verwickelt zu werden 🐱💤. Falls die Rückkehr länger dauert, schickt Snacks 🍫 oder ein Rettungsteam 🚁😆!", true);
				return false;
			}
        	else if (command.Equals("!hey", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage($"{userName} betritt den Chat mit einem epischen Auftritt! 🚀✨ Musik spielt 🎶, Lichter blinken 💡, und die Menge tobt! 🎉 Bereit für Action? Dann lasst uns loslegen! 😎🔥", true);
				return false;
			}
        	else if (command.Equals("!!lebens-eichen", StringComparison.OrdinalIgnoreCase) ||
					 command.Equals("!lebenseichen", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage($"{userName} gibt ein episches Lebens-Eichen! 🌳💥 Die Wurzeln tiefer als der Schlaf nach einem All-you-can-eat-Buffet 🍕😴, die Äste stärker als der Wille, morgens nicht aufzustehen! 💪😂 Egal, wie der Sturm tobt, diese Eiche wackelt höchstens vor Lachen! 🌪️🤣🔥", true);
				return false;
			}
        	else if (command.Equals("!lurk", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage($"Achtung, Achtung! {userName} begibt sich auf eine geheime Mission! 🥷🎮 Tarnmodus aktiviert, Chat-Verkehr minimiert – doch wir wissen: Du beobachtest uns aus dem Schatten! 👀🚀 Bleib undercover, Agent %userName%, und vergiss nicht – wir zählen auf dich! 🤫🔥", true);
				return false;
			}
        	else if (command.Equals("!regeln", StringComparison.OrdinalIgnoreCase) ||
					 command.Equals("!rules", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Regeln: 1. Kein Heulen! – Nur Lachen! 😂 2. Keine Dramen! – Kein Wolfsgeheule! 🐺 3. Memes klauen? – Jagen statt stehlen! 🖼️💥 4. Keine Nackt-Selfies! – Fell bedecken! 📸🙅‍♂️ 5. Trolle? – Nur mit Keksen! 🍪 6. AFK? – Nur auf Jagd, immer mit Kaffee! ☕️🚀 7. Schlechte Witze raus! – Gute ins Rudel! 🏆 Wer nicht folgt, muss den 'Eichentanz' machen! 🌳💃", true);
				return false;
			}
			// STATISTICAL
        	else if (command.Equals("!accountage", StringComparison.OrdinalIgnoreCase)) {
				CPH.TryGetArg("msgId", out string messageId);
				string accountAgeStr = args["accountAge"].ToString();
				string createdAt = args["createdAt"].ToString();

				if (double.TryParse(accountAgeStr, out double secondsTotal)) {
					TimeSpan ts = TimeSpan.FromSeconds(secondsTotal);
					int years = (int)(secondsTotal / (365.25 * 24 * 3600));
					secondsTotal -= years * 365.25 * 24 * 3600;
					int months = (int)(secondsTotal / (30.44 * 24 * 3600));
					secondsTotal -= months * 30.44 * 24 * 3600;
					int days = ts.Days % 30;
					int hours = ts.Hours;
					int minutes = ts.Minutes;
					int seconds = ts.Seconds;

					List<string> parts = new();
					if (years > 0) parts.Add($"{years} Jahr(e)");
					if (months > 0) parts.Add($"{months} Monat(e)");
					if (days > 0) parts.Add($"{days} Tag(e)");
					if (hours > 0) parts.Add($"{hours} Stunde(n)");
					if (minutes > 0) parts.Add($"{minutes} Minute(n)");
					if (seconds > 0 || parts.Count == 0) parts.Add($"{seconds} Sekunde(n)");

					string result = string.Join(", ", parts);
					CPH.TwitchReplyToMessage($"Dein Account wurde am {createdAt} erstellt. Der Account ist {result} alt.", messageId, true);
				}
				return false;
			}
			else if (command.Equals("!followage", StringComparison.OrdinalIgnoreCase)) {
				CPH.TryGetArg("msgId", out string messageId);
				if (isFollower) {
					string followAge = args["followAgeLong"].ToString();
					string followDate = args["followDate"].ToString();
					CPH.TwitchReplyToMessage($"Du folgst seit dem {followDate}. Das sind {followAge}.", messageId, true);
				} else {
					CPH.TwitchReplyToMessage("Leider folgst du diesem Kanal noch nicht. Lass gerne ein Follow da, um diese Daten zu sehen", messageId, true);
				}
				return false;
			}
			else if (command.Equals("!watchtime", StringComparison.OrdinalIgnoreCase)) {
				CPH.TryGetArg("msgId", out string messageId);
				if (user == broadcaster) {
					CPH.TwitchReplyToMessage("Als Broadcaster des Streams kannst du wohl schlecht eine Watchtime haben oder?", user, true);
					return false;
				}
				long watchtime = CPH.GetTwitchUserVar<long?>(user, "watchtime", true) ?? 0;
				TimeSpan diff = TimeSpan.FromSeconds(watchtime);
				string formattedtime;
				int years = diff.Days / 365;
				int remainingDays = diff.Days % 365;
				int months = remainingDays / 31;
				int days = remainingDays % 31;
				if (watchtime < 60) formattedtime = $"{diff.Seconds} Sekunde(n)";
				else if (watchtime < 3600) formattedtime = $"{diff.Minutes} Minute(n), {diff.Seconds} Sekunde(n)";
				else if (watchtime < 86400) formattedtime = $"{diff.Hours} Stunde(n), {diff.Minutes} Minute(n), {diff.Seconds} Sekunde(n)";
				else if (watchtime < 2628288) formattedtime = $"{diff.Days} Tag(e), {diff.Hours} Stunde(n), {diff.Minutes} Minute(n), {diff.Seconds} Sekunde(n)";
				else if (watchtime < 31536000) formattedtime = $"{months} Monat(e), {days} Tag(e), {diff.Hours} Stunde(n), {diff.Minutes} Minute(n), {diff.Seconds} Sekunde(n)";
				else formattedtime = $"{years} Jahr(e), {months} Monat(e), {days} Tag(e), {diff.Hours} Stunde(n), {diff.Minutes} Minute(n), {diff.Seconds} Sekunde(n)";
				CPH.SetArgument("watchtimeFormatted", formattedtime);
				return false;
			}
			else if (command.Equals("!chatstats", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Die Chatstatistiken findest du hier: 🐺 https://stats.streamelements.com/c/apixoffiziell 🐺", true);
				return false;
			}
			// SUPPORT
			else if (command.Equals("!donation", StringComparison.OrdinalIgnoreCase) ||
					 command.Equals("!spende", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Vielen Dank für deine Unterstützung! 💖 Du kannst gerne hier spenden: https://www.tipeeestream.com/apixoffiziell oder gerne auch über https://bunq.me/LNMediaCreators", true);
				return false;
			}
			else if (command.Equals("!partner", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Schau mal bei meinen Partnern vorbei! 🎮 InstantGaming – Die beste Adresse für günstige Games und DLCs: https://www.instant-gaming.com/?igr=apixoffiziell 🚀 Streamboost – Dein Partner in Sachen 'Werbung im Stream': https://streamboost.de Unterstützt die Partner, die auch mich unterstützen! 🐺", true);
				return false;
			}
			else if (command.Equals("!instantgaming", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Wenn du auf der Suche nach günstigen Games und DLCs bist, dann schau unbedingt bei InstantGaming vorbei! 👉 https://www.instant-gaming.com/?igr=apixoffiziell", true);
				return false;
			}
			else if (command.Equals("!streamboost", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Streamboost ist eine Werbeplattform, mit der Streamer ganz einfach Werbung direkt im Stream einblenden können 👉 https://streamboost.de", true);
				return false;
			}
			else if (command.Equals("!subscribe", StringComparison.OrdinalIgnoreCase) ||
					 command.Equals("!abonnieren", StringComparison.OrdinalIgnoreCase)) {
				CPH.SendMessage("Du willst mich unterstützen, während ich versuche, im Spiel nicht immer direkt ins Verderben zu rennen? Dann nutze diesen Link: 🐺 https://subs.twitch.tv/apixoffiziell 🐺", true);
				return false;
			}
        }
        return true;
	}
}


// economy system
using System;
using System.Text;
using System.Collections.Generic;

public class CPHInline {
	public bool Execute() {
		string rawInput = args["rawInput"].ToString();
		string command = rawInput.Split(' ')[0].ToLowerInvariant();
		string userName = args["user"].ToString().ToLower();
		CPH.TryGetArg("msgId", out string messageId);
        
        bool isFollower = (bool)args["isFollowing"];
        bool isFollowerPlus = (bool)args["isFollowing"] || (bool)args["isSubscribed"] || (bool)args["isVip"] || (bool)args["isModerator"];
        bool isSubPlus = (bool)args["isSubscribed"] || (bool)args["isVip"] || (bool)args["isModerator"];
        bool isVipPlus = (bool)args["isVip"] || (bool)args["isModerator"];
        bool isMod = (bool)args["isModerator"];
        bool isEveryone = true;
        
		bool IsAccountLocked(string targetUser) {
			string status = CPH.GetTwitchUserVar<string>(targetUser, "accountStatus", true);
			if (status == "locked") {
				CPH.SendMessage($"Das Bankkonto von {targetUser} ist aktuell gesperrt und kann nicht bearbeitet werden.");
				return true;
			}
			return false;
		}
		bool IsMyAccountLocked(string user) {
			string status = CPH.GetTwitchUserVar<string>(user, "accountStatus", true);
			CPH.TryGetArg("msgId", out string messageId);
			if (status == "locked") {
				CPH.TwitchReplyToMessage("Dein Kontostatus ist: GESPERRT ⛔", messageId, true);
				return true;
			} else {
				CPH.TwitchReplyToMessage("Dein Kontostatus ist: AKTIV ✅", messageId, true);
				return true;
			}
			return false;
		}
		bool IsMyAccountCheckLocked(string user) {
			string status = CPH.GetTwitchUserVar<string>(user, "accountStatus", true);
			CPH.TryGetArg("msgId", out string messageId);
			if (status == "locked") {
				CPH.TwitchReplyToMessage("Dein Kontostatus ist: GESPERRT ⛔", messageId, true);
				return true;
			}
			return false;
		}
		
		if (isMod) {
			if (command.Equals("!deletebankaccount", StringComparison.OrdinalIgnoreCase)) {
				string[] parts = rawInput.Split(new char[] { ' ' }, 2);
				if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1])) {
					CPH.SendMessage("Bitte gib einen Namen nach dem Befehl ein, z. B. !deletebankaccount Username", true);
					return false;
				}
				string targetUser = parts[1].Trim().TrimStart('@').ToLower();
				int accountValue = CPH.GetTwitchUserVar<int>(targetUser, "moneyBank", true);
				if (accountValue > 0) {
					CPH.UnsetTwitchUserVar(targetUser, "moneyBank");
					CPH.UnsetTwitchUserVar(targetUser, "accountStatus");
					CPH.UnsetTwitchUserVar(targetUser, "transactions");
					CPH.SendMessage($"Das Bankkonto von {targetUser} wurde erfolgreich gelöscht.", true);
				} else {
					CPH.SendMessage($"Der Nutzer {targetUser} hat kein Konto oder ist nicht registriert.", true);
				}
				return false;
			}
			else if (command.Equals("!setaccountbalance", StringComparison.OrdinalIgnoreCase)) {
				string[] parts = rawInput.Split(new char[] { ' ' }, 3);
				if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1])) {
					CPH.SendMessage("Bitte gib einen Namen nach dem Befehl ein, z. B. !setaccountbalance Username 1500", true);
					return false;
				}
				string targetUser = parts[1].Trim().TrimStart('@').ToLower();
				string amountStr = parts[2].Trim();
				if (IsAccountLocked(targetUser)) return false;
				
				if (!int.TryParse(amountStr, out int newBalance) || newBalance < 0) {
					CPH.SendMessage("Bitte gib einen gültigen positiven Betrag an.", true);
					return false;
				}
				int accountValue = CPH.GetTwitchUserVar<int>(targetUser, "moneyBank", true);
				if (accountValue >= 1) {
					CPH.SetTwitchUserVar(targetUser, "moneyBank", newBalance, true);
					CPH.SendMessage($"Das Bankkonto von {targetUser} wurde auf {newBalance} ŁNX gesetzt.", true);
				} else {
					CPH.SendMessage($"Der Nutzer {targetUser} hat kein Konto oder ist nicht registriert.", true);
				}
				return false;
			}
			else if (command.Equals("!addmoney", StringComparison.OrdinalIgnoreCase)) {
				string[] parts = rawInput.Split(new char[] { ' ' }, 3);
				
				if (parts.Length < 3 || string.IsNullOrWhiteSpace(parts[1]) || string.IsNullOrWhiteSpace(parts[2])) {
					CPH.SendMessage("Bitte gib einen Namen und einen Betrag nach dem Befehl ein, z. B. !addmoney Username 100", true);
					return false;
				}
				
				string targetUser = parts[1].Trim().TrimStart('@').ToLower();
				string amountStr = parts[2].Trim();
				if (IsAccountLocked(targetUser)) return false;
				
				if (!int.TryParse(amountStr, out int amount) || amount <= 0) {
					CPH.SendMessage("Bitte gib einen gültigen positiven Betrag an.", true);
					return false;
				}
				
				int currentBalance = CPH.GetTwitchUserVar<int>(targetUser, "moneyBank", true);
				if (currentBalance >= 1) {
					int newBalance = currentBalance + amount;
					CPH.SetTwitchUserVar(targetUser, "moneyBank", newBalance, true);
					CPH.SendMessage($"{amount} Coins wurden dem Konto von {targetUser} hinzugefügt. Neuer Kontostand: {newBalance} ŁNX.", true);
				} else {
					CPH.SendMessage($"Der Nutzer {targetUser} hat kein Konto oder ist nicht registriert.", true);
				}
				
				return false;
			}
			else if (command.Equals("!removemoney", StringComparison.OrdinalIgnoreCase)) {
				string[] parts = rawInput.Split(new char[] { ' ' }, 3);
				
				if (parts.Length < 3 || string.IsNullOrWhiteSpace(parts[1]) || string.IsNullOrWhiteSpace(parts[2])) {
					CPH.SendMessage("Bitte gib einen Namen und einen Betrag nach dem Befehl ein, z. B. !removemoney Username 100", true);
					return false;
				}
				
				string targetUser = parts[1].Trim().TrimStart('@').ToLower();
				string amountStr = parts[2].Trim();
				if (IsAccountLocked(targetUser)) return false;
					
				
				if (!int.TryParse(amountStr, out int amount) || amount <= 0) {
					CPH.SendMessage("Bitte gib einen gültigen positiven Betrag an.", true);
					return false;
				}
				
				int currentBalance = CPH.GetTwitchUserVar<int>(targetUser, "moneyBank", true);
				if (currentBalance >= 1) {
					if (currentBalance < amount) {
						CPH.SendMessage($"Das Konto von {targetUser} hat nicht genug ŁNX, um {amount} zu entfernen. Aktueller Kontostand: {currentBalance} ŁNX.", true);
					} else {
						int newBalance = currentBalance - amount;
						CPH.SetTwitchUserVar(targetUser, "moneyBank", newBalance, true);
						CPH.SendMessage($"{amount} Coins wurden vom Konto von {targetUser} abgezogen. Neuer Kontostand: {newBalance} Coins.", true);
					}
				} else {
					CPH.SendMessage($"Der Nutzer {targetUser} hat kein Konto oder ist nicht registriert.", true);
				}
				
				return false;
			}
			else if (command.Equals("!lockaccount", StringComparison.OrdinalIgnoreCase)) {
				string[] parts = rawInput.Split(new char[] { ' ' }, 2);
				if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1])) {
					CPH.SendMessage("Bitte gib einen Namen nach dem Befehl ein, z. B. !lockaccount Username", true);
					return false;
				}
				string targetUser = parts[1].Trim().TrimStart('@').ToLower();
				string accountStatus = CPH.GetTwitchUserVar<string>(targetUser, "accountStatus", true);
				
				if (accountStatus == "active") {
					CPH.SetTwitchUserVar(targetUser, "accountStatus", "locked", true);
					CPH.SendMessage($"Das Bankkonto von {targetUser} wurde gesperrt.");
				} else if (accountStatus == "locked"){
					CPH.SendMessage($"Das Bankkonto von {targetUser} ist bereits gesperrt.");
				} else {
					CPH.SendMessage($"Der Nutzer {targetUser} hat kein Konto oder ist nicht registriert.", true);
				}
				
				return false;
			}
			else if (command.Equals("!unlockaccount", StringComparison.OrdinalIgnoreCase)) {
				string[] parts = rawInput.Split(new char[] { ' ' }, 2);
				if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1])) {
					CPH.SendMessage("Bitte gib einen Namen nach dem Befehl ein, z. B. !unlockaccount Username", true);
					return false;
				}
				string targetUser = parts[1].Trim().TrimStart('@').ToLower();
				string accountStatus = CPH.GetTwitchUserVar<string>(targetUser, "accountStatus", true);
				
				if (accountStatus == "locked") {
					CPH.SetTwitchUserVar(targetUser, "accountStatus", "active", true);
					CPH.SendMessage($"Das Bankkonto von {targetUser} wurde entsperrt.");
				} else if (accountStatus == "active"){
					CPH.SendMessage($"Das Bankkonto von {targetUser} ist bereits entsperrt.");
				} else {
					CPH.SendMessage($"Der Nutzer {targetUser} hat kein Konto oder ist nicht registriert.", true);
				}
				return false;
			}
			else if (command.Equals("!cleartransactions", StringComparison.OrdinalIgnoreCase)) {
				string[] parts = rawInput.Split(new char[] { ' ' }, 2);
				if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1])) {
					CPH.SendMessage("Bitte gib einen Namen nach dem Befehl ein, z. B. !cleartransactions Username 100", true);
					return false;
				}
				string targetUser = parts[1].Trim().TrimStart('@').ToLower();
				if (IsAccountLocked(targetUser)) return false;
				string userTransactions = CPH.GetTwitchUserVar<string>(targetUser, "transactions", true);
				
				if (!string.IsNullOrWhiteSpace(userTransactions)) {
					CPH.UnsetTwitchUserVar(targetUser, "transactions");
					CPH.SendMessage($"Die Transaktionen von {targetUser} wurden erfolgreich gelöscht", true);
				} else {
					CPH.SendMessage($"Für den Nutzer {targetUser} sind keine Transaktionen vorhanden.");
				}
				
				return false;
			}
			else if (command.Equals("!clearrichlist", StringComparison.OrdinalIgnoreCase)) {
				string richListTop1 = CPH.GetGlobalVar<string>("richlistTop1", true);
				string richListTop2 = CPH.GetGlobalVar<string>("richlistTop2", true);
				string richListTop3 = CPH.GetGlobalVar<string>("richlistTop3", true);
				string richListTop4 = CPH.GetGlobalVar<string>("richlistTop4", true);
				string richListTop5 = CPH.GetGlobalVar<string>("richlistTop5", true);
				
				if (string.IsNullOrWhiteSpace(richListTop1)) {
					CPH.SendMessage("Aktuell ist keine Richlist verfügbar, die man löschen kann.");
				} else {
					CPH.SetGlobalVar("richListTop1", "", true);
					CPH.SetGlobalVar("richListTop2", "", true);
					CPH.SetGlobalVar("richListTop3", "", true);
					CPH.SetGlobalVar("richListTop4", "", true);
					CPH.SetGlobalVar("richListTop5", "", true);
					CPH.SendMessage("Die Richliste wurde erfolgreich gelöscht.");
				}
				return false;
			}
			else if (command.Equals("!getallaccounts", StringComparison.OrdinalIgnoreCase)) {
				List<UserVariableValue<long>> userVarList = CPH.GetTwitchUsersVar<long>("moneyBank", true);
				List<UserBalance> userBalances = new List<UserBalance>();

				foreach (UserVariableValue<long> userVar in userVarList) {
					string userLogin = userVar.UserLogin;
					int bank = (int)userVar.Value;

					userBalances.Add(new UserBalance {
						UserName = userLogin,
						TotalMoney = bank
					});
				}

				userBalances.Sort((a, b) => a.UserName.CompareTo(b.UserName));

				StringBuilder messageBuilder = new StringBuilder();
				List<string> messages = new List<string>();
				messageBuilder.Append("💰 Aktive Accounts mit Guthaben:\n");

				int counter = 1;
				foreach (UserBalance ub in userBalances) {
					string line = $"{counter}. {ub.UserName} ({ub.TotalMoney} ŁNX)  ";
					if (messageBuilder.Length + line.Length > 450) {
						messages.Add(messageBuilder.ToString().Trim());
						messageBuilder.Clear();
					}
					messageBuilder.Append(line);
					counter++;
				}
				if (messageBuilder.Length > 0)
					messages.Add(messageBuilder.ToString().Trim());

				foreach (string msg in messages) {
					CPH.SendMessage(msg, true);
				}

				return false;
			}
			
		}
		if (isFollowerPlus) {
			if (command.Equals("!accountstatus", StringComparison.OrdinalIgnoreCase)) {
				if (IsMyAccountLocked(userName)) return false;
				return false;
			}
			else if (command.Equals("!guthaben", StringComparison.OrdinalIgnoreCase) || 
					 command.Equals("!balance", StringComparison.OrdinalIgnoreCase)) {
				if (IsMyAccountCheckLocked(userName)) return false;
				int bankBalance = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
				if (bankBalance > 0) {
					CPH.TwitchReplyToMessage($"Dein Kontostand beträgt aktuell: {bankBalance} ŁNX", messageId, true);
				} else {
					CPH.TwitchReplyToMessage("Also entweder bist du ziemlich broke oder du hast einfach kein Konto.", messageId, true);
				}
				return false;
			}
			else if (command.Equals("!money", StringComparison.OrdinalIgnoreCase) ||
					 command.Equals("!bargeld", StringComparison.OrdinalIgnoreCase)) {
				if (IsMyAccountCheckLocked(userName)) return false;
				int handBalance = CPH.GetTwitchUserVar<int>(userName, "moneyHand", true);
				if (handBalance > 0) {
					CPH.TwitchReplyToMessage($"Du hast aktuell {handBalance} ŁNX an Bargeld dabei.", messageId, true);
				} else {
					CPH.TwitchReplyToMessage("Du scheinst aktuell kein Bargeld dabei zu haben. Schau mal mit '!guthaben' auf dein Konto und hebe es mit '!abheben' ab.", messageId, true);
				}
				return false;
			}
			else if (command.Equals("!transactions", StringComparison.OrdinalIgnoreCase)) {
				if (IsMyAccountCheckLocked(userName)) return false;
				string transactions = CPH.GetTwitchUserVar<string>(userName, "transactions", true);
				if (!string.IsNullOrWhiteSpace(transactions)) {
					CPH.TwitchReplyToMessage($"Letzte Transaktionen auf deinem Konto: {transactions}", messageId, true);
				} else {
					CPH.TwitchReplyToMessage("Du hast noch keine Transaktionen auf deinem Konto.", messageId, true);
				}
				return false;
			}
			else if (command.Equals("!überweisung", StringComparison.OrdinalIgnoreCase)) {
				if (IsMyAccountCheckLocked(userName)) return false;
				string[] parts = rawInput.Split(new char[] { ' ' }, 3);
				if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1])) {
					CPH.TwitchReplyToMessage("Bitte gib einen Namen und einen Betrag an, z. B. !überweisung Username 150", messageId, true);
					return false;
				}
				string targetUser = parts[1].Trim().TrimStart('@').ToLower();
				if (targetUser.Equals(userName, StringComparison.OrdinalIgnoreCase)) {
					CPH.TwitchReplyToMessage("Du kannst dir selber kein Geld überweisen.", messageId, true);
					return false;
				}

				string targetUserBank = CPH.GetTwitchUserVar<string>(targetUser, "accountStatus", true);
				if (targetUserBank != "active") {
					CPH.TwitchReplyToMessage($"Der Account von '{targetUser}' ist aktuell gesperrt oder nicht verfügbar. Die Transaktion wird abgebrochen.", messageId, true);
					return false;
				}

				if (!int.TryParse(parts[2], out int amount) || amount <= 0) {
					CPH.TwitchReplyToMessage("Bitte gib einen gültigen Betrag an, z. B. !überweisung Username 150", messageId, true);
					return false;
				}

				int userBank = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
				if (userBank < amount) {
					CPH.TwitchReplyToMessage($"Du hast nicht genug Guthaben auf der Bank. Aktuell: {userBank} ŁNX.", messageId, true);
					return false;
				}

				int targetBank = CPH.GetTwitchUserVar<int>(targetUser, "moneyBank", true);
				CPH.SetTwitchUserVar(userName, "moneyBank", userBank - amount, true);
				CPH.SetTwitchUserVar(targetUser, "moneyBank", targetBank + amount, true);
				
				// Transaktionstext vorbereiten und Transaktionen holen
				string transactionTextUser = $" Überweisung an {targetUser}: -{amount} ŁNX ";
				string transactionTextTarget = $" Überweisung von {userName}: +{amount} ŁNX ";
				string userTransactions = CPH.GetTwitchUserVar<string>(userName, "transactions", true);
				string targetTransactions = CPH.GetTwitchUserVar<string>(targetUser, "transactions", true);
				
				string[] userTransactionArray = string.IsNullOrWhiteSpace(userTransactions)
					? new string[0]
					: userTransactions.Split('|');
				List<string> userTransactionList = new List<string>(userTransactionArray);
				userTransactionList.Add(transactionTextUser);
				if (userTransactionList.Count > 3) userTransactionList.RemoveAt(0); // Älteste entfernen

				// Gleiches für den Empfänger
				string[] targetTransactionArray = string.IsNullOrWhiteSpace(targetTransactions)
					? new string[0]
					: targetTransactions.Split('|');
				List<string> targetTransactionList = new List<string>(targetTransactionArray);
				targetTransactionList.Add(transactionTextTarget);
				if (targetTransactionList.Count > 3) targetTransactionList.RemoveAt(0);

				// Zurück in die UserVars speichern
				CPH.SetTwitchUserVar(userName, "transactions", string.Join(" | ", userTransactionList), true);
				CPH.SetTwitchUserVar(targetUser, "transactions", string.Join(" | ", targetTransactionList), true);
				int newBalance = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
				CPH.TwitchReplyToMessage($"Überweisung erfolgreich: {amount} ŁNX wurden an {targetUser} überwiesen. Dein neuer Kontostand beträgt: {newBalance} ŁNX", messageId, true);
				return false;
			}
			else if (command.Equals("!einzahlen", StringComparison.OrdinalIgnoreCase)) {
				if (IsMyAccountCheckLocked(userName)) return false;
				string[] parts = rawInput.Split(new char[] { ' ' }, 2);
				if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1])) {
					CPH.TwitchReplyToMessage("Bitte gib einen Namen und einen Betrag an, z. B. !überweisung Username 150", messageId, true);
					return false;
				}
				if (!int.TryParse(parts[1], out int amount) || amount <= 0) {
					CPH.TwitchReplyToMessage("Bitte gib einen gültigen Betrag an, z. B. !einzahlen 150", messageId, true);
					return false;
				}
				int handBalance = CPH.GetTwitchUserVar<int>(userName, "moneyHand", true);
				if (handBalance < amount) {
					CPH.TwitchReplyToMessage($"Du hast nur {handBalance} ŁNX an Bargeld dabei.", messageId, true);
					return false;
				}

				int bankBalance = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
				CPH.SetTwitchUserVar(userName, "moneyHand", handBalance - amount, true);
				CPH.SetTwitchUserVar(userName, "moneyBank", bankBalance + amount, true);

				// Transaktionstext vorbereiten und Transaktionen holen
				string transactionTextUser = $" Einzahlung: {amount} ŁNX ";
				string userTransactions = CPH.GetTwitchUserVar<string>(userName, "transactions", true);
				
				string[] userTransactionArray = string.IsNullOrWhiteSpace(userTransactions)
					? new string[0]
					: userTransactions.Split('|');
				List<string> userTransactionList = new List<string>(userTransactionArray);
				userTransactionList.Add(transactionTextUser);
				if (userTransactionList.Count > 3) userTransactionList.RemoveAt(0); // Älteste entfernen

				// Zurück in die UserVars speichern
				CPH.SetTwitchUserVar(userName, "transactions", string.Join(" | ", userTransactionList), true);
				int newBalanceBank = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
				int newBalanceHand = CPH.GetTwitchUserVar<int>(userName, "moneyHand", true);
				CPH.TwitchReplyToMessage($"Einzahlung erfolgreich: Dein neuer Kontostand beträgt {newBalanceBank} ŁNX und du hast {newBalanceHand} ŁNX Bargeld dabei.", messageId, true);
				return false;
			}
			else if (command.Equals("!auszahlen", StringComparison.OrdinalIgnoreCase)) {
				if (IsMyAccountCheckLocked(userName)) return false;
				string[] parts = rawInput.Split(new char[] { ' ' }, 2);
				if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1])) {
					CPH.TwitchReplyToMessage("Bitte gib einen Namen und einen Betrag an, z. B. !auszahlen 150", messageId, true);
					return false;
				}
				if (!int.TryParse(parts[1], out int amount) || amount <= 0) {
					CPH.TwitchReplyToMessage("Bitte gib einen gültigen Betrag an, z. B. !auszahlen 150", messageId, true);
					return false;
				}
				int bankBalance = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
				if (bankBalance < amount) {
					CPH.TwitchReplyToMessage($"Du hast nur {bankBalance} ŁNX auf der Bank.", messageId, true);
					return false;
				}

				int handBalance = CPH.GetTwitchUserVar<int>(userName, "moneyHand", true);
				CPH.SetTwitchUserVar(userName, "moneyBank", bankBalance - amount, true);
				CPH.SetTwitchUserVar(userName, "moneyHand", handBalance + amount, true);

				// Transaktionstext vorbereiten und Transaktionen holen
				string transactionTextUser = $" Abheben: {amount} ŁNX ";
				string userTransactions = CPH.GetTwitchUserVar<string>(userName, "transactions", true);
				
				string[] userTransactionArray = string.IsNullOrWhiteSpace(userTransactions)
					? new string[0]
					: userTransactions.Split('|');
				List<string> userTransactionList = new List<string>(userTransactionArray);
				userTransactionList.Add(transactionTextUser);
				if (userTransactionList.Count > 3) userTransactionList.RemoveAt(0); // Älteste entfernen

				// Zurück in die UserVars speichern
				CPH.SetTwitchUserVar(userName, "transactions", string.Join(" | ", userTransactionList), true);
				int newBalanceBank = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
				int newBalanceHand = CPH.GetTwitchUserVar<int>(userName, "moneyHand", true);
				CPH.TwitchReplyToMessage($"Auszahlung erfolgreich: Dein neuer Kontostand beträgt {newBalanceBank} ŁNX und du hast {newBalanceHand} ŁNX Bargeld dabei.", messageId, true);
				return false;
			}
			else if (command.Equals("!reward", StringComparison.OrdinalIgnoreCase)) {
				if (IsMyAccountCheckLocked(userName)) return false;

				long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				long lastDaily = CPH.GetTwitchUserVar<long>(userName, "lastDaily", true);

				long cooldown = 3600; // in Sekunden
				long elapsed = now - lastDaily;
				long remaining = cooldown - elapsed;

				TimeSpan remainingTimeSpan = TimeSpan.FromSeconds(remaining);
				string remainingTimeFormatted = $"{remainingTimeSpan.Minutes} Minuten und {remainingTimeSpan.Seconds} Sekunden";

				if (elapsed < cooldown) {
					CPH.TwitchReplyToMessage($"Du hast deine Belohnung bereits erhalten. Versuche es in {remainingTimeFormatted} erneut.", messageId, true);
					return false;
				}

				int dailyAmount = 100;
				int bankBalance = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
				CPH.SetTwitchUserVar(userName, "moneyBank", bankBalance + dailyAmount, true);

				// Transaktionen aktualisieren
				string transactionTextUser = $" Stream Reward: +{dailyAmount} ŁNX ";
				string userTransactions = CPH.GetTwitchUserVar<string>(userName, "transactions", true);
				string[] userTransactionArray = string.IsNullOrWhiteSpace(userTransactions) ? new string[0] : userTransactions.Split('|');
				List<string> userTransactionList = new List<string>(userTransactionArray);
				userTransactionList.Add(transactionTextUser);
				if (userTransactionList.Count > 3) userTransactionList.RemoveAt(0);
				CPH.SetTwitchUserVar(userName, "transactions", string.Join(" | ", userTransactionList), true);

				// Zeit speichern
				CPH.SetTwitchUserVar(userName, "lastDaily", now, true);

				int newBalance = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
				CPH.TwitchReplyToMessage($"Belohnung ausgezahlt: Dein Kontostand beträgt jetzt {newBalance} ŁNX. Du kannst die nächste Belohnung in 1 Stunde holen.", messageId, true);
				return false;
			}
			else if (command.Equals("!work", StringComparison.OrdinalIgnoreCase)) {
				if (IsMyAccountCheckLocked(userName)) return false;
				
				long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				long lastWork = CPH.GetTwitchUserVar<long>(userName, "lastWork", true);
				long workInProgress = CPH.GetTwitchUserVar<long>(userName, "workInProgress", true);

				// Prüfen ob schon gearbeitet wird
				if (workInProgress > now) {
					long waitSeconds = workInProgress - now;
					CPH.TwitchReplyToMessage($"Du arbeitest gerade noch, bitte warte {waitSeconds / 60} Minute(n).", messageId, true);
					return false;
				}

				// Prüfen ob cooldown von 30min vorbei ist
				if (now - lastWork < 1800) {
					long waitSeconds = 1800 - (now - lastWork);
					CPH.TwitchReplyToMessage($"Du kannst erst in {waitSeconds / 60} Minuten wieder arbeiten.", messageId, true);
					return false;
				}

				// Work starten: Arbeite 5 Minuten
				long workEndTime = now + 300; // 5 Minuten in Sekunden
				CPH.SetTwitchUserVar(userName, "workInProgress", workEndTime, true);
				int endTime = 300000;
				CPH.SetGlobalVar("userEndTime", endTime, false);
				
				CPH.TwitchReplyToMessage("Du hast mit der Arbeit begonnen! In 5 Minuten bekommst du deine Belohnung.", messageId, true);
				CPH.RunAction("-- Add Money for Work --", true);
				
				return false;
			}
			else if (command.Equals("!balancetop", StringComparison.OrdinalIgnoreCase)) {
				string richListTop1 = CPH.GetGlobalVar<string>("richlistTop1", true);
				string richListTop2 = CPH.GetGlobalVar<string>("richlistTop2", true);
				string richListTop3 = CPH.GetGlobalVar<string>("richlistTop3", true);
				string richListTop4 = CPH.GetGlobalVar<string>("richlistTop4", true);
				string richListTop5 = CPH.GetGlobalVar<string>("richlistTop5", true);
				
				if (string.IsNullOrWhiteSpace(richListTop1)) {
					CPH.SendMessage("Aktuell ist keine Topliste verfügbar. Die Liste wird alle 5 Minuten aktualisiert. Versuche es bitte später erneut.");
				} else if (string.IsNullOrWhiteSpace(richListTop2)) {
					CPH.SendMessage($"Aktuell in der Top 5 Reichenliste (Die Liste wird alle 5 Minuten aktualisiert): Top 1 {richListTop1}");
				} else if (string.IsNullOrWhiteSpace(richListTop3)) {
					CPH.SendMessage($"Aktuell in der Top 5 Reichenliste (Die Liste wird alle 5 Minuten aktualisiert): Top 1 {richListTop1}, Top 2 {richListTop2}");
				} else if (string.IsNullOrWhiteSpace(richListTop4)) {
					CPH.SendMessage($"Aktuell in der Top 5 Reichenliste (Die Liste wird alle 5 Minuten aktualisiert): Top 1 {richListTop1}, Top 2 {richListTop2}, Top 3 {richListTop3}");
				} else if (string.IsNullOrWhiteSpace(richListTop5)) {
					CPH.SendMessage($"Aktuell in der Top 5 Reichenliste (Die Liste wird alle 5 Minuten aktualisiert): Top 1 {richListTop1}, Top 2 {richListTop2}, Top 3 {richListTop3}, Top 4 {richListTop4}");
				} else {
					CPH.SendMessage($"Aktuell in der Top 5 Reichenliste (Die Liste wird alle 5 Minuten aktualisiert): Top 1 {richListTop1}, Top 2 {richListTop2}, Top 3 {richListTop3}, Top 4 {richListTop4}, Top 5 {richListTop5}");
				}
				return false;
			}
			else if (command.Equals("!shop", StringComparison.OrdinalIgnoreCase)) {
				if (IsMyAccountCheckLocked(userName)) return false;
				CPH.RunAction("ECONOMY -> Shop Handler", true);
				return false;
			}
			else if (command.Equals("!inventar", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("ECONOMY -> Inventory", true);
				return false;
			}
			else if (command.Equals("!opengame", StringComparison.OrdinalIgnoreCase) ||
					 command.Equals("!startgame", StringComparison.OrdinalIgnoreCase) ||
					 command.Equals("!join", StringComparison.OrdinalIgnoreCase) ||
					 command.Equals("!games", StringComparison.OrdinalIgnoreCase) ||
					 command.Equals("!gameinfo", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("ECONOMY -> Chatgames", true);
				return false;
			}
		}
		return true;
	}
	public class UserBalance {
		public string UserName;
		public int TotalMoney;
	}
}


// stream cmds
using System;
using System.Text;
using System.Collections.Generic;

public class CPHInline {
	public bool Execute() {
		string rawInput = args["rawInput"].ToString();
		string command = rawInput.Split(' ')[0].ToLowerInvariant();
		string userName = args["user"].ToString().ToLower();
		
		bool isFollower = (bool)args["isFollowing"];
        bool isFollowerPlus = (bool)args["isFollowing"] || (bool)args["isSubscribed"] || (bool)args["isVip"] || (bool)args["isModerator"];
        bool isSubPlus = (bool)args["isSubscribed"] || (bool)args["isVip"] || (bool)args["isModerator"];
        bool isVipPlus = (bool)args["isVip"] || (bool)args["isModerator"];
        bool isMod = (bool)args["isModerator"];
        bool isEveryone = true;
		
		if (isMod) {
			if (command.Equals("!setgame", StringComparison.OrdinalIgnoreCase)) {
				string oldGame = CPH.GetGlobalVar<string>("ws_gameName", false);
				string commandInput = args["rawInput"].ToString();
				string newGame = commandInput.Substring(commandInput.IndexOf(' ') + 1).Trim();
				if (!commandInput.Contains(" ")) {
					CPH.SendMessage("Bitte gib einen Spielnamen nach dem Befehl ein, z. B. !setgame Minecraft", true);
					return false;
				}
				CPH.SetGlobalVar("newGame", newGame, false);
				CPH.RunAction("-- set Game --", true);
				
				bool gameSuccess = CPH.GetGlobalVar<bool>("gameSuccess", false);
				if (gameSuccess) {
					string updatedGame = CPH.GetGlobalVar<string>("ws_gameName", false);
					string updatedImage = CPH.GetGlobalVar<string>("ws_gameImage", false);
					CPH.SetGlobalVar("ws_gameName", updatedGame, true);
					CPH.SetGlobalVar("ws_gameImage", updatedImage, true);
					CPH.SendMessage($"Die Kategorie wurde erfolgreich zu \"{updatedGame}\" geupdated", true);
					CPH.ExecuteMethod("Websocket", "Send");
					CPH.ClearNonPersistedGlobals();
					return false;
				} else {
					CPH.SendMessage($"Das Game \"{newGame}\" ist nicht verfügbar. Bitte überprüfe deine Schreibweise.", true);
					CPH.ClearNonPersistedGlobals();
					return false;
				}
				return false;
			}
			else if (command.Equals("!settitle", StringComparison.OrdinalIgnoreCase)) {
				string commandInput = args["rawInput"].ToString();
				string newTitle = commandInput.Substring(commandInput.IndexOf(' ') + 1).Trim();
				
				if (!commandInput.Contains(" ")) {
					CPH.SendMessage("Bitte gib einen neuen Titel nach dem Befehl ein, z. B. !settitle Dieser neue Titel ist total toll.", true);
					return false;
				}
				if (!string.IsNullOrWhiteSpace(newTitle)) {
					CPH.SetChannelTitle(newTitle);
					CPH.SendMessage($"Der Titel wurde auf \"{newTitle}\" gesetzt.", true);
				} else {
					CPH.SendMessage("Der eingegebene Titel ist ungültig. Bitte gib einen sinnvollen Titel ein.", true);
				}
				return false;
			}
			else if (command.Equals("!clear", StringComparison.OrdinalIgnoreCase) || 
					 command.Equals("!cc", StringComparison.OrdinalIgnoreCase)) {
				CPH.TwitchClearChatMessages(true);
				return false;
			}
		}
		if (isFollowerPlus) {
			if (command.Equals("!clip", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("-- set TimeStamp --", true);
				CPH.RunAction("-- create Clip --", true);
				bool clipSuccess = CPH.GetGlobalVar<bool>("createClipSuccess", false);
				if (clipSuccess) {
					bool clipCreated = CPH.GetGlobalVar<bool>("ws_clipCreated", false);
					bool clipUrl = CPH.GetGlobalVar<bool>("ws_clipUrl", false);
					CPH.SendMessage($"Ein neuer 30s Clip wurde erstellt: {clipCreated} - {clipUrl}");
					CPH.SetGlobalVar("eventType", "clip", false);
					CPH.SetGlobalVar("actionType", "createView", false);
					CPH.ExecuteMethod("Websocket", "Send");
					CPH.ClearNonPersistedGlobals();
				} else {
					CPH.SendMessage("Leider konnte der Clip nicht erstellt werden. Bitte versuche es später erneut.");
					CPH.ClearNonPersistedGlobals();
				}
				return false;
			}
		}
		return true;
	}
}


// spotify commands
using System;
using System.Text;
using System.Collections.Generic;

public class CPHInline {
	public bool Execute() {
		string rawInput = args["rawInput"].ToString();
		string command = rawInput.Split(' ')[0].ToLowerInvariant();
		string userName = args["user"].ToString().ToLower();
        
        bool isFollower = (bool)args["isFollowing"];
        bool isFollowerPlus = (bool)args["isFollowing"] || (bool)args["isSubscribed"] || (bool)args["isVip"] || (bool)args["isModerator"];
        bool isSubPlus = (bool)args["isSubscribed"] || (bool)args["isVip"] || (bool)args["isModerator"];
        bool isVipPlus = (bool)args["isVip"] || (bool)args["isModerator"];
        bool isMod = (bool)args["isModerator"];
        bool isEveryone = true;
        
		if (isMod) {
			if (command.Equals("!addsong", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## ADD SONG ##", true);
				return false;
			}
			else if (command.Equals("!blocksong", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## BLOCK SONG ##", true);
				return false;
			}
			else if (command.Equals("!unblocksong", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## UNBLOCK SONG ##", true);
				return false;
			}
			else if (command.Equals("!pause", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## PAUSE MUSIC ##", true);
				return false;
			}
			else if (command.Equals("!play", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## PLAY MUSIC ##", true);
				return false;
			}
			else if (command.Equals("!restartsong", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## REPLAY SONG ##", true);
				return false;
			}
			else if (command.Equals("!prevsong", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## PREV SONG ##", true);
				return false;
			}
			else if (command.Equals("!skipsong", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## SONG SKIP ##", true);
				return false;
			}
			else if (command.Equals("!excluded", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## EXCLUDED SONGS ##", true);
				return false;
			}
		}
		if (isSubPlus) {
			if (command.Equals("!sr", StringComparison.OrdinalIgnoreCase) ||
				command.Equals("!srequest", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## SONG REQUEST ##", true);
				return false;
			}
			else if (command.Equals("!songqueue", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## SHOW QUEUE ##", true);
				return false;
			}
		}
		if (isEveryone) {
			if (command.Equals("!song", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## SONG NOW ##", true);
				return false;
			}
			else if (command.Equals("!songlink", StringComparison.OrdinalIgnoreCase)) {
				CPH.RunAction("## SONG LINK ##", true);
				return false;
			}
		}
		return true;
	}
}