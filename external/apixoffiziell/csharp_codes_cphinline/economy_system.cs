// Shop handler
using System;
using System.Collections.Generic;

public class CPHInline {
    private static Dictionary<string, ShopItem> shopItems = new Dictionary<string, ShopItem>() {
        {"shoutout", new ShopItem("shoutout", "Shoutout Token", 500)},
        {"doubleDaily", new ShopItem("doubleDaily", "Doppelter Tagesbonus (7 Tage)", 2500)},
        {"timeout", new ShopItem("timeout", "Beliebigen Nutzer timeouten", 50000)},
    };
    public bool Execute() {
        string commandFull = args["rawInput"].ToString().ToLower();
        string userName = args["user"].ToString().ToLower();

        if (!commandFull.StartsWith("!shop")) return false;

        // Suffix extrahieren
        string[] parts = commandFull.Split(' ');
        if (parts.Length < 2) {
            CPH.SendMessage($"{userName}, bitte verwende !shop items oder !shop buy <item>.");
            return false;
        }

        string action = parts[1];
        if (action == "items") {
            ShowItems(userName);
        }
        else if (action == "buy") {
            if (parts.Length < 3)
            {
                CPH.SendMessage($"{userName}, bitte gib ein Item an, z.B. !shop buy highlight.");
                return false;
            }
            string itemKey = parts[2];
            BuyItem(userName, itemKey);
        } else {
            CPH.SendMessage($"{userName}, unbekannter Shop-Befehl. Nutze !shop items oder !shop buy <item>.");
        }

        return false;
    }

    private void ShowItems(string userName) {
        string itemList = "verfügbare Shop-Items: ";
        foreach (var item in shopItems.Values) {
            itemList += $"'{item.Key}' - {item.DisplayName} ({item.Price} ŁNX), ";
        }
        itemList = itemList.TrimEnd(' ', ',');
        CPH.SendMessage($"{userName}, {itemList}");
    }

    private void BuyItem(string userName, string itemKey) {
        if (!shopItems.ContainsKey(itemKey)) {
            CPH.SendMessage($"{userName}, dieses Item gibt es nicht.");
            return;
        }

        ShopItem item = shopItems[itemKey];
        int userBalance = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
        if (userBalance < item.Price) {
            CPH.SendMessage($"{userName}, du hast nicht genug ŁNX für {item.DisplayName}. Du brauchst {item.Price - userBalance} mehr.");
            return;
        }

        // Inventory auslesen
        string inventoryStr = CPH.GetTwitchUserVar<string>(userName, "inventory", true);
        List<string> inventory = new List<string>();
        if (!string.IsNullOrWhiteSpace(inventoryStr)) {
            inventory.AddRange(inventoryStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }

        // Maximal 5 Items erlaubt
        if (inventory.Count >= 5) {
            CPH.SendMessage($"{userName}, dein Inventar ist voll (max. 5 Items). Du kannst keine weiteren Items kaufen.");
            return;
        }

        // Geld abziehen
        int newBalance = userBalance - item.Price;
        CPH.SetTwitchUserVar(userName, "moneyBank", newBalance, true);

        // Item hinzufügen und speichern
        inventory.Add(item.Key);
        string newInventoryStr = string.Join(",", inventory);
        CPH.SetTwitchUserVar(userName, "inventory", newInventoryStr, true);

        CPH.SendMessage($"{userName}, du hast {item.DisplayName} für {item.Price} ŁNX gekauft! Neuer Kontostand: {newBalance} ŁNX.");
    }

    private class ShopItem {
        public string Key { get; }
        public string DisplayName { get; }
        public int Price { get; }

        public ShopItem(string key, string displayName, int price) {
            Key = key;
            DisplayName = displayName;
            Price = price;
        }
    }
}


// Inventory handler
using System;
using System.Collections.Generic;

public class CPHInline {
    private static Dictionary<string, string> shopItemsDisplayNames = new Dictionary<string, string>() {
        {"shoutout", "Shoutout Token"},
        {"doubleDaily", "Doppelter Tagesbonus (7 Tage)"},
        {"timeout", "Timeout-Token"}
    };

    private static List<string> itemsWithParameter = new List<string>() {
        "timeout"
    };

    public bool Execute() {
        string userName = args["user"].ToString().ToLower();
        string commandFull = args["rawInput"].ToString();
        string[] parts = commandFull.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0 || (!parts[0].StartsWith("!inventar") && !parts[0].StartsWith("!inventory") && !parts[0].StartsWith("!inv"))) { return false; }

