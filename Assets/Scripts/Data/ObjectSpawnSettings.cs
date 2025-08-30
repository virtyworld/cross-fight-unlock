using UnityEngine;

namespace CrossFightUnlock.Data
{
    /// <summary>
    /// Настройки для спавна объектов на одной линии
    /// </summary>
    [CreateAssetMenu(fileName = "ObjectSpawnSettings", menuName = "Cross Fight Unlock/Object Spawn Settings")]
    public class ObjectSpawnSettings : ScriptableObject
    {
        [Header("Line Settings")]
        [SerializeField] private Vector3 _lineStartPosition = Vector3.zero;
        [SerializeField] private Vector3 _lineEndPosition = Vector3.zero;
        [SerializeField] private float _lineYPosition = 0f;

        [Header("Movement Settings")]
        [SerializeField] private float _movementSpeed = 2f;
        [SerializeField] private bool _isRightDirection = true;

        [Header("Spawn Settings")]
        [SerializeField] private GameObject _objectPrefab;
        [SerializeField] private int _minObjectsPerSpawn = 1;
        [SerializeField] private int _maxObjectsPerSpawn = 3;
        [SerializeField] private float _spawnRadius = 1f;
        [SerializeField] private float _minSpawnInterval = 0.1f;
        [SerializeField] private float _maxSpawnInterval = 2f;
        [SerializeField] private int _maxObjectsOnLine = 20;

        [Header("Density Settings")]
        [SerializeField] private float _minDistanceBetweenObjects = 1f;
        [SerializeField] private bool _useRandomDensity = true;

        // Свойства для доступа к настройкам
        public Vector3 LineStartPosition => _lineStartPosition;
        public Vector3 LineEndPosition => _lineEndPosition;
        public float LineYPosition => _lineYPosition;
        public float MovementSpeed => _movementSpeed;
        public bool IsRightDirection => _isRightDirection;
        public GameObject ObjectPrefab => _objectPrefab;

        public float SpawnRadius => _spawnRadius;
        public float MinSpawnInterval => _minSpawnInterval;
        public float MaxSpawnInterval => _maxSpawnInterval;
        public int MaxObjectsOnLine => _maxObjectsOnLine;
        public float MinDistanceBetweenObjects => _minDistanceBetweenObjects;
        public bool UseRandomDensity => _useRandomDensity;

        /// <summary>
        /// Получить случайный интервал спавна
        /// </summary>
        public float GetRandomSpawnInterval()
        {
            return Random.Range(_minSpawnInterval, _maxSpawnInterval);
        }

        /// <summary>
        /// Получить случайное количество объектов для спавна
        /// </summary>
        public int GetRandomObjectCount()
        {
            if (_useRandomDensity)
            {
                return Random.Range(_minObjectsPerSpawn, _maxObjectsPerSpawn);
            }
            return _minObjectsPerSpawn;
        }

        /// <summary>
        /// Получить направление движения как Vector3
        /// </summary>
        public Vector3 GetMovementDirection()
        {
            return _isRightDirection ? Vector3.right : Vector3.left;
        }

        /// <summary>
        /// Получить случайную позицию на линии в пределах радиуса спавна
        /// </summary>
        public Vector3 GetRandomSpawnPosition()
        {
            Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
            return _lineStartPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
        }

        private void OnValidate()
        {
            // Ограничиваем значения
            _minSpawnInterval = Mathf.Max(0.1f, _minSpawnInterval);
            _maxSpawnInterval = Mathf.Max(_minSpawnInterval, _maxSpawnInterval);
            _spawnRadius = Mathf.Max(0.1f, _spawnRadius);
            _minDistanceBetweenObjects = Mathf.Max(0.1f, _minDistanceBetweenObjects);
            _maxObjectsOnLine = Mathf.Max(1, _maxObjectsOnLine);
        }
    }
}
