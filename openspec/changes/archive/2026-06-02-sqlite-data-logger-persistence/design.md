# Design - Système de Persistance et Data Logging SQLite

## 1. Objectif
Mettre en place un stockage local asynchrone et résilient pour enregistrer chaque session de conduite (Trip) et ses données de télémétrie associées à 10 Hz sur le Raspberry Pi, sans impacter les performances du tableau de bord en temps réel.

## 2. Architecture et Contraintes
* **Emplacement & Namespaces :** Tous les composants (`TelemetryModel.cs`, `TelemetryDbContext.cs`, `SessionStats.cs`, `DatabaseProcessor.cs`) doivent résider à la racine de `src-backend` et partager le namespace unique `AutoPi.TelemetryApi`.
* **Chemin Absolu :** Le fichier `autopi.db` doit être forcé à la racine du code source via `builder.Environment.ContentRootPath` pour éviter sa création dans le dossier de build `/bin`.
* **Pipeline Asynchrone :** Le canal (Channel) dans `Program.cs` doit être configuré en multi-lecteurs (`SingleReader = false`) pour alimenter simultanément le relais SignalR et le processeur de base de données.

## 3. Comportement du DatabaseProcessor (Règles d'Ingénierie)
* **Initialisation de Session :** Crée une nouvelle entrée dans la table `Sessions` dès que `EngineRpm > 500` et qu'aucune session n'est active.
* **Écriture par Lots (Batching) :** Accumule les messages en mémoire et exécute une écriture groupée dans SQLite toutes les 3 secondes ou tous les 30 enregistrements pour protéger la carte SD du Pi contre l'usure prématurée.
* **Fermeture de Session :** Clôture la session en cours avec un `EndTime` dès que le `EngineRpm` retombe et reste à 0.

## 4. Liste des Tâches (Tasks)
- [x] Installer et valider les packages NuGet `Microsoft.EntityFrameworkCore.Sqlite` et `Microsoft.EntityFrameworkCore.Design`.
- [x] Harmoniser les structures de données entre le modèle d'ingestion (Payload JSON) et les entités EF Core (`TelemetryRecord`) pour éliminer tout conflit de mapping.
- [x] Intégrer un bloc de diagnostic `try/catch` explicite autour de `db.Database.EnsureCreated()` dans `Program.cs`.
- [x] Valider la création physique du fichier `autopi.db` à la racine de `src-backend` et confirmer l'apparition des logs `[DB]` lors de la réception des trames du simulateur Python.