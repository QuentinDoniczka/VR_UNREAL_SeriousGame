using Interaction;
using UI;

namespace Interaction.Fire
{
    public static class FireSafetyMessages
    {
        public static void LogIncompatibility(FireType fireType, ExtinguisherType extinguisherType)
        {
            string message = GetWarningMessage(fireType, extinguisherType);
            if (string.IsNullOrEmpty(message)) return;

            var warningHUD = WarningHUD.Instance;
            if (warningHUD != null)
                warningHUD.ShowWarning(message);
        }

        public static string GetWarningMessage(FireType fireType, ExtinguisherType extinguisherType)
        {
            return (fireType, extinguisherType) switch
            {
                // Feu électrique
                (FireType.Electrical, ExtinguisherType.Water) =>
                    "DANGER: Ne jamais utiliser d'eau sur un feu électrique! L'eau conduit l'électricité et risque de provoquer une électrocution.",

                (FireType.Electrical, ExtinguisherType.Foam) =>
                    "DANGER: Ne jamais utiliser de mousse sur un feu électrique! La mousse contient de l'eau et conduit l'électricité, risque d'électrocution.",

                // Feu de matière solide
                (FireType.SolidMaterial, ExtinguisherType.CO2) =>
                    "INEFFICACE: Le CO2 est peu efficace sur les feux de matières solides. Il ne refroidit pas suffisamment les braises qui peuvent se rallumer. Utilisez de l'eau ou de la mousse.",

                // Feu de liquide inflammable
                (FireType.FlammableLiquid, ExtinguisherType.Water) =>
                    "DANGER: Ne jamais utiliser d'eau sur un feu de liquide inflammable! L'eau peut projeter le liquide en feu et propager l'incendie. Utilisez de la mousse ou du CO2.",

                _ => null
            };
        }
    }
}
