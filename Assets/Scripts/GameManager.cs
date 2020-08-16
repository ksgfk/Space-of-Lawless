using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace KSGFK
{
    public enum GameState
    {
        PreInit,
        Init,
        Running,
        Pause,
        Exit
    }

    /// <summary>
    /// TODO:枪跟随鼠标指针旋转，而且以后AI也能用
    /// TODO:玩家模型正面始终朝鼠标，而且以后AI也能用
    /// TODO:子弹正确从枪口而不是弹匣冒出来
    /// TODO:子弹有生存时间，到时间自动销毁
    /// TODO:枪械开火间隔、弹匣容量、换弹冷却
    /// TODO:直接将物品塞入玩家背包
    /// TODO:可以在Debug面板指定物品生成坐标
    /// TODO:自动释放一次性事件
    ///
    /// 属性如果没有set，但有Set方法，说明只能修改一次，尽量不要调用
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
        private InputCenter _input;
        private MetaData _meta;
        private RegisterCenter _register;
        private EventCenter _event;

        public Camera MainCamera => _mainCamera;
        public InputCenter Input => _input;
        public Canvas UiCanvas => _uiCanvas;
        public GameState NowState => _nowState;
        public MetaData MetaData => _meta;
        public RegisterCenter Register => _register;
        public Nullable<World> World => new Nullable<World>(_world);
        public EventCenter Event => _event;

        /// <summary>
        /// 在各模块初始化完毕，未开始PreInit时设置事件回调
        /// </summary>
        public event Action BeforePreInit;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            ReadMetaData();
            InitComponents();
        }

        private async void Start()
        {
            BeforePreInit?.Invoke();
            BeforePreInit = null;
            await Addressables.InitializeAsync().Task;
            await PreInitGame();
            Addressables.InstantiateAsync("panel.debug").Completed += h => h.Result.GetComponent<PanelDebug>().Init();
            InitGame();
        }

        private void LateUpdate() { Jobs.Update(); }

        private void OnDestroy() { Jobs.Release(); }

        private void ReadMetaData()
        {
            using (var reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, "metadata.json")))
            {
                var str = reader.ReadToEnd();
                _meta = JsonUtility.FromJson<MetaData>(str);
            }
        }

        private void InitComponents()
        {
            _register = new RegisterCenter(this);
            _input = new InputCenter();
            _event = new EventCenter();
            Jobs.Update();
        }

        private async Task PreInitGame()
        {
            Register.RegisterRegistry();
            await Register.GetRegisterEntryData();
            await Register.PreRegister();
            Register.Register();
            Register.Remap();
            Register.Clean();
            Register.LogStatistic();
            _nowState = GameState.Init;
        }

        private void InitGame()
        {
            _input.Enable();
            _nowState = GameState.Running;
            Event.Post(this, new EventOnGameStart(this));
            Event.Unsubscribe(typeof(EventOnGameStart));
        }

        public void LoadWorld(int worldId, Action callback = null) { LoadWorld(Register.World[worldId], callback); }

        public void LoadWorld(string worldName, Action callback = null)
        {
            LoadWorld(Register.World[worldName], callback);
        }

        private async void LoadWorld(EntryWorld worldEntry, Action callback = null)
        {
            if (World.HasValue)
            {
                Debug.LogWarning("有活动中世界");
                return;
            }

            if (worldEntry == null)
            {
                return;
            }

            var instance = await Addressables.LoadSceneAsync(worldEntry.AssetAddr, LoadSceneMode.Additive).Task;
            var c = instance.Scene.GetRootGameObjects().FirstOrDefault(go => go.CompareTag("WorldCenter"));
            if (c == null)
            {
                Addressables.UnloadSceneAsync(instance);
                Debug.LogWarning("不是标准世界");
                return;
            }

            if (c.TryGetComponent(out _world))
            {
                _world.Init(this, in instance);
                SceneManager.SetActiveScene(instance.Scene);
                callback?.Invoke();
            }
            else
            {
                Addressables.UnloadSceneAsync(instance);
                Debug.LogWarning("不是标准世界");
            }
        }

        public async void UnloadWorld(Action callback = null)
        {
            if (!World.HasValue)
            {
                Debug.LogWarning("没有活动中世界");
                return;
            }

            await Addressables.UnloadSceneAsync(_world.Scene).Task;
            _world = null;
            callback?.Invoke();
        }

        public static void SetCameraFollowTarget(Transform target) { Instance._virtualCamera.Follow = target; }

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