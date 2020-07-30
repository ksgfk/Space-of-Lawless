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

    /// <summary>
    /// TODO:将DataCenter职能并入RegisterCenter
    /// TODO:World管理场景中实体，而不是用EntityManager
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static readonly string DataRoot = Path.Combine(Application.streamingAssetsPath, "Data");
        public static GameManager Instance { get; private set; }
        public LoadManager Load => _load;
        public JobCenter Job => _job;
        public Camera MainCamera => mainCamera;
        public InputActionAsset InputAsset => playerInput;
        public EntityManager Entity => _entity;
        public InputCenter Input => _input;
        public PoolCenter Pool => _pool;
        public Canvas UiCanvas => uiCanvas;
        public GameState NowState => nowState;
        public MetaData MetaData => _meta;
        public RegisterCenter Register => _register;

        [SerializeField] private GameState nowState = GameState.PreInit;
        [SerializeField] private AssetReference playerInputAddr = null;
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private CinemachineVirtualCamera virtualCamera = null;
        [SerializeField] private Canvas uiCanvas = null;
        private LoadManager _load;
        private JobCenter _job;
        private EntityManager _entity;
        private InputCenter _input;
        [SerializeField] private InputActionAsset playerInput = null;
        private PoolCenter _pool;
        private MetaData _meta;
        private RegisterCenter _register;

        /// <summary>
        /// 读取游戏数据
        /// </summary>
        public event Action<GameManager> PerInit;

        /// <summary>
        /// 加载所需资源
        /// </summary>
        public event Action<GameManager> Init;

        /// <summary>
        /// 后期处理
        /// </summary>
        public event Action<GameManager> PostInit;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            using (var reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, "metadata.json")))
            {
                var str = reader.ReadToEnd();
                _meta = JsonUtility.FromJson<MetaData>(str);
            }

            _load = GetComponent<LoadManager>();
            _job = new JobCenter(this);
            _pool = new PoolCenter();
            _entity = GetComponent<EntityManager>();
            _register = new RegisterCenter(this);

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
        
        private void OnDestroy() { _job.Dispose(); }

        public static void SetCameraFollowTarget(Transform target) { Instance.virtualCamera.Follow = target; }

        private void InvokePerInit()
        {
            _load.Init();
            _load.Ready();
            _entity.Init(this);
            PerInit?.Invoke(this);
            _load.Complete += () => Instance.nowState = GameState.Init;
            _load.Work();
        }

        private void OnPerInitComplete()
        {
            _load.Ready();
            LoadPlayerInput();
            _load.Complete += () => Instance.nowState = GameState.PostInit;
            Init?.Invoke(this);
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
            PostInit?.Invoke(this);
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
            req.Completed += request => Instance.playerInput = request.Result;
            _load.Request(req);
        }

        public static string GetDataPath(string fileName) { return Path.Combine(DataRoot, fileName); }

        public static string GetPlatform()
        {
#if UNITY_EDITOR_WIN
            var platform = RuntimePlatform.WindowsPlayer.ToString();
#else
            var platform = Application.platform.ToString();
#endif
            return platform;
        }
    }
}