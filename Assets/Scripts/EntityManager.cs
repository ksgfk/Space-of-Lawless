using System;
using System.IO;
using UnityEngine;

namespace KSGFK
{
    public class EntityManager : MonoBehaviour
    {
        public static readonly string DataRoot = Path.Combine(Application.streamingAssetsPath, "Data");
        public static readonly string ShipFramePath = Path.Combine(DataRoot, "ship_frame.csv");

        private StageRegistry<EntityShip> _entity;

        public event Action Register;
        public event Action PostRegister;

        public void Init()
        {
            _entity = new StageRegistry<EntityShip>("Ship", new ShipFrameProcessor());
            GameManager.Instance.PerInit += OnGamePreInit;
            GameManager.Instance.Init += OnGameInit;
            GameManager.Instance.PostInit += OnGamePostInit;
        }

        public void RegisterShip(ShipFrameEntry entry) { _entity.AddToWaitRegister(entry); }

        private void OnGamePreInit() { GameManager.Data.AddPath(typeof(ShipFrameEntry), ShipFramePath); }

        private void OnGameInit()
        {
            foreach (var frameEntry in GameManager.Data.Query<ShipFrameEntry>(ShipFramePath))
            {
                RegisterShip(frameEntry);
            }

            Register?.Invoke();
        }

        private void OnGamePostInit()
        {
            _entity.RegisterAll();
            PostRegister?.Invoke();
            Register = null;
            PostRegister = null;
        }
    }
}