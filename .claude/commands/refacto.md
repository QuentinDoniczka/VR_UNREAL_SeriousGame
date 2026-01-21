---
description: Liste les problèmes de refactoring du plus critique au moins critique
---

Analyse le code et liste TOUS les problèmes du plus critique au moins critique.

## Critères à analyser

### Comparaisons null en Unity
- `== null` utilise l'opérateur surchargé Unity (vérifie les objets détruits)
- `is null` vérifie uniquement la référence C# (ignore l'état Unity)
- Toujours préférer `== null` pour les UnityEngine.Object

### Code vs Éditeur Unity
Identifie le code qui devrait plutôt être configuré dans l'éditeur :
- Meshes ou géométries statiques créées en code → prefab ou asset dans l'éditeur
- Hiérarchies de GameObjects fixes → prefab
- Valeurs hardcodées → champs sérialisés dans l'Inspector
- Chaînes de GetComponent/Find évitables → références assignées dans l'éditeur

### Extraction de classes
Identifie les portions de code qui peuvent être extraites dans une classe séparée :
- Méthodes qui forment un groupe logique cohérent
- Code dupliqué qui mérite sa propre classe
- Responsabilités distinctes mélangées dans une même classe (violation Single Responsibility)

Priorité extraction :
- HAUTE : 80+ lignes extractibles ou responsabilité clairement séparée
- MOYENNE : 40-80 lignes ou groupe de 3+ méthodes liées
- BASSE : < 40 lignes mais améliorerait la lisibilité

Rester KISS : on extrait seulement si ça simplifie réellement le code, pas pour le plaisir d'abstraire.

## Format de sortie

IMPORTANT : Format texte simple uniquement. PAS de tableau markdown, PAS de colonnes, PAS de syntaxe `|`. Juste des listes numérotées.

### 🔴 CRITIQUE
1. `Fichier.cs:ligne` - Description du problème
2. ...

### 🟠 HAUTE
1. `Fichier.cs:ligne` - Description du problème
2. ...

### 🟡 MOYENNE
1. `Fichier.cs:ligne` - Description du problème
2. ...

### 🟢 BASSE
1. `Fichier.cs:ligne` - Description du problème
2. ...

Règles :
- Si une section est vide, ne pas l'afficher
- Ne jamais utiliser de tableau markdown
- Garder le format simple : numéro, fichier, description

$ARGUMENTS
