using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public enum GameState
    {
        PreInit,
        Init,
        PostInit,
        Running
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static LoadManager Load => Instance._load;
        public static JobCenter Job => Instance._job;
        public static DataCenter Data => Instance._data;
        public static Camera MainCamera => Instance.mainCamera;
        public static InputActionAsset InputAsset => Instance.playerInput;
        public static EntityManager Entity => Instance._entity;
        public static InputCenter Input => Instance._input;

        [SerializeField] private GameState nowState = GameState.PreInit;
        [SerializeField] private AssetReference playerInputAddr = null;
        public Camera mainCamera;
        public CinemachineVirtualCamera virtualCamera;
        private LoadManager _load;
        private JobCenter _job;
        private DataCenter _data;
        private EntityManager _entity;
        private InputCenter _input;
        [SerializeField] private InputActionAsset playerInput = null;

        /// <summary>
        /// 读取游戏数据
        /// </summary>
        public event Action PerInit;

        /// <summary>
        /// 加载所需资源
        /// </summary>
        public event Action Init;

        /// <summary>
        /// 后期处理
        /// </summary>
        public event Action PostInit;

        private void Awake()
        {
            // Cursor.visible = false;
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            _load = GetComponent<LoadManager>();
            _job = new JobCenter();
            _data = new DataCenter();
            _data.AddDataLoader("WindowsPlayer.csv", new CsvLoader(this));
            _entity = GetComponent<EntityManager>();

            InvokePerInit();
        }

        private void Update()
        {
            switch (nowState)
            {
                case GameState.Running:_job.OnUpdate();
                    break;
                case GameState.Init when _load.NowState == LoadState.Sleep:
                    OnPerInitComplete();
                    break;
                case GameState.PostInit when _load.NowState == LoadState.Sleep:
                    OnInitComplete();
                    break;
            }
        }

        public static void SetCameraFollowTarget(Transform target) { Instance.virtualCamera.Follow = target; }

        private void InvokePerInit()
        {
            _load.Init();
            _load.Ready();
            _entity.Init();
            PerInit?.Invoke();
            _load.Complete += () => nowState = GameState.Init;
            _data.StartLoad();
            _load.Work();
        }

        private void OnPerInitComplete()
        {
            _load.Ready();
            LoadPlayerInput();
            _load.Complete += () => nowState = GameState.PostInit;
            Init?.Invoke();
            _load.Work();
        }

        private void OnInitComplete()
        {
            InitJobSystems();
            PostInit?.Invoke();
            _input = new InputCenter();
            nowState = GameState.Running;
            _input.Enable();
            PerInit = null;
            Init = null;
            PostInit = null;

            var movJob = _job.GetJob<JobMove>("DefaultMoveJob");
            var rotJob = _job.GetJob<JobRotate>("DefaultRotateJob");
            var ship = _entity.SpawnShip(0);
            var engine = (ShipEngine) _entity.AddModuleToShip(ship, 0);
            var engineGo = engine.gameObject;
            var movProxy = engineGo.AddComponent<JobMoveProxy>();
            movJob.AddData(engine.CopyMoveData, movProxy);
            movProxy.target = ship.transform;
            var rotProxy = engineGo.AddComponent<JobRotateProxy>();
            rotJob.AddData(engine.CopyRotateData, rotProxy);
            rotProxy.target = ship.transform;
            
            _input.Player.Move.started += movProxy.OnInputCallback;
            _input.Player.Move.performed += movProxy.OnInputCallback;
            _input.Player.Move.canceled += movProxy.OnInputCallback;
            _input.Player.Delta.started += rotProxy.OnInputCallback;
            _input.Player.Delta.performed += rotProxy.OnInputCallback;
            _input.Player.Delta.canceled += rotProxy.OnInputCallback;

            SetCameraFollowTarget(ship.transform);
        }

        private void LoadPlayerInput()
        {
            var req = playerInputAddr.LoadAssetAsync<InputActionAsset>();
            req.Completed += request => playerInput = request.Result;
            _load.Request(req);
        }

        private void OnDestroy() { _job.Dispose(); }

        private void InitJobSystems()
        {
            _job.AddJob(new JobMove("DefaultMoveJob"));
            _job.AddJob(new JobRotate("DefaultRotateJob"));
        }
    }
}