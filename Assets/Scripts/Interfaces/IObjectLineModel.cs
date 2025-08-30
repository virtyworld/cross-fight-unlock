using System.Collections.Generic;
using UnityEngine;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Interfaces
{
    /// <summary>
    /// Интерфейс для модели линии движения объектов
    /// </summary>
    public interface IObjectLineModel
    {
        /// <summary>
        /// Инициализация модели
        /// </summary>
        void Initialize(ObjectSpawnSettings settings);

        /// <summary>
        /// Запуск спавна объектов
        /// </summary>
        void StartSpawning();

        /// <summary>
        /// Остановка спавна объектов
        /// </summary>
        void StopSpawning();

        /// <summary>
        /// Пауза спавна
        /// </summary>
        void PauseSpawning();

        /// <summary>
        /// Возобновление спавна
        /// </summary>
        void ResumeSpawning();

        /// <summary>
        /// Изменение направления движения
        /// </summary>
        void ChangeDirection(bool moveTowardsPositiveX);

        /// <summary>
        /// Изменение скорости движения
        /// </summary>
        void ChangeSpeed(float newSpeed);

        /// <summary>
        /// Получить текущие настройки
        /// </summary>
        ObjectSpawnSettings GetSettings();

        /// <summary>
        /// Получить количество активных объектов на линии
        /// </summary>
        int GetActiveObjectCount();

        /// <summary>
        /// Получить все активные объекты
        /// </summary>
        List<GameObject> GetActiveObjects();

        /// <summary>
        /// Очистка всех объектов с линии
        /// </summary>
        void ClearAllObjects();

        /// <summary>
        /// Проверить, активен ли спавн
        /// </summary>
        bool IsSpawningActive();

        /// <summary>
        /// Получить текущее направление движения
        /// </summary>
        bool GetCurrentDirection();

        /// <summary>
        /// Получить текущую скорость движения
        /// </summary>
        float GetCurrentSpeed();
    }
}
