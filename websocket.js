const WebSocket = require("ws");
const http = require("http");

module.exports = (expressApp) => {
    // Re-use HTTP server from Express
    const server = http.createServer(expressApp);
    const wss = new WebSocket.Server({ server });

    let clients = [];

    wss.on("connection", (ws) => {
        console.log("🔌 WebSocket-Client verbunden");
        clients.push(ws);

        ws.on('message', (data) => {
            const message = data.toString();
        
            try {
                const parsed = JSON.parse(message);
                console.log("🎯 Parsed Message:", parsed);
            
                // Nachricht an alle Clients weiterleiten (optional)
                wss.clients.forEach((client) => {
                    if (client.readyState === WebSocket.OPEN) {
                    client.send(JSON.stringify(parsed));
                    }
                });
            } catch (err) {
                console.error("❌ Fehler beim Parsen der Nachricht:", err.message);
            }
        });
        ws.on("close", () => {
            clients = clients.filter(c => c !== ws);
            console.log("❌ WebSocket-Client getrennt");
        });
    });

    // Starte den kombinierten Server
    server.listen(3251, () => {
        console.log("📡 WebSocket-Server auf ws://localhost:3251");
    });
};
