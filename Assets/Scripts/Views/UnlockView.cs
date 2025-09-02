using UnityEngine;
using TMPro;

namespace CrossFightUnlock.Views
{
    public class UnlockView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private GameObject unlockPanel;

        private CrossFightUnlock.Presenters.UnlockPresenter _unlockPresenter;

        public void Initialize(CrossFightUnlock.Presenters.UnlockPresenter unlockPresenter)
        {
            _unlockPresenter = unlockPresenter;
            UpdateView();
        }

        public void UpdateView()
        {
            if (_unlockPresenter == null) return;

            var model = _unlockPresenter.GetComponent<UnlockModel>();
            if (model == null) return;

            // Обновляем прогресс
            if (progressText != null)
            {
                progressText.text = $"{model.KeysCollected}/{model.RequiredKeys}";
            }
        }

        public void ShowUnlockPanel()
        {
            if (unlockPanel != null)
            {
                unlockPanel.SetActive(true);
            }
        }

        public void HideUnlockPanel()
        {
            if (unlockPanel != null)
            {
                unlockPanel.SetActive(false);
            }
        }

        public void OnLockUnlocked()
        {
            Debug.Log("UnlockView: Lock unlocked event received!");
            // Здесь можно добавить анимацию или другие эффекты
        }
    }
}
