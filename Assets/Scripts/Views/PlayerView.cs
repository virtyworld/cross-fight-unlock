using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Views
{
    /// <summary>
    /// Представление игрока, отвечающее за визуальное отображение и физику
    /// </summary>
    public class PlayerView : MonoBehaviour, IView
    {
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _groundCheck;

        [Header("Ground Check")]
        [SerializeField] private float _groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask _groundLayerMask = 1;

        // События
        private GameEvents _gameEvents;
        private float _health = 100f;
        // Состояние
        private bool _isInitialized = false;
        private bool _isVisible = true;

        // Компоненты
        private Rigidbody Rigidbody => _rigidbody;
        private SpriteRenderer SpriteRenderer => _spriteRenderer;
        private Animator Animator => _animator;
        private Transform GroundCheck => _groundCheck;

        public void Initialize()
        {
            if (_isInitialized) return;

            // Получаем компоненты если они не назначены
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_animator == null) _animator = GetComponent<Animator>();

            // Создаем ground check если его нет
            if (_groundCheck == null)
            {
                GameObject groundCheckObj = new GameObject("GroundCheck");
                _groundCheck = groundCheckObj.transform;
                _groundCheck.SetParent(transform);
                _groundCheck.localPosition = new Vector3(0, -0.5f, 0);
            }

            // Настраиваем Rigidbody2D
            if (Rigidbody != null)
            {
                Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }

            _isInitialized = true;

            Debug.Log("PlayerView initialized");
        }

        public void HandlePlayerAttackInput()
        {
            if (!_isInitialized) return;
            _animator.Play("Attack");
        }

        public void Show()
        {
            if (!_isInitialized) return;

            _isVisible = true;
            gameObject.SetActive(true);

            if (SpriteRenderer != null)
                SpriteRenderer.enabled = true;
        }

        public void Hide()
        {
            if (!_isInitialized) return;

            _isVisible = false;
            gameObject.SetActive(false);
        }

        public void Cleanup()
        {
            _gameEvents = null;
            _isInitialized = false;
        }

        /// <summary>
        /// Установка позиции игрока
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            if (!_isInitialized) return;

            transform.position = position;
        }

        /// <summary>
        /// Движение игрока
        /// </summary>
        public void Move(Vector3 direction, float speed)
        {
            if (!_isInitialized || Rigidbody == null) return;

            Vector3 velocity = Rigidbody.linearVelocity;
            velocity.x = direction.x * speed;
            velocity.z = direction.z * speed; // Добавляем движение по оси Z
            Rigidbody.linearVelocity = velocity;

            // Поворот спрайта в зависимости от направления движения по X
            if (direction.x != 0 && SpriteRenderer != null)
            {
                SpriteRenderer.flipX = direction.x < 0;
            }

            // Анимация движения (учитываем общую скорость движения)
            if (Animator != null)
            {
                //float totalSpeed = Mathf.Sqrt(direction.x * direction.x + direction.z * direction.z);
                //Animator.SetFloat("Speed", totalSpeed);
            }
        }
        public void TakeDamage(float damage)
        {
            _health -= damage;

            if (_health <= 0)
            {
                Die();
            }
        }
        private void Die()
        {
            Debug.Log($"Player {gameObject.name} died!");
            Hide();
            _gameEvents.OnPlayerDeath?.Invoke();
        }
        /// <summary>
        /// Прыжок игрока
        /// </summary>
        public void Jump(float force)
        {
            if (!_isInitialized || Rigidbody == null) return;

            Rigidbody.AddForce(Vector2.up * force, ForceMode.Impulse);

            // Анимация прыжка
            if (Animator != null)
            {
                Animator.SetTrigger("Jump");
            }
        }

        /// <summary>
        /// Проверка, находится ли игрок на земле
        /// </summary>
        public bool IsGrounded()
        {
            if (!_isInitialized || GroundCheck == null) return false;

            return Physics2D.OverlapCircle(GroundCheck.position, _groundCheckRadius, _groundLayerMask);
        }

        /// <summary>
        /// Получение текущей позиции
        /// </summary>
        public Vector3 GetPosition()
        {
            return transform.position;
        }

        /// <summary>
        /// Получение текущей скорости
        /// </summary>
        public Vector2 GetVelocity()
        {
            return Rigidbody != null ? Rigidbody.linearVelocity : Vector2.zero;
        }

        /// <summary>
        /// Установка событий
        /// </summary>
        public void SetGameEvents(GameEvents gameEvents)
        {
            _gameEvents = gameEvents;
        }
        private void OnDrawGizmosSelected()
        {
            if (GroundCheck != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(GroundCheck.position, _groundCheckRadius);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[PlayerView] OnTriggerEnter {other.gameObject.name} {other.tag}");
            if (other.CompareTag("EnemyFist"))
            {
                _gameEvents.OnEnemyAttackedPlayer?.Invoke(other.gameObject);
                TakeDamage(50);
            }
            if (other.CompareTag("Obstacle"))
            {
                TakeDamage(100);
            }
        }
    }
}
