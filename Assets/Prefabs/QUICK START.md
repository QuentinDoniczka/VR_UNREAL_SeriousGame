# ⚡ QUICK START - Nouvelle Scène VR

## 🎯 2 Prefabs à Drag & Drop

### **Setup Complet en 30 secondes:**

```
1️⃣ Assets/Prefabs/VR EventSystem.prefab       → Scène
   ✓ Système UI VR configuré

2️⃣ Assets/Prefabs/XR Origin - Configured.prefab → Scène
   ✓ Contrôleurs VR + Raycast configurés
   ✓ Distance: 10m par défaut

✅ TERMINÉ! Play et testez
```

---

## 🔧 Modifier la Distance du Raycast

**Dans la Scène:**
```
XR Origin - Configured
  └─ Inspector
     └─ Simple VR Raycast Config
        └─ Max Raycast Distance: [slider 1-50m]
```

**En Play Mode:**
```
Clic droit sur le component
  → "Appliquer la Configuration"
```

---

## 📦 Contenu des Prefabs

### **VR EventSystem.prefab**
- ✅ XR UI Input Module (détection automatique)
- ✅ Event System
- ✅ Support VR + Mouse

### **XR Origin - Configured.prefab**
- ✅ XR Origin (XR Rig) standard
- ✅ Left/Right Controllers avec Near-Far Interactors
- ✅ SimpleVRRaycastConfig (10m par défaut)
- ✅ Camera Offset + Locomotion

---

## 🎨 Pour l'UI

**Canvas Requirements:**
```
Canvas
  ├─ Render Mode: World Space
  ├─ Event Camera: Main Camera (XR Origin)
  └─ Components:
     ├─ Graphic Raycaster (mouse)
     └─ Tracked Device Graphic Raycaster (VR) ✓ Déjà ajouté
```

**Boutons UI:**
```
Button
  └─ Image
     └─ Raycast Target: ✓ Activé
```

---

## 🐛 Checklist Debug

❌ **Raycast ne fonctionne pas?**
- [ ] VR EventSystem présent dans la scène
- [ ] Canvas a TrackedDeviceGraphicRaycaster
- [ ] Boutons ont RaycastTarget activé
- [ ] XR Origin a SimpleVRRaycastConfig

❌ **Distance ne change pas?**
- [ ] Clic droit → "Appliquer la Configuration"
- [ ] Vérifier valeur dans Inspector

❌ **Pas de ray visuel?**
- [ ] Vérifier que Near-Far Interactors sont actifs
- [ ] LineVisual enabled sur les interactors

---

## 📁 Structure Projet Recommandée

```
Assets/
├─ Prefabs/
│  ├─ VR EventSystem.prefab          ⭐ Réutiliser partout
│  ├─ XR Origin - Configured.prefab  ⭐ Réutiliser partout
│  └─ README - Setup VR.md
│
├─ Scripts/
│  ├─ SimpleVRRaycastConfig.cs       ⭐ Script principal
│  ├─ VRRaycastSettings.cs           (optionnel)
│  └─ VRRaycastConfigurator.cs       (avancé)
│
└─ Scenes/
   └─ [Vos scènes]
```

---

## 💡 Tips

- 🎯 **Distance Recommandées:**
  - UI proche (menus): **5m**
  - Usage général: **10m**
  - Grandes salles: **15-20m**

- 🔄 **Réutilisation:**
  - Les prefabs sont **réutilisables** dans toutes vos scènes
  - Modifier le prefab = modifier toutes les instances

- ⚡ **Performance:**
  - Distance plus courte = meilleure performance
  - Ajuster selon vos besoins

---

📖 **Guide complet:** Voir `README - Setup VR.md`
