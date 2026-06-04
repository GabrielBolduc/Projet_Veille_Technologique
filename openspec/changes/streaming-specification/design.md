# Spécification Technique : Flux de Télémétrie (Mode Présentation)

## Objectif
Permettre la démonstration du projet AutoPI en simulant un flux de données en direct (10 Hz) à partir des données réelles enregistrées dans la base de données SQLite (`autopi.db`). Avec un interface

## Architecture du Streamer
1. **Interface :** `ITelemetryStreamer`
   - Méthode : `IAsyncEnumerable<TelemetryPayload> StreamDataAsync(CancellationToken cancellationToken);`
2. **Implémentation :** `FileTelemetryStreamer`
   - Dépendance : Injecter `IDbContextFactory<TelemetryDbContext>`.
   - Comportement : 
     - Récupérer tous les enregistrements de la base de données ordonnés par leur `Timestamp`.
     - Boucler à l'infini (`while`) sur le jeu de données pour que la démo tourne en boucle.
     - Utiliser `yield return` pour pousser chaque élément dans le flux.
     - Appliquer un `Task.Delay(100)` entre chaque élément pour simuler la fréquence de 10 Hz du Raspberry Pi.
     - Modifier le `Timestamp` à la volée avec le mot-clé `with` pour y mettre l'heure actuelle au format ISO 8601 (`DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")`).

## Injection de Dépendances
Dans `Program.cs`, enregistrer le service en tant que Singleton :
`builder.Services.AddSingleton<ITelemetryStreamer, FileTelemetryStreamer>();`