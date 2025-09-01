using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;
using CrossFightUnlock.Models;
using System.Collections;

namespace CrossFightUnlock.Views
{
    /// <summary>
    /// Представление врага, отвечающее за визуальное отображение и физику
    /// </summary>
    public class EnemyView : MonoBehaviour, IView
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody enemyRigidbody;
        [SerializeField] private Animator enemyAnimator;

        // Ссылки на презентер и события
        private GameEvents _gameEvents;
        private EnemyModel _enemyModel; // Только для чтения данных

        // Состояние
        private bool _isInitialized = false;
        private bool _isVisible = true;
        private bool _isAttackCoroutineRunning = false;
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

            _isInitialized = true;
            Debug.Log($"EnemyView initialized: {gameObject.name}");
        }

        /// <summary>
        /// Инициализация с настройками поведения
        /// </summary>
        public void InitializeWithSettings(EnemySpawnSettings settings, GameEvents gameEvents)
        {
            if (!_isInitialized) Initialize();

            _gameEvents = gameEvents;

            // Подписываемся на событие смерти врага
            _gameEvents.OnEnemyDeath.AddListener(OnEnemyDeath);

            Debug.Log($"EnemyView initialized with settings");
        }

        /// <summary>
        /// Установка модели для чтения данных (вызывается презентером)
        /// </summary>
        public void SetModel(EnemyModel model)
        {
            _enemyModel = model;
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
            // Отписываемся от событий
            if (_gameEvents != null)
            {
                _gameEvents.OnEnemyDeath.RemoveListener(OnEnemyDeath);
            }

            _enemyModel = null;
            _gameEvents = null;
            _isInitialized = false;
            _isAttackCoroutineRunning = false;
        }

        private void Update()
        {
            if (!_isInitialized || !_isVisible || _enemyModel == null) return;

            // Обновляем анимации
            UpdateAnimations();
        }

        private void FixedUpdate()
        {
            if (!_isInitialized || !_isVisible || _enemyModel == null) return;

            UpdateMovement();
        }

        /// <summary>
        /// Движение к игроку
        /// </summary>
        private void MoveTowardsPlayer()
        {
            if (_enemyModel == null) return;

            Vector3 directionToPlayer = _enemyModel.GetDirectionToPlayer();

            // Поворачиваемся к игроку
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _enemyModel.RotationSpeed * Time.fixedDeltaTime);
            }

            // Поворачиваем спрайт (если есть)
            if (spriteRenderer != null && directionToPlayer.x != 0)
            {
                spriteRenderer.flipX = directionToPlayer.x < 0;
            }

            // Двигаемся к игроку
            Vector3 movement = directionToPlayer * _enemyModel.MoveSpeed * Time.fixedDeltaTime;

            if (enemyRigidbody != null)
            {
                enemyRigidbody.MovePosition(enemyRigidbody.position + movement);
                // Обновляем позицию в модели после движения
                _enemyModel.SetPosition(enemyRigidbody.position);
            }
            else
            {
                transform.position += movement;
                // Обновляем позицию в модели после движения
                _enemyModel.SetPosition(transform.position);
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
            if (_enemyModel == null) return;

            Vector3 direction = _enemyModel.GetDirectionToStartPosition();

            if (Vector3.Distance(transform.position, _enemyModel.Position) > 1f)
            {
                Vector3 movement = direction * _enemyModel.MoveSpeed * 0.5f * Time.fixedDeltaTime;

                if (enemyRigidbody != null)
                {
                    enemyRigidbody.MovePosition(enemyRigidbody.position + movement);
                    // Обновляем позицию в модели после движения
                    _enemyModel.SetPosition(enemyRigidbody.position);
                }
                else
                {
                    transform.position += movement;
                    // Обновляем позицию в модели после движения
                    _enemyModel.SetPosition(transform.position);
                }
            }
        }

        /// <summary>
        /// Атака игрока (вызывается презентером)
        /// </summary>
        public void AttackPlayer()
        {
            if (_enemyModel == null || _isAttackCoroutineRunning)
            {
                Debug.Log($"EnemyView: AttackPlayer blocked - Model null: {_enemyModel == null}, Coroutine running: {_isAttackCoroutineRunning}");
                return;
            }

            Debug.Log($"EnemyView: Starting attack - CanAttack: {_enemyModel.CanAttack}, IsAttacking: {_enemyModel.IsAttacking}");

            // Останавливаем движение во время атаки
            StopMovement();

            // Запускаем анимацию атаки
            if (enemyAnimator != null)
            {
                enemyAnimator.Play("Attack");
                Debug.Log("EnemyView: Attack animation started");
            }
            else
            {
                Debug.LogWarning("EnemyView: EnemyAnimator is null!");
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
            _isAttackCoroutineRunning = true;
            Debug.Log("EnemyView: Attack cooldown coroutine started");

            // Ждем завершения атаки
            yield return new WaitForSeconds(0.5f); // Время анимации атаки

            Debug.Log("EnemyView: Attack cooldown finished, calling FinishAttack");

            // Завершаем атаку в модели (модель сама вызовет событие)
            if (_enemyModel != null)
            {
                _enemyModel.FinishAttack();
            }

            _isAttackCoroutineRunning = false;
            Debug.Log("EnemyView: Attack cooldown coroutine finished");
        }

        /// <summary>
        /// Обновление анимаций
        /// </summary>
        private void UpdateAnimations()
        {
            if (enemyAnimator == null) return;

            // Анимация движения

        }

        /// <summary>
        /// Получение урона (вызывается презентером)
        /// </summary>
        public void TakeDamage(float damage)
        {
            // View не должен напрямую изменять модель
            // Это должно делаться через презентер
            Debug.Log($"EnemyView: Taking damage {damage}");
        }

        /// <summary>
        /// Установка агрессивности (вызывается презентером)
        /// </summary>
        public void SetAggressive(bool aggressive)
        {
            // View не должен напрямую изменять модель
            Debug.Log($"EnemyView: Set aggressive {aggressive}");
        }

        /// <summary>
        /// Установка дистанции обнаружения (вызывается презентером)
        /// </summary>
        public void SetDetectionRange(float range)
        {
            // View не должен напрямую изменять модель
            Debug.Log($"EnemyView: Set detection range {range}");
        }

        /// <summary>
        /// Установка игрока для преследования (вызывается презентером)
        /// </summary>
        public void SetPlayer(Transform player)
        {
            // View не должен напрямую изменять модель
            Debug.Log($"EnemyView: Set player {player?.name}");
        }

        #region Private Methods

        /// <summary>
        /// Обновление движения врага
        /// </summary>
        private void UpdateMovement()
        {
            if (_enemyModel == null) return;

            // Обновляем позицию в модели
            _enemyModel.SetPosition(transform.position);

            // Если враг должен остановиться
            if (_enemyModel.ShouldStop())
            {
                StopMovement();
                return;
            }

            // Если враг должен двигаться к игроку
            if (_enemyModel.ShouldMoveTowardsPlayer())
            {
                MoveTowardsPlayer();
            }
            // Если враг должен вернуться на начальную позицию
            else if (_enemyModel.ShouldReturnToStartPosition())
            {
                ReturnToStartPosition();
            }
        }

        #endregion

        /// <summary>
        /// Обработка события смерти врага
        /// </summary>
        private void OnEnemyDeath(GameObject deadEnemy)
        {
            // Проверяем, что событие относится к этому врагу
            if (deadEnemy == gameObject)
            {
                Debug.Log($"Enemy {gameObject.name} died, hiding...");
                Hide();
            }
        }

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

        void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[EnemyView] OnTriggerEnter {other.gameObject.name} {other.tag}");
            if (other.CompareTag("PlayerFist"))
            {
                _gameEvents.OnPlayerAttackedEnemy?.Invoke(gameObject);
                // Уведомляем презентер о получении урона через существующее событие
                // Презентер будет слушать OnPlayerAttackedEnemy и обрабатывать урон
            }
        }
    }
}
