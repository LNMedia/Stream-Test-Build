# Streaming Test Build
A custom Node.js server designed for Twitch streaming automation and dynamic overlay integration using Streamer.bot and WebSockets.

---

## Table of contents
- [Streaming Test Build](#streaming-test-build)
  - [Table of contents](#table-of-contents)
  - [📌 About the Project](#-about-the-project)
  - [💡 Project Intention](#-project-intention)
  - [⚙️ Technologies Used](#️-technologies-used)
  - [🚀 Status](#-status)

---

## 📌 About the Project
This projects was built as a custom backend for my personal streaming setup. It integrates tightly with [Streamer.bot](https://streamer.bot) to intercept Twitch chat messages and commands, process them accordingly, and respond either directly or via external modules.

There are two types of chat commands:

**Chatonly commands**: These are handled directly inside the C# code of Streamer.bot and respond immediately with a chat message.

**Redirect commands**: These are forwarded to the Node.js server using WebSocket and processed further there (e.g., triggering overlays, alerts, or beamer projections).

---

## 💡 Project Intention

Originally created to integrate a projector into my stream setup, the server was used to:
- Show Twitch chat messages on a secondary screen (beamer)
- Display dynamic alerts
- Serve data to a dynamic OBS overlay
- Extend Streamer.bot functionality with custom backend logic

Although the system is not complete and has much room for improvement, it was partially functional and used during live streams.

---

## ⚙️ Technologies Used
- Node.js (Express)
- WebSocket
- Streamer.bot (as the core event engine)
- OBS (planned integration)
- HTML/CSS for overlays and displays

---

## 🚀 Status
⚠️ This project is experimental and no longer actively used, but remains a foundation for future development.