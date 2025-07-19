const socket = new WebSocket("ws://192.168.2.101:3251");
socket.onopen = () => { console.log("✅ WebSocket-Verbindung im Browser hergestellt"); };
    
socket.onmessage = (event) => {
    try {
        const data = JSON.parse(event.data);
        console.log("🎯 Event erhalten (Client):", data);

        if (data.mainData.eventType === "streamEvent") {
            handleStreamEvents(data);
        } 
        if (data.mainData.eventType === "alert") {
            handleAlertEvents(data);
        }
        if (data.mainData.eventType === "sceneSwitch") {
            handleSceneSwitch(data);
        }
        if (data.mainData.eventType === "chatMessage") {
            handleChatMessages(data);
        }
        if (data.mainData.eventType === "command") {
            if (data.message.includes("!")) {
                const command = data.message.split(" ")[0].substring(1).toLowerCase();
                executeCommand(command);
            }
        }
        
    } catch (e) { console.error("❌ Fehler beim Parsen der Nachricht im Client:", e.message); }
};
socket.onerror = (err) => { console.error("💥 WebSocket-Fehler:", err); };
socket.onclose = () => { console.warn("🔌 WebSocket getrennt (Client)"); };




