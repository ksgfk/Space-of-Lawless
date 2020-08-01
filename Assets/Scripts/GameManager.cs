using System;
using System.IO;
using Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

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
    /// TODO:加载World
    /// TODO:PoolCenter移入World中，随World一起释放
    /// TODO:World管理场景中实体，而不是用EntityManager
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static readonly string DataRoot = Path.Combine(Application.streamingAssetsPath, "Data");
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameState _nowState = GameState.PreInit;
        [SerializeField] private Camera _mainCamera = null;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera = null;
        [SerializeField] private Canvas _uiCanvas = null;
        [SerializeField] private World _world = null;
        private LoadManager _load;
        private JobCenter _job;
        private EntityManager _entity;
        private InputCenter _input;
        private PoolCenter _pool;
        private MetaData _meta;
        private RegisterCenter _register;

        public LoadManager Load => _load;
        public JobCenter Job => _job;
        public Camera MainCamera => _mainCamera;
        public EntityManager Entity => _entity;
        public InputCenter Input => _input;
        public PoolCenter Pool => _pool;
        public Canvas UiCanvas => _uiCanvas;
        public GameState NowState => _nowState;
        public MetaData MetaData => _meta;
        public RegisterCenter Register => _register;
        public Nullable<World> World => new Nullable<World>(_world);

        /// <summary>
        /// 在各模块初始化完毕，未开始PreInit时设置事件回调
        /// </summary>
        public event Action SetCallbackBeforePreInit;

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

        private async void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            using (var reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, "metadata.json")))
            {
                var str = await reader.ReadToEndAsync();
                _meta = JsonUtility.FromJson<MetaData>(str);
            }

            await Addressables.InitializeAsync().Task;
            _load = GetComponent<LoadManager>();
            _job = new JobCenter(this);
            _pool = new PoolCenter();
            _entity = GetComponent<EntityManager>();
            _register = new RegisterCenter(this);
            _input = new InputCenter();
            SetCallbackBeforePreInit?.Invoke();
            SetCallbackBeforePreInit = null;

            StartPerInit();
        }

        private void Update()
        {
            switch (_nowState)
            {
                case GameState.Running:
                    _job.OnUpdate();
                    break;
                case GameState.Init when _load.NowState == LoadState.Sleep:
                    CompletePerInit();
                    break;
                case GameState.PostInit when _load.NowState == LoadState.Sleep:
                    CompleteInit();
                    break;
            }
        }

        private void OnDestroy() { _job?.Dispose(); }

        public static void SetCameraFollowTarget(Transform target) { Instance._virtualCamera.Follow = target; }

        private void StartPerInit()
        {
            _load.Init();
            _load.Ready();
            _entity.Init(this);
            PerInit?.Invoke(this);
            _load.AddCompleteCallback(() => Instance._nowState = GameState.Init);
            _load.Work();
        }

        private void CompletePerInit()
        {
            _load.Ready();
            _load.AddCompleteCallback(() => Instance._nowState = GameState.PostInit);
            Init?.Invoke(this);
            _load.Request("panel.debug",
                (AsyncOperationHandle<GameObject> handle) =>
                {
                    Helper.GetAsyncOpResult(handle)
                        ?.Instantiate(Instance.UiCanvas.transform)
                        ?.GetComponent<PanelDebug>()
                        .Init();
                });
            _load.Work();
        }

        private void CompleteInit()
        {
            PostInit?.Invoke(this);
            _nowState = GameState.Running;
            _input.Enable();
            PerInit = null;
            Init = null;
            PostInit = null;
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