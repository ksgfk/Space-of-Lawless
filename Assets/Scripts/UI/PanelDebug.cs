using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace KSGFK
{
    public class PanelDebug : MonoBehaviour
    {
        public Text entityCountTxt;
        private int _lastEntityCount;
        public InputField spawnEntityName;
        public Player player;
        public InputField createItemName;

        public void Init() { StartCoroutine(OnUpdate()); }

        private IEnumerator OnUpdate()
        {
            while (true)
            {
                switch (GameManager.NowState)
                {
                    case GameState.Running:
                        if (GameManager.Entity.ActiveEntity.Count != _lastEntityCount)
                        {
                            _lastEntityCount = GameManager.Entity.ActiveEntity.Count;
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
            var em = GameManager.Entity;
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
    }
}