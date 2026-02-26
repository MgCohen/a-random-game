using CardMatch.Audio;
using CardMatch.Navigation;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.MainMenu
{
    public class MainMenuView : View<MainMenuViewContext>
    {
        [SerializeField] private AudioService audioService;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Transform levelListContent;
        [SerializeField] private LevelEntry levelEntryPrefab;
        [SerializeField] private float entryRevealStagger = 0.05f;
        [SerializeField] private float entryRevealDuration = 0.25f;
        [SerializeField] private AudioClip spawnSound;

        private readonly List<LevelEntry> levelEntryButtons = new List<LevelEntry>();

        protected override void OnShow()
        {
            BindButtons();
            if (levelEntryButtons.Count > 0)
                return;
            RefreshLevelList();
        }

        protected override void OnHide()
        {
            UnbindButtons();
        }

        protected override void OnClose()
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
                if (entry == null) continue;
                levelEntryButtons.Add(entry);
                AnimateEntrySpawn(entry, i + 1);
            }
        }

        private void AnimateEntrySpawn(LevelEntry entry, int index)
        {
            entry.transform.localScale = Vector3.zero;
            entry.transform.DOScale(Vector3.one, entryRevealDuration)
                           .SetDelay(index * entryRevealStagger)
                           .SetEase(Ease.OutBack)
                           .OnStart(() => audioService.PlaySound(spawnSound));
        }

        private LevelEntry CreateLevelEntryButton(MainMenuLevelEntry entry)
        {
            LevelEntry instance = Instantiate(levelEntryPrefab, levelListContent);
            bool selectable = Context.CanSelect(entry.Level);
            instance.Set(entry, selectable, HandleLevelSelected, audioService);
            return instance;
        }

        private void ClearLevelEntries()
        {
            foreach (var entry in levelEntryButtons)
            {
                if (entry != null)
                {
                    if (entry.transform is RectTransform rt)
                        DOTween.Kill(rt);
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
