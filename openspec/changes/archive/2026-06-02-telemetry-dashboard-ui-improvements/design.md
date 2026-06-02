# Design - Refonte Épurée et Haute Densité du Dashboard

## 1. Objectif
Remplacer l'interface typée "jeu vidéo" par un tableau de bord de télémétrie de niveau professionnel, hautement lisible, dense en données et épuré (style minimaliste industriel).

## 2. Spécifications de la Refonte
* **Mise en page :** Structure en grille stricte, asymétrique. Grande section centrale numérique pour la vitesse et le RPM. Panneau latéral pour les données thermiques et dynamiques.
* **Nouvelles données :** Ajout de la vitesse maximale de la session, du RPM maximal atteint, et du calcul du delta de vitesse.
* **Graphique :** Remplacer les lignes courbes fluides par un graphique à barres verticales en temps réel (Histogramme de charge) ou une ligne fine de type oscilloscope.

## 3. Liste des Tâches (Tasks)
- [x] Supprimer tous les filtres de lueur (glow), les dégradés et les polices de style "racing".
- [x] Implémenter une typographie suisse/géométrique épurée (style Inter/Roboto Mono).
- [x] Ajouter le suivi des statistiques de session (`MaxRpm`, `MaxSpeed`).
- [x] Restructurer le HTML de `Dashboard.razor` pour une disposition asymétrique haute densité.
- [x] Concevoir un mini-graphique compact de type "Sparkline" intégré sous chaque métrique majeure.