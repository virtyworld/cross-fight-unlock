using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Models
{
    /// <summary>
    /// Модель игрока, содержащая все данные о состоянии игрока
    /// </summary>
    public class PlayerModel : IModel
    {
        // Данные игрока
        private float _health;
        private float _maxHealth;
        private Vector3 _position;
        private Vector3 _startPosition;
        private bool _isAlive;
        private bool _isGrounded;

        // Настройки
        private GameSettings _gameSettings;
        private GameEvents _gameEvents;

        // Свойства для доступа к данным
        public float Health => _health;
        public float MaxHealth => _maxHealth;
        public Vector3 Position => _position;
        public bool IsAlive => _isAlive;
        public bool IsGrounded => _isGrounded;

        public PlayerModel(GameSettings gameSettings, GameEvents gameEvents, Vector3 startPosition)
        {
            _gameSettings = gameSettings;
            _gameEvents = gameEvents;
            _startPosition = startPosition;
        }

        public void Initialize()
        {
            _maxHealth = _gameSettings.PlayerMaxHealth;
            _health = _maxHealth;
            _position = _startPosition;
            _isAlive = true;
            _isGrounded = false;

            Debug.Log("PlayerModel initialized");
        }

        public void Cleanup()
        {
            _gameSettings = null;
            _gameEvents = null;
        }

        /// <summary>
        /// Установка позиции игрока
        /// </summary>
        public void SetPosition(Vector3 newPosition)
        {
            _position = newPosition;
            _gameEvents.OnPlayerPositionChanged?.Invoke(_position);
        }

        /// <summary>
        /// Изменение здоровья игрока
        /// </summary>
        public void ChangeHealth(float deltaHealth)
        {
            _health = Mathf.Clamp(_health + deltaHealth, 0f, _maxHealth);
            _gameEvents.OnPlayerHealthChanged?.Invoke(_health);

            if (_health <= 0f && _isAlive)
            {
                Die();
            }
        }

        /// <summary>
        /// Установка состояния "на земле"
        /// </summary>
        public void SetGrounded(bool grounded)
        {
            _isGrounded = grounded;
        }

        /// <summary>
        /// Смерть игрока
        /// </summary>
        public void Die()
        {
            _isAlive = false;
            _gameEvents.OnPlayerDeath?.Invoke();
        }

        /// <summary>
        /// Возрождение игрока
        /// </summary>
        public void Respawn()
        {
            _health = _maxHealth;
            _position = _startPosition;
            _isAlive = true;
            _isGrounded = false;
        }

        /// <summary>
        /// Получение скорости движения из настроек
        /// </summary>
        public float GetMoveSpeed()
        {
            return _gameSettings.PlayerMoveSpeed;
        }

        /// <summary>
        /// Получение силы прыжка из настроек
        /// </summary>
        public float GetJumpForce()
        {
            return _gameSettings.PlayerJumpForce;
        }
    }
}
