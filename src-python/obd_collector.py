import math
import random
import time
from datetime import datetime, timezone
import requests

SIMULATION = False # Mode Simulation 
API_URL = "http://127.0.0.1:5000/api/telemetry"
PORT = "/dev/rfcomm0"
HIGH_FREQ_INTERVAL = 0.1  # 10 Hz
LOW_FREQ_INTERVAL = 10.0  # 0.1 Hz

if not SIMULATION:
    import obd


def _val(response):
    """Extrait la valeur numérique d'une réponse OBD, ou None si vide."""
    if response.is_null():
        return None
    return response.value.magnitude

CURRENT_DEVICE_ID = "Volvo-S60-T5" 
def connect():
    global CURRENT_DEVICE_ID
    if SIMULATION:
        print("[INFO] [MODE SIMULATION] Connexion virtuelle établie.")
        CURRENT_DEVICE_ID += "Simulateur"
        # On retourne un objet factice qui possède la méthode is_connected()
        class MockConn:
            def is_connected(self):
                return True
        return MockConn()

    print(f"[INFO] Tentative de connexion à l'adaptateur sur {PORT}...")
    connection = obd.OBD(PORT)
    if connection.is_connected():
        print("Connecter a la voiture")
        response = connection.query(obd.commands.VIN)
        if response and not response.is_null():
            raw_vin = str(response.value).strip()
            if raw_vin:
                CURRENT_DEVICE_ID = raw_vin
                print(f"VIN détecté : {CURRENT_DEVICE_ID}")
        else:
            print("VIN non detecter")
            CURRENT_DEVICE_ID = "Volvo-S60-T5"
    return connection


# Variables globales pour générer une simulation fluide (ondes sinusoïdales)
_sim_tick = 0


def query_high_freq(conn):
    global _sim_tick
    if SIMULATION:
        _sim_tick += 0.05
        # courbe 
        base_wave = (math.sin(_sim_tick) + 1) / 2  # Entre 0 et 1

        # monter les rpm pour simuler une accélération progressive
        rpm = 800 + (base_wave * 5400) + random.randint(-50, 50)
        # La vitesse suit logiquement le régime
        speed = base_wave * 130 + random.randint(-1, 1)
        # Le throttle simule des coups de gaz
        throttle = base_wave * 100

        return {
            "rpm": max(800, rpm),
            "speed_kmh": max(0, speed),
            "throttle_pos_pct": max(0, min(100, throttle)),
        }

    return {
        "rpm": _val(conn.query(obd.commands.RPM)),
        "speed_kmh": _val(conn.query(obd.commands.SPEED)),
        "throttle_pos_pct": _val(conn.query(obd.commands.THROTTLE_POS)),
    }


def query_low_freq(conn):
    if SIMULATION:
        # Simule un moteur chaud stable et une charge variable
        return {
            "coolant_temp_c": 92 + random.randint(-1, 1),
            "engine_load_pct": random.uniform(15.0, 85.0),
        }

    return {
        "coolant_temp_c": _val(conn.query(obd.commands.COOLANT_TEMP)),
        "engine_load_pct": _val(conn.query(obd.commands.ENGINE_LOAD)),
    }


def build_payload(high_data, low_data):
    iso_timestamp = datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")

    dtc_present = False
    dtc_codes = []
    # if random.random() > 0.85:
    #     dtc_present = True
    #     dtc_codes = ["P0300", "P0101"]

    return {
        "device_id": CURRENT_DEVICE_ID,
        "timestamp": iso_timestamp,
        "metrics": {
            "engine_rpm": (
                int(high_data["rpm"]) if high_data["rpm"] is not None else None
            ),
            "vehicle_speed": (
                int(high_data["speed_kmh"])
                if high_data["speed_kmh"] is not None
                else None
            ),
            "throttle_position": high_data["throttle_pos_pct"],
            "engine_load": low_data["engine_load_pct"],
            "coolant_temperature": low_data["coolant_temp_c"],
        },
        "diagnostics": {"dtc_present": dtc_present, "dtc_codes": dtc_codes},
    }


def send_payload(payload):
    try:
        response = requests.post(API_URL, json=payload, timeout=2)
        response.raise_for_status()
    except Exception as e:
        print(f"[WARN] Envoi échoué vers {API_URL} : {e}")


def main():
    conn = connect()
    if not conn.is_connected():
        print(
            f"[ERROR] Impossible de se connecter à l'adaptateur OBD-II sur {PORT}"
        )
        return

    print(
        f"[INFO] Connecté ({'SIMULATION' if SIMULATION else PORT}). Démarrage..."
    )

    low_freq_cache = {"coolant_temp_c": None, "engine_load_pct": None}
    last_low_freq_time = 0.0

    while True:
        loop_start = time.time()

        if loop_start - last_low_freq_time >= LOW_FREQ_INTERVAL:
            low_freq_cache = query_low_freq(conn)
            last_low_freq_time = loop_start

        high_data = query_high_freq(conn)
        payload = build_payload(high_data, low_freq_cache)
        send_payload(payload)

        elapsed = time.time() - loop_start
        time.sleep(max(0.0, HIGH_FREQ_INTERVAL - elapsed))


if __name__ == "__main__":
    main()