using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KSGFK
{
    public class RegisterCenter
    {
        private readonly GameManager _gm;
        private readonly StageRegistry<EntryEntity> _entities;
        private readonly RegistryImpl<EntryJob> _jobs;
        private readonly StageRegistry<EntryItem> _items;
        private readonly RawDataCollection _rawData;

        public IRegistry<EntryEntity> Entity => _entities;
        public IRegistry<EntryJob> Job => _jobs;
        public IRegistry<EntryItem> Item => _items;

        public event Action<ICollection<DataLoader>> AddDataLoader;
        public event Action<ICollection<(Type, string)>> AddDataPath;

        public RegisterCenter(GameManager gm)
        {
            _gm = gm;
            _entities = new StageRegistry<EntryEntity>("entity");
            _jobs = new RegistryImpl<EntryJob>("job");
            _items = new StageRegistry<EntryItem>("item");
            _rawData = new RawDataCollection();
            gm.PerInit += LoadData;
            gm.Init += PerRegisterEntries;
            gm.PostInit += RegisterEntries;
        }

        private void LoadData(GameManager gm)
        {
            if (!gm.Load.CanLoad())
            {
                throw new InvalidOperationException("当前上下文不可以加载资源，可能是bug");
            }

            var dataLoaders = GetDataLoaders();
            var dataPaths = GetDataPaths(gm);
            var platform = GameManager.GetPlatform();
            foreach (var (type, path) in dataPaths)
            {
                var ext = Path.GetExtension(path);
                var loaderType = $"{platform}{ext}";
                if (!dataLoaders.TryGetValue(loaderType, out var loader))
                {
                    Debug.LogWarningFormat("未找到文件类型{2}的Loader,忽略.[平台{1}][路径{0}]", path, platform, ext);
                    continue;
                }

                IAsyncHandleWrapper wrapper;
                try
                {
                    wrapper = loader.StartLoad(type, path, _rawData);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    continue;
                }

                gm.Load.Request(wrapper);
            }
        }

        private void PerRegisterEntries(GameManager gm)
        {
            WaitRegister(gm.MetaData.EntityInfo, _entities);
            WaitRegister(gm.MetaData.ItemInfo, _items);
            NormalRegister(gm.MetaData.JobInfo, _jobs);
        }

        private void RegisterEntries(GameManager gm)
        {
            _entities.RegisterAll();
            _items.RegisterAll();
        }

        private void WaitRegister<T>(IEnumerable<MetaData.Info> infos, StageRegistry<T> stageRegistry)
            where T : IStageProcessEntry
        {
            foreach (var info in infos)
            {
                // var it = _gm.TempData.Query<T>(GameManager.GetDataPath(info.Path));
                // try
                // {
                //     foreach (var entity in it)
                //     {
                //         stageRegistry.AddToWaitRegister(entity);
                //     }
                // }
                // catch (Exception e)
                // {
                //     Debug.LogError(e);
                // }
            }
        }

        private void NormalRegister<T>(IEnumerable<MetaData.Info> infos, IRegistry<T> registry)
            where T : IRegisterEntry
        {
            foreach (var jobInfo in infos)
            {
                // foreach (var job in _gm.TempData.Query<T>(GameManager.GetDataPath(jobInfo.Path)))
                // {
                //     registry.Register(job);
                // }
            }
        }

        private IDictionary<string, DataLoader> GetDataLoaders()
        {
            var collector = new List<DataLoader>
            {
                new CsvWinLoader()
            };
            AddDataLoader?.Invoke(collector);
            AddDataLoader = null;
            return collector.ToDictionary(loader => $"{loader.Platform}.{loader.FileExt}");
        }

        private IEnumerable<(Type, string)> GetDataPaths(GameManager gm)
        {
            var dataPath = new List<(Type, string)>();
            GetDataPathFromMetaData(gm, dataPath);
            AddDataPath?.Invoke(dataPath);
            AddDataPath = null;
            return dataPath;
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
                        type = Type.GetType(info.Type);
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
    }
}