using System;
using System.Collections.Generic;

public class CPHInline {
    private static readonly Random rand = new Random(); // Einmalig, damit Seed nicht immer gleich ist

    public bool Execute() {
        var categories = new Dictionary<string, List<string>> {
            { "Funfact", new List<string> {
                    "Wusstest du, dass Oktopusse drei Herzen haben – genau wie ein Gamer, der kurz vorm Ragequit steht?",
                    "Funfact: Kühe haben beste Freunde und werden gestresst, wenn man sie trennt. Genau wie Streamer ohne Kaffee.",
                    "Ein durchschnittlicher Mensch läuft im Leben etwa viermal um die Welt. Ich hab’s nach drei Schritten ins Bett geschafft.",
                    "Honig verdirbt nie. Also ist dieser Stream genauso haltbar wie ein Glas Honig. Oder besser!",
                    "In Japan gibt es einen Melonengeschmack bei Pepsi. Vielleicht brauchen wir einen Gaming-Geschmack?",
                    "Ein Blitz enthält genug Energie, um 100.000 Toasts zu machen. Oder einen PC zum Absturz zu bringen. Beides gut.",
                    "Wusstest du, dass eine Schnecke bis zu drei Jahre schlafen kann? Also quasi wie ich nach dem Stream.",
					"Funfact: Das Wort 'Computer' wurde früher für Menschen verwendet, die rechnen – jetzt rechnet mein PC… meistens.",
					"In Frankreich ist es gesetzlich verboten, ein Schwein Napoleon zu nennen. Versuch das mal in einem Strategiespiel.",
					"Ein Tag auf der Venus dauert länger als ein Jahr. Also wenn du denkst, dein Tag zieht sich – sei froh, du bist nicht auf der Venus.",
					"Menschen und Bananen teilen 60 % ihrer DNA. Plötzlich ergibt mein Verhalten morgens Sinn.",
					"Der Eiffelturm kann im Sommer bis zu 15 cm wachsen – meiner Geduld beim Debuggen auch. Vielleicht.",
					"Ein Nilpferd kann schneller rennen als ein Mensch. Aber wie sieht’s mit Controller-Skills aus, hm?"
                    
                }
            },
            { "Lustig", new List<string> {
                    "Mein Gehirn ist wie ein Browser mit 23 Tabs offen. Drei sind abgestürzt. Keiner weiß, wo die Musik herkommt.",
                    "Wenn du dich heute nutzlos fühlst, denk dran: Es gibt einen 'Caps Lock Day'.",
                    "Ich bin nicht faul – ich befinde mich im Energiesparmodus.",
                    "Technisch gesehen ist der Montag der Bosskampf der Woche.",
                    "Wenn du dich mal dumm fühlst: Ich habe mal 'Strg+Alt+Entf' auf einem Toaster versucht. Kein Witz.",
                    "Ich spiele nicht zu viel – ich trainiere für die Apokalypse!",
                    "Ich bin heute nicht faul – ich bin auf Standby.",
					"Warum ich streame? Weil Netflix mich nicht casten wollte.",
					"Mein Körper ist ein Tempel… ungeheizt, voller Spinnweben und mit WLAN-Problemen.",
					"Ich hab letztens versucht, mich selbst zu motivieren. War nicht da.",
					"Wenn Kaffee nicht hilft, hilft nur noch… nein, doch wieder Kaffee.",
					"Ich bin kein Morgenmensch. Oder ein Nachmensch. Eigentlich bin ich eher so… Mensch mit Internet.",
					"Hab heute meine To-Do-Liste angeschaut… und direkt wieder zu gemacht.",
					"Ich habe keine Zeit zu prokrastinieren – ich verschiebe das auf später."
                }
            },
            { "Socials", new List<string> {
                    "Folge mir auf TikTok: https://tiktok.com/@apixoffiziell – Content, der nicht mal ich verstehe!",
                    "Instagram? Jep, da gibt’s Bilder und Unsinn: https://instagram.com/einfachandi98",
                    "YouTube ist wie meine zweite Heimat – Chaos garantiert: https://youtube.com/@apixoffiziell",
                    "Discord ist, wo der Wahnsinn wohnt: https://discord.gg/vjg7kKyH2m – Kekse inklusive!",
                    "Podcast mit Herz und Unsinn: https://pixelundpils.de – Gib’s dir.",
                    "Mehr Unsinn außerhalb vom Stream? Klar, hier: https://linktr.ee/apixoffiziell",
					"Wenn du dachtest, das hier ist verrückt – warte, bis du meine Storys auf Insta siehst: https://instagram.com/einfachandi98",
					"Ich mache auch YouTube! Videos, bei denen du nicht sicher bist, ob du lachen oder weinen sollst: https://youtube.com/@apixoffiziell",
					"Mein TikTok hat mehr Chaos als mein Kleiderschrank – und das will was heißen: https://tiktok.com/@apixoffiziell",
					"Join the cult – äh, Community auf Discord: https://discord.gg/vjg7kKyH2m",
					"Pixel, Pils und Podcast – was will man mehr? https://pixelundpils.de"
                }
            },
            { "Donation", new List<string> {
                    "Willst du diesen Streamer glücklich machen? Tippe einfach !donation oder !spende 😍",
                    "Unterstützung gefällig? !tip oder !support bringt dich zum Spendenbefehl 💖",
                    "Mit !spende oder !tip kannst du mir ein Lächeln ins Gesicht zaubern (und evtl. Kaffee finanzieren ☕).",
                    "Du magst, was du siehst? Dann lass ein Trinkgeld da mit !spende – Streamer-Herz freut sich!",
					"Mit !donation hilfst du, diesen Content am Laufen zu halten – und vielleicht meinen Kühlschrank auch.",
					"Spenden ist wie Magie – nur echter! !spende oder !tip zaubert mir ein Lächeln ins Gesicht.",
					"Möchtest du ein Held sein? Dann tippe !support oder !spende – ganz ohne Umhang.",
					"Unterstützung in Form von Bits, Subs oder Liebe – alles willkommen! Aber !donation ist der geheime Knopf."
                }
            },
            { "Partner", new List<string> {
                    "Neugierig auf meine Partner? !partner zeigt dir, wer mich unterstützt (und dir vielleicht auch helfen kann!)",
                    "Mit !instantgaming oder !streamboost kannst du meine Partner abchecken – lohnt sich doppelt!",
                    "Unterstütze meine Partner mit !partner – sie machen den Stream möglich (und mir das Leben leichter).",
                    "Gaming günstiger? Dann klick !instantgaming – meine Empfehlung für digitale Spiele-Schnäppchen.",
					"Mit !partner entdeckst du coole Unterstützer des Streams – Support für beide Seiten!",
					"Boost your Stream mit !streamboost – meine Partner machen’s möglich.",
					"Meine Partner helfen mir – und dir vielleicht auch! Check !partner für mehr Infos.",
					"Ob Games oder Tools: Mit !partner findest du, was mich besser macht (und vielleicht dich auch)."
                }
            }
        };
        List<string> keys = new List<string>();
        foreach (string key in categories.Keys) { keys.Add(key); }
        string chosenCategory = keys[rand.Next(keys.Count)];
        List<string> messages = categories[chosenCategory];
        string chosenMessage = messages[rand.Next(messages.Count)];
        CPH.SendMessage(chosenMessage, true);
        return false;
    }
}
