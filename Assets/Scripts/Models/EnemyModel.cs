using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Models
{
    /// <summary>
    /// Модель врага, содержащая всю логику поведения врага
    /// </summary>
    public class EnemyModel : IModel
    {
        // Данные врага
        private float _health;
        private float _maxHealth;
        private Vector3 _position;
        private Vector3 _startPosition;
        private bool _isAlive;
        private bool _isAggressive;
        private bool _isPlayerDetected;
        private bool _isAttacking;
        private bool _canAttack;
        private float _lastAttackTime;
        private float _distanceToPlayer;
        private Vector3 _lastPlayerPosition;

        // Настройки поведения
        private float _moveSpeed;
        private float _detectionRange;
        private float _attackRange;
        private float _rotationSpeed;
        private float _attackCooldown;
        private float _stoppingDistance;
        private int _damage;

        // Ссылки
        private Transform _playerTransform;
        private GameEvents _gameEvents;
        private EnemySpawnSettings _spawnSettings;
        private GameObject _enemyGameObject;

        // Свойства для доступа к данным
        public float Health => _health;
        public float MaxHealth => _maxHealth;
        public Vector3 Position => _position;
        public bool IsAlive => _isAlive;
        public bool IsAggressive => _isAggressive;
        public bool IsPlayerDetected => _isPlayerDetected;
        public bool IsAttacking => _isAttacking;
        public bool CanAttack => _canAttack;
        public float DistanceToPlayer => _distanceToPlayer;
        public Vector3 LastPlayerPosition => _lastPlayerPosition;

        // Настройки поведения
        public float MoveSpeed => _moveSpeed;
        public float DetectionRange => _detectionRange;
        public float AttackRange => _attackRange;
        public float RotationSpeed => _rotationSpeed;
        public float AttackCooldown => _attackCooldown;
        public float StoppingDistance => _stoppingDistance;
        public int Damage => _damage;

        public EnemyModel(EnemySpawnSettings spawnSettings, GameEvents gameEvents, Vector3 startPosition, Transform playerTransform, GameObject enemyGameObject)
        {
            _spawnSettings = spawnSettings;
            _gameEvents = gameEvents;
            _startPosition = startPosition;
            _playerTransform = playerTransform;
            _enemyGameObject = enemyGameObject;
        }

        public void Initialize()
        {
            // Инициализируем здоровье
            _maxHealth = 100f; // Можно добавить в настройки
            _health = _maxHealth;

            // Инициализируем позицию
            _position = _startPosition;

            // Инициализируем состояние
            _isAlive = true;
            _isAggressive = _spawnSettings.EnemiesAggressive;
            _isPlayerDetected = false;
            _isAttacking = false;
            _canAttack = true;
            _distanceToPlayer = 0f;
            _lastPlayerPosition = Vector3.zero;

            // Инициализируем настройки поведения
            _moveSpeed = _spawnSettings.EnemyMoveSpeed;
            _detectionRange = _spawnSettings.EnemyDetectionRange;
            _attackRange = _spawnSettings.EnemyAttackRange;
            _rotationSpeed = _spawnSettings.EnemyRotationSpeed;
            _attackCooldown = _spawnSettings.EnemyAttackCooldown;
            _stoppingDistance = _spawnSettings.EnemyStoppingDistance;
            _damage = 10; // Можно добавить в настройки

            // Инициализируем время последней атаки после настройки кулдауна
            _lastAttackTime = -_attackCooldown; // Устанавливаем время так, чтобы кулдаун не сработал сразу

            Debug.Log($"EnemyModel initialized - CanAttack: {_canAttack}, IsAttacking: {_isAttacking}, LastAttackTime: {_lastAttackTime}, AttackCooldown: {_attackCooldown}, IsAggressive: {_isAggressive}");
        }

        public void Cleanup()
        {
            _spawnSettings = null;
            _gameEvents = null;
            _playerTransform = null;
        }

        /// <summary>
        /// Обновление обнаружения игрока
        /// </summary>
        public void UpdatePlayerDetection()
        {
            if (_playerTransform == null)
            {
                Debug.Log("PlayerTransform is null!");
                return;
            }

            _distanceToPlayer = Vector3.Distance(_position, _playerTransform.position);

            _isPlayerDetected = _distanceToPlayer <= _detectionRange;

            if (_isPlayerDetected)
            {
                _lastPlayerPosition = _playerTransform.position;
            }
        }

        /// <summary>
        /// Обновление поведения врага
        /// </summary>
        public void UpdateBehavior()
        {
            // Отладочная информация для понимания, почему метод может не работать
            if (!_isPlayerDetected)
            {
                return;
            }

            if (!_isAggressive)
            {
                return;
            }

            // Проверяем кулдаун атаки
            if (!_canAttack && !_isAttacking && Time.time - _lastAttackTime >= _attackCooldown)
            {
                ResetAttackCooldown();
            }

            // Если игрок слишком далеко для атаки, прекращаем атаку
            if (_isAttacking && _distanceToPlayer > _attackRange)
            {
                FinishAttack(); // Правильно завершаем атаку
            }

            // Проверяем возможность атаки
            if (_distanceToPlayer <= _attackRange && _canAttack && !_isAttacking)
            {
                AttackPlayer();
            }
        }

        /// <summary>
        /// Атака игрока
        /// </summary>
        public void AttackPlayer()
        {
            if (!_canAttack || _isAttacking) return;

            _isAttacking = true;
            _canAttack = false;
            _lastAttackTime = Time.time;

            // Уведомляем о начале атаки
            _gameEvents.OnEnemyAttackStarted?.Invoke();

            Debug.Log($"Enemy is attacking player! Damage: {_damage}");
        }

        /// <summary>
        /// Завершение атаки
        /// </summary>
        public void FinishAttack()
        {
            if (!_isAttacking) return; // Предотвращаем множественные вызовы

            _isAttacking = false;
            // НЕ сбрасываем _canAttack здесь - это будет сделано через кулдаун

            // Уведомляем о завершении атаки
            _gameEvents.OnEnemyAttackFinished?.Invoke();

            Debug.Log($"Enemy attack finished. CanAttack: {_canAttack}, LastAttackTime: {_lastAttackTime}, AttackCooldown: {_attackCooldown}");
        }

        /// <summary>
        /// Сброс кулдауна атаки
        /// </summary>
        public void ResetAttackCooldown()
        {
            _canAttack = true;
        }

        /// <summary>
        /// Получение урона
        /// </summary>
        public void TakeDamage(float damage)
        {
            _health -= damage;
            _gameEvents.OnEnemyHealthChanged?.Invoke(_health);

            if (_health <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Смерть врага
        /// </summary>
        public void Die()
        {
            _isAlive = false;
            _gameEvents.OnEnemyDeath?.Invoke(_enemyGameObject);
            Debug.Log($"Enemy {_enemyGameObject?.name} died!");
        }

        /// <summary>
        /// Установка позиции врага
        /// </summary>
        public void SetPosition(Vector3 newPosition)
        {
            _position = newPosition;
            _gameEvents.OnEnemyPositionChanged?.Invoke(_position);
        }

        /// <summary>
        /// Установка агрессивности
        /// </summary>
        public void SetAggressive(bool aggressive)
        {
            _isAggressive = aggressive;
        }

        /// <summary>
        /// Установка дистанции обнаружения
        /// </summary>
        public void SetDetectionRange(float range)
        {
            _detectionRange = range;
        }

        /// <summary>
        /// Установка игрока для преследования
        /// </summary>
        public void SetPlayer(Transform player)
        {
            _playerTransform = player;
        }

        /// <summary>
        /// Проверка, должен ли враг двигаться к игроку
        /// </summary>
        public bool ShouldMoveTowardsPlayer()
        {
            return _isPlayerDetected && _isAggressive && !_isAttacking && _distanceToPlayer > _stoppingDistance;
        }

        /// <summary>
        /// Проверка, должен ли враг остановиться
        /// </summary>
        public bool ShouldStop()
        {
            return _distanceToPlayer <= _stoppingDistance || _isAttacking;
        }

        /// <summary>
        /// Получение направления к игроку
        /// </summary>
        public Vector3 GetDirectionToPlayer()
        {
            if (_playerTransform == null) return Vector3.zero;

            Vector3 direction = (_playerTransform.position - _position).normalized;
            direction.y = 0; // Игнорируем вертикальное движение
            return direction;
        }

        /// <summary>
        /// Получение направления к начальной позиции
        /// </summary>
        public Vector3 GetDirectionToStartPosition()
        {
            Vector3 direction = (_startPosition - _position).normalized;
            direction.y = 0;
            return direction;
        }

        /// <summary>
        /// Проверка, должен ли враг вернуться на начальную позицию
        /// </summary>
        public bool ShouldReturnToStartPosition()
        {
            return !_isPlayerDetected && Vector3.Distance(_position, _startPosition) > 1f;
        }
    }
}
