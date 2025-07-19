// DECLARE VARIABLES (MAIN)
const bodyElement = document.querySelector('body');
const streamBoost = document.querySelector('.streamBoost');
// DECLARE VARIABLES (OVERLAY-CONTAINER)
const overlayContainer = document.querySelector('.overlayContainer');
const backgroundContainer = document.querySelector('.backgroundContainer');
const stateName = document.querySelector('.stateName');
const countDown = document.querySelector('.countDown');
const gameShow = document.querySelector('.gameShow');
const gameImage = document.querySelectorAll('.gameImage');
const gameName = document.querySelectorAll('.gameName');
// DECLARE VARIABLES (ALERT-CONTAINER)
const alertContainer = document.querySelector('.alertMainContainer');
const alertHeader = document.querySelector('.alertHeader');
const alertUserData = document.querySelector('.alertUserData');
const alertProfileImage = document.querySelector('.alertContainer .userImage');
const alertUserName = document.querySelector('.alertContainer .userName');
const alertData = document.querySelector('.alertContainer .alertData');
const alertTimeStamp = document.querySelector('.alertContainer .timeStamp');
// DECLARE VARIABLES (OVERLAYS APIX)
const overlayAPIX = document.querySelector('.overlayAPIX');
const overlayAPIXJC = document.querySelector('.overlayAPIXJC');
const overlayAPIXGMG = document.querySelector('.overlayAPIXGMG');
// DECLARE VARIABLES (OVERLAYS PP)
const overlayPP = document.querySelector('.overlayPP');
const overlayPPPC = document.querySelector('.overlayPPPC');
const overlayPPGMG = document.querySelector('.overlayPPGMG');
//DECLARE VARIABLES (CHATBOX)
const chatOverlay = document.querySelector('.chatOverlay');
const chatBox = document.querySelector('.chatBox');


// STREAM EVENTS (OVERLAYS)
function handleStreamEvents(data) {
    // CHECK FOR VARIABLES
    if (
        !bodyElement ||
        !overlayContainer ||
        !backgroundContainer ||
        !stateName ||
        !countDown ||
        !gameShow ||
        !gameImage ||
        !gameName ||
        !streamBoost ||
        !chatOverlay
    ) return;

    // CHECK FOR DATA
    if (
        !data ||
        !data.mainData ||
        !data.channelData ||
        !data.latestData ||
        !data.counterData
    ) return;

    // CHECK EVENTTYPE (ACTIONTYPE) AND PUSH ACTIONS
    if (data.mainData.actionType === "streamStart") {
        bodyElement.classList.remove('apixoffiziell', 'pixelundpils');
        stateName.classList.remove('starting', 'pause', 'ending');
        gameShow.classList.remove('shown');
        switch (data.channelData.streamerChannel) {
            case 'APiXoffiziell':
                chatOverlay.classList.remove('shown');
                bodyElement.classList.add('apixoffiziell');
                stateName.classList.add('starting');
                gameShow.classList.add('shown');
                overlayContainer.classList.remove('displayNone');
                startStreamEventCountDown(data);
                break;
            case 'PixelUndPils':
                chatOverlay.classList.remove('shown');
                bodyElement.classList.add('pixelundpils');
                stateName.classList.add('starting');
                gameShow.classList.add('shown');
                overlayContainer.classList.remove('displayNone');
                startStreamEventCountDown(data);
                break;
            default:
                bodyElement.classList.remove('apixoffiziell', 'pixelundpils');
                gameShow.classList.remove('shown');
                chatOverlay.classList.remove('shown');
        }
        if (data.channelData?.gameName) {
            const gameNameElements = document.querySelectorAll('.gameName');
            gameNameElements.forEach(el => {
                el.textContent = data.channelData.gameName;
            });
        }
        if (data.channelData?.gameImage) {
            const formattedUrl = formatGameImageUrl(data.channelData.gameImage);
            const gameImageElements = document.querySelectorAll('.gameImage');
        
            gameImageElements.forEach(el => {
                if (el.tagName.toLowerCase() === 'img') {
                    el.src = formattedUrl;
                } else {
                    el.style.backgroundImage = `url(${formattedUrl})`;
                }
            });
        }
    } else if (data.mainData.actionType === "streamPause") {
        bodyElement.classList.remove('apixoffiziell', 'pixelundpils');
        stateName.classList.remove('starting', 'pause', 'ending');
        gameShow.classList.remove('shown');
        switch (data.channelData.streamerChannel) {
            case 'APiXoffiziell':
                chatOverlay.classList.remove('shown');
                bodyElement.classList.add('apixoffiziell');
                stateName.classList.add('pause');
                gameShow.classList.add('shown');
                overlayContainer.classList.remove('displayNone');
                break;
            default:
                bodyElement.classList.remove('apixoffiziell', 'pixelundpils');
                gameShow.classList.remove('shown');
                chatOverlay.classList.remove('shown');
        }
    } else if (data.mainData.actionType === "streamResume") {
        overlayContainer.classList.add('displayNone');
        countDown.textContent = "";
        stateName.classList.remove('starting', 'pause', 'ending');
        chatOverlay.classList.add('shown');
        showOverlay(data);
    } else if (data.mainData.actionType === "streamEnding") {
        bodyElement.classList.remove('apixoffiziell', 'pixelundpils');
        stateName.classList.remove('starting', 'pause', 'ending');
        gameShow.classList.remove('shown');
        switch (data.channelData.streamerChannel) {
            case 'APiXoffiziell':
                chatOverlay.classList.remove('shown');
                bodyElement.classList.add('apixoffiziell');
                stateName.classList.add('ending');
                gameShow.classList.add('shown');
                overlayContainer.classList.remove('displayNone');
                break;
            default:
                bodyElement.classList.remove('apixoffiziell', 'pixelundpils');
                gameShow.classList.remove('shown');
                chatOverlay.classList.remove('shown');
        }
    }
}
function startStreamEventCountDown(data) {
    let countDownTime = 5 * 60; // 5 Minuten in Sekunden

    const interval = setInterval(function() {
        // Berechne Minuten und Sekunden
        const minutes = Math.floor(countDownTime / 60);
        const seconds = countDownTime % 60;

        // Zeige die verbleibende Zeit an (optional)
        if (countDown) {
            countDown.textContent = `${minutes}:${seconds.toString().padStart(2, '0')}`;
        }

        // Wenn Countdown vorbei ist
        if (countDownTime <= 0) {
            clearInterval(interval);
            if (overlayContainer) {
                overlayContainer.classList.add('displayNone');
                countDown.textContent = "";
                stateName.classList.remove('starting', 'pause', 'ending');
                gameShow.classList.remove('shown');
                chatOverlay.classList.add('shown');
                showOverlay(data);
            }
        }

        countDownTime--;
    }, 1000);
}

