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
        public static readonly string BulletPath = Path.Combine(DataRoot, "entity_bullet.csv");

        public int spawnCount;
        public int generation;
        private StageRegistry<Entity> _entity;
        private StageRegistry<IShipModule> _shipModules;
        private LinkedList<Entity> _active;

        public event Action Register;
        public event Action PostRegister;

        public void Init()
        {
            _entity = new StageRegistry<Entity>("entity");
            _shipModules = new StageRegistry<IShipModule>("ship_module");
            _active = new LinkedList<Entity>();
            GameManager.Instance.PerInit += OnGamePreInit;
            GameManager.Instance.Init += OnGameInit;
            GameManager.Instance.PostInit += OnGamePostInit;
        }

        public void RegisterShip(ShipFrameEntry frame) { _entity.AddToWaitRegister(frame); }

        public void RegisterShipModule(ShipModuleEntry module) { _shipModules.AddToWaitRegister(module); }

        public void RegisterBullet(BulletEntry bullet) { _entity.AddToWaitRegister(bullet); }

        private void OnGamePreInit()
        {
            var data = GameManager.Data;
            data.AddPath(typeof(ShipFrameEntry), ShipFramePath);
            data.AddPath(typeof(ShipEngineEntry), ShipEnginePath);
            data.AddPath(typeof(BulletEntry), BulletPath);
        }

        private void OnGameInit()
        {
            var data = GameManager.Data;
            foreach (var frameEntry in data.Query<ShipFrameEntry>(ShipFramePath))
            {
                RegisterShip(frameEntry);
            }

            foreach (var engine in data.Query<ShipEngineEntry>(ShipEnginePath))
            {
                RegisterShipModule(engine);
            }

            foreach (var bullet in data.Query<BulletEntry>(BulletPath))
            {
                RegisterBullet(bullet);
            }

            Register?.Invoke();
        }

        private void OnGamePostInit()
        {
            _entity.RegisterAll();
            _shipModules.RegisterAll();
            PostRegister?.Invoke();
            Register = null;
            PostRegister = null;
        }

        public EntityShip SpawnShip(int id) { return SpawnEntity<EntityShip>(id); }

        public EntityShip SpawnShip(string registerName) { return SpawnEntity<EntityShip>(registerName); }

        public EntityBullet SpawnBullet(int id) { return SpawnEntity<EntityBullet>(id); }
        
        public EntityBullet SpawnBullet(string registerName) { return SpawnEntity<EntityBullet>(registerName); }

        public T SpawnEntity<T>(int id) where T : Entity
        {
            var entry = _entity[id];
            return SpawnEntity((EntityRegisterEntry<T>) entry);
        }

        public T SpawnEntity<T>(string registerName) where T : Entity
        {
            var entry = _entity[registerName];
            return SpawnEntity((EntityRegisterEntry<T>) entry);
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