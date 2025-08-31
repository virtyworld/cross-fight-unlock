using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;
using System.Collections;

namespace CrossFightUnlock.Views
{
    /// <summary>
    /// Простое представление врага
    /// </summary>
    public class EnemyView : MonoBehaviour, IView
    {
        [Header("Enemy Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float detectionRange = 100f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float rotationSpeed = 3f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float stoppingDistance = 1.5f;
        [SerializeField] private int damage = 10;

        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody enemyRigidbody;
        [SerializeField] private Animator enemyAnimator;

        // Состояние
        private bool _isInitialized = false;
        private bool _isVisible = true;
        private bool _isAggressive = true;
        private bool _isPlayerDetected = false;
        private bool _isAttacking = false;
        private bool _canAttack = true;
        private float _lastAttackTime = 0f;
        private Transform _playerTransform;
        private Vector3 _startPosition;
        private Vector3 _lastPlayerPosition;
        private float _health = 100f;
        private float _maxHealth = 100f;
        private float _distanceToPlayer;

        public void Initialize()
        {
            if (_isInitialized) return;

            // Получаем компоненты
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (enemyRigidbody == null) enemyRigidbody = GetComponent<Rigidbody>();
            if (enemyAnimator == null) enemyAnimator = GetComponent<Animator>();

            // Настраиваем Rigidbody
            if (enemyRigidbody != null)
            {
                enemyRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

            // Сохраняем начальную позицию
            _startPosition = transform.position;

            // Находим игрока
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }

            _isInitialized = true;
            Debug.Log($"EnemyView initialized: {gameObject.name}");
        }

        /// <summary>
        /// Инициализация с настройками поведения
        /// </summary>
        public void InitializeWithSettings(EnemySpawnSettings settings)
        {
            if (!_isInitialized) Initialize();

            _isAggressive = settings.EnemiesAggressive;
            detectionRange = settings.EnemyDetectionRange;
            moveSpeed = settings.EnemyMoveSpeed;
            rotationSpeed = settings.EnemyRotationSpeed;
            attackRange = settings.EnemyAttackRange;
            attackCooldown = settings.EnemyAttackCooldown;
            stoppingDistance = settings.EnemyStoppingDistance;

            Debug.Log($"EnemyView initialized with settings: Aggressive={_isAggressive}, DetectionRange={detectionRange}");
        }

        public void Show()
        {
            if (!_isInitialized) return;

            _isVisible = true;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (!_isInitialized) return;

            _isVisible = false;
            gameObject.SetActive(false);
        }

        public void Cleanup()
        {
            _isInitialized = false;
        }

        private void Update()
        {
            if (!_isInitialized || !_isVisible) return;

            UpdatePlayerDetection();
            UpdateBehavior();
        }

        private void FixedUpdate()
        {
            if (!_isInitialized || !_isVisible) return;

            UpdateMovement();
        }

        /// <summary>
        /// Движение к игроку
        /// </summary>
        private void MoveTowardsPlayer()
        {
            if (_playerTransform == null) return;

            Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
            directionToPlayer.y = 0; // Игнорируем вертикальное движение

            // Поворачиваемся к игроку
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }

            // Поворачиваем спрайт (если есть)
            if (spriteRenderer != null && directionToPlayer.x != 0)
            {
                spriteRenderer.flipX = directionToPlayer.x < 0;
            }

            // Двигаемся к игроку
            Vector3 movement = directionToPlayer * moveSpeed * Time.fixedDeltaTime;

            if (enemyRigidbody != null)
            {
                enemyRigidbody.MovePosition(enemyRigidbody.position + movement);
            }
            else
            {
                transform.position += movement;
            }
        }

        /// <summary>
        /// Остановка движения
        /// </summary>
        private void StopMovement()
        {
            if (enemyRigidbody != null)
            {
                enemyRigidbody.linearVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Возврат на начальную позицию
        /// </summary>
        private void ReturnToStartPosition()
        {
            if (enemyRigidbody == null) return;

            Vector3 direction = (_startPosition - transform.position).normalized;
            direction.y = 0;

            if (Vector3.Distance(transform.position, _startPosition) > 1f)
            {
                enemyRigidbody.linearVelocity = new Vector3(direction.x * moveSpeed * 0.5f, enemyRigidbody.linearVelocity.y, direction.z * moveSpeed * 0.5f);
            }
            else
            {
                enemyRigidbody.linearVelocity = new Vector3(0, enemyRigidbody.linearVelocity.y, 0);
            }
        }

        /// <summary>
        /// Атака игрока
        /// </summary>
        private void AttackPlayer()
        {
            if (!_canAttack || _isAttacking) return;

            _isAttacking = true;
            _canAttack = false;
            _lastAttackTime = Time.time;

            // Останавливаем движение во время атаки
            StopMovement();

            // Запускаем анимацию атаки
            if (enemyAnimator != null)
            {
                enemyAnimator.SetTrigger("Attack");
            }

            // Логируем атаку
            Debug.Log($"Enemy {gameObject.name} is attacking player!");

            // Запускаем корутину для завершения атаки
            StartCoroutine(AttackCooldownCoroutine());
        }

        /// <summary>
        /// Корутина для кулдауна атаки
        /// </summary>
        private IEnumerator AttackCooldownCoroutine()
        {
            // Ждем завершения атаки
            yield return new WaitForSeconds(0.5f); // Время анимации атаки
            _isAttacking = false;

            // Ждем кулдаун
            yield return new WaitForSeconds(attackCooldown - 0.5f);
            _canAttack = true;
        }

        /// <summary>
        /// Обновление анимаций
        /// </summary>
        private void UpdateAnimations()
        {
            if (enemyAnimator == null) return;

            // Анимация движения
            bool isMoving = _isPlayerDetected && _distanceToPlayer > stoppingDistance && !_isAttacking;
            enemyAnimator.SetBool("IsMoving", isMoving);

            // Анимация атаки
            enemyAnimator.SetBool("IsAttacking", _isAttacking);

            // Скорость движения для анимации
            float moveSpeedValue = isMoving ? moveSpeed : 0f;
            enemyAnimator.SetFloat("MoveSpeed", moveSpeedValue);
        }

        /// <summary>
        /// Получение урона
        /// </summary>
        public void TakeDamage(float damage)
        {
            _health -= damage;

            if (_health <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Смерть врага
        /// </summary>
        private void Die()
        {
            Debug.Log($"Enemy {gameObject.name} died!");
            Destroy(gameObject);
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
            detectionRange = range;
        }

        /// <summary>
        /// Установка игрока для преследования
        /// </summary>
        public void SetPlayer(Transform player)
        {
            _playerTransform = player;
        }

        #region Private Methods

        /// <summary>
        /// Обновление обнаружения игрока
        /// </summary>
        private void UpdatePlayerDetection()
        {
            if (_playerTransform == null) return;

            _distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
            _isPlayerDetected = _distanceToPlayer <= detectionRange;

            if (_isPlayerDetected)
            {
                _lastPlayerPosition = _playerTransform.position;
            }
        }

        /// <summary>
        /// Обновление поведения врага
        /// </summary>
        private void UpdateBehavior()
        {
            if (!_isPlayerDetected || !_isAggressive) return;

            // Проверяем возможность атаки
            if (_distanceToPlayer <= attackRange && _canAttack)
            {
                AttackPlayer();
            }

            // Обновляем анимации
            UpdateAnimations();
        }

        /// <summary>
        /// Обновление движения врага
        /// </summary>
        private void UpdateMovement()
        {
            if (!_isPlayerDetected || !_isAggressive || _isAttacking) return;

            // Если игрок слишком близко, останавливаемся
            if (_distanceToPlayer <= stoppingDistance)
            {
                StopMovement();
                return;
            }

            // Двигаемся к игроку
            MoveTowardsPlayer();
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            // Рисуем радиус обнаружения
            Gizmos.color = _isPlayerDetected ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Рисуем радиус атаки
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Дистанция остановки
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance);

            // Линия к игроку
            if (_playerTransform != null && _isPlayerDetected)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _playerTransform.position);
            }
        }
    }
}
