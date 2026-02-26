using CardMatch.Audio;
using CardMatch.Navigation;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.Settings
{
    public class SettingsView : View<SettingsViewContext>
    {
        [SerializeField] private Toggle muteButton;
        [SerializeField] private Button returnButton;

        protected override void OnShow()
        {
            BindButtons();
            UpdateMuteButtonLabel();
        }

        protected override void OnHide()
        {
            UnbindButtons();
        }

        protected override void OnClose()
        {
            UnbindButtons();
        }

        private void BindButtons()
        {
            if (muteButton != null)
            {
                muteButton.onValueChanged.AddListener(HandleMuteClicked);
            }
            if (returnButton != null)
            {
                returnButton.onClick.AddListener(HandleReturnClicked);
            }
        }

        private void UnbindButtons()
        {
            if (muteButton != null)
            {
                muteButton.onValueChanged.RemoveListener(HandleMuteClicked);
            }
            if (returnButton != null)
            {
                returnButton.onClick.RemoveListener(HandleReturnClicked);
            }
        }

        private void HandleMuteClicked(bool state)
        {
            if (Context == null || Context.AudioService == null) return;
            IAudioService audio = Context.AudioService;
            audio.SetMute(state);
            UpdateMuteButtonLabel();
        }

        private void HandleReturnClicked()
        {
            if (Context == null || Context.Navigation == null) return;
            Context.Navigation.GoBack();
        }

        private void UpdateMuteButtonLabel()
        {
            if (muteButton == null || Context == null || Context.AudioService == null) return;
            Text label = muteButton.GetComponentInChildren<Text>();
            if (label == null) return;
            bool isMuted = Context.AudioService.IsMuted;
            label.text = isMuted ? "Unmute" : "Mute";
        }
    }
}
