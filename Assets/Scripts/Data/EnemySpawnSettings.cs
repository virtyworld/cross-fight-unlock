using UnityEngine;

namespace CrossFightUnlock.Data
{
    /// <summary>
    /// Настройки спавна врагов
    /// </summary>
    [CreateAssetMenu(fileName = "EnemySpawnSettings", menuName = "Cross Fight Unlock/Enemy Spawn Settings")]
    public class EnemySpawnSettings : ScriptableObject
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int enemiesToSpawn = 2;
        [SerializeField] private float spawnRadius = 200f;
        [SerializeField] private float minDistanceFromPlayer = 50f;
        [SerializeField] private float maxDistanceFromPlayer = 200f;

        [Header("Spawn Timing")]
        [SerializeField] private float spawnDelay = 0.01f;
        [SerializeField] private bool spawnOnTriggerEnter = true;

        [Header("Enemy Behavior")]
        [SerializeField] private bool enemiesAggressive = true;
        [SerializeField] private float enemyDetectionRange = 100f;
        [SerializeField] private float enemyMoveSpeed = 5f;
        [SerializeField] private float enemyRotationSpeed = 3f;
        [SerializeField] private float enemyAttackRange = 2f;
        [SerializeField] private float enemyAttackCooldown = 1f;
        [SerializeField] private float enemyStoppingDistance = 1.5f;

        // Свойства для доступа к настройкам
        public GameObject EnemyPrefab => enemyPrefab;
        public int EnemiesToSpawn => enemiesToSpawn;
        public float SpawnRadius => spawnRadius;
        public float MinDistanceFromPlayer => minDistanceFromPlayer;
        public float MaxDistanceFromPlayer => maxDistanceFromPlayer;
        public float SpawnDelay => spawnDelay;
        public bool SpawnOnTriggerEnter => spawnOnTriggerEnter;
        public bool EnemiesAggressive => enemiesAggressive;
        public float EnemyDetectionRange => enemyDetectionRange;
        public float EnemyMoveSpeed => enemyMoveSpeed;
        public float EnemyRotationSpeed => enemyRotationSpeed;
        public float EnemyAttackRange => enemyAttackRange;
        public float EnemyAttackCooldown => enemyAttackCooldown;
        public float EnemyStoppingDistance => enemyStoppingDistance;

        /// <summary>
        /// Сброс настроек к значениям по умолчанию
        /// </summary>
        public void ResetToDefaults()
        {
            enemiesToSpawn = 2;
            spawnRadius = 200f;
            minDistanceFromPlayer = 50f;
            maxDistanceFromPlayer = 200f;
            spawnDelay = 0.01f;
            spawnOnTriggerEnter = true;
            enemiesAggressive = true;
            enemyDetectionRange = 100f;
            enemyMoveSpeed = 5f;
            enemyRotationSpeed = 3f;
            enemyAttackRange = 2f;
            enemyAttackCooldown = 1f;
            enemyStoppingDistance = 1.5f;
        }

        private void OnValidate()
        {
            // Проверяем корректность значений
            if (enemiesToSpawn < 1) enemiesToSpawn = 1;
            if (spawnRadius < 1f) spawnRadius = 1f;
            if (minDistanceFromPlayer < 0f) minDistanceFromPlayer = 0f;
            if (maxDistanceFromPlayer < minDistanceFromPlayer) maxDistanceFromPlayer = minDistanceFromPlayer;
            if (spawnDelay < 0f) spawnDelay = 0f;
            if (enemyDetectionRange < 0f) enemyDetectionRange = 0f;
            if (enemyMoveSpeed < 0.1f) enemyMoveSpeed = 0.1f;
            if (enemyRotationSpeed < 0.1f) enemyRotationSpeed = 0.1f;
            if (enemyAttackRange < 0.1f) enemyAttackRange = 0.1f;
            if (enemyAttackCooldown < 0.1f) enemyAttackCooldown = 0.1f;
            if (enemyStoppingDistance < 0.1f) enemyStoppingDistance = 0.1f;
        }
    }
}
