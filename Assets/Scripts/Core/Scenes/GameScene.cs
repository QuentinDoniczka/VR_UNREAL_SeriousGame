using UnityEngine;
using Core.Managers;

namespace Core.Scenes
{
    public class GameScene : MonoBehaviour
    {
        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            Debug.Log("GameScene initialized");
        }
    }
}
