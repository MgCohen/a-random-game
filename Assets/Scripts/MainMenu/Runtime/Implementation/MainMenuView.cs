using System;
using System.Collections.Generic;
using CardMatch.CardMatch;
using CardMatch.Navigation;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.MainMenu
{
    public class MainMenuView : View<MainMenuViewContext>
    {
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Transform levelListContent;
        [SerializeField] private Button levelEntryPrefab;

        private readonly List<Button> levelEntryButtons = new List<Button>();

        protected override void OnShow()
        {
            BindButtons();
            RefreshLevelList();
        }

        protected override void OnHide()
        {
            UnbindButtons();
            RemoveLevelEntryListeners();
        }

        private void BindButtons()
        {
            if (settingsButton != null) settingsButton.onClick.AddListener(HandleSettingsClicked);
            if (playButton != null) playButton.onClick.AddListener(HandlePlayClicked);
        }

        private void UnbindButtons()
        {
            if (settingsButton != null) settingsButton.onClick.RemoveListener(HandleSettingsClicked);
            if (playButton != null) playButton.onClick.RemoveListener(HandlePlayClicked);
        }

        private void RefreshLevelList()
        {
            if (Context == null || levelListContent == null || levelEntryPrefab == null) return;
            ClearLevelEntries();
            IReadOnlyList<MainMenuLevelEntry> entries = Context.GetLevelEntries();
            for (int i = 0; i < entries.Count; i++)
            {
                Button entryButton = CreateLevelEntryButton(entries[i]);
                if (entryButton != null) levelEntryButtons.Add(entryButton);
            }
        }

        private Button CreateLevelEntryButton(MainMenuLevelEntry entry)
        {
            Button instance = Instantiate(levelEntryPrefab, levelListContent);
            bool selectable = Context.CanSelect(entry.Level);
            instance.interactable = selectable;
            SetLevelEntryLabel(instance, entry);
            if (selectable) AddLevelClickListener(instance, entry.Level);
            return instance;
        }

        private void AddLevelClickListener(Button button, Level level)
        {
            button.onClick.AddListener(() => HandleLevelSelected(level));
        }

        private void SetLevelEntryLabel(Button entryButton, MainMenuLevelEntry entry)
        {
            Text label = entryButton.GetComponentInChildren<Text>();
            if (label == null) return;
            label.text = entry.Level.LevelId + GetStateSuffix(entry.State);
        }

        private static string GetStateSuffix(LevelProgressState state)
        {
            if (state == LevelProgressState.Completed) return " (Completed)";
            if (state == LevelProgressState.Unlocked) return " (Unlocked)";
            return " (Locked)";
        }

        private void ClearLevelEntries()
        {
            RemoveLevelEntryListeners();
            for (int i = levelEntryButtons.Count - 1; i >= 0; i--)
            {
                Button entry = levelEntryButtons[i];
                if (entry != null && entry.gameObject != null) Destroy(entry.gameObject);
            }
            levelEntryButtons.Clear();
        }

        private void RemoveLevelEntryListeners()
        {
            for (int i = 0; i < levelEntryButtons.Count; i++)
            {
                Button entry = levelEntryButtons[i];
                if (entry != null) entry.onClick.RemoveAllListeners();
            }
        }

        private void HandleSettingsClicked()
        {
            Context?.OnSettingsClicked();
        }

        private void HandlePlayClicked()
        {
            Context?.OnPlayClicked();
        }

        private void HandleLevelSelected(Level level)
        {
            Context?.SelectLevel(level);
        }
    }
}
