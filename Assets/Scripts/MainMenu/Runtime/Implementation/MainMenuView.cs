using CardMatch.Navigation;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CardMatch.MainMenu
{
    public class MainMenuView : View<MainMenuViewContext>
    {
        [SerializeField] private Button settingsButton;
        [SerializeField] private Transform levelListContent;
        [SerializeField] private LevelEntry levelEntryPrefab;

        private readonly List<LevelEntry> levelEntryButtons = new List<LevelEntry>();

        protected override void OnShow()
        {
            BindButtons();
            RefreshLevelList();
        }

        protected override void OnHide()
        {
            UnbindButtons();
            ClearLevelEntries();
        }

        private void BindButtons()
        {
            if (settingsButton != null) settingsButton.onClick.AddListener(HandleSettingsClicked);
        }

        private void UnbindButtons()
        {
            if (settingsButton != null) settingsButton.onClick.RemoveListener(HandleSettingsClicked);
        }

        private void RefreshLevelList()
        {
            if (Context == null || levelListContent == null || levelEntryPrefab == null) return;
            ClearLevelEntries();
            IReadOnlyList<MainMenuLevelEntry> entries = Context.GetLevelEntries();
            for (int i = 0; i < entries.Count; i++)
            {
                LevelEntry entry = CreateLevelEntryButton(entries[i]);
                if (entry != null) levelEntryButtons.Add(entry);
            }
        }

        private LevelEntry CreateLevelEntryButton(MainMenuLevelEntry entry)
        {
            LevelEntry instance = Instantiate(levelEntryPrefab, levelListContent);
            bool selectable = Context.CanSelect(entry.Level);
            instance.Set(entry, selectable, HandleLevelSelected);
            return instance;
        }

        private void ClearLevelEntries()
        {
            foreach (var entry in levelEntryButtons)
            {
                if (entry != null)
                {
                    Destroy(entry.gameObject);
                }
            }
            levelEntryButtons.Clear();
        }

        private void HandleSettingsClicked()
        {
            Context?.OnSettingsClicked();
        }

        private void HandleLevelSelected(MainMenuLevelEntry entry)
        {
            Context?.SelectLevel(entry.Level);
        }
    }
}
