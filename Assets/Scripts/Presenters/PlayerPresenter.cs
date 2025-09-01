using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Models;
using CrossFightUnlock.Views;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Presenters
{
    /// <summary>
    /// Презентер игрока, связывающий модель и представление
    /// </summary>
    public class PlayerPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerView _playerViewPrefab;
        private PlayerModel _playerModel;
        private PlayerView _view;

        // Настройки и события
        private GameSettings _gameSettings;
        private GameEvents _gameEvents;
        private PlayerView _playerView;
        // Позиция спавна
        private Vector3 _spawnPosition = Vector3.zero;

        // Состояние
        private bool _isInitialized = false;

        public void Initialize(GameSettings gameSettings, GameEvents gameEvents, Vector3 spawnPosition)
        {
            _gameSettings = gameSettings;
            _gameEvents = gameEvents;
            _spawnPosition = spawnPosition;

            _playerModel = new PlayerModel(_gameSettings, _gameEvents, spawnPosition);
            _playerModel.Initialize();

            // Подписываемся на события
            SubscribeToEvents();
            SpawnPlayer();
            _isInitialized = true;
            Debug.Log("PlayerPresenter initialized");
        }


        private void Update()
        {
            if (!_isInitialized) return;

            // Обновляем состояние "на земле" в модели
            if (_view != null && _playerModel != null)
            {
                bool isGrounded = _view.IsGrounded();
                _playerModel.SetGrounded(isGrounded);

                // Обновляем позицию в модели
                Vector3 currentPosition = _view.GetPosition();
                _playerModel.SetPosition(currentPosition);
            }
        }
        private void SpawnPlayer()
        {
            _playerView = Instantiate(_playerViewPrefab, _spawnPosition, Quaternion.identity);
            _view = _playerView;
            _view.SetGameEvents(_gameEvents);
            _view.Initialize();
            _view.SetPosition(_playerModel.Position);
            _view.Show();

            // Вызываем событие спавна игрока
            _gameEvents.OnPlayerSpawned?.Invoke();
            Debug.Log("PlayerPresenter: Player spawned event invoked");
        }
        /// <summary>
        /// Подписка на события
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnPlayerMoveInput.AddListener(HandlePlayerMoveInput);
            _gameEvents.OnPlayerJumpInput.AddListener(HandlePlayerJumpInput);
            _gameEvents.OnPlayerAttackInput.AddListener(HandlePlayerAttackInput);
            _gameEvents.OnPlayerRespawn.AddListener(RespawnPlayer);
            _gameEvents.OnCleanup.AddListener(HandleCleanup);
            _gameEvents.OnPlayerAttackInput.AddListener(HandlePlayerAttackInput);
        }

        /// <summary>
        /// Отписка от событий
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (_gameEvents == null) return;

            _gameEvents.OnPlayerMoveInput.RemoveListener(HandlePlayerMoveInput);
            _gameEvents.OnPlayerJumpInput.RemoveListener(HandlePlayerJumpInput);
            _gameEvents.OnPlayerAttackInput.RemoveListener(HandlePlayerAttackInput);
            _gameEvents.OnPlayerRespawn.RemoveListener(RespawnPlayer);
            _gameEvents.OnCleanup.RemoveListener(HandleCleanup);
        }

        /// <summary>
        /// Обработка ввода движения
        /// </summary>
        private void HandlePlayerMoveInput(Vector3 input)
        {
            if (!_isInitialized || _playerModel == null || _view == null) return;

            // Применяем dead zone для всех осей
            if (Mathf.Abs(input.x) < _gameSettings.InputDeadZone)
                input.x = 0f;
            if (Mathf.Abs(input.z) < _gameSettings.InputDeadZone)
                input.z = 0f;

            // Двигаем игрока
            float moveSpeed = _playerModel.GetMoveSpeed();
            _view.Move(input, moveSpeed);

            // Обновляем позицию в модели
            Vector3 newPosition = _view.GetPosition();
            _playerModel.SetPosition(newPosition);
        }

        /// <summary>
        /// Обработка ввода прыжка
        /// </summary>
        private void HandlePlayerJumpInput()
        {
            if (!_isInitialized || _playerModel == null || _view == null) return;

            // Проверяем, находится ли игрок на земле
            bool isGrounded = _view.IsGrounded();
            _playerModel.SetGrounded(isGrounded);

            if (isGrounded)
            {
                float jumpForce = _playerModel.GetJumpForce();
                _view.Jump(jumpForce);
            }
        }

        /// <summary>
        /// Обработка ввода атаки
        /// </summary>
        private void HandlePlayerAttackInput()
        {
            if (!_isInitialized || _playerModel == null) return;

            // Здесь можно добавить логику атаки
            _view.HandlePlayerAttackInput();
            Debug.Log("Player attack input received");
        }


        /// <summary>
        /// Получение модели игрока
        /// </summary>
        public PlayerModel GetPlayerModel()
        {
            return _playerModel;
        }

        /// <summary>
        /// Получение представления игрока
        /// </summary>
        public PlayerView GetPlayerView()
        {
            return _view;
        }

        /// <summary>
        /// Обработка смерти игрока
        /// </summary>
        public void HandlePlayerDeath()
        {
            if (!_isInitialized) return;

            Debug.Log("PlayerPresenter: Handling player death");
        }

        /// <summary>
        /// Респаун игрока
        /// </summary>
        private void RespawnPlayer()
        {
            if (!_isInitialized || _playerModel == null || _view == null) return;

            Debug.Log("PlayerPresenter: Respawning player");

            // Вызываем респаун в модели
            _playerModel.Respawn();

            // Устанавливаем позицию представления на позицию спавна
            _view.SetPosition(_spawnPosition);

            // Показываем игрока
            _view.Show();
        }

        /// <summary>
        /// Обработка события очистки
        /// </summary>
        private void HandleCleanup()
        {
            if (!_isInitialized) return;

            Debug.Log("PlayerPresenter: Cleanup event received");
            Cleanup();
        }


        public void Cleanup()
        {
            if (!_isInitialized) return;

            // Отписываемся от событий
            UnsubscribeFromEvents();

            // Очищаем компоненты
            _view?.Cleanup();
            _playerModel?.Cleanup();

            _gameSettings = null;
            _gameEvents = null;
            _isInitialized = false;
        }

    }
}