// ALERT EVENTS
function handleAlertEvents(data) {
    console.log(`Alert wurde erfolgreich erkannt: ${data.mainData.actionType}`);
    if (
        !alertContainer ||
        !alertHeader ||
        !alertTimeStamp ||
        !alertData
    ) return;

    const actionType = data.mainData?.actionType;
    const userPicture = data.userData?.userPicture || data.donationData?.donationPicture || "";
    const userName = data.userData?.userName?.trim() || data.donationData?.donationUser?.trim() || "";
    const timeStamp = data.mainData?.timeStamp;

    if (
        !actionType?.trim() ||
        !userName?.trim() ||
        !timeStamp?.trim()
    ) return;
    // Wellenanimation mit Farbe kombinieren
    let color = "";
    alertData.innerHTML = '';
    alertUserData.style.justifyContent = '';
    
    switch(actionType) {
        case "cheer":
            color = "#FFD700"; // Gold
            if(data.cheerData) {
                const liAmount = document.createElement('li');
                liAmount.textContent = `Bits: ${data.cheerData.cheerValue}`;
                alertData.appendChild(liAmount);
            }
            break;
        case "donation":
            color = "#FF7F50"; // Coral
            if(data.donationData) {
                const liAmount = document.createElement('li');
                liAmount.textContent = `Spendenhöhe: ${data.donationData?.donationAmount} ${data.donationData?.donationCurrency}`;
                alertData.appendChild(liAmount);
                const liMessage = document.createElement('li');
                liMessage.textContent = `Nachricht: ${data.donationData?.donationMessage}`;
                alertData.appendChild(liMessage);
            }
            break;
        case "follow":
            color = "#1E90FF"; // DodgerBlue
            alertUserData.style.justifyContent = 'center';
            break;
        case "raid":
            color = "#8A2BE2"; // BlueViolet
            if(data.raidData) {
                const liAmount = document.createElement('li');
                liAmount.textContent = `Raider: ${data.raidData?.raidViewers}`;
                alertData.appendChild(liAmount);
            }
            break;
        case "subgiftbomb":
            color = "#FF69B4"; // HotPink
            if(data.subscriptionData) {
                const liAmount = document.createElement('li');
                liAmount.textContent = `Anzahl: ${data.subscriptionData?.userSubsGiftedNow}, ${data.subscriptionData?.userSubTier.toUpperCase()} SUBS`;
                alertData.appendChild(liAmount);
                const liTotal = document.createElement('li');
                liTotal.textContent = `Insgesamt verschenkte Subs: ${data.subscriptionData?.userSubsGiftedTotal}`;
                alertData.appendChild(liTotal);
            }
            break;
        case "gifted sub":
            color = "#40E0D0"; // Turquoise
            if(data.subscriptionData) {
                const liAmount = document.createElement('li');
                liAmount.textContent = `Verschenkt
                                        ${data.subscriptionData?.userSubsGiftedMonths} Monat(e),
                                        ${data.subscriptionData?.userSubTier.toUpperCase()} Sub an
                                        ${data.subscriptionData?.userSubRecipients}`;
                alertData.appendChild(liAmount);
                const liTotal = document.createElement('li');
                liTotal.textContent = `Insgesamt verschenkte Subs: ${data.subscriptionData?.userSubsGiftedTotal}`;
                alertData.appendChild(liTotal);
            }
            break;
        case "subscription":
            color = "#32CD32"; // LimeGreen
            if(data.subscriptionData) {
                const liAmount = document.createElement('li');
                liAmount.textContent = `Abonniert für ${data.subscriptionData?.userSubMonth} Monat(e) auf
                                        ${data.subscriptionData?.userSubTier.toUpperCase()}`;
                alertData.appendChild(liAmount);
            }
            break;
        case "resubscription":
            color = "#228B22"; // ForestGreen
            if(data.subscriptionData) {
                const liAmount = document.createElement('li');
                liAmount.textContent = `Abonniert für ${data.subscriptionData?.userSubMonth} Monat(e) auf
                                        ${data.subscriptionData?.userSubTier.toUpperCase()}`;
                alertData.appendChild(liAmount);
                const liMessage = document.createElement('li');
                liMessage.textContent = `Nachricht: ${data.mainData?.message}`;
                alertData.appendChild(liMessage);
            }
            break;
        default:
            color = ""; // Reset oder Standardfarbe
    }
    alertProfileImage.src = userPicture;
    alertUserName.textContent = userName.toUpperCase();
    alertTimeStamp.textContent = correctTo24Hour(timeStamp);
    alertContainer.classList.add('shown');
    let countDownTime = 10; // 5 Minuten in Sekunden
    const interval = setInterval(function() {
        if (countDownTime <= 0) {
            clearInterval(interval);
            if (alertContainer) {
                alertContainer.classList.remove('shown');
            }
        }
        countDownTime--;
    }, 1000);
    animateWaveText(alertHeader, actionType.toUpperCase(), color);
}
function animateWaveText(element, text, color) {
    element.innerHTML = '';
    const chars = text.split('');
    chars.forEach((char, i) => {
        const span = document.createElement('span');
        span.textContent = char;
        span.style.color = color;
        span.style.animationDelay = `${i * 0.1}s`;
        element.appendChild(span);
    });
}

