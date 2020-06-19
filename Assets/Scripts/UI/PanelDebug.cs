using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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
        public EntityShip lastSpawnPlayer;
        private Action<InputAction.CallbackContext> _movAct;
        private Action<InputAction.CallbackContext> _rotAct;
        private Action<InputAction.CallbackContext> _firStartAct;
        private Action<InputAction.CallbackContext> _firCancelAct;

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

        public void OnSpawnEntityBtnPress()
        {
            var txt = spawnEntityName.text;
            if (txt.StartsWith("id:"))
            {
                lastSpawn = GameManager.Entity.SpawnEntity(int.Parse(txt.Substring(3)));
            }
            else
            {
                lastSpawn = GameManager.Entity.SpawnEntity(txt);
            }

            if (lastSpawn is EntityShip ship)
            {
                lastSpawnPlayer = ship;
            }
        }

        public void OnAddShipModuleBtnPress()
        {
            if (lastSpawnPlayer == null)
            {
                Debug.LogWarningFormat("{0}不是{1}", lastSpawn.name, typeof(EntityShip));
                return;
            }

            var txt = shipModuleName.text;
            var modulesName = txt.Split(',');
            foreach (var moduleName in modulesName)
            {
                ShipModule module;
                if (moduleName.StartsWith("id:"))
                {
                    module = GameManager.Entity.InstantiateShipModule(int.Parse(moduleName.Substring(3)));
                }
                else
                {
                    module = GameManager.Entity.InstantiateShipModule(moduleName);
                }

                lastSpawnPlayer.AddModule(module);
            }
        }

        public void OnAsPlayerBtnPress()
        {
            if (lastSpawnPlayer == null)
            {
                Debug.LogWarningFormat("{0}不是{1}", lastSpawn.name, typeof(EntityShip));
                return;
            }

            var input = GameManager.Input;
            var engine = lastSpawnPlayer
                .Modules
                .FirstOrDefault(module => module is ShipModuleEngineJob) as ShipModuleEngineJob;
            if (engine != null)
            {
                void Move(InputAction.CallbackContext ctx) => engine.OnInputCallbackJobMove(ctx);
                void Rotate(InputAction.CallbackContext ctx) => engine.OnInputCallbackShipEngineRotate(ctx);
                _movAct = Move;
                _rotAct = Rotate;
                input.Player.Move.started += Move;
                input.Player.Move.performed += Move;
                input.Player.Move.canceled += Move;
                input.Player.Point.started += Rotate;
                input.Player.Point.performed += Rotate;
                input.Player.Point.canceled += Rotate;
            }

            var weapon = lastSpawnPlayer.Modules
                    .FirstOrDefault(module => module is ShipModuleWeapon)
                as ShipModuleWeapon;
            if (weapon != null)
            {
                void FireStart(InputAction.CallbackContext ctx) => weapon.OnInputCallbackFireStart(ctx);
                void FireCancel(InputAction.CallbackContext ctx) => weapon.OnInputCallbackFireCancel(ctx);
                _firStartAct = FireStart;
                _firCancelAct = FireCancel;
                input.Player.Fire.started += FireStart;
                input.Player.Fire.canceled += FireCancel;
            }

            // GameManager.SetCameraFollowTarget(lastSpawnPlayer.transform);
        }

        public void OnDestroyEntityBtnPress()
        {
            if (lastSpawn is EntityShip)
            {
                var input = GameManager.Input;
                if (_movAct != null)
                {
                    input.Player.Move.canceled -= _movAct;
                    input.Player.Move.performed -= _movAct;
                    input.Player.Move.started -= _movAct;
                }

                if (_rotAct != null)
                {
                    input.Player.Point.started -= _rotAct;
                    input.Player.Point.performed -= _rotAct;
                    input.Player.Point.canceled -= _rotAct;
                }

                if (_firStartAct != null)
                {
                    input.Player.Fire.started -= _firStartAct;
                    input.Player.Fire.performed -= _firStartAct;
                    input.Player.Fire.canceled -= _firStartAct;
                }

                if (_firCancelAct != null)
                {
                    input.Player.Fire.started -= _firCancelAct;
                    input.Player.Fire.performed -= _firCancelAct;
                    input.Player.Fire.canceled -= _firCancelAct;
                }

                GameManager.SetCameraFollowTarget(null);
                GameManager.Entity.DestroyEntity(lastSpawnPlayer);
                _movAct = null;
                _rotAct = null;
                _firStartAct = null;
                _firCancelAct = null;
                lastSpawnPlayer = null;
            }
            else
            {
                GameManager.Entity.DestroyEntity(lastSpawn);
            }

            lastSpawn = null;
        }

        public void OnDestroyAllEntityBtnPress()
        {
            foreach (var entity in GameManager.Entity.ActiveEntity.ToArray())
            {
                if (entity == lastSpawnPlayer)
                {
                    OnDestroyEntityBtnPress();
                }
                else
                {
                    GameManager.Entity.DestroyEntity(entity);
                }
            }
        }
    }
}