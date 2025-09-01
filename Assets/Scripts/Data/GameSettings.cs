using UnityEngine;

namespace CrossFightUnlock.Data
{
    /// <summary>
    /// Настройки игры, хранимые в ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Cross Fight Unlock/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Player Settings")]
        [SerializeField] private float playerMoveSpeed = 5f;
        [SerializeField] private float playerJumpForce = 10f;
        [SerializeField] private float playerMaxHealth = 100f;

        [Header("Game Settings")]
        [SerializeField] private float gameTimeScale = 1f;
        [SerializeField] private bool enableDebugMode = false;

        [Header("Input Settings")]
        [SerializeField] private float inputDeadZone = 0.1f;
        [SerializeField] private bool enableMouseLook = true;

        // Свойства для доступа к настройкам
        public float PlayerMoveSpeed => playerMoveSpeed;
        public float PlayerJumpForce => playerJumpForce;
        public float PlayerMaxHealth => playerMaxHealth;
        public float GameTimeScale => gameTimeScale;
        public bool EnableDebugMode => enableDebugMode;
        public float InputDeadZone => inputDeadZone;
        public bool EnableMouseLook => enableMouseLook;

        /// <summary>
        /// Сброс настроек к значениям по умолчанию
        /// </summary>
        public void ResetToDefaults()
        {
            playerMoveSpeed = 5f;
            playerJumpForce = 10f;
            playerMaxHealth = 100f;
            gameTimeScale = 1f;
            enableDebugMode = false;
            inputDeadZone = 0.1f;
            enableMouseLook = true;
        }
    }
}
