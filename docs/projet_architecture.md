# 2. Explication globale du projet et architecture

## 2.1 Vue d'ensemble et fonctionnement général
Le projet AutoPi est un système de télémétrie embarqué qui permet de capter et afficher les données d'un véhicule en temps réel. Le fonctionnement est le suivant:

1. Le véhicule roule et génère des données.
2. L'adaptateur OBD2 capte ces données par Bluetooth.
3. Le Raspberry Pi Zero 2 W recueille ces informations, les traite, les stocke et les envoie vers l'interface.
4. L'utilisateur consulte ses statistiques sur un tableau de bord.

L'architecture a été séparée en trois couches logicielles distinctes qui communiquent ensemble de manière transparente.

## 2.2 L'architecture technique en trois couches

### 2.2.1 La couche d'acquisition (Le script Python)
Cette couche s'occupe de la communication physique avec la voiture. Un script Python tourne en boucle sur le Raspberry Pi. Son rôle est de demander des informations précises à l'adaptateur OBD2(RPM, vitesse ou la position de la pédale), de recevoir les réponses et de les conserver en format JSON. Ensuite, le script envoie les données via une requête HTTP POST.

### 2.2.2 La couche de traitement et de persistance (Le Backend ASP.NET Core)
C'est le system central, API Web ASP.NET Core qui tourne sur le Raspberry Pi. Ce backend reçoit les paquets JSON envoyés par le script Python. 
Son utilité :

* **La persistance :** Il valide les données et les enregistre de façon permanente dans une base de données SQLite, triées par trajets (sessions).
* **La diffusion :** Il pousse ces données vers un canal de communication en temps réel (SignalR Hub) pour qu'elles soient disponibles instantanément sur le réseau.

### 2.2.3 La couche de présentation (Le Dashboard)
Cette couche est l'interface visuelle avec laquelle l'utilisateur interagit. Développé en Blazor, cette interface se connecte au Hub SignalR du backend. Lorsqu'une nouvelle donnée est détectée, l'interface se met à jour sans que l'utilisateur ait besoin de rafraîchir la page. Elle permet aussi de consulter l'historique des trajets enregistrer.

---

## 2.3 Étape pour reproduire 
Pour pouvoir utiliser le projet, voici la marche à suivre exacte :

1. **Brancher le matériel :** Insérer l'adaptateur dans la prise OBD2 de la voiture.
2. **Démarrer le Raspberry Pi :** Brancher le Pi. Le Pi est configuré pour connecter à un réseau WiFi automatiquement.
3. **Lancer le Backend :** Le serveur ASP.NET Core doit être démarré en premier sur le Pi afin d'ouvrir le port d'écoute (port 5000) et d'initialiser la base de données SQLite.
4. **Activer la collecte Python :** Le script Python se connecte au Bluetooth de l'adapteur via un port série virtuel (`/dev/rfcomm0`). Dès que le lien est fait, il commence à envoyer la télémétrie au backend.
5. **Consulter le Dashboard :** Avec un téléphone ou un ordinateur connecté au même réseau WiFi que le Pi, accéder au dashboard web.