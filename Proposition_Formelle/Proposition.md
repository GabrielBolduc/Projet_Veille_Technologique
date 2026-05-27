# Gabriel Bolduc

## Exploration des idées

### 1- API d’inventaire et calcul de coûts pour imprimante 3D
* **Description :** Un système backend qui gère virtuellement les bobines de filament (PLA, PETG, etc.) pour mon imprimante 3D. Il calcule le coût d’une impression selon le poids et déduit automatiquement la quantité de filaments utilisée dans mon inventaire. 
* **Faisabilité en 7 jours :** Je crois que la faisabilité en 7 jours de ce projet est élevée. Les contraintes sont surtout mathématiques et c’est quelque chose que l’IA gère très bien. Rendre le tout agréable et utile est le plus grand défi. 
* **Intérêt personnel :** Ce projet est réellement utile pour moi, car j’ai une imprimante 3D et je trouve que la gestion du filament est parfois une tache plate. Manquer de filament pendant une impression est une perte de temps, donc posséder un inventaire virtuel est pratique. 
 
### 2- API d’analyse d’entrainement et d’activité physique
* **Description :** Un serveur qui gère des données d’entrainement et d’activité physique quotidienne (temps de vélo, fréquence cardiaque, musculation). L’API applique des algorithmes afin de créer des alertes de progression ou de surentrainement.
* **Faisabilité en 7 jours :** Je crois que la faisabilité de ce projet en 7 jours est totalement réalisable. Ce sont majoritairement des calculs d’agrégation (sommes des charges soulevées en musculation, moyenne cardiaque) et des validations (alertes de validation si amélioration constater par l’algorithme). 
* **Intérêt personnel :** Puisque je fais de plus en plus de sport, combiner mon intérêt pas l’activité physique avec le développement serait très intéressent et plus d’être pratique au quotidien.
 
### 3- API de télémétrie automobile (Data Logger)
* **Description :** Un serveur conçu pour recevoir les données de ma voiture (RPM, température, vitesse) envoyées par un Raspberry Pi branché dans la voiture. L’API trie les données reçues depuis le port OBD2 et créées des alertes en cas de problèmes.
* **Faisabilité en 7 jours :** Je crois que ce projet est un réel défi, mais je crois que c’est totalement réalisable. La réception des données et la création d’alertes se font très bien en instruction textuelles pour l’IA, et le script sur le Raspberry Pi est relativement facile à faire.
* **Intérêt personnel :** Ce projet combine ma passion pour l’automobile et la programmation embarquée. De plus, ça fait longtemps que je souhaite faire un projet avec un Rasberry PI. De plus, ce projet est réellement utile pour moi, car ça va me permettre de comprend plus facilement quelles sont les problèmes sur ma voiture.  
 
## Choix du projet 

J’ai décider de choisir le projet d’API de télémétrie automobile pour plusieurs raison.
* **Pourquoi ce choix :** Ce projet regroupe beaucoup de sujets qui me passionnent. J’ai toujours aimé la programmation embarquée et j’ai adoré travailler avec un Arduino. De plus, j’ai longtemps voulu découvrir Raspberry Pi. Ce projet est l’occasion parfaite de découvrir concrètement le potentiel d’un Rasberry pi Zero 2 W. D’une autre part, l’automobile et la mécanique est une autre de mes passions, donc faire un projet qui permet de concevoir un système de télémétrie pour voiture est la combinaison de mes 2 principales passions. 
* **Défis anticipés :** Selon moi, le plus grand défi auquel de crois devoir faire face sera de rédiger des spécifications assez précises pour que l’IA sache comment analyser les data reçues (ex : déclencher une alerte si la température du liquide de refroidissement est au-dessus de X degré). De plus la configuration du Raspberry Pi avec le OBD2 Bluetooth me fait peur, car je n’ai jamais vraiment utilisé de Raspberry Pi avant, mais il existe plusieurs tutoriels sur internet et c’est un défi que je souhaite relever.
* **Ressources nécessaires :** Un outil SDD (OpenSpec ou SpecKit), Postman pour faire des requêtes API (ou autres outils de test API), un Raspberry Pi, un adapteur OBD2 Bluetooth et mon véhicule. 
 
## Proposition formelle
### AutoPI
### Introduction
L’intelligence artificielle transforme le développement logiciel ainsi que la façon dont nous apprenons la programmation. La rédaction manuelle du code semble progressivement se transformer vers l’architecture par IA. C’est pourquoi je souhaite explorer le volet développement en découvrant pour la première fois le Specification Driven Development. L’objectif de mon projet est de démontrer comment l’IA peut permettre de générer un gérer une infrastructure backend dédier à l’IoT.
 
