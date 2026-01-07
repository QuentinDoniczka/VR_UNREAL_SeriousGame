# 🎮 Guide Setup VR Simplifié

## ✅ Setup Nouvelle Scène (2 étapes SEULEMENT!)

### **1. Ajouter le VR EventSystem**
- Drag & Drop: `Assets/Prefabs/VR EventSystem.prefab` dans la scène
- ✓ C'est tout! Le système UI VR est configuré automatiquement

### **2. Ajouter le XR Origin Préconfiguré** ⭐ RECOMMANDÉ
- Drag & Drop: `Assets/Prefabs/XR Origin - Configured.prefab`
- ✓ **Déjà configuré avec:**
  - SimpleVRRaycastConfig attaché
  - Distance raycast: 10m (modifiable)
  - Auto-configuration au démarrage

### **Alternative: XR Origin Manuel**
Si vous préférez configurer manuellement:
- Drag & Drop: Le prefab XR Origin standard
- Options disponibles:
  - `Starter Assets/Prefabs/XR Origin (XR Rig).prefab` - Contrôleurs VR
  - `VRTemplateAssetsDemo/Prefabs/XR Origin Hands.prefab` - Hand tracking
- Puis ajouter manuellement `SimpleVRRaycastConfig` (voir section suivante)

---

## 🎯 Configurer la Distance du Raycast

### **Méthode 1: Quick Config (Recommandée)**

1. **Sélectionner le XR Origin** dans la scène
2. **Add Component** → `SimpleVRRaycastConfig`
3. **Modifier** le paramètre `Max Raycast Distance`:
   ```
   5m  = Interactions proches (menus)
   10m = Par défaut (équilibré)
   15m = Grandes pièces VR
   30m = Interactions distantes
   ```
4. **Appliquer**:
   - Automatique au Play
   - Ou clic droit → "Appliquer la Configuration"

### **Méthode 2: Configuration Avancée (ScriptableObject)**

1. **Créer le fichier de config**:
   - Clic droit → `Create/VR/Raycast Settings`
   - Nommer: `VRRaycastSettings`

2. **Configurer**:
   ```
   Max Raycast Distance: 10
   Max Visual Distance: 10
   Sphere Cast Radius: 0.1
   Cone Cast Angle: 6
   ```

3. **Utiliser**:
   - Remplacer `SimpleVRRaycastConfig` par `VRRaycastConfigurator`
   - Assigner le ScriptableObject créé

---

## 📋 Checklist Nouvelle Scène

- [ ] VR EventSystem ajouté
- [ ] XR Origin ajouté
- [ ] SimpleVRRaycastConfig configuré sur XR Origin
- [ ] Canvas avec TrackedDeviceGraphicRaycaster
- [ ] Boutons UI avec RaycastTarget activé
- [ ] Tester en Play mode

---

## 🐛 Troubleshooting

**Raycast ne fonctionne pas?**
1. Vérifier que l'EventSystem est bien dans la scène
2. Vérifier que le Canvas a `TrackedDeviceGraphicRaycaster`
3. Lancer "Appliquer la Configuration" sur le XR Origin

**Distance ne change pas?**
- Le script applique la config au Start()
- Pour appliquer en temps réel: clic droit → "Appliquer la Configuration"

**EventSystem manquant?**
- Il ne devrait y avoir qu'un seul EventSystem par scène
- Si vous avez plusieurs scènes, chargez le VR EventSystem en DontDestroyOnLoad
