using System;
using System.Collections.Generic;

public class CPHInline {
	public bool Execute() {
		bool IsAccountLocked(string user) {
			string status = CPH.GetTwitchUserVar<string>(user, "accountStatus", true);
			return status == "locked";
		}
		int userCounter = Convert.ToInt32(args["counter"]);
		int rewardCost = Convert.ToInt32(args["rewardCost"]);
		string userName = args["user"].ToString().ToLower();
		string rewardName = args["rewardName"].ToString().ToLower();
		string userMessage = args["rawInput"].ToString().ToLower();
		if (rewardName == "täglicher bonus") {
			if (IsAccountLocked(userName)) return false;
			int currentBalance = CPH.GetTwitchUserVar<int>(userName, "moneyBank", true);
			CPH.SendMessage($"{userName}, du hast erfolgreich deinen {userCounter}. täglichen Bonus geholt. Checke deinen Kontostand mit !guthaben oder !bank.");
			int dailyReward = 250;
			if (currentBalance >= 1) {
				int newBalance = currentBalance + dailyReward;
				CPH.SetTwitchUserVar(userName, "moneyBank", newBalance, true);
				CPH.SendMessage($"{dailyReward} ŁNX deinem Konto hinzugefügt {userName}. Neuer Kontostand: {newBalance} ŁNX.", true);
				
				// Transaktionen aktualisieren
				string transactionTextUser = $" Stream Reward: +{dailyReward} ŁNX ";
				string userTransactions = CPH.GetTwitchUserVar<string>(userName, "transactions", true);
				string[] userTransactionArray = string.IsNullOrWhiteSpace(userTransactions) ? new string[0] : userTransactions.Split('|');
				List<string> userTransactionList = new List<string>(userTransactionArray);
				userTransactionList.Add(transactionTextUser);
				if (userTransactionList.Count > 3) userTransactionList.RemoveAt(0);
				CPH.SetTwitchUserVar(userName, "transactions", string.Join(" | ", userTransactionList), true);
			} else {
				CPH.SendMessage($"Leider hast du noch kein Konto {userName}. Schreibe eine Nachricht, um ein Konto erstellen zu lassen.", true);
			}
			return false;
		}
		
		return true;
	}
}