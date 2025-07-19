// Script for Bits
using System;

public class CPHInline
{
	public bool Execute()
	{
		CPH.RunAction("-- User Checker --", true);
		CPH.RunAction("-- set TimeStamp --", true);
		
		CPH.TryGetArg("eventSource", out string sourceType);
		CPH.SetGlobalVar("ws_sourceType", sourceType, false);
		
		string eventType = "alert";
		CPH.SetGlobalVar("ws_eventType", eventType, false);
		
		string actionType = "cheer";
		CPH.SetGlobalVar("ws_actionType", actionType, false);
		
		CPH.TryGetArg("user", out string userName);
		CPH.SetGlobalVar("ws_userName", userName, false);
		CPH.SetGlobalVar("ws_latestCheerer", userName, true);
		
		CPH.TryGetArg("targetUserProfileImageUrl", out string userPicture);
		CPH.SetGlobalVar("ws_userPicture", userPicture, false);
		
		CPH.TryGetArg("bits", out int amount);
		CPH.SetGlobalVar("ws_cheerValue", amount, false);
		
		CPH.ExecuteMethod("Websocket", "Send");
		CPH.ClearNonPersistedGlobals();
		return true;
	}
}


// Script for donations
using System;

public class CPHInline
{
	public bool Execute()
	{
		CPH.RunAction("-- set TimeStamp --", true);
		
		string sourceType = "Tipeeestream";
		CPH.SetGlobalVar("ws_sourceType", sourceType, false);
		
		string eventType = "alert";
		CPH.SetGlobalVar("ws_eventType", eventType, false);
		
		string actionType = "donation";
		CPH.SetGlobalVar("ws_actionType", actionType, false);
		
		CPH.TryGetArg("username", out string userName);
		CPH.SetGlobalVar("ws_donationUser", userName, false);
		CPH.SetGlobalVar("ws_latestDonator", userName, true);
		
		CPH.TryGetArg("avatar", out string userPicture);
		CPH.SetGlobalVar("ws_donationPicture", userPicture, false);
		
		CPH.TryGetArg("amount", out int amount);
		CPH.SetGlobalVar("ws_donationAmount", amount, false);
		
		CPH.TryGetArg("currency", out string currency);
		CPH.SetGlobalVar("ws_donationCurrency", currency, false);
		
		CPH.TryGetArg("message", out string message);
		CPH.SetGlobalVar("ws_donationMessage", message, false);
		
		CPH.ExecuteMethod("Websocket", "Send");
		CPH.ClearNonPersistedGlobals();
		return true;
	}
}

// Script for follows
using System;

public class CPHInline
{
	public bool Execute()
	{
		CPH.RunAction("-- User Checker --", true);
		CPH.RunAction("-- set TimeStamp --", true);
		
		CPH.TryGetArg("eventSource", out string sourceType);
		CPH.SetGlobalVar("ws_sourceType", sourceType, false);
		
		string eventType = "alert";
		CPH.SetGlobalVar("ws_eventType", eventType, false);
		
		string actionType = "follow";
		CPH.SetGlobalVar("ws_actionType", actionType, false);
		
		CPH.TryGetArg("user", out string userName);
		CPH.SetGlobalVar("ws_userName", userName, false);
		CPH.SetGlobalVar("ws_latestFollower", userName, true);
		
		CPH.TryGetArg("targetUserProfileImageUrl", out string userPicture);
		CPH.SetGlobalVar("ws_userPicture", userPicture, false);
		
		CPH.ExecuteMethod("Websocket", "Send");
		CPH.ClearNonPersistedGlobals();
		return true;
	}
}


// Script for raid
using System;

public class CPHInline
{
	public bool Execute()
	{
		CPH.RunAction("-- User Checker --", true);
		CPH.RunAction("-- set TimeStamp --", true);
		
		CPH.TryGetArg("eventSource", out string sourceType);
		CPH.SetGlobalVar("ws_sourceType", sourceType, false);
		
		string eventType = "alert";
		CPH.SetGlobalVar("ws_eventType", eventType, false);
		
		string actionType = "raid";
		CPH.SetGlobalVar("ws_actionType", actionType, false);
		
		CPH.TryGetArg("user", out string userName);
		CPH.SetGlobalVar("ws_userName", userName, false);
		
		CPH.TryGetArg("targetUserProfileImageUrl", out string userPicture);
		CPH.SetGlobalVar("ws_userPicture", userPicture, false);
		
		CPH.TryGetArg("viewers", out int amount);
		CPH.SetGlobalVar("ws_raidViewers", amount, false);
		
		CPH.ExecuteMethod("Websocket", "Send");
		CPH.ClearNonPersistedGlobals();
		return true;
	}
}


// Script for subbomb
using System;

public class CPHInline
{
	public bool Execute()
	{
		CPH.RunAction("-- User Checker --", true);
		CPH.RunAction("-- set TimeStamp --", true);
		
		CPH.TryGetArg("eventSource", out string sourceType);
		CPH.SetGlobalVar("ws_sourceType", sourceType, false);
		
		string eventType = "alert";
		CPH.SetGlobalVar("ws_eventType", eventType, false);
		
		string actionType = "subgiftbomb";
		CPH.SetGlobalVar("ws_actionType", actionType, false);
		
		CPH.TryGetArg("user", out string userName);
		CPH.SetGlobalVar("ws_userName", userName, false);
		
		CPH.TryGetArg("targetUserProfileImageUrl", out string userPicture);
		CPH.SetGlobalVar("ws_userPicture", userPicture, false);
		
		CPH.TryGetArg("tier", out string subTier);
		CPH.SetGlobalVar("ws_userSubTier", subTier, false);
		
		CPH.TryGetArg("gifts", out int amount);
		CPH.SetGlobalVar("ws_userSubsGiftedNow", amount, false);
		
		CPH.TryGetArg("totalGifts", out int total);
		CPH.SetGlobalVar("ws_userSubsGiftedTotal", total, false);
		
		CPH.ExecuteMethod("Websocket", "Send");
		CPH.ClearNonPersistedGlobals();
		return true;
	}
}


