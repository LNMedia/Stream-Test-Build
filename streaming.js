const express = require("express");
const path = require("path");
require('dotenv').config();
const axios = require('axios');
const qs = require('querystring');
const browserSync = require('browser-sync');
const app = express();
const port = 3250;
// EJS konfigurieren
app.set("view engine", "ejs");
app.set("views", path.join(__dirname, "views"));

// Statische Dateien (z. B. CSS, JS)
app.use(express.static(path.join(__dirname, "public")));

// Route für den Beamer
app.get("/beamer", (req, res) => {
  res.render("beamer", { title: "Beamer" });
});

app.get("/overlay", (req, res) => {
  res.render("overlays/broadcast", { title: "OBS" });
});

app.get("/overlay/desktop", (req, res) => {
    res.render("overlays/desktop", { title: "Desktop" });
});

app.get("/commands", (req, res) => {
    res.render("organization", { title: "Befehlsliste" });
});

// SPOTIFY DATA
let accessToken = process.env.SPOTIFY_ACCESS_TOKEN;

async function refreshAccessToken() {
  const params = new URLSearchParams();
  params.append('grant_type', 'refresh_token');
  params.append('refresh_token', process.env.SPOTIFY_REFRESH_TOKEN);
  params.append('client_id', process.env.SPOTIFY_CLIENT_ID);
  params.append('client_secret', process.env.SPOTIFY_CLIENT_SECRET);

  try {
    const response = await axios.post('https://accounts.spotify.com/api/token', params);
    const newAccessToken = response.data.access_token;
    accessToken = newAccessToken;  // Update local variable
    console.log('Access Token erfolgreich erneuert');
    return newAccessToken;
  } catch (error) {
    console.error('Fehler beim Erneuern des Access Tokens:', error.response?.data || error.message);
    throw error;
  }
}

async function fetchCurrentSongAndQueue(token) {
  // Hilfsfunktion, um Song und Queue abzurufen
  const songResponse = await axios.get('https://api.spotify.com/v1/me/player/currently-playing', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  if (songResponse.status === 204 || !songResponse.data?.item) {
    return { playing: false };
  }

  const track = songResponse.data.item;
  const isPlaying = songResponse.data.is_playing === true;

  const queueResponse = await axios.get('https://api.spotify.com/v1/me/player/queue', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  // Nächste 3 Songs aus der Queue (außer aktuell spielendem)
  const queueTracks = queueResponse.data?.queue?.slice(0, 3) || [];

  return {
    playing: isPlaying,
    id: track.id,
    name: track.name,
    artist: track.artists.map(a => a.name).join(', '),
    album: track.album.name,
    cover: track.album.images?.[0]?.url || null,
    duration: track.duration_ms,
    progress: songResponse.data.progress_ms,
    queue: queueTracks
      .map(q => ({
        name: q.name,
        artist: q.artists.map(a => a.name).join(', '),
        cover: q.album.images?.[0]?.url || null
      }))
      .reverse() // Damit der nächste Song unten ist
  };
}

app.get('/currentsong', async (req, res) => {
  try {
    const data = await fetchCurrentSongAndQueue(accessToken);
    res.json(data);
  } catch (error) {
    if (error.response?.status === 401) {
      try {
        // Access Token erneuern
        const newToken = await refreshAccessToken();
        const data = await fetchCurrentSongAndQueue(newToken);
        res.json(data);
      } catch (refreshError) {
        console.error('Fehler beim Erneuern des Tokens:', refreshError.response?.data || refreshError.message);
        res.status(400).json({ error: 'Fehler beim Erneuern des Tokens', detail: refreshError.response?.data || refreshError.message });
      }
    } else {
      console.error('Spotify API Fehler:', error.response?.data || error.message);
      res.status(400).json({ error: 'Spotify API Fehler', detail: error.response?.data || error.message });
    }
  }
});

// WebSocket-Server laden
require("./websocket.js")(app);


// BrowserSync
browserSync.init({
  proxy: `http://localhost:${port}`,
  files: ['streaming.js', 'public/**/*', 'views/**/*'],
  watchOptions: {
      ignored: 'node_modules/**',
  },
  open: false,
  notify: false,
});

// Server starten
app.listen(port, () => {
  console.log(`🚀 Stream Server läuft auf http://localhost:${port}`);
});
