# 7. Médiagraphie / Utilisation de l'IA

## 7.1 Références
Voici les article et documentation que j'ai consulter pendant ma recherche : 

* D. Werner, « python-obd: A Python module for handling data from car OBD-II ports ». Disponible sur : https://github.com/brendan-w/python-obd

*  Microsoft Learn, « Minimal APIs overview | ASP.NET Core ». Disponible sur : https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis

*  Microsoft Learn, « Introduction to System.Threading.Channels ». Disponible sur : https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels/

* Microsoft Learn, « Introduction to ASP.NET Core SignalR ». Disponible sur : https://learn.microsoft.com/aspnet/core/signalr/introduction

* Raspberry Pi Foundation, « Raspberry Pi Documentation ». Disponible sur : https://www.raspberrypi.com/documentation/

---

## 7.2 Utilisation de l'AI dans mon projet (Claude)

### OpenSpec
Puisque j'ai utiliser l'outil OpenSpec, tout les Prompt OpenSpec utiliser sont dans le dossier openspec/archive de mon projet.

De plus, j'ai utiliser l'AI pour pour 2 contexts. 


### Contexte 1 — Configuration du Raspberry Pi Zero 2 W

La connexion SSH depuis VS Code crashait de façon répétée. L'IA a été utilisée pour diagnostiquer et optimiser la configuration du Pi (RAM, swap, overclock, services inutiles).

**Prompts utilisés :**

« Je viens de flasher Raspberry Pi OS Lite sur un Pi Zero 2 W avec SSH et Wi-Fi activés via Raspberry Pi Imager. Donne-moi les étapes complètes pour me connecter en SSH depuis VS Code et optimiser les performances du Pi pour cette utilisation. »

« Comment optimiser un Raspberry Pi Zero 2 W headless pour une connexion SSH stable ? Je veux maximiser la RAM disponible, configurer le swap et overclocker de façon stable. »

« VS Code Remote SSH boucle en permanence sur "Downloading VS Code Server" avec mon Raspberry Pi Zero 2 W. Le Pi tourne sous Raspberry Pi OS Lite, il a 415 MB de RAM et 414 MB de swap. Comment régler ce problème ? »

---

### Contexte 2 — Mode streaming pour la présentation du projet

L'IA ma aider à trouver une solution efficase pour le mode streaming afin de pouvoir présenter le projet. De plus l'AI ma aider à générer un dictionaire pour traduire le code WMI d'un véhicule. 

**Prompts utilisés :**

« En C# ASP.NET Core, j'ai des données qui arrivent 10 fois par seconde via une API et je dois les afficher sur une page Blazor. Comment faire pour recevoir les données rapidement sans bloquer ou ralentir l'affichage à l'écran ? »

« En C#, je veux créer un système pour simuler des données réelles récoltées. Je veux une interface ITelemetryStreamer. Explique comment je pourrais faire. »

« Je veux programmer la classe qui utilise mon interface ITelemetryStreamer. Comment faire pour qu'elle lise mes anciennes données dans SQLite une par une et remplace l'heure par l'heure actuelle ? »

« Aide moi pour faire une classe static en C# nommée VehicleDictionary. Elle doit contenir un dictionnaire qui associe des codes à 3 lettres de constructeurs automobiles (comme YV1 pour Volvo, 1FT pour Ford, 2HK pour Honda) avec le nom de la marque et du modèle. Ajoute une méthode ResolveVehicleName(string deviceId) qui prend le numéro de série d'un véhicule, extrait les 3 premières lettres pour chercher la marque dans le dictionnaire, et retourne le résultat. »