using UnityEngine;
using CrossFightUnlock.Data;
using CrossFightUnlock.Views;

namespace CrossFightUnlock.Managers
{
    /// <summary>
    /// Менеджер для управления камерой
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private CameraSettings cameraSettings;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private CameraFollow cameraFollow;
        [SerializeField] private bool autoFindCamera = true;
        [SerializeField] private bool autoAddCameraFollow = true;

        private GameEvents _gameEvents;
        private bool _isInitialized = false;

        #region Unity Lifecycle

        private void Awake()
        {
            // Автоматически находим главную камеру если не назначена
            if (autoFindCamera && mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    Debug.LogError("CameraManager: Main camera not found!");
                    return;
                }
            }

            // Автоматически добавляем компонент CameraFollow если его нет
            if (autoAddCameraFollow && cameraFollow == null)
            {
                cameraFollow = mainCamera.GetComponent<CameraFollow>();
                if (cameraFollow == null)
                {
                    cameraFollow = mainCamera.gameObject.AddComponent<CameraFollow>();
                    Debug.Log("CameraManager: Added CameraFollow component to main camera");
                }
            }
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Инициализация менеджера камеры
        /// </summary>
        public void Initialize(GameEvents gameEvents)
        {
            if (_isInitialized) return;

            _gameEvents = gameEvents;

            // Проверяем компоненты
            if (mainCamera == null)
            {
                Debug.LogError("CameraManager: Main camera is null!");
                return;
            }

            if (cameraFollow == null)
            {
                Debug.LogError("CameraManager: CameraFollow component is null!");
                return;
            }

            // Подписываемся на события
            SubscribeToEvents();

            _isInitialized = true;
            Debug.Log("CameraManager initialized");
        }

        /// <summary>
        /// Очистка менеджера
        /// </summary>
        public void Cleanup()
        {
            if (!_isInitialized) return;

            // Отписываемся от событий
            UnsubscribeFromEvents();

            // Очищаем компонент CameraFollow
            if (cameraFollow != null)
            {
                cameraFollow.Cleanup();
            }

            _gameEvents = null;
            _isInitialized = false;
        }

        /// <summary>
        /// Установка цели для камеры
        /// </summary>
        public void SetCameraTarget(Transform target)
        {
            if (!_isInitialized || cameraFollow == null) return;

            cameraFollow.SetTarget(target);
            Debug.Log($"CameraManager: Camera target set to {target.name}");
        }

        /// <summary>
        /// Получение главной камеры
        /// </summary>
        public Camera GetMainCamera()
        {
            return mainCamera;
        }

        /// <summary>
        /// Получение компонента CameraFollow
        /// </summary>
        public CameraFollow GetCameraFollow()
        {
            return cameraFollow;
        }

        /// <summary>
        /// Переключение в режим раннера
        /// </summary>
        public void SwitchToRunnerMode()
        {
            if (cameraFollow != null && cameraSettings != null)
            {
                cameraFollow.SetupRunnerCamera(
                    cameraSettings.RunnerHeight,
                    cameraSettings.RunnerDistance,
                    cameraSettings.RunnerForward);
                Debug.Log("CameraManager: Switched to runner mode");
            }
        }

        /// <summary>
        /// Переключение в обычный режим
        /// </summary>
        public void SwitchToNormalMode()
        {
            if (cameraFollow != null)
            {
                cameraFollow.SetupNormalCamera();
                Debug.Log("CameraManager: Switched to normal mode");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Подписка на события
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_gameEvents == null) return;
            Debug.Log("CameraManager: Subscribing to events");
            _gameEvents.OnPlayerSpawned.AddListener(OnPlayerSpawned);
            _gameEvents.OnPlayerDeath.AddListener(OnPlayerDeath);
            _gameEvents.OnPlayerRespawn.AddListener(OnPlayerRespawn);
            _gameEvents.OnCleanup.AddListener(OnCleanup);
        }

        /// <summary>
        /// Отписка от событий
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnPlayerSpawned.RemoveListener(OnPlayerSpawned);
            _gameEvents.OnPlayerDeath.RemoveListener(OnPlayerDeath);
            _gameEvents.OnPlayerRespawn.RemoveListener(OnPlayerRespawn);
            _gameEvents.OnCleanup.RemoveListener(OnCleanup);
        }

        /// <summary>
        /// Обработка спавна игрока
        /// </summary>
        private void OnPlayerSpawned()
        {
            Debug.Log($"CameraManager: OnPlayerSpawned event received {_isInitialized}");
            if (!_isInitialized) return;

            // Находим игрока и устанавливаем его как цель для камеры
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                SetCameraTarget(player.transform);

                // Настраиваем камеру для режима раннера
                if (cameraSettings != null && cameraSettings.IsRunnerMode && cameraFollow != null)
                {
                    cameraFollow.SetupRunnerCamera(
                        cameraSettings.RunnerHeight,
                        cameraSettings.RunnerDistance,
                        cameraSettings.RunnerForward);
                    cameraFollow.Initialize(player.transform, cameraSettings);
                    Debug.Log("CameraManager: Runner camera mode configured");
                }

                Debug.Log("CameraManager: Player spawned, camera target set");
            }
            else
            {
                Debug.LogWarning("CameraManager: Player not found after spawn event");
            }
        }

        /// <summary>
        /// Обработка смерти игрока
        /// </summary>
        private void OnPlayerDeath()
        {
            if (!_isInitialized) return;

            Debug.Log("CameraManager: Player died, camera target cleared");
            // Можно добавить логику для камеры при смерти игрока
        }

        /// <summary>
        /// Обработка респауна игрока
        /// </summary>
        private void OnPlayerRespawn()
        {
            if (!_isInitialized) return;

            // Находим игрока и устанавливаем его как цель для камеры
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                SetCameraTarget(player.transform);
                Debug.Log("CameraManager: Player respawned, camera target set");
            }
        }

        /// <summary>
        /// Обработка события очистки
        /// </summary>
        private void OnCleanup()
        {
            if (!_isInitialized) return;

            Debug.Log("CameraManager: Cleanup event received");
            Cleanup();
        }

        #endregion      
    }
}
