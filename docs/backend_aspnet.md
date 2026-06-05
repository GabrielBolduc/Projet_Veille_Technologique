# 4. Le backend API

## 4.1 API et SQLite
Le backend est fait avec le framework ASP.NET Core. Ce framework permet de créer un serveur web léger et performant, adapté aux capacités matérielles restreintes du Raspberry Pi Zero 2 W.

### 4.1.1 Réception des données
L'API utilise un endpoint HTTP POST (`/api/telemetry`). C'est ceci qui fait en sorte que le script Python envoie ses paquets JSON à haute fréquence. L'API intercepte le payload, extrait les données et y associe un identifiant de session lors du démarage de trajet.

### 4.1.2 Entity Framework Core et SQLite
Pour conserver les données SQLite est encapsulé par Entity Framework Core. 

* **Modèle relationnel :** Une table `Sessions` gère les données des trajets (ID, Nom, Date de début, Date de fin). Une table `Records` stocke les données télémétriques liés à chaque session par une clé étrangère.

---

## 4.2 Architecture des canaux asynchrones et Mode Présentation
L'un des plus grands défis de ce projet était de séparer proprement la collecte physique de l'affichage visuel, en intégrant un système de replay pour les démonstrations.

### 4.2.1 Découpage par System.Threading.Channels (La boîte aux lettres)
Pour éviter que l'application ralentisse ou bloque lorsque les données de l'auto arrivent trop vite, le backend utilise un système de canal mémoire **`System.Threading.Channels`**.

* Dès que le script Python envoie les données à l'API via une requête HTTP, le contrôleur du serveur web dépose les données dans (`uiChannel.Writer`).
* En arrière-plan, un service de fond (.NET BackgroundService) récupère (`uiChannel.Reader`) pour l'envoyer au tableau de bord.

### 4.2.2 Le moteur de simulation (PresentationStreamer)
Pour pouvoir présenter les données récolter il a fallu faire un streameur basé sur une interface `ITelemetryStreamer.cs` :

```csharp
namespace AutoPi.TelemetryApi;

public interface ITelemetryStreamer
{
    bool IsStreaming { get; }
    Task StartAsync(int? sessionId = null);
    Task StopAsync();
}
```

Implémentation de la classe `PresentationStreamer.cs`
Son fonctionnement simule le temps réel :

1. Extraction ciblée : À l'aide du LINQ (`.Select()`), le streamer charge en mémoire seulement les colonnes nécessaires de la table `Records` pour la session demandée, triées chronologiquement.
2. Mise à jour du temps : Le streamer modifie l'heure d'enregistrement d'origine pour y mettre l'heure actuelle (`DateTime.UtcNow`). L'application pense que la donnée vient juste d'être récolter.
3. Contrôle du rythme (10 Hz) : Le script applique une pause de 100 millisecondes (`Task.Delay(100)`) entre chaque ligne de données avant de l'envoyer au (`uiChannel`). Ce qui reproduit le rythme de la conduite.

## 4.3 Cartographie des fichiers du backend et utilité

Voici le rôle et l'utilité de chaque fichier backend :

* **`Program.cs`** : C'est le program principal. Il initialise l'application web, gère l'injection de dépendances, associe le Hub SignalR, configure le rendu des composants Blazor et expose l'API minimale HTTP POST (`/api/telemetry`) qui reçoit les données envoyées par le script Python.
* **`TelemetryDbContext.cs`** : Ce fichier gère le contexte de base de données avec Entity Framework Core. C'est lui qui configure et ouvre la connexion avec le fichier local SQLite et expose les collections d'entités pour interagir avec les tables en C#.
* **`Models.cs`** : Ce fichier regroupe les entités de persistance de la base de données.
* **`TelemetryModel.cs`** : Ce fichier contient la classe `TelemetryData`. Il s'agit du modèle de transfert servant à récevoir  le paquet JSON envoyé par le Pi.
* **`TelemetryProcessor.cs`** : Ce BackgroundService gère le premier canal mémoire. Il surveille le pipeline de réception et pousse chaque nouvelle donnée vers l'écran.
* **`DatabaseProcessor.cs`** : Ce deuxième BackgroundService gère la persistance. Il est asynchrone pour sauvegarder les points dans SQLite sans ralentir l'affichage ni la réception de l'API.
* **`TelemetryHub.cs`** : C'est le Hub SignalR. Il gère le WebSockets avec le navigateurs.
* **`ITelemetryStreamer.cs`** : L'interface de programmation qui définit le contrat du moteur de simulation .
* **`PresentationStreamer.cs`** : L'implémentation de la simulation. Ce fichier extrait un trajet enregistré dans SQLite, pour simuler le direct et injecte les données à 10 Hz dans l'application.
* **`SessionStats.cs`** : Une classe pour calculer et compiler les statistiques globales d'un trajet (valeurs maximales de RPM, vitesse de pointe, moyennes et calcul de distance).
* **`VehicleDictionary.cs`** : Ce fichier sert de dictionnaire de conversion pour le OBD2 afin de connaitre le modèle de la voiture