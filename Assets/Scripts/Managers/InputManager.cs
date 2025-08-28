using UnityEngine;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Managers
{
    /// <summary>
    /// Менеджер ввода, обрабатывающий пользовательский ввод и отправляющий события
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private bool _enableInput = true;
        [SerializeField] private float _inputDeadZone = 0.1f;

        // События игры
        private GameEvents _gameEvents;

        // Состояние
        private bool _isInitialized = false;

        public void Initialize(GameEvents gameEvents)
        {
            if (_isInitialized) return;

            _gameEvents = gameEvents;

            // Подписываемся на событие очистки
            if (_gameEvents != null)
            {
                _gameEvents.OnCleanup.AddListener(HandleCleanup);
            }

            _isInitialized = true;

            Debug.Log("InputManager initialized");
        }

        public void Cleanup()
        {
            // Отписываемся от события очистки
            if (_gameEvents != null)
            {
                _gameEvents.OnCleanup.RemoveListener(HandleCleanup);
            }

            _gameEvents = null;
            _isInitialized = false;
        }

        /// <summary>
        /// Обработка события очистки
        /// </summary>
        private void HandleCleanup()
        {
            if (!_isInitialized) return;

            Debug.Log("InputManager: Cleanup event received");
            Cleanup();
        }

        private void Update()
        {
            if (!_isInitialized || !_enableInput || _gameEvents == null) return;

            HandleMovementInput();
            HandleJumpInput();
            HandleAttackInput();
        }

        /// <summary>
        /// Обработка ввода движения
        /// </summary>
        private void HandleMovementInput()
        {
            Vector3 moveInput = Vector3.zero;

            // Горизонтальное движение (A/D или стрелки влево/вправо)
            float horizontalInput = Input.GetAxis("Horizontal");
            if (Mathf.Abs(horizontalInput) > _inputDeadZone)
            {
                moveInput.x = horizontalInput;
            }

            // Вертикальное движение (W/S или стрелки вверх/вниз) - для оси Z в 3D
            float verticalInput = Input.GetAxis("Vertical");
            if (Mathf.Abs(verticalInput) > _inputDeadZone)
            {
                moveInput.z = verticalInput; // Используем Z вместо Y для 3D движения
            }

            // Отправляем событие движения
            if (moveInput != Vector3.zero)
            {
                _gameEvents.OnPlayerMoveInput?.Invoke(moveInput);
            }
        }

        /// <summary>
        /// Обработка ввода прыжка
        /// </summary>
        private void HandleJumpInput()
        {
            // Прыжок на пробел
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _gameEvents.OnPlayerJumpInput?.Invoke();
            }
        }

        /// <summary>
        /// Обработка ввода атаки
        /// </summary>
        private void HandleAttackInput()
        {
            // Атака на левую кнопку мыши или J
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J))
            {
                _gameEvents.OnPlayerAttackInput?.Invoke();
            }
        }

        /// <summary>
        /// Включение/выключение ввода
        /// </summary>
        public void SetInputEnabled(bool enabled)
        {
            _enableInput = enabled;
        }

        /// <summary>
        /// Установка dead zone для ввода
        /// </summary>
        public void SetInputDeadZone(float deadZone)
        {
            _inputDeadZone = Mathf.Clamp01(deadZone);
        }

        /// <summary>
        /// Получение текущего состояния ввода
        /// </summary>
        public bool IsInputEnabled()
        {
            return _enableInput;
        }
    }
}