        // Inventar auslesen
        string inventoryStr = CPH.GetTwitchUserVar<string>(userName, "inventory", true);
        List<string> inventoryList = new List<string>();
        if (!string.IsNullOrWhiteSpace(inventoryStr)) {
            inventoryList = new List<string>(inventoryStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }

        if (parts.Length == 1) {
            if (inventoryList.Count == 0) {
                CPH.SendMessage($"{userName}, dein Inventar ist leer.");
                return false;
            }

            List<string> itemNames = new List<string>();
            foreach (string itemKey in inventoryList) {
                if (shopItemsDisplayNames.TryGetValue(itemKey, out string displayName)) {
                    itemNames.Add($"'{itemKey}' - {displayName}");
                } else {
                    itemNames.Add(itemKey);
                }
            }

            string itemList = string.Join(", ", itemNames);
            CPH.SendMessage($"{userName}, dein Inventar enthält: {itemList} | Nutze bpsw. '!item use shoutout' um eines davon zu nutzen.");
            return false;
        }

        if (parts.Length >= 3 && parts[1].ToLower() == "use") {
            string itemToUse = parts[2].ToLower();

            if (!inventoryList.Contains(itemToUse)) {
                CPH.SendMessage($"{userName}, du hast das Item '{itemToUse}' nicht in deinem Inventar.");
                return false;
            }

            // Überprüfen, ob das Item zusätzliche Parameter benötigt
            bool needsParam = itemsWithParameter.Contains(itemToUse);
            string param = parts.Length >= 4 ? parts[3] : null;

            if (needsParam && string.IsNullOrWhiteSpace(param)) {
                CPH.SendMessage($"{userName}, das Item '{itemToUse}' benötigt einen zusätzlichen Parameter! Beispiel: !inventar use timeout Nutzername");
                return false;
            }

            // --- ITEM HANDLING LOGIK ---
            switch (itemToUse) {
                case "timeout":
                    CPH.SendMessage($"{userName} hat das Item 'timeout' benutzt und {param} für 60 Sekunden in die Ecke gestellt! 😈");
                    CPH.TwitchTimeoutUser(param, 60, $"{userName} hat das Timeout-Item gegen dich verwendet.", true);
                    break;

                case "shoutout":
                    CPH.SendMessage($"{userName}, du hast ein Shoutout-Token benutzt!");
                    break;

                case "doubleDaily":
                    CPH.SendMessage($"{userName}, dein Tagesbonus wird ab sofort für 7 Tage verdoppelt!");
                    break;

                default:
                    CPH.SendMessage($"{userName}, das Item '{itemToUse}' ist nicht bekannt.");
                    return false;
            }

            // Item aus Inventar entfernen
            inventoryList.Remove(itemToUse);
            string newInventoryStr = string.Join(",", inventoryList);
            CPH.SetTwitchUserVar(userName, "inventory", newInventoryStr, true);

            return false;
        }

        // Wenn der use-Befehl falsch verwendet wurde
        CPH.SendMessage($"{userName}, benutze !inventar zum Anzeigen oder !inventar use <item> [parameter] zum Benutzen eines Items.");
        return false;
    }
}


// Chat games
using System;
using System.Collections.Generic;

public class CPHInline
{
    private Dictionary<string, (string keyName, string displayName, string description, bool isMultiplayer, bool maintenance)> games =
        new Dictionary<string, (string, string, string, bool, bool)>()
        {
            { "luckydice", ("luckydice", "Lucky Dice", "Würfle deine Zahl und hoffe, dass sie mit der des Dealers übereinstimmt.", false, true) },
            { "slots", ("slots", "Slot Machine", "Dreh die Walzen und gewinne den Jackpot!", false, true) },
            { "tower", ("tower", "Tower of Risk", "Steige Stufe für Stufe auf – aber pass auf, dass du nicht stürzt!", false, true) },
            { "boss", ("boss", "Bosskampf", "Kämpfe mit anderen gegen einen starken Boss!", true, true) },
            { "duel", ("duel", "1v1 Duell", "Fordere jemanden heraus und gewinnt im direkten Duell!", true, true) },
            { "heist", ("heist", "Banküberfall", "Wage einen Überfall – mit Glück kommst du davon!", true, true) },
            { "quiz", ("quiz", "Schnellquiz", "Beantworte als Erster die Frage richtig!", true, true) },
            { "race", ("race", "Hühnerrennen", "Setze auf dein Huhn und gewinne das Rennen!", true, true) },
        };

    private static string currentGame = null;
    private static bool isWaitingForPlayers = false;
    private static List<string> joinedPlayers = new List<string>();
    private static DateTime gameStartTime;

