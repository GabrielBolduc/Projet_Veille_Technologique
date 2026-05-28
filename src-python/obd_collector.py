import obd
import requests
import time
from datetime import datetime, timezone

API_URL = "http://localhost:5000/api/telemetry"
PORT = "/dev/rfcomm0"

HIGH_FREQ_INTERVAL = 0.1  # 10 Hz
LOW_FREQ_INTERVAL = 10.0  # 0.1 Hz


def _val(response):
    """Extrait la valeur numérique d'une réponse OBD, ou None si vide."""
    if response.is_null():
        return None
    return response.value.magnitude


def connect():
    connection = obd.OBD(PORT)
    return connection


def query_high_freq(conn):
    return {
        "rpm": _val(conn.query(obd.commands.RPM)),
        "speed_kmh": _val(conn.query(obd.commands.SPEED)),
        "throttle_pos_pct": _val(conn.query(obd.commands.THROTTLE_POS)),
    }


def query_low_freq(conn):
    return {
        "coolant_temp_c": _val(conn.query(obd.commands.COOLANT_TEMP)),
        "engine_load_pct": _val(conn.query(obd.commands.ENGINE_LOAD)),
    }


def build_payload(high_data, low_data):
    iso_timestamp = datetime.now(timezone.utc).strftime('%Y-%m-%dT%H:%M:%SZ')
    return {
        "device_id": "Volvo-S60-T5",
        "timestamp": iso_timestamp,
        "metrics": {
            "engine_rpm": int(high_data["rpm"]) if high_data["rpm"] is not None else None,
            "vehicle_speed": int(high_data["speed_kmh"]) if high_data["speed_kmh"] is not None else None,
            "throttle_position": high_data["throttle_pos_pct"],
            "engine_load": low_data["engine_load_pct"],
            "coolant_temperature": low_data["coolant_temp_c"]
        },
        "diagnostics": {
            "dtc_present": False,
            "dtc_codes": []
        }
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
        print(f"[ERROR] Impossible de se connecter à l'adaptateur OBD-II sur {PORT}")
        return

    print(f"[INFO] Connecté à {PORT}. Démarrage de la collecte...")

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
