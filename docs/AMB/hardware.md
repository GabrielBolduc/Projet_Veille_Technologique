# Architecture Matérielle & Configuration

Cette section documente la configuration physique du projet, l'installation du Raspberry Pi en mode headless ainsi que les étapes de connection du Bluetooth avec l'adaptateur OBD-II.

---

## 1. Composants Matériels

Le système embarqué utilise les composants suivants pour assurer la capture et la transmission des données du véhicule :

* **Calculateur Embarqué :** Raspberry Pi Zero 2 W (choisi pour sa compacité et sa faible consommation électrique).
* **Interface OBD-II :** Adaptateur Bluetooth **Veepeak OBDCheck VP11** (basé sur une puce ELM327 v1.5).
* **Alimentation :** Adaptateur d'alimentation allume-cigare 12V

---

## 2. Configuration du Raspberry Pi (Mode Headless)
Pour configurer le Pi sans y brancher de moniteur ni de clavier, j’utilise le mode Headless. Il faut donc configurer le Pi avec Raspberry Pi Imager et flasher une carte micro sd


## 3. Configuration de l'OBD2

L'adaptateur doit être jumelé avec le Pi afin de créer une passerelle de communication avec la voiture.

### Étape 3.1 : Débloquer et lancer l'utilitaire Bluetooth

Par default le Bluetooth est bloquée par le système au démarrage, il faut exécuter ces commandes :

```bash
sudo rfkill unblock bluetooth
sudo systemctl restart bluetooth
```

Accéder ensuite au commande de gestion Bluetooth :

```bash
sudo bluetoothctl
```

### Étape 3.2 : Connection Bluetooth

Dans le terminal `bluetoothctl`, pour connecter le bluetooth :

```
power on
agent on
default-agent
scan on
```

Lorsque l'adresse MAC est identifiée, on peut faire le jumelage

```
pair 66:1E:87:05:BD:41
trust 66:1E:87:05:BD:41
exit
```

> Le mot de passe par défaut pour valider la connection est `1234`.

### Étape 3.3 : Liaison au port série virtuel avec RFCOMM

Pour permettre aux scripts de lire les données, l'adresse Bluetooth est liée au périphérique virtuel `/dev/rfcomm0` :

```bash
sudo rfcomm bind rfcomm0 66:1E:87:05:BD:41 1
```

Pour valider la création du fichier :

```bash
ls -l /dev/rfcomm0
```

Sortie attendue :

```
crw-rw---- 1 root dialout ... /dev/rfcomm0
```

---

## 4. Environnement Logiciel de Collecte (Python)

Pour lire les données, le Pi doit exécuter un script Python qui communique avec l’adapteur avec la library python-obd.

**Installation des dépendances sur le Pi :**

```bash
sudo apt update
sudo apt install python3-pip python3-serial -y
pip3 install obd --break-system-packages
```
