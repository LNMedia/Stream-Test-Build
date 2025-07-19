using System;

public class CPHInline
{
	public bool Execute()
	{
		CPH.SetGlobalVar("ws_eventType", "sceneSwitch", false);
		
		string sceneName = args["obs.sceneName"].ToString().ToLower();
		CPH.SetGlobalVar("ws_actionType", sceneName, false);
		
		CPH.ExecuteMethod("Websocket", "Send");
		CPH.ClearNonPersistedGlobals();
		
		return false;
	}
}
