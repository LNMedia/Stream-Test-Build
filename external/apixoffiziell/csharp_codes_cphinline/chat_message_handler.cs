using System;

public class CPHInline
{
	public bool Execute()
	{
		CPH.RunAction("-- User Checker --", true);
		CPH.TryGetArg("inGroup", out bool inGroup);
		if (inGroup) return false;
		
		CPH.RunAction("-- set TimeStamp --", true);
		
		if (args.ContainsKey("rawInput") && args["rawInput"] != null) {
			string userInput = args["rawInput"].ToString().Trim().ToLower();

			if (userInput.StartsWith("!")) {
				CPH.RunAction("## COMMAND CHECKER ##", true);
				return false;
			}
		}
		
		CPH.TryGetArg("eventSource", out string sourceType);
		CPH.SetGlobalVar("ws_sourceType", sourceType, false);
		
		string eventType = "chatMessage";
		CPH.SetGlobalVar("ws_eventType", eventType, false);
		
		string actionType = "showMessage";
		CPH.SetGlobalVar("ws_actionType", actionType, false);
		
		CPH.TryGetArg("user", out string userName);
		CPH.SetGlobalVar("ws_userName", userName, false);
		
		CPH.TryGetArg("color", out string userColor);
		CPH.SetGlobalVar("ws_userColor", userColor, false);
		
		CPH.TryGetArg("targetUserType", out string userType);
		CPH.SetGlobalVar("ws_userType", userType, false);
		
		CPH.TryGetArg("targetUserProfileImageUrl", out string userPicture);
		CPH.SetGlobalVar("ws_userPicture", userPicture, false);
		
		CPH.TryGetArg("isFollowing", out bool isFollowing);
		CPH.SetGlobalVar("ws_isFollowing", isFollowing, false);
		
		CPH.TryGetArg("isSubscribed", out bool isSubscriber);
		CPH.SetGlobalVar("ws_isSubscriber", isSubscriber, false);
		
		CPH.TryGetArg("isVip", out bool isVip);
		CPH.SetGlobalVar("ws_isVip", isVip, false);
		
		CPH.TryGetArg("isModerator", out bool isModerator);
		CPH.SetGlobalVar("ws_isModerator", isModerator, false);
		
		CPH.TryGetArg("message", out string message);
		CPH.SetGlobalVar("ws_message", message, false);
		
		CPH.ExecuteMethod("Websocket", "Send");
		CPH.ClearNonPersistedGlobals();
		return false;
	}
}
