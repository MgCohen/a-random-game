using CardMatch.Audio;
using CardMatch.CardMatch;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.MainMenu
{
    public class LevelEntry : MonoBehaviour
    {
        private MainMenuLevelEntry entry;
        private Action<MainMenuLevelEntry> onClick;

        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI count;
        [SerializeField] private Image icon;
        [SerializeField] private Button button;

        public void Set(MainMenuLevelEntry entry, bool selectable, Action<MainMenuLevelEntry> onClick, AudioService audioService)
        {
            this.entry = entry;
            this.onClick = onClick;
            SetButtonClick(selectable);
            SetLevelEntryLabel();
            var soundPlayer = button != null ? button.GetComponent<ButtonSoundPlayer>() : null;
            if (soundPlayer != null)
                soundPlayer.SetAudioService(audioService);
        }

        private void SetLevelEntryLabel()
        {
            label.text = entry.Level.LevelId.ToString();
            count.text = $"{GetCardCount(entry.Level.Config)} cards";
            icon.color = GetRandomColor(entry.Level);
        }

        private int GetCardCount(LevelConfig config)
        {
            return config.Layout.Columns * config.Layout.Rows;
        }

        private Color GetRandomColor(Level level)
        {
            var randomIndex = UnityEngine.Random.Range(0, level.CardFrontColors.Length);
            return level.CardFrontColors[randomIndex];
        }

        private void SetButtonClick(bool selectable)
        {
            button.interactable = selectable;
            button.onClick.RemoveListener(OnClick);
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            onClick?.Invoke(entry);
        }
    }
}
