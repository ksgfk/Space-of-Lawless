using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace KSGFK
{
    public class PanelDebug : MonoBehaviour
    {
        private GameManager _gm;
        public Text entityCountTxt;
        private int _lastEntityCount;
        public InputField spawnEntityName;
        public Player player;
        public InputField createItemName;
        public InputField mapName;

        public void Init() { StartCoroutine(OnUpdate()); }

        private void Awake() { _gm = GameManager.Instance; }

        private IEnumerator OnUpdate()
        {
            while (true)
            {
                switch (_gm.NowState)
                {
                    case GameState.Running:
                        if (_gm.Entity.ActiveEntity.Count != _lastEntityCount)
                        {
                            _lastEntityCount = _gm.Entity.ActiveEntity.Count;
                            entityCountTxt.text = _lastEntityCount.ToString();
                        }

                        break;
                    case GameState.Exit:
                        yield break;
                }

                yield return null;
            }
        }

        private readonly Regex _idNum = new Regex(@"^id:[0-9]+$");

        private Entity SpawnEntity()
        {
            var em = _gm.Entity;
            var txt = spawnEntityName.text;
            var e = _idNum.IsMatch(txt) ? em.SpawnEntity(int.Parse(txt.Substring(3))) : em.SpawnEntity(txt);
            return e;
        }

        public void OnSpawnEntityBtnPress() { SpawnEntity(); }

        public void OnSpawnPlayerBtnPress()
        {
            if (player)
            {
                return;
            }

            var e = SpawnEntity();
            if (!e)
            {
                return;
            }

            player = e.gameObject.AddComponent<Player>();
            player.Setup(e);
        }

        public void OnDestroyEntityBtnPress() { }

        public void OnDestroyAllEntityBtnPress() { }

        public void OnCreateItemBtnPress() { }

        public void OnLoadMapBtnPress()
        {
            var em = GameManager.Instance;
            var txt = mapName.text;
            if (_idNum.IsMatch(txt))
            {
                em.LoadMap(int.Parse(txt.Substring(3)));
            }
            else
            {
                em.LoadMap(txt);
            }
        }
    }
}