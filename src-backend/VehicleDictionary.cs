using System;
using System.Collections.Generic;

namespace AutoPi.TelemetryApi;

public static class VehicleDictionary
{
    private static readonly Dictionary<string, string> CanadianRegistry = new(StringComparer.OrdinalIgnoreCase)
    {
        { "1FT", "Ford F-Series (F-150/F-250)" },
        { "2FT", "Ford F-Series (Généré au Canada)" },
        { "1GC", "Chevrolet Silverado / GMC Sierra" },
        { "2G1", "Chevrolet Equinox" },
        { "1C4", "Ram Pickup / Jeep Cherokee" },
        { "2C4", "Chrysler / Dodge Caravan" },
        { "JTM", "Toyota RAV4 / Highlander" },
        { "2T3", "Toyota RAV4 (Fabriqué au Canada)" },
        { "5YJ", "Tesla Model 3 / Model Y" },
        { "7SA", "Tesla Model X / Model Y" },
        { "JHM", "Honda Civic / Accord" },
        { "1HG", "Honda Civic (USA)" },
        { "2HK", "Honda Civic / CR-V (Canada)" },
        { "JTD", "Toyota Corolla / Prius" },
        { "1NX", "Toyota Corolla (USA)" },
        { "KMH", "Hyundai Elantra / Sonata" },
        { "KNA", "Kia Forte / Sportage" },
        { "JN1", "Nissan Rogue / Sentra" },
        { "1N4", "Nissan Altima (USA)" },
        { "JM1", "Mazda 3 / CX-5" },
        { "JA3", "Mitsubishi Outlander" },
        { "JF1", "Subaru Impreza / Outback" },
        { "1FA", "Ford Focus / Mustang" },
        { "1F6", "Ford Explorer" },
        { "2FM", "Ford Edge" },
        { "1G1", "Chevrolet Cruze / Malibu" },
        { "1G6", "Cadillac" },
        { "3G1", "Chevrolet Trax" },
        { "2C3", "Chrysler 300 / Dodge Charger" },
        { "YV1", "Volvo S60 / V60 / XC60" }, 
        { "WVW", "Volkswagen Jetta / Golf" },
        { "WVG", "Volkswagen Tiguan" },
        { "WBA", "BMW Série 3 / X5" },
        { "WBS", "BMW M Series" },
        { "W1K", "Mercedes-Benz Classe C / GLC" },
        { "WAU", "Audi A4 / Q5" },
        { "W0L", "Opel / Buick" },
        { "ZHW", "Lamborghini" },
        { "4T1", "Toyota Camry (USA)" },
        { "5N1", "Nissan Pathfinder / Hyundai Santa Fe (USA)" },
        { "5XN", "Kia Sorento (USA)" }
    };

    public static string ResolveVehicleName(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId)) return "Véhicule Inconnu";
        if (deviceId == "SIMULATEUR") return ""; // Laisse Blazor décider du nom

        // 1. Essai de correspondance sur les 3 premiers caractères (Standard WMI)
        if (deviceId.Length >= 3)
        {
            string wmi = deviceId.Substring(0, 3);
            if (CanadianRegistry.TryGetValue(wmi, out string? name))
            {
                return name;
            }
        }
        return deviceId;
    }
}