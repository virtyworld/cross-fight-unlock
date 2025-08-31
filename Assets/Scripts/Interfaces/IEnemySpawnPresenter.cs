namespace CrossFightUnlock.Interfaces
{
    /// <summary>
    /// Интерфейс для презентера спавна врагов
    /// </summary>
    public interface IEnemySpawnPresenter
    {
        /// <summary>
        /// Инициализация презентера
        /// </summary>
        void Initialize(CrossFightUnlock.Data.GameEvents gameEvents);

        /// <summary>
        /// Очистка презентера
        /// </summary>
        void Cleanup();

        /// <summary>
        /// Установка событий игры
        /// </summary>
        void SetGameEvents(CrossFightUnlock.Data.GameEvents gameEvents);

        /// <summary>
        /// Очистка всех врагов
        /// </summary>
        void ClearAllEnemies();

        /// <summary>
        /// Получение количества заспавненных врагов
        /// </summary>
        int GetSpawnedEnemiesCount();

        /// <summary>
        /// Проверка, идет ли спавн
        /// </summary>
        bool IsSpawning();
    }
}