// Script for gifted sub
using System;

public class CPHInline
{
	public bool Execute()
	{
		CPH.TryGetArg("fromGiftBomb", out bool giftBomb);
		if (giftBomb) return false;
		
		CPH.RunAction("-- User Checker --", true);
		CPH.RunAction("-- set TimeStamp --", true);
		
		CPH.TryGetArg("eventSource", out string sourceType);
		CPH.SetGlobalVar("ws_sourceType", sourceType, false);
		
		string eventType = "alert";
		CPH.SetGlobalVar("ws_eventType", eventType, false);
		
		string actionType = "gifted sub";
		CPH.SetGlobalVar("ws_actionType", actionType, false);
		
		CPH.TryGetArg("user", out string userName);
		CPH.SetGlobalVar("ws_userName", userName, false);
		
		CPH.TryGetArg("targetUserProfileImageUrl", out string userPicture);
		CPH.SetGlobalVar("ws_userPicture", userPicture, false);
		
		CPH.TryGetArg("tier", out string subTier);
		CPH.SetGlobalVar("ws_userSubTier", subTier, false);
		
		CPH.TryGetArg("monthsGifted", out int amountMonths);
		CPH.SetGlobalVar("ws_userSubsGiftedMonths", amountMonths, false);
		
		CPH.TryGetArg("subBombCount", out int amountNow);
		CPH.SetGlobalVar("ws_userSubsGiftedNow", amountNow, false);
		
		CPH.TryGetArg("totalSubsGifted", out int amountTotal);
		CPH.SetGlobalVar("ws_userSubsGiftedTotal", amountTotal, false);
		
		CPH.TryGetArg("recipientUser", out string recipientUser);
		CPH.SetGlobalVar("ws_userSubRecipients", recipientUser, false);
		CPH.SetGlobalVar("ws_latestSubscriber", recipientUser, true);
		
		CPH.ExecuteMethod("Websocket", "Send");
		CPH.ClearNonPersistedGlobals();
		return true;
	}
}


// Script for sub
using System;

public class CPHInline
{
	public bool Execute()
	{
		CPH.TryGetArg("fromGiftBomb", out bool giftBomb);
		if (giftBomb) return false;
		
		CPH.RunAction("-- User Checker --", true);
		CPH.RunAction("-- set TimeStamp --", true);
		
		CPH.TryGetArg("eventSource", out string sourceType);
		CPH.SetGlobalVar("ws_sourceType", sourceType, false);
		
		string eventType = "alert";
		CPH.SetGlobalVar("ws_eventType", eventType, false);
		
		string actionType = "subscription";
		CPH.SetGlobalVar("ws_actionType", actionType, false);
		
		CPH.TryGetArg("user", out string userName);
		CPH.SetGlobalVar("ws_userName", userName, false);
		CPH.SetGlobalVar("ws_latestSubscriber", userName, true);
		
		CPH.TryGetArg("targetUserProfileImageUrl", out string userPicture);
		CPH.SetGlobalVar("ws_userPicture", userPicture, false);
		
		CPH.TryGetArg("tier", out string subTier);
		CPH.SetGlobalVar("ws_userSubTier", subTier, false);
		
		CPH.TryGetArg("monthsSub", out int amountMonths);
		CPH.SetGlobalVar("ws_userSubMonth", amountMonths, false);
		
		CPH.ExecuteMethod("Websocket", "Send");
		CPH.ClearNonPersistedGlobals();
		return true;
	}
}


// script for resub
using System;

public class CPHInline
{
	public bool Execute()
	{
		CPH.TryGetArg("fromGiftBomb", out bool giftBomb);
		if (giftBomb) return false;
		
		CPH.RunAction("-- User Checker --", true);
		CPH.RunAction("-- set TimeStamp --", true);
		
		CPH.TryGetArg("eventSource", out string sourceType);
		CPH.SetGlobalVar("ws_sourceType", sourceType, false);
		
		string eventType = "alert";
		CPH.SetGlobalVar("ws_eventType", eventType, false);
		
		string actionType = "resubscription";
		CPH.SetGlobalVar("ws_actionType", actionType, false);
		
		CPH.TryGetArg("user", out string userName);
		CPH.SetGlobalVar("ws_userName", userName, false);
		CPH.SetGlobalVar("ws_latestSubscriber", userName, true);
		
		CPH.TryGetArg("targetUserProfileImageUrl", out string userPicture);
		CPH.SetGlobalVar("ws_userPicture", userPicture, false);
		
		CPH.TryGetArg("tier", out string subTier);
		CPH.SetGlobalVar("ws_userSubTier", subTier, false);
		
		CPH.TryGetArg("cumulative", out int amountMonths);
		CPH.SetGlobalVar("ws_userSubMonth", amountMonths, false);
		
		CPH.TryGetArg("message", out string message);
		CPH.SetGlobalVar("ws_message", message, false);
		
		CPH.ExecuteMethod("Websocket", "Send");
		CPH.ClearNonPersistedGlobals();
		return true;
	}
}