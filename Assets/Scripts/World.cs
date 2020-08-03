using System;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace KSGFK
{
    public class World : MonoBehaviour, IDisposable
    {
        private SceneInstance? _sceneInstance;

        public SceneInstance? Scene
        {
            get => _sceneInstance;
            set => _sceneInstance = Helper.SingleAssign(value, _sceneInstance.HasValue);
        }

        public void Dispose() { }
    }
}