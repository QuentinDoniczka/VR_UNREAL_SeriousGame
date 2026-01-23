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

### Réinvention de la roue (Overthinking)
Identifie le code qui refait manuellement ce que Unity/C# fait déjà nativement :
- Détection de collision manuelle (OverlapSphere, Raycast en boucle) → Trigger Collider + OnTriggerEnter/Stay
- Calcul de distance/angle manuel pour zones → Colliders (Sphere, Capsule, Mesh)
- Gestion manuelle de timers → Coroutines ou Invoke
- Pools d'objets custom simples → ObjectPool<T> (Unity 2021+)
- Lerp/Slerp manuel dans Update pour suivre une cible → Transform.SetParent, ou composants existants
- Sérialisation JSON custom → JsonUtility ou PlayerPrefs
- Machine à états avec switch/enum → Animator avec StateMachineBehaviour
- Recherche manuelle dans des listes → LINQ, Dictionary, HashSet

Priorité :
- HAUTE : Code complexe (10+ lignes) qui remplace une feature native Unity
- MOYENNE : Code moyen (5-10 lignes) évitable avec un composant/système existant
- BASSE : Micro-optimisation inutile ou abstraction prématurée

Question clé : "Est-ce que Unity/C# ne fait pas déjà ça ?"

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

### Performance dans les boucles Update/FixedUpdate/LateUpdate
Identifie le code coûteux exécuté chaque frame :

**Comparaisons null répétées sur UnityEngine.Object**
- `== null` et `!= null` sur UnityEngine.Object sont coûteux (opérateur surchargé Unity)
- Chaque comparaison vérifie si l'objet natif C++ existe encore
- Solution : cacher le résultat dans un bool lors d'événements (Awake, OnEnable, Grab, etc.)
- Exemple problématique : `if (myTransform != null && otherTransform != null)` dans Update
- Exemple corrigé : `if (hasValidReferences)` initialisé une seule fois

**Appels de méthodes coûteux chaque frame**
- `base.Update()` / `base.FixedUpdate()` → vérifier si la classe parente est lourde
- Accès répétés à des singletons (`Manager.Instance.Property`) → cacher la référence
- `GetComponent<T>()`, `Find()`, `FindObjectOfType()` → cacher dans Awake
- Multiples `ReadValue<T>()` sur InputAction → acceptable mais à surveiller

**Checks redondants**
- Null checks déjà garantis par un état booléen (ex: `isGrabbed` implique `hand != null`)
- Même condition vérifiée plusieurs fois dans le même chemin d'exécution
- Solution : utiliser des événements plutôt que du polling quand possible

**Polling vs Event-driven**
- Vérifier un état chaque frame alors qu'un événement existe (Input callbacks, OnTriggerEnter, etc.)
- Solution : s'abonner aux événements et maintenir un état local

Priorité :
- CRITIQUE : Code coûteux dans Update en contexte VR (90+ fps requis)
- HAUTE : Multiples null checks sur UnityEngine.Object par frame
- MOYENNE : Singleton access répétés ou checks redondants
- BASSE : Micro-optimisations avec impact négligeable

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
