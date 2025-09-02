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
        private UnityEvent<GameObject> onPlayerAttackedEnemy;
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
        private UnityEvent<GameObject> onEnemyAttackedPlayer;

        // Enemy Events
        private UnityEvent onEnemyAttackStarted;
        private UnityEvent onEnemyAttackFinished;
        private UnityEvent<float> onEnemyHealthChanged;
        private UnityEvent<float> onEnemyTakeDamage;
        private UnityEvent<GameObject> onEnemyDeath;
        private UnityEvent<Vector3> onEnemyPositionChanged;

        // Finish Trigger Events
        private UnityEvent<GameObject> onFinishTriggerEntered;
        private UnityEvent<GameObject> onFinishTriggerExited;

        // Chest Events
        private UnityEvent<GameObject> onChestSpawned;
        private UnityEvent<GameObject> onChestOpened;
        private UnityEvent onAllEnemiesDestroyed;
        private UnityEvent<bool> onShowChestButtonUI;
        private UnityEvent onChestButtonClicked;

        // Unlock Events
        private UnityEvent onLockUnlocked;
        private UnityEvent onRestartButtonClicked;
        // Cleanup Events
        private UnityEvent onCleanup;

        // Свойства для доступа к событиям
        public UnityEvent OnPlayerSpawned => onPlayerSpawned;
        public UnityEvent OnPlayerDeath => onPlayerDeath;
        public UnityEvent OnPlayerRespawn => onPlayerRespawn;
        public UnityEvent<float> OnPlayerHealthChanged => onPlayerHealthChanged;
        public UnityEvent<Vector3> OnPlayerPositionChanged => onPlayerPositionChanged;
        public UnityEvent<GameObject> OnPlayerAttackedEnemy => onPlayerAttackedEnemy;

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
        public UnityEvent<GameObject> OnEnemyAttackedPlayer => onEnemyAttackedPlayer;

        public UnityEvent OnEnemyAttackStarted => onEnemyAttackStarted;
        public UnityEvent OnEnemyAttackFinished => onEnemyAttackFinished;
        public UnityEvent<float> OnEnemyHealthChanged => onEnemyHealthChanged;
        public UnityEvent<float> OnEnemyTakeDamage => onEnemyTakeDamage;
        public UnityEvent<GameObject> OnEnemyDeath => onEnemyDeath;
        public UnityEvent<Vector3> OnEnemyPositionChanged => onEnemyPositionChanged;

        public UnityEvent<GameObject> OnFinishTriggerEntered => onFinishTriggerEntered;
        public UnityEvent<GameObject> OnFinishTriggerExited => onFinishTriggerExited;

        public UnityEvent<GameObject> OnChestSpawned => onChestSpawned;
        public UnityEvent<GameObject> OnChestOpened => onChestOpened;
        public UnityEvent<bool> OnShowChestButtonUI => onShowChestButtonUI;
        public UnityEvent OnChestButtonClicked => onChestButtonClicked;
        public UnityEvent OnAllEnemiesDestroyed => onAllEnemiesDestroyed;

        public UnityEvent OnLockUnlocked => onLockUnlocked;
        public UnityEvent OnRestartButtonClicked => onRestartButtonClicked;
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
            onPlayerAttackedEnemy = new UnityEvent<GameObject>();
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
            onEnemyAttackedPlayer = new UnityEvent<GameObject>();

            onEnemyAttackStarted = new UnityEvent();
            onEnemyAttackFinished = new UnityEvent();
            onEnemyHealthChanged = new UnityEvent<float>();
            onEnemyTakeDamage = new UnityEvent<float>();
            onEnemyDeath = new UnityEvent<GameObject>();
            onEnemyPositionChanged = new UnityEvent<Vector3>();

            onFinishTriggerEntered = new UnityEvent<GameObject>();
            onFinishTriggerExited = new UnityEvent<GameObject>();

            onChestSpawned = new UnityEvent<GameObject>();
            onChestOpened = new UnityEvent<GameObject>();
            onShowChestButtonUI = new UnityEvent<bool>();
            onAllEnemiesDestroyed = new UnityEvent();
            onChestButtonClicked = new UnityEvent();
            onLockUnlocked = new UnityEvent();
            onRestartButtonClicked = new UnityEvent();
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

            onPlayerAttackedEnemy?.RemoveAllListeners();
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
            onEnemyAttackedPlayer?.RemoveAllListeners();

            onEnemyAttackStarted?.RemoveAllListeners();
            onEnemyAttackFinished?.RemoveAllListeners();
            onEnemyHealthChanged?.RemoveAllListeners();
            onEnemyTakeDamage?.RemoveAllListeners();
            onEnemyDeath?.RemoveAllListeners();
            onEnemyPositionChanged?.RemoveAllListeners();

            onFinishTriggerEntered?.RemoveAllListeners();
            onFinishTriggerExited?.RemoveAllListeners();

            onChestSpawned?.RemoveAllListeners();
            onChestOpened?.RemoveAllListeners();
            onShowChestButtonUI?.RemoveAllListeners();
            onAllEnemiesDestroyed?.RemoveAllListeners();
            onChestButtonClicked?.RemoveAllListeners();
            onLockUnlocked?.RemoveAllListeners();
            onRestartButtonClicked?.RemoveAllListeners();
            onCleanup?.RemoveAllListeners();
        }
    }
}
