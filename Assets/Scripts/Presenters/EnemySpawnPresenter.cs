using UnityEngine;
using System.Collections;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;
using CrossFightUnlock.Models;
using CrossFightUnlock.Views;

namespace CrossFightUnlock.Presenters
{
    /// <summary>
    /// Презентер для управления спавном врагов, связывающий модель и представление
    /// </summary>
    public class EnemySpawnPresenter : MonoBehaviour, IEnemySpawnPresenter
    {
        [Header("Spawn Settings")]
        [SerializeField] private EnemySpawnSettings spawnSettings;
        [SerializeField] private FinishTriggerView finishTriggerView;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private bool autoFindPlayer = true;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool enableGizmos = true;

        // Компоненты
        private EnemySpawnModel _spawnModel;
        private GameEvents _gameEvents;

        // Состояние
        private bool _isInitialized = false;
        private bool _isSpawning = false;
        private Coroutine _spawnCoroutine;

        #region Unity Lifecycle

        private void Awake()
        {
            // Автоматически находим игрока если нужно
            if (autoFindPlayer && playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                    Debug.Log("EnemySpawnPresenter: Found player automatically");
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
        /// Инициализация презентера
        /// </summary>
        public void Initialize(GameEvents gameEvents)
        {
            if (_isInitialized) return;

            _gameEvents = gameEvents;

            // Проверяем настройки
            if (spawnSettings == null)
            {
                Debug.LogError("EnemySpawnPresenter: SpawnSettings not assigned!");
                return;
            }

            // Создаем модель
            _spawnModel = new EnemySpawnModel(spawnSettings, _gameEvents);
            _spawnModel.Initialize();

            // Инициализируем представление
            if (finishTriggerView != null)
            {
                finishTriggerView.SetGameEvents(_gameEvents);
                finishTriggerView.Initialize();
            }

            // Подписываемся на события
            SubscribeToEvents();

            _isInitialized = true;
            Debug.Log("EnemySpawnPresenter initialized");
        }

        /// <summary>
        /// Очистка презентера
        /// </summary>
        public void Cleanup()
        {
            if (!_isInitialized) return;

            // Отписываемся от событий
            UnsubscribeFromEvents();

            // Останавливаем корутину спавна
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }

            // Очищаем модель
            _spawnModel?.Cleanup();
            _spawnModel = null;

            _isInitialized = false;
        }

        /// <summary>
        /// Установка событий игры
        /// </summary>
        public void SetGameEvents(GameEvents gameEvents)
        {
            _gameEvents = gameEvents;
        }

        /// <summary>
        /// Очистка всех врагов
        /// </summary>
        public void ClearAllEnemies()
        {
            if (!_isInitialized) return;

            _spawnModel.ClearSpawnedEnemies();
            Debug.Log("EnemySpawnPresenter: All enemies cleared");
        }

        /// <summary>
        /// Получение количества заспавненных врагов
        /// </summary>
        public int GetSpawnedEnemiesCount()
        {
            return _isInitialized ? _spawnModel.SpawnedEnemiesCount : 0;
        }

        /// <summary>
        /// Проверка, идет ли спавн
        /// </summary>
        public bool IsSpawning()
        {
            return _isSpawning;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Подписка на события
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnFinishTriggerEntered.AddListener(OnFinishTriggerEntered);
            _gameEvents.OnFinishTriggerExited.AddListener(OnFinishTriggerExited);
            _gameEvents.OnEnemySpawned.AddListener(OnEnemySpawned);
            _gameEvents.OnEnemyDestroyed.AddListener(OnEnemyDestroyed);
        }

        /// <summary>
        /// Отписка от событий
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnFinishTriggerEntered.RemoveListener(OnFinishTriggerEntered);
            _gameEvents.OnFinishTriggerExited.RemoveListener(OnFinishTriggerExited);
            _gameEvents.OnEnemySpawned.RemoveListener(OnEnemySpawned);
            _gameEvents.OnEnemyDestroyed.RemoveListener(OnEnemyDestroyed);
        }

        /// <summary>
        /// Обработка входа в триггер финиша
        /// </summary>
        private void OnFinishTriggerEntered(GameObject player)
        {
            if (!_isInitialized || _isSpawning) return;

            Debug.Log("EnemySpawnPresenter: Finish trigger entered, starting enemy spawn");

            // Активируем триггер в модели
            _spawnModel.TriggerSpawn();

            // Запускаем спавн врагов
            StartEnemySpawn();
        }

        /// <summary>
        /// Обработка выхода из триггера финиша
        /// </summary>
        private void OnFinishTriggerExited(GameObject player)
        {
            if (!_isInitialized) return;

            Debug.Log("EnemySpawnPresenter: Finish trigger exited");

            // Сбрасываем триггер
            _spawnModel.ResetTrigger();
        }

        /// <summary>
        /// Обработка спавна врага
        /// </summary>
        private void OnEnemySpawned(GameObject enemy)
        {
            if (!_isInitialized) return;

            Debug.Log($"EnemySpawnPresenter: Enemy spawned: {enemy.name}");
        }

        /// <summary>
        /// Обработка уничтожения врага
        /// </summary>
        public void OnEnemyDestroyed(GameObject enemy)
        {
            if (!_isInitialized) return;

            Debug.Log($"EnemySpawnPresenter: Enemy destroyed: {enemy.name}");

            // Удаляем врага из модели
            _spawnModel.RemoveSpawnedEnemy(enemy);
        }

