using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KSGFK
{
    public class EntityManager : MonoBehaviour
    {
        public static readonly string DataRoot = Path.Combine(Application.streamingAssetsPath, "Data");
        public static readonly string ShipFramePath = Path.Combine(DataRoot, "entity_ship.csv");
        public static readonly string ShipEnginePath = Path.Combine(DataRoot, "ship_engine.csv");
        public static readonly string ShipWeaponPath = Path.Combine(DataRoot, "ship_weapon.csv");
        public static readonly string BulletPath = Path.Combine(DataRoot, "entity_bullet.csv");

        [SerializeField] private int spawnCount;
        [SerializeField] private int generation;
        private StageRegistry<EntryEntity> _entity;
        private StageRegistry<EntryShipModule> _shipModules;
        private LinkedList<Entity> _active;

        public IRegistry<EntryEntity> EntityEntry => _entity;
        public IRegistry<EntryShipModule> ShipModuleEntry => _shipModules;
        public ICollection<Entity> ActiveEntity => _active;

        public event Action Register;
        public event Action PostRegister;

        public void Init()
        {
            _entity = new StageRegistry<EntryEntity>("entity");
            _shipModules = new StageRegistry<EntryShipModule>("ship_module");
            _active = new LinkedList<Entity>();
            GameManager.Instance.PerInit += OnGamePreInit;
            GameManager.Instance.Init += OnGameInit;
            GameManager.Instance.PostInit += OnGamePostInit;
        }

        public void RegisterShip(EntryEntityShip frame) { _entity.AddToWaitRegister(frame); }

        public void RegisterShipModule(EntryShipModule module) { _shipModules.AddToWaitRegister(module); }

        public void RegisterBullet(EntryEntityBullet entryEntityBullet)
        {
            _entity.AddToWaitRegister(entryEntityBullet);
        }

        private void OnGamePreInit()
        {
            var data = GameManager.Data;
            data.AddPath(typeof(EntryEntityShip), ShipFramePath);
            data.AddPath(typeof(EntryShipEngine), ShipEnginePath);
            data.AddPath(typeof(EntryEntityBullet), BulletPath);
            data.AddPath(typeof(EntryShipWeapon), ShipWeaponPath);
        }

        private void OnGameInit()
        {
            var data = GameManager.Data;
            foreach (var frameEntry in data.Query<EntryEntityShip>(ShipFramePath))
            {
                RegisterShip(frameEntry);
            }

            foreach (var engine in data.Query<EntryShipEngine>(ShipEnginePath))
            {
                RegisterShipModule(engine);
            }

            foreach (var weapon in data.Query<EntryShipWeapon>(ShipWeaponPath))
            {
                RegisterShipModule(weapon);
            }

            foreach (var bullet in data.Query<EntryEntityBullet>(BulletPath))
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

        public Entity SpawnEntity(int id)
        {
            var entry = _entity[id];
            return SpawnEntity(entry);
        }

        public T SpawnEntity<T>(int id) where T : Entity { return SpawnEntity(id) as T; }

        public Entity SpawnEntity(string registerName)
        {
            var entry = _entity[registerName];
            return SpawnEntity(entry);
        }

        public T SpawnEntity<T>(string id) where T : Entity { return SpawnEntity(id) as T; }

        private Entity SpawnEntity(EntryEntity entry)
        {
            if (entry == null)
            {
                return null;
            }

            var instance = entry.Instantiate();
            var node = new LinkedListNode<Entity>(instance);
            _active.AddLast(node);
            instance.Node = node;
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

        public ShipModule InstantiateShipModule(int moduleId)
        {
            var entry = _shipModules[moduleId];
            return InstantiateShipModule(entry);
        }

        public ShipModule InstantiateShipModule(string moduleName)
        {
            var entry = _shipModules[moduleName];
            return InstantiateShipModule(entry);
        }

        public T InstantiateShipModule<T>(int moduleId) where T : ShipModule
        {
            return InstantiateShipModule(moduleId) as T;
        }

        public T InstantiateShipModule<T>(string moduleName) where T : ShipModule
        {
            return InstantiateShipModule(moduleName) as T;
        }

        private static ShipModule InstantiateShipModule(EntryShipModule entry) { return entry.Instantiate(); }

        public void DestroyEntity(Entity entity)
        {
            if (entity.Node.List == null)
            {
                Debug.LogWarningFormat("已经被销毁的实体:{0}", entity.RuntimeId);
                return;
            }

            var entry = _entity[entity.RuntimeId];
            _active.Remove(entity.Node);
            entry.Destroy(entity);
        }
    }
}