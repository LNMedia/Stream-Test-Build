using System;

public class CPHInline
{
	public bool Execute()
	{
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
			CPH.SendMessage("Leider konnte kein Clip erstellt werden. Bitte versuche es später erneut.");
			CPH.ClearNonPersistedGlobals();
		}
		
		return false;
	}
}
