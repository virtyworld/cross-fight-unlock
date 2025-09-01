using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;
using CrossFightUnlock.Presenters;

namespace CrossFightUnlock.Models
{
    /// <summary>
    /// Модель сундука, содержащая всю логику поведения сундука
    /// </summary>
    public class ChestModel : IModel
    {
        // Данные сундука
        private Vector3 _position;
        private bool _isOpened;
        private bool _isSpawned;
        private GameObject _chestGameObject;
        private ChestPresenter _chestPresenter;

        // Настройки
        private float _openAnimationDuration;
        private int _rewardAmount;
        private string _rewardType;
        private GameObject _chestPrefab;

        // Ссылки
        private GameEvents _gameEvents;

        // Состояние спавна
        private bool _isInitialized = false;
        private Vector3 _lastEnemyPosition;

        // Свойства для доступа к данным
        public Vector3 Position => _position;
        public bool IsOpened => _isOpened;
        public bool IsSpawned => _isSpawned;
        public GameObject ChestGameObject => _chestGameObject;
        public float OpenAnimationDuration => _openAnimationDuration;
        public int RewardAmount => _rewardAmount;
        public string RewardType => _rewardType;
        public ChestPresenter ChestPresenter => _chestPresenter;

        public ChestModel(GameEvents gameEvents, GameObject chestPrefab)
        {
            _gameEvents = gameEvents;
            _chestPrefab = chestPrefab;
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            // Инициализируем состояние
            _isOpened = false;
            _isSpawned = false;

            // Инициализируем настройки
            _openAnimationDuration = 1.0f;
            _rewardAmount = 100; // Можно добавить в настройки
            _rewardType = "Gold"; // Можно добавить в настройки

            // Подписываемся на события
            SubscribeToEvents();

            _isInitialized = true;
            Debug.Log("ChestModel initialized");
        }

        public void Cleanup()
        {
            if (!_isInitialized) return;

            // Отписываемся от событий
            UnsubscribeFromEvents();

            // Уничтожаем заспавненный сундук
            if (_chestGameObject != null)
            {
                Object.DestroyImmediate(_chestGameObject);
                _chestGameObject = null;
                _chestPresenter = null;
            }

            _gameEvents = null;
            _chestPrefab = null;
            _isInitialized = false;
        }

        /// <summary>
        /// Открытие сундука
        /// </summary>
        public void OpenChest()
        {
            if (_isOpened) return;

            _isOpened = true;

            // Уведомляем о открытии сундука
            _gameEvents.OnChestOpened?.Invoke(_chestGameObject);

            Debug.Log($"Chest opened! Reward: {_rewardAmount} {_rewardType}");
        }

        /// <summary>
        /// Установка позиции сундука
        /// </summary>
        public void SetPosition(Vector3 newPosition)
        {
            _position = newPosition;
        }

        /// <summary>
        /// Установка настройки награды
        /// </summary>
        public void SetReward(int amount, string type)
        {
            _rewardAmount = amount;
            _rewardType = type;
        }

        /// <summary>
        /// Установка длительности анимации открытия
        /// </summary>
        public void SetOpenAnimationDuration(float duration)
        {
            _openAnimationDuration = duration;
        }

        /// <summary>
        /// Спавн сундука
        /// </summary>
        public void SpawnChest()
        {
            if (!_isInitialized || _isSpawned) return;

            // Проверяем, что у нас есть позиция последнего врага
            if (_lastEnemyPosition == Vector3.zero)
            {
                Debug.LogError("ChestModel: No enemy position available for chest spawn!");
                return;
            }

            // Проверяем префаб сундука
            if (_chestPrefab == null)
            {
                Debug.LogError("ChestModel: Chest prefab not assigned!");
                return;
            }

            // Создаем сундук
            _chestGameObject = Object.Instantiate(_chestPrefab, _lastEnemyPosition, Quaternion.identity);

            // Получаем презентер
            _chestPresenter = _chestGameObject.GetComponent<ChestPresenter>();
            if (_chestPresenter == null)
            {
                Debug.LogError("ChestModel: ChestPresenter component not found on chest prefab!");
                Object.DestroyImmediate(_chestGameObject);
                _chestGameObject = null;
                return;
            }

            // Инициализируем презентер
            _chestPresenter.Initialize(_gameEvents, _lastEnemyPosition);

            // Показываем сундук
            _chestPresenter.Show();

            _isSpawned = true;
            _position = _lastEnemyPosition;

            // Уведомляем о спавне сундука
            _gameEvents.OnChestSpawned?.Invoke(_chestGameObject);

            Debug.Log($"ChestModel: Chest spawned at position {_lastEnemyPosition}");
        }

        /// <summary>
        /// Уничтожение сундука
        /// </summary>
        public void DestroyChest()
        {
            if (!_isInitialized || !_isSpawned || _chestGameObject == null) return;

            Object.DestroyImmediate(_chestGameObject);
            _chestGameObject = null;
            _chestPresenter = null;
            _isSpawned = false;

            Debug.Log("ChestModel: Chest destroyed");
        }

        /// <summary>
        /// Проверка, заспавнен ли сундук
        /// </summary>
        public bool IsChestSpawned()
        {
            return _isSpawned && _chestGameObject != null;
        }

        /// <summary>
        /// Получение заспавненного сундука
        /// </summary>
        public ChestPresenter GetSpawnedChest()
        {
            return _chestPresenter;
        }

        /// <summary>
        /// Подписка на события
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnAllEnemiesDestroyed.AddListener(OnAllEnemiesDestroyed);
            _gameEvents.OnEnemyDestroyed.AddListener(OnEnemyDestroyed);
        }

        /// <summary>
        /// Отписка от событий
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnAllEnemiesDestroyed.RemoveListener(OnAllEnemiesDestroyed);
            _gameEvents.OnEnemyDestroyed.RemoveListener(OnEnemyDestroyed);
        }

        /// <summary>
        /// Обработка события уничтожения врага
        /// </summary>
        private void OnEnemyDestroyed(GameObject destroyedEnemy)
        {
            if (!_isInitialized) return;

            // Сохраняем позицию последнего убитого врага
            _lastEnemyPosition = destroyedEnemy.transform.position;

            Debug.Log($"ChestModel: Enemy destroyed at position {_lastEnemyPosition}");
        }

        /// <summary>
        /// Обработка события уничтожения всех врагов
        /// </summary>
        private void OnAllEnemiesDestroyed()
        {
            if (!_isInitialized) return;

            Debug.Log("ChestModel: All enemies destroyed, spawning chest at last enemy position");
            SpawnChest();
        }
    }
}
