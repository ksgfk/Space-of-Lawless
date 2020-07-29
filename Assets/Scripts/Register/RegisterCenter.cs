using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class RegisterCenter
    {
        private readonly GameManager _gm;
        private readonly StageRegistry<EntryEntity> _entities;
        private readonly RegistryImpl<EntryJob> _jobs;
        private readonly StageRegistry<EntryItem> _items;

        public IRegistry<EntryEntity> Entity => _entities;
        public IRegistry<EntryJob> Job => _jobs;
        public IRegistry<EntryItem> Item => _items;

        internal RegisterCenter(GameManager gm)
        {
            _gm = gm;
            _entities = new StageRegistry<EntryEntity>("entity");
            _jobs = new RegistryImpl<EntryJob>("job");
            _items = new StageRegistry<EntryItem>("item");
            gm.Init += OnGameInit;
            gm.PostInit += OnGamePostInit;
        }

        private void OnGameInit(GameManager gm)
        {
            WaitRegister(gm.MetaData.EntityInfo, _entities);
            WaitRegister(gm.MetaData.ItemInfo, _items);
            NormalRegister(gm.MetaData.JobInfo, _jobs);
        }

        private void OnGamePostInit(GameManager gm)
        {
            _entities.RegisterAll();
            _items.RegisterAll();
        }

        private void WaitRegister<T>(IEnumerable<MetaData.Info> infos, StageRegistry<T> stageRegistry)
            where T : IStageProcessEntry
        {
            foreach (var info in infos)
            {
                var it = _gm.TempData.Query<T>(GameManager.GetDataPath(info.Path));
                try
                {
                    foreach (var entity in it)
                    {
                        stageRegistry.AddToWaitRegister(entity);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        private void NormalRegister<T>(IEnumerable<MetaData.Info> infos, IRegistry<T> registry)
            where T : IRegisterEntry
        {
            foreach (var jobInfo in infos)
            {
                foreach (var job in _gm.TempData.Query<T>(GameManager.GetDataPath(jobInfo.Path)))
                {
                    registry.Register(job);
                }
            }
        }
    }
}