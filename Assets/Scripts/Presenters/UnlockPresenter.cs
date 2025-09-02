using UnityEngine;
using CrossFightUnlock.Data;
using CrossFightUnlock.Views;
using TMPro;
using System.Collections.Generic;

namespace CrossFightUnlock.Presenters
{
    public class UnlockPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject unlockPanel;

        [Header("Prefabs")]
        [SerializeField] private GameObject emptyCellPrefab;
        [SerializeField] private GameObject redKeyPrefab;
        [SerializeField] private GameObject blueKeyPrefab;
        [SerializeField] private GameObject greenKeyPrefab;

        [Header("UI Elements")]
        [SerializeField] private GameObject unlockContainer;
        [SerializeField] private GameObject blueLock;
        [SerializeField] private GameObject redLock;
        [SerializeField] private GameObject greenLock;
        [SerializeField] private TMP_Text statusLockText;
        [SerializeField] private UnlockView _unlockView;

        [Header("Events")]

        private GameEvents _gameEvents;
        private UnlockModel _unlockModel;

        private List<GameObject> _spawnedCells = new List<GameObject>();
        private GameObject _activeLock;

        public void Initialize(GameEvents gameEvents)
        {
            _gameEvents = gameEvents;
            _unlockModel = GetComponent<UnlockModel>();
            _gameEvents.OnChestOpened.AddListener(OnLockUnlocked);

            if (_unlockModel == null)
            {
                _unlockModel = gameObject.AddComponent<UnlockModel>();
            }

            _unlockModel.Initialize();
            _unlockView.Initialize(this);
            SetupUnlockSystem();
        }
        void Start()
        {
            unlockPanel.SetActive(false);
        }
        private void OnLockUnlocked(GameObject chest)
        {
            unlockPanel.SetActive(true);
        }
        private void SetupUnlockSystem()
        {
            // Очищаем предыдущие ячейки
            ClearSpawnedCells();

            // Спавним 36 ячеек с ключами
            SpawnCells();

            // Настраиваем замки
            SetupLocks();

            // Обновляем UI
            UpdateLockStatus();
        }

        private void ClearSpawnedCells()
        {
            foreach (var cell in _spawnedCells)
            {
                if (cell != null)
                    DestroyImmediate(cell);
            }
            _spawnedCells.Clear();
        }

        private void SpawnCells()
        {
            if (unlockContainer == null || emptyCellPrefab == null)
            {
                Debug.LogError("UnlockContainer or EmptyCellPrefab is not assigned!");
                return;
            }

            var cellColors = _unlockModel.CellColors;

            for (int i = 0; i < cellColors.Count; i++)
            {
                // Создаем ячейку
                GameObject cell = Instantiate(emptyCellPrefab, unlockContainer.transform);
                _spawnedCells.Add(cell);

                // Создаем ключ нужного цвета
                GameObject keyPrefab = GetKeyPrefabByColor(cellColors[i]);
                if (keyPrefab != null)
                {
                    GameObject key = Instantiate(keyPrefab, cell.transform);

                    // Добавляем компонент для drag and drop
                    var keyHandler = key.GetComponent<CrossFightUnlock.Views.KeyDragComponent>();
                    if (keyHandler == null)
                    {
                        keyHandler = key.AddComponent<CrossFightUnlock.Views.KeyDragComponent>();
                    }
                    keyHandler.Initialize(cellColors[i], this);
                }
            }
        }

        private GameObject GetKeyPrefabByColor(KeyColor color)
        {
            switch (color)
            {
                case KeyColor.Red:
                    return redKeyPrefab;
                case KeyColor.Blue:
                    return blueKeyPrefab;
                case KeyColor.Green:
                    return greenKeyPrefab;
                default:
                    return null;
            }
        }

        private void SetupLocks()
        {
            // Скрываем все замки
            if (redLock != null) redLock.SetActive(false);
            if (blueLock != null) blueLock.SetActive(false);
            if (greenLock != null) greenLock.SetActive(false);

            // Показываем только нужный замок
            KeyColor targetColor = _unlockModel.TargetColor;
            switch (targetColor)
            {
                case KeyColor.Red:
                    _activeLock = redLock;
                    break;
                case KeyColor.Blue:
                    _activeLock = blueLock;
                    break;
                case KeyColor.Green:
                    _activeLock = greenLock;
                    break;
            }

            if (_activeLock != null)
            {
                _activeLock.SetActive(true);

                // Добавляем компонент для обработки drop
                var lockHandler = _activeLock.GetComponent<CrossFightUnlock.Views.LockDropComponent>();
                if (lockHandler == null)
                {
                    lockHandler = _activeLock.AddComponent<CrossFightUnlock.Views.LockDropComponent>();
                }
                lockHandler.Initialize(targetColor, this);
            }
        }

        public bool TryDropKey(KeyColor keyColor)
        {
            bool success = _unlockModel.TryCollectKey(keyColor);
            if (success)
            {
                UpdateLockStatus();

                if (_unlockModel.IsLockUnlocked())
                {
                    _gameEvents?.OnLockUnlocked?.Invoke();
                    _unlockView?.OnLockUnlocked();
                    Debug.Log("Lock unlocked!");
                }
            }
            return success;
        }

        private void UpdateLockStatus()
        {
            if (statusLockText != null)
            {
                statusLockText.text = $"{_unlockModel.KeysCollected}/{_unlockModel.RequiredKeys}";
            }

            // Обновляем view
            if (_unlockView != null)
            {
                _unlockView.UpdateView();
            }
        }

        public void ResetUnlockSystem()
        {
            _unlockModel.Reset();
            SetupUnlockSystem();
        }

        public void Cleanup()
        {
            ClearSpawnedCells();
            _gameEvents = null;
            _unlockModel = null;
        }
    }
}
