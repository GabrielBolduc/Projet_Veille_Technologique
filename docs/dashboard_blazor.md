# 5. Dashboard

Il a été développée avec **Blazor Server**, ce qui permet de créer des composants web dynamiques en C# sans avoir à écrire de JavaScript.

Pour que les données de la voitur s'affichent  à l'écran dès qu'elles sont lues par le script Python, Blazor utilise SignalR. 

* **Le fonctionnement :** SignalR ouvre un WebSocke entre le serveur web et le navigateur. 
* **L'utilité :** Dès que le service du backend récupère un paquet de données dans le uiChannel, il le pousse dans SignalR. L'interface graphique reçoit l'information instantanément et se met à jour 10 fois par seconde, sans que l'utilisateur ait besoin de rafraîchir sa page web.

## 5.2 Les composants du tableau de bord (Dashboard)
L'interface graphique est divisée en deux sections principale :

### 5.2.1 L'onglet DIRECT
C'est l'écran principal. Il affiche la télémétrie en direct:

* **Les données principal :** Elles affichent graphiquement le RPM et la vitesse en temps réel.
* **Les Sparklines SVG :** Ce sont de petits graphiques temporels qui se dessinent en direct. Ça permet de voir les données les changement rapide de RMP et vitesse

### 5.2.2 L'onglet HISTORIQUE
Cet onglet communique avec la base de données SQLite pour lister tous les trajets qui ont été enregistrés.

Pour chaque trajet, l'interface affiche les données télémétrique le nom donné par l'utilisateur, la date et la durée.
Il est possible de mettre sur pause un trajet, de le reprendre et de le supprimer. 

De plus, lorsque le trajet est arréter, il y a un bouton afin de lancer le mode streamer. 