        /// <summary>
        /// Запуск спавна врагов
        /// </summary>
        private void StartEnemySpawn()
        {
            if (_isSpawning) return;

            _isSpawning = true;
            _spawnCoroutine = StartCoroutine(SpawnEnemiesCoroutine());
        }

        /// <summary>
        /// Корутина для спавна врагов
        /// </summary>
        private IEnumerator SpawnEnemiesCoroutine()
        {
            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

                if (playerTransform == null)
                {
                    Debug.LogError("EnemySpawnPresenter: Player transform is null, cannot spawn enemies!");
                    yield break;
                }
            }

            int enemiesToSpawn = spawnSettings.EnemiesToSpawn;
            float spawnDelay = spawnSettings.SpawnDelay;

            Debug.Log($"EnemySpawnPresenter: Starting to spawn {enemiesToSpawn} enemies");

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                if (!_spawnModel.CanSpawnEnemies()) break;

                // Спавним врага
                SpawnEnemy();

                // Ждем перед следующим спавном
                if (i < enemiesToSpawn - 1) // Не ждем после последнего врага
                {
                    yield return new WaitForSeconds(spawnDelay);
                }
            }

            _isSpawning = false;
            _spawnCoroutine = null;
            Debug.Log("EnemySpawnPresenter: Enemy spawn sequence completed");
        }

        /// <summary>
        /// Спавн одного врага
        /// </summary>
        private void SpawnEnemy()
        {
            Debug.Log($"[EnemySpawnPresenter] SpawnEnemy {playerTransform} {spawnSettings.EnemyPrefab}");
            if (playerTransform == null || spawnSettings.EnemyPrefab == null) return;

            // Генерируем случайную позицию вокруг игрока
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // Создаем врага
            GameObject enemy = Instantiate(spawnSettings.EnemyPrefab, spawnPosition, Quaternion.identity);

            // Настраиваем врага
            SetupEnemy(enemy);

            // Добавляем в модель
            _spawnModel.AddSpawnedEnemy(enemy);

            Debug.Log($"EnemySpawnPresenter: Spawned enemy at position {spawnPosition}");
        }

        /// <summary>
        /// Получение случайной позиции для спавна
        /// </summary>
        private Vector3 GetRandomSpawnPosition()
        {
            Vector3 playerPosition = playerTransform.position;
            float minDistance = spawnSettings.MinDistanceFromPlayer;
            float maxDistance = spawnSettings.MaxDistanceFromPlayer;

            // Генерируем случайный угол и расстояние
            float randomAngle = Random.Range(0f, 360f);
            float randomDistance = Random.Range(minDistance, maxDistance);

            // Вычисляем позицию
            Vector3 direction = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;
            Vector3 spawnPosition = playerPosition + direction * randomDistance;

            // Проверяем, что позиция находится в пределах радиуса спавна
            float distanceFromPlayer = Vector3.Distance(playerPosition, spawnPosition);
            if (distanceFromPlayer > spawnSettings.SpawnRadius)
            {
                spawnPosition = playerPosition + direction * spawnSettings.SpawnRadius;
            }

            return spawnPosition;
        }

        /// <summary>
        /// Настройка заспавненного врага
        /// </summary>
        private void SetupEnemy(GameObject enemy)
        {
            if (enemy == null) return;

            // Устанавливаем имя
            enemy.name = $"Enemy_{_spawnModel.SpawnedEnemiesCount}";

            // Инициализируем EnemyView если есть
            var enemyView = enemy.GetComponent<EnemyView>();
            if (enemyView != null)
            {
                enemyView.InitializeWithSettings(spawnSettings);
                enemyView.SetPlayer(playerTransform);
            }

            // Добавляем компонент для отслеживания уничтожения
            var enemyTracker = enemy.GetComponent<EnemyTracker>();
            if (enemyTracker == null)
            {
                enemyTracker = enemy.AddComponent<EnemyTracker>();
            }
            enemyTracker.Initialize(this);
        }

        #endregion

        #region Debug & Gizmos

        private void OnDrawGizmos()
        {
            if (!enableGizmos || playerTransform == null || spawnSettings == null) return;

            // Рисуем радиус спавна
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, spawnSettings.SpawnRadius);

            // Рисуем минимальную и максимальную дистанцию
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTransform.position, spawnSettings.MinDistanceFromPlayer);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, spawnSettings.MaxDistanceFromPlayer);
        }

        private void OnGUI()
        {
            if (!showDebugInfo || !_isInitialized) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("EnemySpawnPresenter Debug Info", GUI.skin.box);
            GUILayout.Label($"Spawned Enemies: {GetSpawnedEnemiesCount()}");
            GUILayout.Label($"Is Spawning: {IsSpawning()}");
            GUILayout.Label($"Can Spawn: {_spawnModel?.CanSpawnEnemies()}");
            GUILayout.Label($"Remaining: {_spawnModel?.GetRemainingEnemiesToSpawn()}");
            GUILayout.EndArea();
        }

        #endregion
    }

    /// <summary>
    /// Компонент для отслеживания врага
    /// </summary>
    public class EnemyTracker : MonoBehaviour
    {
        private EnemySpawnPresenter _presenter;

        public void Initialize(EnemySpawnPresenter presenter)
        {
            _presenter = presenter;
        }

        private void OnDestroy()
        {
            if (_presenter != null)
            {
                _presenter.OnEnemyDestroyed(gameObject);
            }
        }
    }


}
