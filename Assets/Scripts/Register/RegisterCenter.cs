using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace KSGFK
{
    public class RegisterCenter
    {
        private readonly GameManager _gm;
        private readonly List<RegisterEntry> _registered;
        private readonly Dictionary<string, int> _queryDict;
        private readonly Registry<Registry> _registry;
        private readonly StageRegistry<EntryEntity> _entityRegistry;
        private readonly Registry<EntryWorld> _worldRegistry;
        private readonly StageRegistry<EntryItem> _itemRegistry;

        private EntryEntityItem _entryEntityItem;

        private RegisterDataCollection _registerData;
        private HashSet<string> _entryName;

        public IReadOnlyCollection<RegisterEntry> Entry => _registered;
        public Registry<Registry> Registry => _registry;
        public Registry<EntryEntity> Entity => _entityRegistry;
        public Registry<EntryWorld> World => _worldRegistry;
        public Registry<EntryItem> Item => _itemRegistry;
        public EntityItem NewEntityItem => (EntityItem) _entryEntityItem.Instantiate();

        public RegisterCenter(GameManager gm)
        {
            _gm = gm;
            _registered = new List<RegisterEntry>();
            _queryDict = new Dictionary<string, int>();
            _entryName = new HashSet<string>();

            _registry = new Registry<Registry>("registry", _entryName);
            _entityRegistry = new StageRegistry<EntryEntity>("entity", _entryName);
            _worldRegistry = new Registry<EntryWorld>("world", _entryName);
            _itemRegistry = new StageRegistry<EntryItem>("item", _entryName);

            _gm.BeforePreInit += () =>
            {
                _gm.Event.Subscribe<EventRegister<EntryEntity>>((o, e) =>
                {
                    e.Registry.Register(new EntryEntityItem());
                });
            };
        }

        public void RegisterRegistry()
        {
            Registry.Register(_entityRegistry);
            Registry.Register(_worldRegistry);
            Registry.Register(_itemRegistry);
        }

        public async Task GetRegisterEntryData()
        {
            _registerData = new RegisterDataCollection();
            await LoadDataFromCsv(_registerData);
        }

        public async Task PreRegister()
        {
            RegisterIter(_entityRegistry, _gm.MetaData.EntityInfo);
            RegisterIter(_worldRegistry, _gm.MetaData.WorldInfo);
            RegisterIter(_itemRegistry, _gm.MetaData.ItemInfo);

            foreach (var registry in Registry)
            {
                if (registry is IStageRegistry stageRegistry)
                {
                    await stageRegistry.PreProcessEntry();
                }
            }
        }

        public void Register()
        {
            foreach (var registry in Registry)
            {
                if (registry is IStageRegistry stageRegistry)
                {
                    stageRegistry.RegisterAll();
                }
            }
        }

        public void Remap()
        {
            Registry.Remap(0);
            _registered.Add(Registry);
            _queryDict.Add(Registry.RegisterName, Registry.RuntimeId);
            Registry.Remap(_registered, _queryDict);
            foreach (var registry in Registry)
            {
                registry.Remap(_registered, _queryDict);
            }

            _entryEntityItem = (EntryEntityItem) Entity["entity_item"];
        }

        public void Clean()
        {
            _entryName = null;
            _registerData = null;
            ReleaseEvent();
        }

        private void ReleaseEvent()
        {
            foreach (var registry in Registry)
            {
                _gm.Event.Unsubscribe(typeof(EventRegister<>).MakeGenericType(registry.EntryType));
            }
        }

        private async Task LoadDataFromCsv(RegisterDataCollection collection)
        {
            var dataPath = new List<(Type, string)>();
            GetDataPathFromMetaData(_gm, dataPath);
            var task = new List<Task<RegisterData>>();
            task.AddRange(dataPath.Select(d => CsvWinLoader.ReadAsync(d.Item2, d.Item1)));
            foreach (var t in task)
            {
                if (t == null)
                {
                    Debug.LogError("数据是空");
                    continue;
                }

                collection.Push(await t);
            }
        }

        private static void GetDataPathFromMetaData(GameManager gm, ICollection<(Type, string)> dataPath)
        {
            var metadataType = typeof(MetaData);
            var publicFields = metadataType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in publicFields.Where(field => field.FieldType == typeof(MetaData.Info[])))
            {
                foreach (var info in (MetaData.Info[]) field.GetValue(gm.MetaData))
                {
                    Type type;
                    try
                    {
                        type = Type.GetType(info.DataType, true);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        continue;
                    }

                    bool hasExt;
                    try
                    {
                        hasExt = Path.HasExtension(info.Path);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        continue;
                    }

                    if (hasExt)
                    {
                        dataPath.Add((type, GameManager.GetDataPath(info.Path)));
                    }
                    else
                    {
                        Debug.LogError($"{info.Path}不是文件名,忽略");
                    }
                }
            }
        }

        private static object ConvertRegisterDataToRegisterEntry(object registerData, MetaData.Info info)
        {
            var entryType = Type.GetType(info.EntryType);
            if (entryType == null)
            {
                throw new ArgumentException();
            }

            return Activator.CreateInstance(entryType, registerData);
        }

        private void RegisterIter<T>(Registry<T> registry, IEnumerable<MetaData.Info> info) where T : RegisterEntry
        {
            foreach (var i in info)
            {
                var data = _registerData.Query(GameManager.GetDataPath(i.Path));
                foreach (var d in data)
                {
                    try
                    {
                        registry.Register(ConvertRegisterDataToRegisterEntry(d, i));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }

            _gm.Event.Post(this, new EventRegister<T>(registry));
        }

        public void LogStatistic()
        {
            Debug.LogFormat("Registry:{0}", Registry.Count);
            Debug.LogFormat("Entity:{0}", Entity.Count);
            Debug.LogFormat("Item:{0}", Item.Count);
            Debug.LogFormat("World:{0}", World.Count);
            Debug.LogFormat("All:{0}", _registered.Count);
        }
    }
}