using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;
using CrossFightUnlock.Models;
using CrossFightUnlock.Views;

namespace CrossFightUnlock.Presenters
{
    /// <summary>
    /// Презентер сундука, связывающий модель и представление
    /// </summary>
    public class ChestPresenter : MonoBehaviour, IPresenter
    {
        [Header("Chest Settings")]
        [SerializeField] private int _rewardAmount = 100;
        [SerializeField] private float _openAnimationDuration = 1.0f;

        // Компоненты
        private ChestModel _chestModel;
        private ChestView _chestView;
        private GameEvents _gameEvents;

        // Состояние
        private bool _isInitialized = false;
        private Vector3 _spawnPosition;

        #region Unity Lifecycle

        private void Awake()
        {
            // Получаем компоненты
            _chestView = GetComponent<ChestView>();
            if (_chestView == null)
            {
                Debug.LogError("ChestPresenter: ChestView component not found!");
            }
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Инициализация презентера (реализация интерфейса)
        /// </summary>
        public void Initialize()
        {
            Debug.LogWarning("ChestPresenter: Initialize() called without parameters. Use Initialize(GameEvents, Vector3) instead.");
        }

        /// <summary>
        /// Инициализация презентера с параметрами
        /// </summary>
        public void Initialize(GameEvents gameEvents, Vector3 spawnPosition)
        {
            if (_isInitialized) return;

            _gameEvents = gameEvents;
            _spawnPosition = spawnPosition;

            // Создаем модель (теперь без префаба, так как объект уже создан)
            _chestModel = new ChestModel(_gameEvents, null);
            _chestModel.Initialize();
            _chestModel.SetOpenAnimationDuration(_openAnimationDuration);
            _chestModel.SetPosition(_spawnPosition);

            // Инициализируем представление
            if (_chestView != null)
            {
                _chestView.SetGameEvents(_gameEvents);
                _chestView.Initialize();
                _chestView.SetPosition(_spawnPosition);
            }

            // Подписываемся на события
            SubscribeToEvents();

            _isInitialized = true;
            Debug.Log("ChestPresenter initialized");
        }

        /// <summary>
        /// Очистка презентера
        /// </summary>
        public void Cleanup()
        {
            if (!_isInitialized) return;

            // Отписываемся от событий
            UnsubscribeFromEvents();

            // Очищаем модель
            _chestModel?.Cleanup();
            _chestModel = null;

            // Очищаем представление
            _chestView?.Cleanup();

            _isInitialized = false;
        }

        /// <summary>
        /// Показать сундук
        /// </summary>
        public void Show()
        {
            if (!_isInitialized) return;

            _chestView?.Show();
        }

        /// <summary>
        /// Скрыть сундук
        /// </summary>
        public void Hide()
        {
            if (!_isInitialized) return;

            _chestView?.Hide();
        }

        /// <summary>
        /// Открыть сундук
        /// </summary>
        public void OpenChest()
        {
            if (!_isInitialized || _chestModel.IsOpened) return;

            _chestModel.OpenChest();
            _chestView?.OpenChest();
        }

        /// <summary>
        /// Проверка, может ли игрок взаимодействовать с сундуком
        /// </summary>
        public bool CanInteract(Transform playerTransform)
        {
            if (!_isInitialized) return false;

            return _chestView?.CanInteract(playerTransform) ?? false;
        }

        /// <summary>
        /// Получение позиции сундука
        /// </summary>
        public Vector3 GetPosition()
        {
            if (!_isInitialized) return Vector3.zero;

            return _chestView?.GetPosition() ?? Vector3.zero;
        }

        /// <summary>
        /// Проверка, открыт ли сундук
        /// </summary>
        public bool IsOpened()
        {
            if (!_isInitialized) return false;

            return _chestModel?.IsOpened ?? false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Подписка на события
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnChestOpened.AddListener(OnChestOpened);
        }

        /// <summary>
        /// Отписка от событий
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnChestOpened.RemoveListener(OnChestOpened);
        }

        /// <summary>
        /// Обработка события открытия сундука
        /// </summary>
        private void OnChestOpened(GameObject chest)
        {
            // Проверяем, что событие относится к этому сундуку
            if (chest == gameObject)
            {
                Debug.Log($"ChestPresenter: Chest {gameObject.name} opened!");
                // Здесь можно добавить дополнительную логику при открытии сундука
                _gameEvents.OnChestButtonClicked?.Invoke();
            }
        }

        #endregion
    }
}
