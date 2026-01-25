using UnityEngine;

namespace Interaction.Fire
{
    public interface IFireSpawnLocation
    {
        Vector3 GetSpawnPosition();
        Quaternion GetSpawnRotation();
        bool IsAvailable { get; }
        bool CanSpawnMultiple { get; }
        void MarkOccupied();
        void MarkFree();
    }
}
