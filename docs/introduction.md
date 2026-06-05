# 1. Introduction

## 1.1 Mise en contexte
Le domaine du diagnostic et de la télémétrie automobile repose principalement sur la norme OBD-II (On-Board Diagnostics). Conçue initialement pour surveiller les émissions polluantes des moteurs thermiques, cette interface donne un accès direct aux données de la voiture. 

Cependant, les outils de diagnostic grand public restent souvent limités et dépendent d'applications mobiles propriétaires remplies de publicités ou d'options payantes, empêchant une exploitation libre et personnalisée des données. C'est pour cette raison que j'ai décidé de concevoir ce projet.

## 1.2 Objectif du projet
Ce projet a pour but de concevoir et déployer une architecture logicielle et matérielle de télémétrie automobile. L'objectif principal est de capturer les données physiques d'un véhicule, de les acheminer vers un serveur local embarqué, et de les consulter sur une interface web réactive.

Pour valider ce que j'imaginais, le projet doit réusir les trois exigences suivante :

1. **L'autonomie absolue :** Le système embarqué doit s'exécuter, se connecter et collecter les données de manière autonomne dès le démarrage du véhicule, sans intervention humaine.
2. **Le découpage des responsabilités :** Séparer la capture bas niveau (Python), la gestion des flux et la persistance (API ASP.NET Core/SQLite), et l'affichage sur un dashboard (Blazor/SignalR).
3. **La reproductibilité :** Avoir un mode permettant de rejouer un trajet dans le but d'analyser les données.
