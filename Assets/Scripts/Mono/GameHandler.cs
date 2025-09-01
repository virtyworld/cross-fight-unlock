using UnityEngine;
using CrossFightUnlock.Data;
using CrossFightUnlock.Managers;
using CrossFightUnlock.Presenters;
using CrossFightUnlock.Views;
using CrossFightUnlock.Interfaces;
using System.Collections;

namespace CrossFightUnlock.Mono
{
    /// <summary>
    /// Главный управляющий класс игры, инициализирующий все компоненты
    /// </summary>
    public class GameHandler : MonoBehaviour
    {
        [Header("Game Data")]
        [SerializeField] private GameSettings _gameSettings;


        [Header("Player")]
        [SerializeField] private PlayerPresenter _playerPresenter;
        [SerializeField] private Transform _playerSpawnPoint;

        [Header("Managers")]
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private ObjectLinesManager _objectLinesManager;
        [SerializeField] private EnemySpawnManager _enemySpawnManager;
        [SerializeField] private ChestSpawnManager _chestSpawnManager;
        [SerializeField] private CameraManager _cameraManager;
        [SerializeField] private UnlockPresenter _unlockPresenter;
        [Header("Game State")]
        [SerializeField] private bool _isGamePaused = false;
        [SerializeField] private bool _isGameInitialized = false;
        [SerializeField] private UIHandler _uiHandler;


        private InputManager _input;
        private GameEvents _gameEvents;

        #region Unity Lifecycle

        private void Awake()
        {
            InitGameEvents();
        }

        private void Start()
        {
            SubscribeToEvents();
            InitInputManager();
            InitCameraManager();
            InitPlayer();
            InitObjectLinesManager();
            InitEnemySpawnManager();
            InitChestSpawnManager();
            InitUIHandler();
            InitUnlockPresenter();
            StartGame();
        }


        private void OnDestroy()
        {
            CleanupGame();
            UnsubscribeFromEvents();
        }


        #endregion

        #region Initialization


        private void InitGameEvents()
        {
            Debug.Log("InitGameEvents");
            // Создаем GameEvents как обычный класс
            _gameEvents = new GameEvents();
            _gameEvents.Init();
        }
        /// <summary>
        /// Подписка на игровые события
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnGameStart.AddListener(HandleGameStart);
            _gameEvents.OnGamePause.AddListener(HandleGamePause);
            _gameEvents.OnGameResume.AddListener(HandleGameResume);
            _gameEvents.OnGameEnd.AddListener(HandleGameEnd);
            _gameEvents.OnCleanup.AddListener(HandleCleanup);
        }
        private void InitInputManager()
        {
            _inputManager.Initialize(_gameEvents);
            _input = _inputManager;
        }
        private void InitPlayer()
        {
            _playerPresenter.Initialize(_gameSettings, _gameEvents, _playerSpawnPoint.position);
        }
        private void InitObjectLinesManager()
        {
            _objectLinesManager.Initialize();
            _objectLinesManager.StartAllLines();
        }
        private void InitEnemySpawnManager()
        {
            _enemySpawnManager.Initialize(_gameEvents);
        }
        private void InitChestSpawnManager()
        {
            _chestSpawnManager.Initialize(_gameEvents);
        }
        private void InitUIHandler()
        {
            _uiHandler.Initialize(_gameEvents);
        }
        private void InitCameraManager()
        {
            _cameraManager.Initialize(_gameEvents);
        }
        private void InitUnlockPresenter()
        {
            _unlockPresenter.Initialize(_gameEvents);
        }
        /// <summary>
        /// Отписка от игровых событий
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnGameStart.RemoveListener(HandleGameStart);
            _gameEvents.OnGamePause.RemoveListener(HandleGamePause);
            _gameEvents.OnGameResume.RemoveListener(HandleGameResume);
            _gameEvents.OnGameEnd.RemoveListener(HandleGameEnd);
            _gameEvents.OnCleanup.RemoveListener(HandleCleanup);
        }

        #endregion

        #region Game Control

        /// <summary>
        /// Запуск игры
        /// </summary>
        private void StartGame()
        {
            _gameEvents.OnGameStart?.Invoke();
            Debug.Log("Game started!");
        }

        /// <summary>
        /// Пауза игры
        /// </summary>
        public void PauseGame()
        {
            if (_isGamePaused) return;

            _isGamePaused = true;
            Time.timeScale = 0f;

            if (_input != null)
                _input.SetInputEnabled(false);

            _gameEvents.OnGamePause?.Invoke();
            Debug.Log("Game paused");
        }

        /// <summary>
        /// Возобновление игры
        /// </summary>
        public void ResumeGame()
        {
            if (!_isGamePaused) return;

            _isGamePaused = false;
            Time.timeScale = _gameSettings.GameTimeScale;

            if (_input != null)
                _input.SetInputEnabled(true);

            _gameEvents.OnGameResume?.Invoke();
            Debug.Log("Game resumed");
        }

        /// <summary>
        /// Остановка игры
        /// </summary>
        public void StopGame()
        {
            _gameEvents.OnGameEnd?.Invoke();
            Debug.Log("Game stopped");
        }

        #endregion

        #region Event Handlers

        private void HandleGameStart()
        {
            Debug.Log("Game start event received");
        }

        private void HandleGamePause()
        {
            Debug.Log("Game pause event received");
        }

        private void HandleGameResume()
        {
            Debug.Log("Game resume event received");
        }

        private void HandleGameEnd()
        {
            Debug.Log("Game end event received");
        }



        private void HandleCleanup()
        {
            Debug.Log("Cleanup event received");
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Очистка игры
        /// </summary>
        private void CleanupGame()
        {
            if (!_isGameInitialized) return;

            Debug.Log("Cleaning up game...");

            // Вызываем событие очистки
            _gameEvents?.OnCleanup?.Invoke();

            // Отписываемся от событий
            UnsubscribeFromEvents();

            // Очищаем компоненты (они уже очистились через событие OnCleanup)
            _playerPresenter = null;
            _input = null;

            _isGameInitialized = false;
            Debug.Log("Game cleanup completed");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Переключение паузы
        /// </summary>
        public void TogglePause()
        {
            if (_isGamePaused)
                ResumeGame();
            else
                PauseGame();
        }

        /// <summary>
        /// Получение настроек игры
        /// </summary>
        public GameSettings GetGameSettings()
        {
            return _gameSettings;
        }

        /// <summary>
        /// Получение событий игры
        /// </summary>
        public GameEvents GetGameEvents()
        {
            return _gameEvents;
        }

        /// <summary>
        /// Проверка, инициализирована ли игра
        /// </summary>
        public bool IsGameInitialized()
        {
            return _isGameInitialized;
        }

        /// <summary>
        /// Проверка, на паузе ли игра
        /// </summary>
        public bool IsGamePaused()
        {
            return _isGamePaused;
        }

        #endregion
    }
}
