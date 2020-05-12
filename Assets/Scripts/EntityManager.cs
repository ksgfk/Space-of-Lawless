using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KSGFK
{
    public class EntityManager : MonoBehaviour
    {
        public static readonly string DataRoot = Path.Combine(Application.streamingAssetsPath, "Data");
        public static readonly string ShipFramePath = Path.Combine(DataRoot, "ship_frame.csv");
        public static readonly string ShipEnginePath = Path.Combine(DataRoot, "ship_engine.csv");

        public int spawnCount;
        public int generation;
        private StageRegistry<EntityShip> _ship;
        private StageRegistry<IShipModule> _shipModules;
        private LinkedList<Entity> _active;

        public event Action Register;
        public event Action PostRegister;

        public void Init()
        {
            _ship = new StageRegistry<EntityShip>("ship");
            _shipModules = new StageRegistry<IShipModule>("ship_module");
            _active = new LinkedList<Entity>();
            GameManager.Instance.PerInit += OnGamePreInit;
            GameManager.Instance.Init += OnGameInit;
            GameManager.Instance.PostInit += OnGamePostInit;
        }

        public void RegisterShip(ShipFrameEntry entry) { _ship.AddToWaitRegister(entry); }

        public void RegisterShipModule(ShipModuleEntry module) { _shipModules.AddToWaitRegister(module); }

        private void OnGamePreInit()
        {
            GameManager.Data.AddPath(typeof(ShipFrameEntry), ShipFramePath);
            GameManager.Data.AddPath(typeof(ShipEngineEntry), ShipEnginePath);
        }

        private void OnGameInit()
        {
            foreach (var frameEntry in GameManager.Data.Query<ShipFrameEntry>(ShipFramePath))
            {
                RegisterShip(frameEntry);
            }

            foreach (var engine in GameManager.Data.Query<ShipEngineEntry>(ShipEnginePath))
            {
                RegisterShipModule(engine);
            }

            Register?.Invoke();
        }

        private void OnGamePostInit()
        {
            _ship.RegisterAll();
            _shipModules.RegisterAll();
            PostRegister?.Invoke();
            Register = null;
            PostRegister = null;
        }

        public EntityShip SpawnShip(int id)
        {
            var entry = _ship[id];
            return SpawnEntity((ShipFrameEntry) entry);
        }

        public EntityShip SpawnShip(string registerName)
        {
            var entry = _ship[registerName];
            return SpawnEntity((ShipFrameEntry) entry);
        }

        private T SpawnEntity<T>(EntityRegisterEntry<T> entry) where T : Entity
        {
            if (entry == null)
            {
                return null;
            }

            var instance = entry.Instantiate();
            instance.runtimeId = spawnCount;
            instance.generation = generation;
            var node = new LinkedListNode<Entity>(instance);
            _active.AddLast(node);
            instance.node = node;
            AfterSpawn();
            return instance;
        }

        private void AfterSpawn()
        {
            if (spawnCount == int.MaxValue)
            {
                spawnCount = 0;
                generation += 1;
            }
            else
            {
                spawnCount += 1;
            }
        }

        public IShipModule InstantiateShipModule(int moduleId)
        {
            var entry = _shipModules[moduleId];
            return InstantiateShipModule((ShipModuleEntry) entry);
        }

        public IShipModule InstantiateShipModule(string moduleName)
        {
            var entry = _shipModules[moduleName];
            return InstantiateShipModule((ShipModuleEntry) entry);
        }

        private static IShipModule InstantiateShipModule(ShipModuleEntry entry) { return entry.Instantiate(); }

        public IShipModule AddModuleToShip(EntityShip ship, int moduleId)
        {
            var module = InstantiateShipModule(moduleId);
            AddModuleToShip(ship, module);
            return module;
        }

        public IShipModule AddModuleToShip(EntityShip ship, string moduleName)
        {
            var module = InstantiateShipModule(moduleName);
            AddModuleToShip(ship, module);
            return module;
        }

        public static void AddModuleToShip(EntityShip ship, IShipModule module)
        {
            ship.AddModule(module);
            var engineGo = module.BaseGameObject;
            engineGo.transform.SetParent(ship.transform);
            module.Frame = ship;
        }
    }
}