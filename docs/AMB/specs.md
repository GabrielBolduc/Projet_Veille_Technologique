# Fonctionnement de OpenSpec

Cette section documente le fonctionnement du framework OpenSpec. 


## 1. L'Architecture du Répertoire OpenSpec

L'initialisation d'OpenSpec ajoute un dossier racine `openspec/` au dépôt du projet. Ce dossier est séparé en deux section distinctes :

### A. La Bibliothèque de Spécifications (`openspec/specs/`)
C'est la documentation. Elle décrit l'état actuel et réel de l'application (ex: protocoles de communication du Pi, contrats d'API, architecture de la base de données). Les agents IA s'y réfèrent en tout temps pour comprendre le contexte global avant toute modification.

### B. Le Système de Changements (`openspec/changes/`)
C'est l'element central *Specification-Driven Development*. Chaque nouvelle fonctionnalité ou modification logicielle fait l'objet d'un sous-dossier isolé dans cette section. C'est ici que l'évolution du code est planifiée et validée avant d'être écrite.

---

## 2. Le Cycle de Développement en 3 Étapes

Le framework impose un workflow déterministe pour garantir la qualité du design logiciel.

### Étape 2.1 : La Proposition (`/opsx:propose`)

L'IA génère automatiquement plusieurs fichiers Markdown standardisés dans le dossier `openspec/changes/add-user-auth/` :

- `proposal.md` : Définit le quoi et le pourquoi de la fonctionnalité (les objectifs commerciaux et techniques).
- `design.md` : Décrit l'approche technique exacte, les choix d'architecture et les impacts sur les systèmes existants.
- `tasks.md` : Une checklist d'implémentation atomique (tâches courtes et précises).
- `specs/` (Deltas) : Les fichiers montrant précisément comment la documentation vivante (`openspec/specs/`) sera modifiée une fois le code écrit.

## Étape 2.2 : La Revue

Il faut analyser, corrige et valide les fichiers Markdown générés.

## Étape 2.3 : L'Application du Plan (`/opsx:apply`)

Une fois le design et la liste des tâches validés par l'humain, la commande d'implémentation est lancée :

```bash
/opsx:apply
```

