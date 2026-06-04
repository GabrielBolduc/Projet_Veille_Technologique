# Design - Contrôle Manuel et Persistance Résiliente des Trajets

## 1. Objectif
Permettre à l'utilisateur de piloter manuellement l'enregistrement de la télémétrie (Démarrer/Clôturer un trajet) depuis le Dashboard Blazor. Le trajet doit être résilient aux redémarrages du véhicule (coupures d'alimentation du Raspberry Pi) et accumuler les données sur une longue période (ex: une semaine) tant que l'utilisateur n'a pas cliqué explicitement sur "Terminer".

## 2. Comportement du Backend (`DatabaseProcessor.cs`)
* **Démarrage / Reprise :** Lorsqu'un suivi est actif (`IsTrackingActive = true`), le processeur cherche en base de données s'il existe une `TripSession` avec `EndTime == null`. 
  * Si oui : Il récupère son `Id` et ajoute les nouveaux `Records` à ce trajet existant.
  * Si non : Il crée une nouvelle `TripSession`.
* **Coupure du véhicule :** Si le script Python s'arrête ou que le Pi s'éteint, la session reste ouverte (`EndTime` reste `null`). Au prochain démarrage de la voiture, le traitement reprend de manière transparente sur le même trajet.
* **Clôture manuelle :** Ce n'est que lorsque l'utilisateur clique sur "Terminer le trajet" dans Blazor que le système applique un `EndTime = DateTime.UtcNow`, archivant définitivement le trajet.

## 3. Interface Utilisateur (`Dashboard.razor`)
* **Onglet DIRECT :** 
  * Un bouton principal qui bascule selon l'état : "DÉMARRER LE SUIVI" (vert) / "TERMINER ET ARCHIVER LE TRAJET" (rouge).
* **Onglet HISTORIQUE :**
  * Un tableau minimaliste des trajets clôturés ou en cours.
  * Un bouton "Supprimer" (Corbeille) sur chaque ligne pour effacer un trajet et purger ses données associées (suppression en cascade).

## 4. Liste des Tâches (Tasks)
- [x] Modifier `DatabaseProcessor.cs` pour exposer un état `IsTrackingActive` et la méthode de clôture `CloseCurrentSession()`.
- [x] Adapter la logique d'insertion : utiliser une requête `.FirstOrDefaultAsync(s => s.EndTime == null)` pour réutiliser le trajet en cours au démarrage.
- [x] Ajouter la méthode de suppression de session via Entity Framework.
- [x] Mettre à jour les boutons et l'état visuel dans `Dashboard.razor`.