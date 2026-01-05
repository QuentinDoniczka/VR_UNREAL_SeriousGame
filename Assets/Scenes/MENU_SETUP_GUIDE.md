# Guide de Setup du Menu VR

## Étape 1 : Créer la scène du menu

1. Dans Unity : `File > New Scene` (Basic Built-in)
2. Sauvegarder : `File > Save As...` → `Assets/Scenes/MenuScene.unity`

---

## Étape 2 : Ajouter le XR Origin (Caméra VR)

1. **Clic droit dans Hierarchy** → `XR > XR Origin (VR)`
2. Cela créera automatiquement :
   - XR Origin (container)
   - Main Camera (caméra VR)
   - Left/Right Controllers

---

## Étape 3 : Ajouter l'Event System pour VR

1. **Clic droit dans Hierarchy** → `UI > Event System`
2. **Sélectionner l'Event System** dans la Hierarchy
3. Dans l'Inspector, **supprimer** le composant `Standalone Input Module`
4. Cliquer sur `Add Component` → chercher `XR UI Input Module`
5. Ajouter le composant `XR UI Input Module`

---

## Étape 4 : Créer le Canvas VR (World Space)

1. **Clic droit dans Hierarchy** → `UI > Canvas`
2. Renommer en `MainMenuCanvas`
3. **Dans l'Inspector du Canvas** :
   - **Render Mode** : `World Space`
   - **Transform** :
     - Position : `X: 0, Y: 1.5, Z: 2.5` (devant le joueur, à hauteur des yeux)
     - Rotation : `X: 0, Y: 0, Z: 0`
     - Scale : `X: 0.01, Y: 0.01, Z: 0.01`
   - **Rect Transform** :
     - Width : `1920`
     - Height : `1080`

4. **Ajouter un Canvas Scaler** (si pas déjà présent)
   - Dans le Canvas, vérifier qu'il y a un composant `Canvas Scaler`
   - UI Scale Mode : `Scale With Screen Size`
   - Reference Resolution : `1920 x 1080`

5. **Ajouter un Graphic Raycaster** (doit être déjà là par défaut)
   - Vérifier qu'il y a un composant `Graphic Raycaster` sur le Canvas

---

## Étape 5 : Créer les boutons du menu

### A) Créer le bouton "PLAY"

1. **Clic droit sur MainMenuCanvas** → `UI > Button - TextMeshPro`
2. Renommer en `PlayButton`
3. **Transform** (dans Rect Transform) :
   - Pos X: `0`, Pos Y: `200`, Pos Z: `0`
   - Width: `800`, Height: `200`
4. **Sélectionner l'enfant "Text (TMP)"** sous PlayButton
   - Changer le texte : `JOUER`
   - Font Size : `72`
   - Alignment : Center + Middle
   - Color : Blanc
5. **Ajouter le script VRMenuButton** :
   - Sélectionner `PlayButton`
   - `Add Component` → chercher `VR Menu Button`
   - Configurer les couleurs si souhaité (Normal Color, Hover Color)

### B) Créer le bouton "OPTIONS"

1. **Dupliquer PlayButton** : Clic droit sur PlayButton → `Duplicate`
2. Renommer en `OptionsButton`
3. **Rect Transform** :
   - Pos Y: `0` (au centre)
4. Modifier le texte : `OPTIONS`

### C) Créer le bouton "QUITTER"

1. **Dupliquer OptionsButton**
2. Renommer en `QuitButton`
3. **Rect Transform** :
   - Pos Y: `-200` (en bas)
4. Modifier le texte : `QUITTER`

---

## Étape 6 : Ajouter le script MenuUI

1. **Sélectionner le Canvas** `MainMenuCanvas`
2. `Add Component` → chercher `Menu UI`
3. **Dans l'Inspector**, assigner les boutons :
   - **Play Button** : glisser `PlayButton` depuis la Hierarchy
   - **Options Button** : glisser `OptionsButton`
   - **Quit Button** : glisser `QuitButton`

---

## Étape 7 : Ajouter le SceneManager

1. **Clic droit dans Hierarchy** → `Create Empty`
2. Renommer en `GameManagers`
3. `Add Component` → chercher `Scene Manager` (Core.Managers)

---

## Étape 8 : Configurer le Build Settings

1. `File > Build Settings`
2. **Ajouter les scènes** :
   - Cliquer sur `Add Open Scenes` (avec MenuScene ouverte)
   - Ouvrir `GameScene` et l'ajouter aussi
   - **MenuScene doit être en index 0** (première scène à charger)

---

## Étape 9 : Tester en VR

1. **Assurez-vous que le XR Origin a bien les Ray Interactors** :
   - Sélectionner `XR Origin > Left/Right Controller`
   - Vérifier qu'il y a un composant `XR Ray Interactor`
   - Si absent : `Add Component` → `XR Ray Interactor`

2. **Play** et tester avec un casque VR ou en simulation

---

## Étape 10 (Optionnelle) : Améliorer le visuel

### Ajouter un fond au menu
1. Clic droit sur `MainMenuCanvas` → `UI > Image`
2. Renommer en `Background`
3. Placer en **premier** dans la hiérarchie (tout en haut sous Canvas)
4. Stretch to fill : Anchors → cliquer sur le carré en bas à droite → sélectionner `Stretch/Stretch`
5. Color : noir semi-transparent (R:0, G:0, B:0, A:180)

### Ajouter un titre
1. Clic droit sur `MainMenuCanvas` → `UI > Text - TextMeshPro`
2. Renommer en `TitleText`
3. Texte : `MON JEU VR`
4. Pos Y : `400`
5. Font Size : `120`
6. Color : Blanc
7. Alignment : Center

---

## Structure finale dans la Hierarchy

```
MenuScene
├── XR Origin
│   ├── Camera Offset
│   │   └── Main Camera
│   ├── Left Controller
│   └── Right Controller
├── Event System
├── GameManagers (avec SceneManager)
└── MainMenuCanvas (avec MenuUI)
    ├── Background (Image)
    ├── TitleText
    ├── PlayButton (avec VRMenuButton)
    │   └── Text (TMP)
    ├── OptionsButton (avec VRMenuButton)
    │   └── Text (TMP)
    └── QuitButton (avec VRMenuButton)
        └── Text (TMP)
```

---

## Notes importantes

- **Le Canvas DOIT être en World Space** pour fonctionner en VR
- **Les boutons doivent avoir un Image component** pour recevoir les raycast
- **Le XR UI Input Module** est essentiel pour que les controllers interagissent avec l'UI
- **La position du Canvas** (0, 1.5, 2.5) place le menu à hauteur des yeux à 2.5m devant le joueur
- **Le scale 0.01** est nécessaire car le Canvas en World Space a une taille énorme par défaut

---

## Troubleshooting

**Les boutons ne répondent pas ?**
- Vérifier que l'Event System a le `XR UI Input Module`
- Vérifier que les controllers ont le `XR Ray Interactor`
- Vérifier que le Canvas a le `Graphic Raycaster`

**Le menu est trop grand/petit ?**
- Ajuster le **Scale** du Canvas (essayer 0.005 à 0.02)

**Le menu est mal positionné ?**
- Ajuster la **Position** du Canvas (Y pour hauteur, Z pour distance)