// SCENE SWITCH
function handleSceneSwitch(data) {
    if (
        !overlayAPIX ||
        !overlayAPIXJC ||
        !overlayAPIXGMG ||
        !overlayPP ||
        !overlayPPPC ||
        !overlayPPGMG
    ) return;
    if (!data.mainData) return;
    let scene = data?.mainData?.actionType;
    console.log(scene);
    switch(scene) {
        case 'apixoffiziell-jc':
            removeAllOverlays();
            overlayAPIX.classList.add('shown');
            overlayAPIXJC.classList.add('shown');
            break;
        case 'apixoffiziell-gmg':
            removeAllOverlays();
            overlayAPIX.classList.add('shown');
            overlayAPIXGMG.classList.add('shown');
            break;
        case 'pixelundpils-pc':
            removeAllOverlays();
            overlayPP.classList.add('shown');
            overlayPPPC.classList.add('shown');
            break;
        case 'pixelundpils-gmg':
            removeAllOverlays();
            overlayPP.classList.add('shown');
            overlayPPGMG.classList.add('shown');
            break;
        default:
    }
}
function removeAllOverlays() {
    overlayAPIX.classList.remove('shown');
    overlayAPIXJC.classList.remove('shown');
    overlayAPIXGMG.classList.remove('shown');
    overlayPP.classList.remove('shown');
    overlayPPPC.classList.remove('shown');
    overlayPPGMG.classList.remove('shown');
}
function showOverlay(data) {
    // CHECK FOR VARIABLES
    if (!data || !data.channelData) return;

    // CHECK GAMENAME AND PUSH OVERLAYS
    if (data.channelData.gameName === "Just Chatting") {
        removeAllOverlays();
        overlayAPIX.classList.add('shown');
        overlayAPIXJC.classList.add('shown');
    } else if (data.channelData.gameName === "Talk Shows & Podcasts") {
        removeAllOverlays();
        overlayPP.classList.add('shown');
        overlayPPPC.classList.add('shown');
    } else if (
        data.channelData.streamerChannel === "APiXoffiziell" &&
        data.channelData.gameName !== "Just Chatting" &&
        data.channelData.gameName !== "Talk Shows & Podcasts"
    ) {
        removeAllOverlays();
        overlayAPIX.classList.add('shown');
        overlayAPIXGMG.classList.add('shown');
    } else if (
        data.channelData.streamerChannel === "Pixel & Pils" &&
        data.channelData.gameName !== "Just Chatting" &&
        data.channelData.gameName !== "Talk Shows & Podcasts"
    ) {
        removeAllOverlays();
        overlayPP.classList.add('shown');
        overlayPPGMG.classList.add('shown');
    }
}

