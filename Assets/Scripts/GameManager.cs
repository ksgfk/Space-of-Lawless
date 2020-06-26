using System;
using System.IO;
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
        Running,
        Pause,
        Exit
    }
    
    public class GameManager : MonoBehaviour
    {
        public static readonly string DataRoot = Path.Combine(Application.streamingAssetsPath, "Data");
        public static GameManager Instance { get; private set; }
        public static LoadManager Load => Instance._load;
        public static JobCenter Job => Instance._job;
        public static DataCenter Data => Instance._data;
        public static Camera MainCamera => Instance.mainCamera;
        public static InputActionAsset InputAsset => Instance.playerInput;
        public static EntityManager Entity => Instance._entity;
        public static InputCenter Input => Instance._input;
        public static PoolCenter Pool => Instance._pool;
        public static Canvas UiCanvas => Instance.uiCanvas;
        public static GameState NowState => Instance.nowState;

        [SerializeField] private GameState nowState = GameState.PreInit;
        [SerializeField] private AssetReference playerInputAddr = null;
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private CinemachineVirtualCamera virtualCamera = null;
        [SerializeField] private Canvas uiCanvas = null;
        private LoadManager _load;
        private JobCenter _job;
        private DataCenter _data;
        private EntityManager _entity;
        private InputCenter _input;
        [SerializeField] private InputActionAsset playerInput = null;
        private PoolCenter _pool;

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
            _pool = new PoolCenter();
            _data.AddDataLoader("WindowsPlayer.csv", new CsvLoader(this));
            _entity = GetComponent<EntityManager>();

            InvokePerInit();
        }

        private void Update()
        {
            switch (nowState)
            {
                case GameState.Running:
                    _job.OnUpdate();
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
            _load.Request("panel.debug",
                (GameObject prefab) =>
                {
                    var go = Instantiate(prefab, Instance.uiCanvas.transform);
                    var debug = go.GetComponent<PanelDebug>();
                    debug.Init();
                });
            _load.Work();
        }

        private void OnInitComplete()
        {
            PostInit?.Invoke();
            _input = new InputCenter();
            nowState = GameState.Running;
            _input.Enable();
            PerInit = null;
            Init = null;
            PostInit = null;
        }

        private void LoadPlayerInput()
        {
            var req = playerInputAddr.LoadAssetAsync<InputActionAsset>();
            req.Completed += request => playerInput = request.Result;
            _load.Request(req);
        }

        private void OnDestroy() { _job.Dispose(); }
    }
}