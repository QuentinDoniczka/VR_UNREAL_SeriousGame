using Interaction;

namespace Interaction.Fire
{
    public static class FireCompatibility
    {
        public static bool IsExtinguisherAllowed(FireType fireType, ExtinguisherType extinguisherType)
        {
            return fireType switch
            {
                // Feu électrique: CO2 uniquement (pas d'eau ni mousse = conducteurs)
                FireType.Electrical => extinguisherType == ExtinguisherType.CO2,

                // Feu de matière solide: Eau ou Mousse (pas CO2 = inefficace)
                FireType.SolidMaterial => extinguisherType is ExtinguisherType.Water or ExtinguisherType.Foam,

                // Feu de liquide inflammable: Mousse ou CO2 (pas d'eau = propagation)
                FireType.FlammableLiquid => extinguisherType is ExtinguisherType.Foam or ExtinguisherType.CO2,

                _ => false
            };
        }
    }
}
