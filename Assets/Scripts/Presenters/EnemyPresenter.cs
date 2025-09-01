using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Models;
using CrossFightUnlock.Views;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Presenters
{
    /// <summary>
    /// Презентер врага, связывающий модель и представление
    /// </summary>
    public class EnemyPresenter : MonoBehaviour, IPresenter
    {
        [Header("Components")]
        [SerializeField] private EnemyView _enemyView;

        // Модель и представление
        private EnemyModel _enemyModel;
        private EnemyView _view;

        // Настройки и события
        private EnemySpawnSettings _spawnSettings;
        private GameEvents _gameEvents;
        private Transform _playerTransform;

        // Состояние
        private bool _isInitialized = false;
        private bool _isVisible = true;

        #region IPresenter Implementation

        public void Initialize()
        {
            if (_isInitialized) return;

            // Получаем компоненты
            if (_enemyView == null) _enemyView = GetComponent<EnemyView>();
            if (_enemyView == null)
            {
                Debug.LogError("EnemyPresenter: EnemyView component not found!");
                return;
            }

            _view = _enemyView;
            _view.Initialize();

            _isInitialized = true;
            Debug.Log("EnemyPresenter initialized");
        }

        public void Cleanup()
        {
            if (!_isInitialized) return;

            // Отписываемся от событий
            UnsubscribeFromEvents();

            // Очищаем модель
            _enemyModel?.Cleanup();
            _enemyModel = null;

            // Очищаем представление
            _view?.Cleanup();
            _view = null;

            _gameEvents = null;
            _playerTransform = null;
            _isInitialized = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Инициализация с настройками поведения
        /// </summary>
        public void InitializeWithSettings(EnemySpawnSettings settings, GameEvents gameEvents, Transform playerTransform)
        {
            if (!_isInitialized) Initialize();

            _spawnSettings = settings;
            _gameEvents = gameEvents;
            _playerTransform = playerTransform;

            // Создаем модель врага
            _enemyModel = new EnemyModel(_spawnSettings, _gameEvents, transform.position, _playerTransform, gameObject);
            _enemyModel.Initialize();

            // Инициализируем представление
            _view.InitializeWithSettings(_spawnSettings, _gameEvents);
            _view.SetModel(_enemyModel);
            _view.SetPlayer(_playerTransform);

            // Подписываемся на события
            SubscribeToEvents();

            Debug.Log($"EnemyPresenter initialized with settings: Aggressive={_enemyModel.IsAggressive}, DetectionRange={_enemyModel.DetectionRange}");
        }

        /// <summary>
        /// Показать врага
        /// </summary>
        public void Show()
        {
            if (!_isInitialized) return;

            _isVisible = true;
            _view?.Show();
        }

        /// <summary>
        /// Скрыть врага
        /// </summary>
        public void Hide()
        {
            if (!_isInitialized) return;

            _isVisible = false;
            _view?.Hide();
        }

        /// <summary>
        /// Получение урона
        /// </summary>
        public void TakeDamage(float damage)
        {
            _enemyModel?.TakeDamage(damage);
        }

        /// <summary>
        /// Установка агрессивности
        /// </summary>
        public void SetAggressive(bool aggressive)
        {
            _enemyModel?.SetAggressive(aggressive);
        }

        /// <summary>
        /// Установка дистанции обнаружения
        /// </summary>
        public void SetDetectionRange(float range)
        {
            _enemyModel?.SetDetectionRange(range);
        }

        /// <summary>
        /// Установка игрока для преследования
        /// </summary>
        public void SetPlayer(Transform player)
        {
            _playerTransform = player;
            _enemyModel?.SetPlayer(player);
            _view?.SetPlayer(player);
        }

        /// <summary>
        /// Получение данных модели
        /// </summary>
        public EnemyModel GetModel()
        {
            return _enemyModel;
        }

        /// <summary>
        /// Получение представления
        /// </summary>
        public EnemyView GetView()
        {
            return _view;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Подписка на события
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnEnemyDeath.AddListener(OnEnemyDeath);
            _gameEvents.OnEnemyHealthChanged.AddListener(OnEnemyHealthChanged);
            _gameEvents.OnEnemyPositionChanged.AddListener(OnEnemyPositionChanged);
            _gameEvents.OnEnemyAttackStarted.AddListener(OnEnemyAttackStarted);
            _gameEvents.OnEnemyAttackFinished.AddListener(OnEnemyAttackFinished);
            _gameEvents.OnPlayerAttackedEnemy.AddListener(OnPlayerAttackedEnemy);
        }

        /// <summary>
        /// Отписка от событий
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnEnemyDeath.RemoveListener(OnEnemyDeath);
            _gameEvents.OnEnemyHealthChanged.RemoveListener(OnEnemyHealthChanged);
            _gameEvents.OnEnemyPositionChanged.RemoveListener(OnEnemyPositionChanged);
            _gameEvents.OnEnemyAttackStarted.RemoveListener(OnEnemyAttackStarted);
            _gameEvents.OnEnemyAttackFinished.RemoveListener(OnEnemyAttackFinished);
            _gameEvents.OnPlayerAttackedEnemy.RemoveListener(OnPlayerAttackedEnemy);
        }

        /// <summary>
        /// Обработка события смерти врага
        /// </summary>
        private void OnEnemyDeath(GameObject deadEnemy)
        {
            // Проверяем, что событие относится к этому врагу
            if (deadEnemy == gameObject)
            {
                Debug.Log($"EnemyPresenter: Enemy {gameObject.name} died, hiding...");
                Hide();
            }
        }

        /// <summary>
        /// Обработка изменения здоровья врага
        /// </summary>
        private void OnEnemyHealthChanged(float newHealth)
        {
            // Здесь можно добавить логику для обновления UI здоровья
            Debug.Log($"EnemyPresenter: Health changed to {newHealth}");
        }

        /// <summary>
        /// Обработка изменения позиции врага
        /// </summary>
        private void OnEnemyPositionChanged(Vector3 newPosition)
        {
            // Здесь можно добавить логику для обновления позиции

        }

        /// <summary>
        /// Обработка начала атаки врага
        /// </summary>
        private void OnEnemyAttackStarted()
        {
            Debug.Log("EnemyPresenter: Attack started");

            // Вызываем атаку в представлении
            _view?.AttackPlayer();
        }

        /// <summary>
        /// Обработка завершения атаки врага
        /// </summary>
        private void OnEnemyAttackFinished()
        {
            // Событие уже обработано в модели, здесь только логируем
            Debug.Log("EnemyPresenter: Attack finished event received");
        }

        /// <summary>
        /// Обработка атаки игрока по врагу
        /// </summary>
        private void OnPlayerAttackedEnemy(GameObject enemy)
        {
            // Проверяем, что атака направлена на этого врага
            if (enemy == gameObject)
            {
                _enemyModel?.TakeDamage(100f); // Стандартный урон
            }
        }

        #endregion

        #region Unity Lifecycle

        private void Update()
        {
            if (!_isInitialized || !_isVisible || _enemyModel == null) return;

            // Обновляем модель
            _enemyModel.UpdatePlayerDetection();
            _enemyModel.UpdateBehavior();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        #endregion

        #region Debug & Gizmos

        private void OnDrawGizmosSelected()
        {
            if (_enemyModel == null) return;

            // Рисуем радиус обнаружения
            Gizmos.color = _enemyModel.IsPlayerDetected ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _enemyModel.DetectionRange);

            // Рисуем радиус атаки
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _enemyModel.AttackRange);

            // Дистанция остановки
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _enemyModel.StoppingDistance);

            // Линия к игроку
            if (_enemyModel.IsPlayerDetected)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _enemyModel.LastPlayerPosition);
            }
        }

        #endregion
    }
}
