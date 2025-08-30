using UnityEngine;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Interfaces
{
    /// <summary>
    /// Интерфейс для презентера управления линиями движения объектов
    /// </summary>
    public interface IObjectLinesPresenter
    {
        /// <summary>
        /// Инициализация презентера
        /// </summary>
        void Initialize(ObjectLinesSettings settings);

        /// <summary>
        /// Запуск всех линий
        /// </summary>
        void StartAllLines();

        /// <summary>
        /// Остановка всех линий
        /// </summary>
        void StopAllLines();

        /// <summary>
        /// Пауза всех линий
        /// </summary>
        void PauseAllLines();

        /// <summary>
        /// Возобновление всех линий
        /// </summary>
        void ResumeAllLines();

        /// <summary>
        /// Управление конкретной линией
        /// </summary>
        void StartLine(int lineIndex);
        void StopLine(int lineIndex);
        void PauseLine(int lineIndex);
        void ResumeLine(int lineIndex);

        /// <summary>
        /// Изменение направления движения для конкретной линии
        /// </summary>
        void ChangeLineDirection(int lineIndex, bool moveTowardsPositiveX);

        /// <summary>
        /// Изменение скорости движения для конкретной линии
        /// </summary>
        void ChangeLineSpeed(int lineIndex, float newSpeed);

        /// <summary>
        /// Очистка всех объектов с конкретной линии
        /// </summary>
        void ClearLine(int lineIndex);

        /// <summary>
        /// Получение информации о линии
        /// </summary>
        int GetLineObjectCount(int lineIndex);
        bool IsLineActive(int lineIndex);
        bool GetLineDirection(int lineIndex);
        float GetLineSpeed(int lineIndex);

        /// <summary>
        /// Получение общих настроек
        /// </summary>
        ObjectLinesSettings GetSettings();

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        void Cleanup();
    }
}