    public bool Execute()
    {
        string userName = args["user"].ToString().ToLower();
        string rawInput = args["rawInput"].ToString().Trim();
        string[] parts = rawInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return false;

        string command = parts[0].ToLower();

        // === !games ===
        if (command == "!games")
		{
			var availableGames = new List<string>();

			foreach (var game in games)
			{
				if (!game.Value.maintenance)
				{
					string cmd = game.Value.isMultiplayer ? $"!opengame {game.Key}" : $"!startgame {game.Key}";
					availableGames.Add($"• {game.Value.keyName} - \"{game.Value.displayName}\" – {cmd}");
				}
			}

			if (availableGames.Count == 0)
			{
				CPH.SendMessage("⚙ Derzeit befinden sich alle Spiele im Wartungsmodus.");
			}
			else
			{
				CPH.SendMessage("🎮 Verfügbare Spiele:");
				foreach (var entry in availableGames)
				{
					CPH.SendMessage(entry);
				}
				CPH.SendMessage("ℹ Mehr Infos mit `!gameinfo <name>`.");
			}

			return true;
		}

        // === !gameinfo <name> ===
        if (command == "!gameinfo")
        {
            if (parts.Length < 2)
            {
                CPH.SendMessage("⚠ Bitte gib ein Spiel an, z. B. !gameinfo tower");
                return false;
            }

            string gameName = parts[1].ToLower();
            if (!games.ContainsKey(gameName) || games[gameName].maintenance)
            {
                CPH.SendMessage($"❌ '{gameName}' ist kein bekanntes oder momentan nicht verfügbares Spiel.");
                return false;
            }

            var g = games[gameName];
            CPH.SendMessage($"ℹ **{g.displayName}** – {g.description}");
            CPH.SendMessage(g.isMultiplayer
                ? $"👉 Starte mit `!startgame {gameName}` – andere können mit `!join` teilnehmen."
                : $"👉 Starte direkt mit `!game {gameName}`.");
            return true;
        }

        // === !game <name> → nur für Singleplayer ===
        if (command == "!startgame")
        {
            if (parts.Length < 2)
            {
                CPH.SendMessage("⚠ Bitte gib ein Spiel an, z. B. !startgame luckydice");
                return false;
            }

            string gameName = parts[1].ToLower();
            if (!games.ContainsKey(gameName) || games[gameName].maintenance)
            {
                CPH.SendMessage($"❌ '{gameName}' ist kein bekanntes oder momentan nicht verfügbares Spiel.");
                return false;
            }

            if (games[gameName].isMultiplayer)
            {
                CPH.SendMessage($"⚠ {games[gameName].displayName} ist ein Multiplayer-Spiel. Starte es mit `!startgame {gameName}`.");
                return false;
            }

            CPH.SendMessage($"🎮 {userName} startet **{games[gameName].displayName}** – {games[gameName].description}");
            CPH.SendMessage($"(Hier würde deine Einzelspiel-Logik folgen)");
            return true;
        }

        // === !startgame <name> → Multiplayer starten ===
        if (command == "!opengame")
        {
            if (parts.Length < 2)
            {
                CPH.SendMessage("⚠ Bitte gib ein Spiel an, z. B. !opengame quiz");
                return false;
            }

            string gameName = parts[1].ToLower();
            if (!games.ContainsKey(gameName) || games[gameName].maintenance)
            {
                CPH.SendMessage($"❌ '{gameName}' ist kein bekanntes oder momentan nicht verfügbares Spiel.");
                return false;
            }

            if (!games[gameName].isMultiplayer)
            {
                CPH.SendMessage($"⚠ {games[gameName].displayName} ist ein Einzelspiel. Starte es mit `!opengame {gameName}`.");
                return false;
            }

            if (isWaitingForPlayers)
            {
                CPH.SendMessage($"⛔ Es läuft bereits ein Spiel: **{games[currentGame].displayName}**. Bitte warte, bis es vorbei ist.");
                return false;
            }

            currentGame = gameName;
            isWaitingForPlayers = true;
            joinedPlayers.Clear();
            gameStartTime = DateTime.Now;

            CPH.SendMessage($"🎮 **{games[gameName].displayName}** wurde gestartet!");
            CPH.SendMessage($"{games[gameName].description}");
            CPH.SendMessage("👉 Gib `!join` ein, um teilzunehmen!");
            return true;
        }

        // === !join ===
        if (command == "!join")
        {
            if (!isWaitingForPlayers || currentGame == null)
            {
                CPH.SendMessage($"{userName}, es läuft gerade kein Spiel zum Mitmachen.");
                return false;
            }

            if (joinedPlayers.Contains(userName))
            {
                CPH.SendMessage($"{userName}, du bist schon dabei!");
                return false;
            }

            joinedPlayers.Add(userName);
            CPH.SendMessage($"✅ {userName} ist dem Spiel **{games[currentGame].displayName}** beigetreten! ({joinedPlayers.Count} Spieler)");
            return true;
        }

        return false;
    }

