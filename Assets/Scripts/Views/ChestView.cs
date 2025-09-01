using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Views
{
    /// <summary>
    /// Представление сундука, отвечающее за визуальное отображение и взаимодействие
    /// </summary>
    public class ChestView : MonoBehaviour, IView
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _interactionPoint;

        [Header("Visual Settings")]
        [SerializeField] private Sprite _closedSprite;
        [SerializeField] private Sprite _openedSprite;
        [SerializeField] private Color _closedColor = Color.white;
        [SerializeField] private Color _openedColor = Color.gray;

        [Header("Interaction")]
        [SerializeField] private float _interactionRange = 2f;
        [SerializeField] private LayerMask _playerLayerMask = 1;

        // События
        private GameEvents _gameEvents;

        // Состояние
        private bool _isInitialized = false;
        private bool _isVisible = true;
        private bool _isOpened = false;

        // Компоненты
        private SpriteRenderer SpriteRenderer => _spriteRenderer;
        private Animator Animator => _animator;
        private Collider Collider => _collider;
        private Transform InteractionPoint => _interactionPoint;

        public void Initialize()
        {
            if (_isInitialized) return;

            // Получаем компоненты если они не назначены
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_animator == null) _animator = GetComponent<Animator>();
            if (_collider == null) _collider = GetComponent<Collider>();

            // Создаем точку взаимодействия если её нет
            if (_interactionPoint == null)
            {
                GameObject interactionPointObj = new GameObject("InteractionPoint");
                _interactionPoint = interactionPointObj.transform;
                _interactionPoint.SetParent(transform);
                _interactionPoint.localPosition = Vector3.zero;
            }

            // Настраиваем начальное состояние
            if (SpriteRenderer != null)
            {
                SpriteRenderer.sprite = _closedSprite;
                SpriteRenderer.color = _closedColor;
            }

            _isInitialized = true;
            Debug.Log("ChestView initialized");
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
        /// Установка позиции сундука
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            if (!_isInitialized) return;

            transform.position = position;
        }

        /// <summary>
        /// Открытие сундука
        /// </summary>
        public void OpenChest()
        {
            if (!_isInitialized || _isOpened) return;

            _isOpened = true;

            // Меняем спрайт и цвет
            if (SpriteRenderer != null)
            {
                SpriteRenderer.sprite = _openedSprite;
                SpriteRenderer.color = _openedColor;
            }

            // Запускаем анимацию открытия
            if (Animator != null)
            {
                Animator.SetTrigger("Open");
            }

            // Отключаем коллайдер для взаимодействия
            if (Collider != null)
            {
                Collider.enabled = false;
            }

            Debug.Log("Chest opened!");
        }

        /// <summary>
        /// Проверка, может ли игрок взаимодействовать с сундуком
        /// </summary>
        public bool CanInteract(Transform playerTransform)
        {
            if (!_isInitialized || _isOpened || playerTransform == null) return false;

            float distance = Vector3.Distance(transform.position, playerTransform.position);
            return distance <= _interactionRange;
        }

        /// <summary>
        /// Получение текущей позиции
        /// </summary>
        public Vector3 GetPosition()
        {
            return transform.position;
        }

        /// <summary>
        /// Установка событий
        /// </summary>
        public void SetGameEvents(GameEvents gameEvents)
        {
            _gameEvents = gameEvents;
        }

        /// <summary>
        /// Проверка, открыт ли сундук
        /// </summary>
        public bool IsOpened()
        {
            return _isOpened;
        }

        private void OnDrawGizmosSelected()
        {
            if (InteractionPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(InteractionPoint.position, _interactionRange);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isInitialized || _isOpened) return;

            if (other.CompareTag("Player"))
            {
                Debug.Log($"[ChestView] Player entered chest interaction area");
                // Здесь можно добавить логику для показа подсказки о взаимодействии
                _gameEvents.OnShowChestButtonUI?.Invoke(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_isInitialized || _isOpened) return;

            if (other.CompareTag("Player"))
            {
                Debug.Log($"[ChestView] Player exited chest interaction area");
                _gameEvents.OnShowChestButtonUI?.Invoke(false);
            }
        }
    }
}