// SPOTIFY FUNCTIONS
async function updateCurrentSong() {
    try {
        const res = await fetch('/currentsong');
        const data = await res.json();

        const songNameEl = document.getElementById('songName');
        const songArtistEl = document.getElementById('songArtist');
        const albumNameEl = document.querySelector('.albumName');
        const cdOverlay = document.querySelector('.cdOverlay');
        const wishList = document.querySelector('.wishList');

        wishList.innerHTML = ''; // Warteschlange leeren

        if (!data.playing) {
            songNameEl.textContent = 'Kein Song läuft gerade';
            songArtistEl.textContent = '';
            albumNameEl.textContent = '';
            cdOverlay.style.background = 'radial-gradient(circle at center, #fff 0%, #ccc 30%, #999 70%, #000 100%)';
            return;
        }

        songNameEl.textContent = data.name || 'Unbekannter Titel';
        songArtistEl.textContent = data.artist || 'Unbekannter Künstler';
        albumNameEl.textContent = data.album || 'Unbekanntes Album';

        // Hintergrundbild vom CD-Overlay setzen
        cdOverlay.style.background = data.cover
            ? `url('${data.cover}') center center / cover no-repeat`
            : 'radial-gradient(circle at center, #fff 0%, #ccc 30%, #999 70%, #000 100%)';

        // Warteschlange (nächste Songs) anzeigen, wenn vorhanden
        if (data.queue && Array.isArray(data.queue) && data.queue.length > 0) {
            data.queue.slice(0, 3).forEach(song => {
                const li = document.createElement('li');
                li.style.display = 'flex';
                li.style.alignItems = 'flex-end';
                li.style.textAlign = 'right';
                li.style.marginBottom = '3px';

                const text = document.createElement('div');
                text.innerHTML = `<strong>${song.name}</strong><br><small>${song.artist}</small>`;
                li.appendChild(text);

                if (song.cover) {
                    const img = document.createElement('img');
                    img.src = song.cover;
                    img.alt = `${song.name} Cover`;
                    img.style.width = '35px';
                    img.style.height = '35px';
                    img.style.objectFit = 'cover';
                    img.style.marginLeft = '10px';
                    li.appendChild(img);
                }
                wishList.appendChild(li);
            });
        } else {
            const li = document.createElement('li');
            li.textContent = 'Keine weiteren Songs in der Warteschlange.';
            wishList.appendChild(li);
        }
    } catch (err) {
        console.error('Fehler beim Abrufen des aktuellen Songs:', err);
    }
}

// CHATOVERLAY
function handleChatMessages(data) {
    if (!chatBox) return;
    if (
        !data.mainData.message ||
        !data.userData
    ) return;

    if (data.mainData.actionType === "showMessage") {
        const chatMessage = document.createElement('li');
        chatMessage.innerHTML = `
            <img src="${data.userData.userPicture}">
            <div class="chatContent">
                <h4 style="color: ${data.userData.userColor}">${data.userData.userName}:</h4>
                <p>${data.mainData.message}</p>
            </div>
        `;
        chatBox.appendChild(chatMessage);
    }
}

// HELPER
function correctTo24Hour(timeString) {
    const now = new Date();
    const [hourStr, minuteStr, secondStr] = timeString.split(':');
    let hour = parseInt(hourStr, 10);

    // Wenn übergebene Stunde < 12 UND aktuelle Stunde > 12 => vermutlich PM
    if (hour < 12 && now.getHours() >= 12) {
        hour += 12;
    }

    // Sonderfall: Wenn es Mitternacht ist und Stunde 12, dann zurück auf 0
    if (hour === 24) hour = 0;

    const corrected = `${hour.toString().padStart(2, '0')}:${minuteStr}:${secondStr}`;
    return corrected;
}
function formatGameImageUrl(url) {
    return url
        .replace("{width}", "400")
        .replace("{height}", "500");
}


// DOMCONTENTLOADED
window.addEventListener('DOMContentLoaded', () => {
    setInterval(updateCurrentSong, 5000);
    updateCurrentSong();
})