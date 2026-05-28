# Design - Initialisation de l'Environnement AutoPI

## 1. Objectif
Mettre en place la structure de dossiers racine pour le code source et préparer les configurations de base pour le développement multi-langage (C# et Python).

## 2. Changements requis
* Créer un répertoire `src-backend/` à la racine pour l'application web ASP.NET Core 9 / Blazor.
* Créer un répertoire `src-python/` à la racine pour le script de collecte OBD-II.
* Ajouter un fichier `.gitignore` global mis à jour pour ignorer les dossiers de build `.NET` (`bin/`, `obj/`) et l'environnement virtuel Python (`.venv/`).

## 3. Liste des Tâches (Tasks)
- [x] Créer le dossier `src-backend/`
- [x] Créer le dossier `src-python/`
- [x] Mettre à jour le `.gitignore` racine avec les patterns C# et Python.