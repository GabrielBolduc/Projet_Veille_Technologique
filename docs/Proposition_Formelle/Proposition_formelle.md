## AutoPi 

J’ai décider de choisir le projet d’API de télémétrie automobile pour plusieurs raison.
* **Pourquoi ce choix :** Ce projet regroupe beaucoup de sujets qui me passionnent. J’ai toujours aimé la programmation embarquée et j’ai adoré travailler avec un Arduino. De plus, j’ai longtemps voulu découvrir Raspberry Pi. Ce projet est l’occasion parfaite de découvrir concrètement le potentiel d’un Rasberry pi Zero 2 W. D’une autre part, l’automobile et la mécanique est une autre de mes passions, donc faire un projet qui permet de concevoir un système de télémétrie pour voiture est la combinaison de mes 2 principales passions. 
* **Défis anticipés :** Selon moi, le plus grand défi auquel de crois devoir faire face sera de rédiger des spécifications assez précises pour que l’IA sache comment analyser les data reçues (ex : déclencher une alerte si la température du liquide de refroidissement est au-dessus de X degré). De plus la configuration du Raspberry Pi avec le OBD2 Bluetooth me fait peur, car je n’ai jamais vraiment utilisé de Raspberry Pi avant, mais il existe plusieurs tutoriels sur internet et c’est un défi que je souhaite relever.
* **Ressources nécessaires :** Un outil SDD (OpenSpec ou SpecKit), Postman pour faire des requêtes API (ou autres outils de test API), un Raspberry Pi, un adapteur OBD2 Bluetooth et mon véhicule. 

### Introduction
L’intelligence artificielle transforme le développement logiciel ainsi que la façon dont nous apprenons la programmation. La rédaction manuelle du code semble progressivement se transformer vers l’architecture par IA. C’est pourquoi je souhaite explorer le volet développement en découvrant pour la première fois le Specification Driven Development. L’objectif de mon projet est de démontrer comment l’IA peut permettre de générer un gérer une infrastructure backend dédier à l’IoT.
 
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
