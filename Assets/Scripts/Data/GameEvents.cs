using UnityEngine;
using UnityEngine.Events;

namespace CrossFightUnlock.Data
{

    public class GameEvents
    {
        // Player Events
        private UnityEvent onPlayerSpawned;
        private UnityEvent onPlayerDeath;
        private UnityEvent onPlayerRespawn;
        private UnityEvent<float> onPlayerHealthChanged;
        private UnityEvent<Vector3> onPlayerPositionChanged;

        // Game State Events
        private UnityEvent onGameStart;
        private UnityEvent onGamePause;
        private UnityEvent onGameResume;
        private UnityEvent onGameEnd;

        // Input Events
        private UnityEvent<Vector3> onPlayerMoveInput;
        private UnityEvent onPlayerJumpInput;
        private UnityEvent onPlayerAttackInput;

        // UI Events
        private UnityEvent onUIOpen;
        private UnityEvent onUIClose;

        // Enemy Spawn Events
        private UnityEvent onEnemySpawnTriggered;
        private UnityEvent<GameObject> onEnemySpawned;
        private UnityEvent<GameObject> onEnemyDestroyed;

        // Finish Trigger Events
        private UnityEvent<GameObject> onFinishTriggerEntered;
        private UnityEvent<GameObject> onFinishTriggerExited;

        // Cleanup Events
        private UnityEvent onCleanup;

        // Свойства для доступа к событиям
        public UnityEvent OnPlayerSpawned => onPlayerSpawned;
        public UnityEvent OnPlayerDeath => onPlayerDeath;
        public UnityEvent OnPlayerRespawn => onPlayerRespawn;
        public UnityEvent<float> OnPlayerHealthChanged => onPlayerHealthChanged;
        public UnityEvent<Vector3> OnPlayerPositionChanged => onPlayerPositionChanged;

        public UnityEvent OnGameStart => onGameStart;
        public UnityEvent OnGamePause => onGamePause;
        public UnityEvent OnGameResume => onGameResume;
        public UnityEvent OnGameEnd => onGameEnd;

        public UnityEvent<Vector3> OnPlayerMoveInput => onPlayerMoveInput;
        public UnityEvent OnPlayerJumpInput => onPlayerJumpInput;
        public UnityEvent OnPlayerAttackInput => onPlayerAttackInput;

        public UnityEvent OnUIOpen => onUIOpen;
        public UnityEvent OnUIClose => onUIClose;

        public UnityEvent OnEnemySpawnTriggered => onEnemySpawnTriggered;
        public UnityEvent<GameObject> OnEnemySpawned => onEnemySpawned;
        public UnityEvent<GameObject> OnEnemyDestroyed => onEnemyDestroyed;

        public UnityEvent<GameObject> OnFinishTriggerEntered => onFinishTriggerEntered;
        public UnityEvent<GameObject> OnFinishTriggerExited => onFinishTriggerExited;

        public UnityEvent OnCleanup => onCleanup;

        /// <summary>
        /// Конструктор - автоматически инициализирует все события
        /// </summary>
        public void Init()
        {
            // Создаем все события при создании объекта
            onPlayerSpawned = new UnityEvent();
            onPlayerDeath = new UnityEvent();
            onPlayerRespawn = new UnityEvent();
            onPlayerHealthChanged = new UnityEvent<float>();
            onPlayerPositionChanged = new UnityEvent<Vector3>();

            onGameStart = new UnityEvent();
            onGamePause = new UnityEvent();
            onGameResume = new UnityEvent();
            onGameEnd = new UnityEvent();

            onPlayerMoveInput = new UnityEvent<Vector3>();
            onPlayerJumpInput = new UnityEvent();
            onPlayerAttackInput = new UnityEvent();

            onUIOpen = new UnityEvent();
            onUIClose = new UnityEvent();

            onEnemySpawnTriggered = new UnityEvent();
            onEnemySpawned = new UnityEvent<GameObject>();
            onEnemyDestroyed = new UnityEvent<GameObject>();

            onFinishTriggerEntered = new UnityEvent<GameObject>();
            onFinishTriggerExited = new UnityEvent<GameObject>();

            onCleanup = new UnityEvent();
        }

        /// <summary>
        /// Очистка всех событий
        /// </summary>
        public void Cleanup()
        {
            onPlayerSpawned?.RemoveAllListeners();
            onPlayerDeath?.RemoveAllListeners();
            onPlayerRespawn?.RemoveAllListeners();
            onPlayerHealthChanged?.RemoveAllListeners();
            onPlayerPositionChanged?.RemoveAllListeners();

            onGameStart?.RemoveAllListeners();
            onGamePause?.RemoveAllListeners();
            onGameResume?.RemoveAllListeners();
            onGameEnd?.RemoveAllListeners();

            onPlayerMoveInput?.RemoveAllListeners();
            onPlayerJumpInput?.RemoveAllListeners();
            onPlayerAttackInput?.RemoveAllListeners();

            onUIOpen?.RemoveAllListeners();
            onUIClose?.RemoveAllListeners();

            onEnemySpawnTriggered?.RemoveAllListeners();
            onEnemySpawned?.RemoveAllListeners();
            onEnemyDestroyed?.RemoveAllListeners();

            onFinishTriggerEntered?.RemoveAllListeners();
            onFinishTriggerExited?.RemoveAllListeners();

            onCleanup?.RemoveAllListeners();
        }
    }
}
