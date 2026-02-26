using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CardMatch.Audio
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleSoundPlayer : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private AudioClip hoverClip;
        [SerializeField] private AudioClip toggleOnClip;
        [SerializeField] private AudioClip toggleOffClip;
        [SerializeField] private AudioService audioService;

        private Toggle toggle;

        private void Awake()
        {
            if (toggle == null)
                toggle = GetComponent<Toggle>();
        }

        private void OnEnable()
        {
            if (toggle == null) toggle = GetComponent<Toggle>();
            if (toggle != null)
                toggle.onValueChanged.AddListener(PlayToggleSound);
        }

        private void OnDisable()
        {
            if (toggle != null)
                toggle.onValueChanged.RemoveListener(PlayToggleSound);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverClip != null && audioService != null)
                audioService.PlaySound(hoverClip);
        }

        private void PlayToggleSound(bool isOn)
        {
            if (audioService == null) return;
            if (isOn && toggleOnClip != null)
                audioService.PlaySound(toggleOnClip);
            else if (!isOn && toggleOffClip != null)
                audioService.PlaySound(toggleOffClip);
        }
    }
}
