# Design - API d'Ingestion de Télémétrie (.NET 9)

## 1. Objectif
Créer une API Web minimale (Minimal API) avec ASP.NET Core 9 dans le dossier `src-backend/`. Cette API doit exposer un point de terminaison HTTP POST pour recevoir la télémétrie du script Python, la valider par rapport au contrat global `system.json`, et retourner une réponse immédiate.

## 2. Spécifications Techniques
* **Framework :** .NET 9.0 (ASP.NET Core)
* **Endpoint :** `POST /api/telemetry`
* **Architecture de Performance :** Utilisation d'un `System.Threading.Channels.Channel<T>` en mémoire (pattern Producteur/Consommateur). Dès qu'un JSON arrive, l'endpoint le pousse dans le Channel et répond instantanément `202 Accepted` au script Python pour ne pas bloquer la collecte à 10 Hz. Un `BackgroundService` asynchrone consomme le Channel en arrière-plan.

## 3. Modèles de Données (Records C#)
Création de structures immuables mappant exactement `openspec/specs/system.json` :
* `TelemetryPayload` (DeviceId, Timestamp, Metrics, Diagnostics)
* `TelemetryMetrics` (EngineRpm, VehicleSpeed, ThrottlePosition, EngineLoad, CoolantTemperature)
* `TelemetryDiagnostics` (DtcPresent, DtcCodes)

## 4. Liste des Tâches (Tasks)
- [x] Initialiser le projet Web .NET 9 dans `src-backend/` via `dotnet new web`.
- [x] Créer les modèles de données C# (`Models.cs`) alignés sur le contrat JSON.
- [x] Implémenter le processeur d'arrière-plan (`TelemetryProcessor.cs`) héritant de `BackgroundService`.
- [x] Configurer le `Channel` et l'endpoint d'ingestion dans `Program.cs`.