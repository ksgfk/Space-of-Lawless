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
    /// TODO:RegisterCenter管理所有注册项
    /// TODO:World管理场景中实体，而不是用EntityManager
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static readonly string DataRoot = Path.Combine(Application.streamingAssetsPath, "Data");
        public static GameManager Instance { get; private set; }
        public static LoadManager Load => Instance._load;
        public static JobCenter Job => Instance._job;

        /// <summary>
        /// 游戏初始化时暂时的数据存放点
        /// 生命周期于PostInit事件发布后截至
        /// </summary>
        public static DataCenter TempData => Instance._data;

        public static Camera MainCamera => Instance.mainCamera;
        public static InputActionAsset InputAsset => Instance.playerInput;
        public static EntityManager Entity => Instance._entity;
        public static InputCenter Input => Instance._input;
        public static PoolCenter Pool => Instance._pool;
        public static Canvas UiCanvas => Instance.uiCanvas;
        public static GameState NowState => Instance.nowState;
        public static MetaData MetaData => Instance._meta;

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
        private MetaData _meta;

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

            using (var reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, "metadata.json")))
            {
                var str = reader.ReadToEnd();
                _meta = JsonUtility.FromJson<MetaData>(str);
            }

            _load = GetComponent<LoadManager>();
            _job = new JobCenter();
            _data = new DataCenter();
            _pool = new PoolCenter();
            _data.AddDataLoader(new CsvWinLoader());
            _entity = GetComponent<EntityManager>();
            AddAllDataPath();

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
            _data = null;
        }

        private void LoadPlayerInput()
        {
            var req = playerInputAddr.LoadAssetAsync<InputActionAsset>();
            req.Completed += request => playerInput = request.Result;
            _load.Request(req);
        }

        private void AddAllDataPath()
        {
            foreach (var entityInfo in _meta.EntityInfo)
            {
                try
                {
                    _data.AddPath(Type.GetType(entityInfo.Type), GetDataPath(entityInfo.Path));
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            foreach (var jobInfo in _meta.JobInfo)
            {
                try
                {
                    _data.AddPath(Type.GetType(jobInfo.Type), GetDataPath(jobInfo.Path));
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        private void OnDestroy() { _job.Dispose(); }

        public static string GetDataPath(string fileName) { return Path.Combine(DataRoot, fileName); }
    }
}