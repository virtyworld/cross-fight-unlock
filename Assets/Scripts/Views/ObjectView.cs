using UnityEngine;
using CrossFightUnlock.Interfaces;

namespace CrossFightUnlock.Views
{
    /// <summary>
    /// Представление объекта, движущегося по линии
    /// </summary>
    public class ObjectView : MonoBehaviour, IObjectView
    {
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;

        [Header("Movement")]
        [SerializeField] private float _currentSpeed = 2f;
        [SerializeField] private Vector3 _movementDirection = Vector3.right;

        // Состояние
        private bool _isInitialized = false;
        private bool _isActive = false;
        private bool _isMoving = false;

        // Компоненты
        private Rigidbody Rigidbody => _rigidbody;
        private SpriteRenderer SpriteRenderer => _spriteRenderer;
        private Animator Animator => _animator;

        public void Initialize()
        {
            // Получаем компоненты если они не назначены
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_animator == null) _animator = GetComponent<Animator>();

            // Настраиваем Rigidbody
            if (Rigidbody != null)
            {
                Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                Rigidbody.useGravity = false;
            }

            // Сбрасываем состояние для повторного использования
            _isMoving = false;
            if (Rigidbody != null)
            {
                Rigidbody.linearVelocity = Vector3.zero;
            }

            _isInitialized = true;
            _isActive = true;
        }
        private void FixedUpdate()
        {
            // Движение с помощью Rigidbody
            if (Rigidbody != null)
            {
                Vector3 velocity = _movementDirection * _currentSpeed;
                Rigidbody.linearVelocity = velocity;
            }
        }
        public void SetPosition(Vector3 position)
        {
            if (!_isInitialized) return;

            transform.position = position;
        }

        public void Move(Vector3 direction, float speed)
        {
            if (!_isInitialized || !_isActive) return;

            _movementDirection = direction.normalized;
            _currentSpeed = speed;
            _isMoving = true;

            // Обновляем спрайт в зависимости от направления
            if (SpriteRenderer != null && direction.x != 0)
            {
                SpriteRenderer.flipX = direction.x < 0;
            }

            // Анимация движения
            if (Animator != null)
            {
                Animator.SetFloat("Speed", speed);
                Animator.SetBool("IsMoving", true);
            }
        }

        public void Stop()
        {
            if (!_isInitialized) return;

            _isMoving = false;

            if (Rigidbody != null)
            {
                Rigidbody.linearVelocity = Vector3.zero;
            }

            // Анимация остановки
            if (Animator != null)
            {
                Animator.SetBool("IsMoving", false);
                Animator.SetFloat("Speed", 0);
            }
        }

        public void Show()
        {
            if (!_isInitialized) return;

            _isActive = true;
            gameObject.SetActive(true);

            if (SpriteRenderer != null)
                SpriteRenderer.enabled = true;
        }

        public void Hide()
        {
            if (!_isInitialized) return;

            _isActive = false;
            gameObject.SetActive(false);
        }

        public void Cleanup()
        {
            _isInitialized = false;
            _isActive = false;
            _isMoving = false;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public bool IsActive()
        {
            return _isActive && gameObject.activeInHierarchy;
        }

        public void SetMovementDirection(Vector3 direction)
        {
            _movementDirection = direction.normalized;
        }

        public void SetMovementSpeed(float speed)
        {
            _currentSpeed = speed;
        }



        private void OnDrawGizmosSelected()
        {
            // Отображаем направление движения
            if (_isInitialized)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, _movementDirection * 2f);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }
    }
}