    private void ResetGame()
    {
        currentGame = null;
        isWaitingForPlayers = false;
        joinedPlayers.Clear();
    }
}


// Set money first time
using System;
using System.Collections.Generic;

public class CPHInline {
	public bool Execute() {
		string user = args["user"].ToString().ToLower();
		
		bool isFollower = (bool)args["isFollowing"];
		bool isSub = (bool)args["isSubscribed"];
		bool isVip = (bool)args["isVip"];
		bool isMod = (bool)args["isModerator"];
		
		int startMoney = 500;
		
		if (isFollower || isSub || isVip || isMod) {
			int currentMoney = CPH.GetTwitchUserVar<int>(user, "moneyBank", true);
			
			if (currentMoney == 0) {
				string accountStatus = CPH.GetTwitchUserVar<string>(user, "accountStatus", true);
				if (accountStatus == "locked") {
					CPH.SendMessage($"Das Bankkonto von {user} ist gesperrt.");
					return false;
				}
				CPH.SetTwitchUserVar(user, "moneyBank", startMoney, true);
				CPH.SetTwitchUserVar(user, "accountStatus", "active", true);
				CPH.SendMessage($"{user} wurde ein Konto mit {startMoney} ŁNX eingerichtet.", true);
				
				// Transaktionen aktualisieren
				string transactionTextUser = $" Stream Reward: +{startMoney} ŁNX ";
				string userTransactions = CPH.GetTwitchUserVar<string>(user, "transactions", true);
				string[] userTransactionArray = string.IsNullOrWhiteSpace(userTransactions) ? new string[0] : userTransactions.Split('|');
				List<string> userTransactionList = new List<string>(userTransactionArray);
				userTransactionList.Add(transactionTextUser);
				if (userTransactionList.Count > 3) userTransactionList.RemoveAt(0);
				CPH.SetTwitchUserVar(user, "transactions", string.Join(" | ", userTransactionList), true);
				return false;
			}
			return false;
		}
		return false;
	}
}


// money for work
using System;
using System.Collections.Generic;

public class CPHInline {
	public bool Execute() {
		string userName = args["user"].ToString().ToLower();
		CPH.TryGetArg("msgId", out string messageId);

		// Aktuelle Zeit in Sekunden
		long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

		// Auszahlung
		int workAmount = 200; // Betrag für Arbeit
		int bankBalance = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
		CPH.SetTwitchUserVar(userName, "moneyBank", bankBalance + workAmount, true);

		// Transaktionen aktualisieren
		string transactionTextUser = $"Arbeit Belohnung: +{workAmount} ŁNX";
		string userTransactions = CPH.GetTwitchUserVar<string>(userName, "transactions", true);
		string[] userTransactionArray = string.IsNullOrWhiteSpace(userTransactions) ? new string[0] : userTransactions.Split('|');
		List<string> userTransactionList = new List<string>(userTransactionArray);
		userTransactionList.Add(transactionTextUser);
		if (userTransactionList.Count > 3) userTransactionList.RemoveAt(0);
		CPH.SetTwitchUserVar(userName, "transactions", string.Join(" | ", userTransactionList), true);

		// Zeiten zurücksetzen
		CPH.UnsetTwitchUserVar(userName, "workInProgress", true);
		CPH.SetTwitchUserVar(userName, "lastWork", now, true);

		int newBalance = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
		CPH.TwitchReplyToMessage($"Arbeit abgeschlossen! Du hast {workAmount} ŁNX erhalten. Dein Kontostand beträgt jetzt {newBalance} ŁNX. Du kannst in 30 Minuten wieder Arbeiten.", messageId, true);

		CPH.ClearNonPersistedGlobals();
		return true;
	}
}


// money for watchtime
using System;
using System.Collections.Generic;

public class CPHInline {
	public bool Execute() {
		bool IsAccountLocked(string user) {
			string status = CPH.GetTwitchUserVar<string>(user, "accountStatus", true);
			return status == "locked";
		}

		string userName = args["user"].ToString().ToLower();
		if (IsAccountLocked(userName)) return false;

		// Nachrichtenzähler abrufen und um 1 erhöhen
		int viewMinCount = CPH.GetTwitchUserVar<int>(userName, "viewMinutesCount", true) + 1;
		CPH.SetTwitchUserVar(userName, "viewMinutesCount", viewMinCount, true);

		// Nur bei jeder dritten Nachricht belohnen
		if (viewMinCount % 5 == 0) {
			int rewardAmount = 5;
			int currentBalance = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
			int newBalance = currentBalance + rewardAmount;

			CPH.SetTwitchUserVar(userName, "moneyBank", newBalance, true);

			// Transaktionen aktualisieren (optional)
			string transactionText = $" Zuschauer-Belohnung: +{rewardAmount} ŁNX ";
			string userTransactions = CPH.GetTwitchUserVar<string>(userName, "transactions", true);
			string[] transactionArray = string.IsNullOrWhiteSpace(userTransactions) ? new string[0] : userTransactions.Split('|');
			List<string> transactionList = new List<string>(transactionArray);
			transactionList.Add(transactionText);
			if (transactionList.Count > 3) transactionList.RemoveAt(0);
			CPH.SetTwitchUserVar(userName, "transactions", string.Join(" | ", transactionList), true);
		}

		return false;
	}
}


// money for chat message
using System;
using System.Collections.Generic;

public class CPHInline {
	public bool Execute() {
		bool IsAccountLocked(string user) {
			string status = CPH.GetTwitchUserVar<string>(user, "accountStatus", true);
			return status == "locked";
		}

		string userName = args["user"].ToString().ToLower();
		if (IsAccountLocked(userName)) return false;

		// Nachrichtenzähler abrufen und um 1 erhöhen
		int msgCount = CPH.GetTwitchUserVar<int>(userName, "chatMessageCount", true) + 1;
		CPH.SetTwitchUserVar(userName, "chatMessageCount", msgCount, true);

		// Nur bei jeder dritten Nachricht belohnen
		if (msgCount % 3 == 0) {
			int rewardAmount = 5;
			int currentBalance = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
			int newBalance = currentBalance + rewardAmount;

			CPH.SetTwitchUserVar(userName, "moneyBank", newBalance, true);

			// Transaktionen aktualisieren (optional)
			string transactionText = $" Chat-Aktivität: +{rewardAmount} ŁNX ";
			string userTransactions = CPH.GetTwitchUserVar<string>(userName, "transactions", true);
			string[] transactionArray = string.IsNullOrWhiteSpace(userTransactions) ? new string[0] : userTransactions.Split('|');
			List<string> transactionList = new List<string>(transactionArray);
			transactionList.Add(transactionText);
			if (transactionList.Count > 3) transactionList.RemoveAt(0);
			CPH.SetTwitchUserVar(userName, "transactions", string.Join(" | ", transactionList), true);
		}

		return false;
	}
}


// richlist
using System;
using System.Collections.Generic;

public class CPHInline {
	public bool Execute() {
		List<UserBalance> userBalances = new List<UserBalance>();
		List<UserVariableValue<long>> userVarList = CPH.GetTwitchUsersVar<long>("moneyBank", true);

		foreach (UserVariableValue<long> userVar in userVarList)
		{
			string userLogin = userVar.UserLogin;

			// Bank-Wert aus bereits vorhandener Liste
			int bank = (int)userVar.Value;

			// Hand-Wert separat abfragen
			int hand = CPH.GetTwitchUserVar<int>(userLogin, "moneyHand", true);
			int total = bank + hand;

			userBalances.Add(new UserBalance {
				UserName = userLogin,
				TotalMoney = total
			});
		}

		for (int i = 0; i < userBalances.Count - 1; i++) {
			for (int j = i + 1; j < userBalances.Count; j++) {
				if (userBalances[j].TotalMoney > userBalances[i].TotalMoney) {
					UserBalance temp = userBalances[i];
					userBalances[i] = userBalances[j];
					userBalances[j] = temp;
				}
			}
		}

		for (int i = 0; i < 5; i++) {
			string varName = $"richlistTop{i + 1}";
			if (i < userBalances.Count) {
				string entry = $"{userBalances[i].UserName} ({userBalances[i].TotalMoney} ŁNX)";
				CPH.SetGlobalVar(varName, entry, true);
			} else {
				CPH.SetGlobalVar(varName, "", true); // leer setzen, falls nicht genug User
			}
		}

		CPH.LogInfo("Richlist erfolgreich aktualisiert.");
		return true;
	}

	public class UserBalance {
		public string UserName;
		public int TotalMoney;
	}
}