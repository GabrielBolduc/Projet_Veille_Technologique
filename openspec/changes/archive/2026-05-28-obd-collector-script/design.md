# Design - Script de Collecte Télémétrie OBD-II

## 1. Objectif
Développer le script Python autonome (`obd_collector.py`) destiné à s'exécuter sur le Raspberry Pi Zero 2 W. Ce script doit interroger le bus CAN via l'adaptateur Bluetooth Veepeak, extraire les PIDs spécifiques à des fréquences distinctes, et pousser les payloads JSON vers l'API locale.

## 2. Spécifications Techniques

### Cadencement des requêtes (Frequencies)
* **Haute Fréquence (10 Hz / 100ms) :** * Régime Moteur (Engine RPM)
  * Vitesse du Véhicule (Vehicle Speed)
  * Position de la pédale d'accélérateur (Throttle Position)
* **Basse Fréquence (0.1 Hz / 10s) :**
  * Température du liquide de refroidissement (Coolant Temperature)
  * Charge Moteur (Engine Load)

### Gestion du mode Hors-ligne (Fallback)
Puisque le backend ASP.NET s'exécute sur le même appareil, si l'API ne répond pas temporairement (code d'erreur HTTP ou timeout), le script doit intercepter l'exception, lever une alerte dans la console, et continuer sa boucle sans planter.

## 3. Architecture du Code
Le script sera structuré dans `src-python/obd_collector.py` et utilisera la bibliothèque `python-obd` pour l'abstraction des commandes AT et des PIDs. Les requêtes vers l'API locale utiliseront la bibliothèque `requests`.

## 4. Liste des Tâches (Tasks)
- [x] Créer le script `src-python/obd_collector.py` avec la structure de boucle principale.
- [x] Implémenter la connexion au port série Bluetooth (`/dev/rfcomm0`).
- [x] Configurer les boucles temporelles asynchrones ou cadencées (`time.time()`) pour respecter les fréquences de 10 Hz et 10 secondes.
- [x] Modéliser le dictionnaire JSON selon le format OpenSpec validé.
- [x] Ajouter la logique d'envoi HTTP `POST` vers `http://localhost:5000/api/telemetry` avec gestion des erreurs (try/except).