### Recherche
Avant de sélectionner ce projet, deux autres options ont été explorées. Premièrement, j’ai imaginé un projet pour mon imprimante 3D. Un système de gestion d’inventaire virtuel pour l’impression 3D, calculant automatiquement les coûts d’impression et l’utilisation du filament. 
Ensuite, puisque j’ai récemment commencé à faire plus de sport, j’ai imaginé une API capable d’analyser des données d’entrainement physique au quotidien et de déclencher des alertes de progression et de surentrainement.
Finalement, j’ai décidé de faire un projet de télémétrie automobile. Ce projet a été sélectionné pour plusieurs raisons. En premier lieu, ce projet va m’être réellement utilisé, puisque posséder des data de ma voiture vas me permettre de pouvoir régler plus facilement les problèmes mécaniques que je peux potentiellement rencontrer. De plus, ce projet est l’occasion idéale de découvrir un Raspberry Pi en l’utilisant en tant que data logger. Finalement, relier l’API à un vrai véhicule est le meilleur moyen de tester si la solution généré par l’IA résiste à des données réelles et imparfaites. 

 
### Objectifs
* **Expérimenter le SDD :** Faire générer une API complète par l’IA en utilisant uniquement des instructions précises. 
* **Tester la fiabilité de l’IA :** Vérifier si l’IA est capable d’appliquer de vraies règles, comme filtrer les mauvaises données et créer des alertes quand un capteur dépasse certaines limites établies. 
* **Connecter le matériel au logiciel :** Réussir à faire communiquer le Raspberry Pi de façon stable avec le serveur. 

### MVP 
Pour que le projet soit considéré comme un succès en 7 jours, il faudra que l’API et le client physique remplissent les critères suivants : 
* **Réception des données :** L’API doit être capable de recevoir et d’enregistrer les informations de la voiture (ex : RPM, température, vitesse, throttle position, etc).
* **Création d’alertes :** Le système doit créer une alerte automatiquement si une donnée dépasse une limite établie (ex : RMP trop élevé).
* **Le capteur physique (Raspberry Pi) :** Un script doit réussir à lire les vraies données de la voiture via la prise OBD2 et à les envoyer au serveur.
* **Condition de réussite :** Réussir une démonstration complète où les données du véhicule sont envoyées et traitées avec succès par le serveur.
 
### Méthodologie
Mes 7 jours de travailles seront structuré en 4 blocs
* **Bloc 1 (Jour 1 et 2) :** Déterminer exactement quelles données seront utilisées et rédiger les instructions claires pour l’IA.
* **Bloc 2 (Jour 3 et 4) :** Faire l’API, et ensuite utiliser Postman pour envoyer de fausses données au serveur et vérifier s’il crée les alertes correctement.
* **Bloc 3 (Jour 5) :** Préparer le Pi, le connecter au lecteur OBD2 Bluetooth et faire le script Python qui va envoyer les données de la voiture au serveur.
* **Bloc 4 (Jour 6 et 7) :** Faire un vrai trajet en voiture pour tester tout le système. Faire les dernières corrections et ensuite rédiger le rapport final.
 
### Outils et technologie
* **Outil IA:** OpenSpec (Ou SpecKit)
* **Le Backend :** Une API et une base de données, selon le fonctionnement de l’IA
* **Le matériel :** Un Raspberry Pi Zero 2 W et un adapteur Bluetooth OBD2
* **Logiciel embarqué :** Un script Python qui tournera sur le PI afin d’envoyer les données.
 
### Résultats attendus
* **Ce que je vais démontrer :** Je veux démontrer qu’il est possible de créer un API complet pour un projet connecté simplement en donnant des instructions claires et précise à l’IA. Ce projet va démontrer que le développement évolue et qu’il est important, en temps de développeur, de suivre l’évolution du métier.
* **Comment je vais valider mon projet :** Valider mon projet sera plutôt simple, car il se test en conditions réelles. Pour moi, ce sera une réussite si le PI réussit à bien envoyer les vraies data de ma voiture et que l’API crée les alertes (ex : RPM trop élever). 
 
### Utilisation de l’IA dans mon projet
Pour ce travail, puisque l’utilisation de l’IA est obligatoire, j’ai utilisé Gemini Pro pour le côté technique (Spécificité technique pour la réalisation du projet, explication du SDD, explication de ce qu’est OpenSpec). J’ai également souhaité découvrir Perplexity dans le but de m’aider à rédiger la proposition formelle. Perplexity m’a aidé à rédiger en me donnant une structure et des idées, mais j’ai tout fait moi-même à 90%.  
 
Voici les prompts utilisées :
1. Qu’est-ce que le Specification Driven Development.
2. Explique-moi ce qu’est l’outil OpenSpec et à quoi peut-il servir
3. OpenSpec vs SpecKit
4. Je souhaite réaliser un data logger pour ma voiture à l’aide d’un Raspberry Pi Zero 2 et un adapteur OBD2 Bluetooth, de quoi j’ai besoin pour y parvenir.
5. Est-ce que je devrais utiliser le PI en mode remote SSH ou en mode ordinateur via l’os de Rasberry PI. 
6. Donne-moi des idées de projet informatique en lien avec le sport. 
7. Comment réaliser une proposition formelle
