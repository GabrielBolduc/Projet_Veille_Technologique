# 3. Le collecteur embarqué (Raspberry Pi & Python)

## 3.1 Configuration matérielle et système du Pi

Le Raspberry Pi Zero 2 W a été configuré avec la distribution Raspberry Pi OS Lite, 64-bit, sans interface graphique.

Deux configurations systèmes ont été faites afin de rendre le Pi autonome :

### 3.1.1 Configuration Réseau

Le Pi a été configuré pour se connecter automatiquement au partage de connexion de mon téléphone dès le démarrage. Ça permet au Pi de se connecter facilement au Wi-Fi et permet d'accéder au tableau de bord en local facilement directement depuis la voiture.

### 3.1.2 Liaison Bluetooth et gestion des permissions

L'adaptateur OBD2 utilise le protocole Bluetooth classique pour simuler un port série. Sous Linux, pour que le script Python puisse interagir avec lui, il a fallu créer un pont à l'aide de l'utilitaire `rfcomm`.

---

## 3.2 Analyse du script Python et ajustement du protocole


### 3.2.1 Architecture logicielle du script

Le script utilise la librairie open-source `python-obd`. 
Le script est structuré autour d'une boucle qui gère trois tâches principales : la connexion, la lecture et l'expédition.

1. **La phase de connexion `connect()` :** Le script tente d'ouvrir le port virtuel :

    ```python
       connection = obd.OBD("/dev/rfcomm0", baudrate=38400, fast=False)
    ```

2. **La boucle de collecte  :** Une fois le lien établi , le script communique avec les capteurs à l'aide des commandes de la librairie

3. **La sérialisation et l'envoi HTTP :** Les valeurs brutes retournées par l'auto sont converties en types de données standards (nombres à virgule flottante ou entiers) pour éliminer les unités de mesure physiques de la librairie. Le script assemble ensuite un dictionnaire Python, le sérialise en texte JSON, et l'envoie au backend avec une requête `requests.post()`.

### 3.2.2 Structure des données (Payload JSON)

Pour que le backend puisse intercepter et valider la donnée, le script Python standardise la structure :

```json
{
  "rpm": 2450,
  "speed": 62.5,
  "throttlePosition": 34.2,
  "timestamp": "2026-06-04T16:21:15.123Z"
}
```