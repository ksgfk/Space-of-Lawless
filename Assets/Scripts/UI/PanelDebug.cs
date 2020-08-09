using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace KSGFK
{
    public class PanelDebug : MonoBehaviour
    {
        private GameManager _gm;

        public GameObject loadWorld;
        public GameObject unloadWorld;
        public GameObject spawnEntity;
        public GameObject destroyEntity;
        public GameObject createItem;

        public Text entityCountTxt;
        private int _lastEntityCount;
        public InputField spawnEntityName;
        public Player player;
        public InputField createItemName;
        public InputField mapName;

        public void Init()
        {
            transform.SetParent(GameManager.Instance.UiCanvas.transform, false);
            StartCoroutine(OnUpdate());
        }

        private void Awake()
        {
            _gm = GameManager.Instance;
            loadWorld.SetActive(true);
            unloadWorld.SetActive(true);
        }

        private IEnumerator OnUpdate()
        {
            while (true)
            {
                var world = _gm.World;
                switch (_gm.NowState)
                {
                    case GameState.Running when world.HasValue:
                        spawnEntity.SetActive(true);
                        destroyEntity.SetActive(true);
                        createItem.SetActive(true);
                        World w = world;
                        if (w.ActiveEntity.Count() != _lastEntityCount)
                        {
                            _lastEntityCount = w.ActiveEntity.Count();
                            entityCountTxt.text = _lastEntityCount.ToString();
                        }

                        break;
                    case GameState.Running when !world.HasValue:
                        spawnEntity.SetActive(false);
                        destroyEntity.SetActive(false);
                        createItem.SetActive(false);
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
            World em = _gm.World;
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

        public void OnDestroyAllEntityBtnPress()
        {
            World em = _gm.World;
            foreach (var entity in em.ActiveEntity.ToArray())
            {
                em.DestroyEntity(entity);
            }
        }

        public void OnCreateItemBtnPress()
        {
            World em = _gm.World;
            var txt = createItemName.text;
            if (_idNum.IsMatch(txt))
            {
                em.CreateItem(int.Parse(txt.Substring(3)));
            }
            else
            {
                em.CreateItem(txt);
            }
        }

        public void OnLoadWorldBtnPress()
        {
            var em = GameManager.Instance;
            var txt = mapName.text;
            if (_idNum.IsMatch(txt))
            {
                em.LoadWorld(int.Parse(txt.Substring(3)));
            }
            else
            {
                em.LoadWorld(txt);
            }
        }

        public void OnUnloadWorldBtnPress() { GameManager.Instance.UnloadWorld(); }

        public void DefaultSet()
        {
            // GameManager.Instance.StartLoadWorld(0,
            //     () =>
            //     {
            //         var e = GameManager.Instance.World.Value.SpawnEntity(1);
            //         var p = e.gameObject.AddComponent<Player>();
            //         p.Setup(e);
            //     });
        }
    }
}