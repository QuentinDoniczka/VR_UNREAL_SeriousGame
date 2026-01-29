1. Créer le Canvas HUD

1. Créer le Canvas                                                                                                                                
   - Clic droit dans la Hierarchy → UI → Canvas                                                                                                    
   - Renommer en InspectionCanvas                                                                                                                  
   - Canvas component:
    - Render Mode: Screen Space - Overlay                                                                                                       
      - Canvas Scaler:
    - UI Scale Mode: Scale With Screen Size
    - Reference Resolution: 1920 x 1080
2. Créer le Panel HUD                                                                                                                             
   - Clic droit sur InspectionCanvas → UI → Panel                                                                                                  
   - Renommer en InspectionPanel                                                                                                                   
   - RectTransform:
    - Anchor: Top Right
    - Pivot: (1, 1)
    - Pos X: -20, Pos Y: -20
    - Width: 300, Height: 100                                                                                                                     
      - Image component:
        - Color: (0, 0, 0, 0.7) (noir semi-transparent)                                                                                             
          - Ajouter component: Canvas Group                                                                                                               
          - Ajouter component: InspectionHUD
3. Créer le texte du nom                                                                                                                          
   - Clic droit sur InspectionPanel → UI → Text - TextMeshPro                                                                                      
   - Renommer en NameText                                                                                                                          
   - RectTransform:
    - Anchor: Top, Stretch horizontal       ICI
    - Height: 40
    - Margins: Left 10, Right 10, Top 10                                                                                                          
      - TextMeshPro:
        - Font Size: 28
    - Alignment: Center
    - Color: White
4. Créer le texte des détails                                                                                                                     
   - Clic droit sur InspectionPanel → UI → Text - TextMeshPro                                                                                      
   - Renommer en DetailsText                                                                                                                       
   - RectTransform:
    - Anchor: Bottom, Stretch horizontal
    - Height: 50
    - Margins: Left 10, Right 10, Bottom 10                                                                                                       
      - TextMeshPro:
        - Font Size: 22
    - Alignment: Center
    - Color: (200, 200, 200) (gris clair)

2. Configurer InspectionHUD

Sur le component InspectionHUD de InspectionPanel:
- Canvas Group: Drag InspectionPanel (lui-même)
- Name Text: Drag NameText
- Details Text: Drag DetailsText
- Fade Speed: 5
- Target Alpha: 0.85

3. Créer le InspectionManager

1. Créer un GameObject vide dans la scène                                                                                                         
   - Renommer en InspectionManager                                                                                                                 
   - Ajouter component: InspectionManager
2. Configurer le component:                                                                                                                       
   - Min Distance: 1                                                                                                                               
   - Max Distance: 10                                                                                                                              
   - Sphere Cast Radius: 0.05                                                                                                                      
   - Inspection Layer Mask: Default (ou créer un layer Inspectable)                                                                                
   - Inspection HUD: Drag InspectionPanel

4. Vérifier les tags des mains

Assure-toi que tes mains VR ont les tags:
- Main gauche: LeftHand
- Main droite: RightHand

  ---                                                                                                                                               
Hiérarchie finale

Scene                                                                                                                                             
├── InspectionManager          [InspectionManager.cs]                                                                                             
└── InspectionCanvas           [Canvas]                                                                                                           
└── InspectionPanel        [CanvasGroup, InspectionHUD.cs]                                                                                    
├── NameText           [TextMeshProUGUI]                                                                                                  
└── DetailsText        [TextMeshProUGUI]
                                                                                                                                                    
---                                                                                                                                               
Test

1. Lance le jeu
2. Pointe un feu ou extincteur avec ta main (direction forward)
3. Entre 1m et 10m → le HUD apparaît avec fade
4. Détourne le regard → le HUD disparaît avec fade                        