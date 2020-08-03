using System;
using System.IO;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
    /// TODO:PoolCenter移入World中，随World一起释放
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
        private InputCenter _input;
        private PoolCenter _pool;
        private MetaData _meta;
        private RegisterCenter _register;

        public LoadManager Load => _load;
        public JobCenter Job => _job;
        public Camera MainCamera => _mainCamera;
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

        public void StartLoadWorld(int worldId, Action callback = null)
        {
            var entryMap = Register.Map[worldId];
            if (entryMap == null)
            {
                return;
            }

            StartLoadWorld(entryMap, callback);
        }

        public void StartLoadWorld(string worldId, Action callback = null)
        {
            var entryMap = Register.Map[worldId];
            if (entryMap == null)
            {
                return;
            }

            StartLoadWorld(entryMap, callback);
        }

        private void StartLoadWorld(EntryWorld entryWorld, Action callback)
        {
            if (World.HasValue)
            {
                Debug.LogWarning("已经有地图被加载");
                return;
            }

            if (!Load.CanReady)
            {
                Debug.LogWarning("现在不能加载地图");
                return;
            }

            _nowState = GameState.Pause;
            Load.Ready();
            var handle = Addressables.LoadSceneAsync(entryWorld.Addr, LoadSceneMode.Additive, false);
            handle.Completed += h =>
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError("加载场景失败");
                    return;
                }

                var result = h.Result;
                result.ActivateAsync().completed += op =>
                {
                    var objs = result.Scene
                        .GetRootGameObjects()
                        .FirstOrDefault(obj => obj.CompareTag("WorldCenter"));
                    if (objs == null)
                    {
                        Debug.LogError("找不到WorldCenter，不是标准场景");
                        Addressables.UnloadSceneAsync(result);
                    }
                    else
                    {
                        if (!objs.TryGetComponent<World>(out var world))
                        {
                            Debug.LogError("找不到World组件，不是标准场景");
                            Addressables.UnloadSceneAsync(result);
                        }
                        else
                        {
                            Instance._world = world;
                            SceneManager.SetActiveScene(result.Scene);
                            world.Init(this, in result);
                        }
                    }

                    Instance._nowState = GameState.Running;
                };
            };
            Load.Request(handle);
            Load.AddCompleteCallback(callback);
            Load.Work();
        }

        public void UnloadWorld(Action callback = null)
        {
            if (!World.HasValue)
            {
                Debug.LogWarning("没有地图被加载");
                return;
            }

            var world = World.Value;
            var sceneIns = world.Scene;
            world.Dispose();
            _world = null;
            var op = Addressables.UnloadSceneAsync(sceneIns);
            op.Completed += handle =>
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
                callback?.Invoke();
            };
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