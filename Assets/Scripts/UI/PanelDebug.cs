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
        public InputField shipModuleName;
        public Entity lastSpawn;

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

        private readonly Regex _idNum = new Regex("^[0-9]*$");

        public void OnSpawnEntityBtnPress()
        {
            var txt = spawnEntityName.text;
            if (_idNum.IsMatch(txt))
            {
                GameManager.Entity.SpawnEntity(int.Parse(txt));
            }
            else
            {
                GameManager.Entity.SpawnEntity(txt);
            }
        }

        public void OnAddShipModuleBtnPress() { }

        public void OnAsPlayerBtnPress() { }

        public void OnDestroyEntityBtnPress() { }

        public void OnDestroyAllEntityBtnPress() { }
    }
}