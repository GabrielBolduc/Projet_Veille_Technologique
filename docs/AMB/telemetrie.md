# Logique d'Acquisition de la Télémétrie

Cette section détaille l'architecture logicielle du script de collecte de données exécuté sur le Raspberry Pi Zero 2 W.

---

## 1. Télémétrie collecter

Pour assurer un suivi précis du comportement du véhicule sans saturer le Pi, je me limite à 6 données. La bibliothèque `python-obd` mappe ces paramètres sur des commandes standardisées :

| Métrique | Commande `python-obd` | Fréquence cible | Utilité technique |
| :--- | :--- | :--- | :--- |
| **Régime Moteur** | `obd.commands.RPM` | Haute (~10Hz) | Analyse de la sollicitation moteur et des passages de rapports. |
| **Vitesse** | `obd.commands.SPEED` | Haute (~10Hz) | Calcul des distances, de l'efficience et corrélations dynamiques. |
| **Charge Moteur** | `obd.commands.ENGINE_LOAD` | Moyenne (~2Hz) | Mesure de la puissance demandée relative à la capacité maximale. |
| **Température Reliquat** | `obd.commands.COOLANT_TEMP`| Basse (~0.1Hz) | Surveillance thermique et validation de la température optimale. |
| **Position Accélérateur**| `obd.commands.THROTTLE_POS`| Haute (~10Hz) | Capture instantanée de l'intention du conducteur. |
| **Codes d'Erreur (DTC)** | `obd.commands.GET_DTC` | Basse (Au démarrage) | Diagnostic à distance et alertes de maintenance préventive. |

---


