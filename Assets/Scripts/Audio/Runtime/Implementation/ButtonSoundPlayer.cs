using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CardMatch.Audio
{
    [RequireComponent(typeof(Button))]
    public class ButtonSoundPlayer : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        [SerializeField] private AudioClip enableClip;
        [SerializeField] private AudioClip hoverClip;
        [SerializeField] private AudioClip clickClip;
        [SerializeField] private AudioService audioService;

        private void Start()
        {
            if (enableClip != null && audioService != null)
            {
                audioService.PlaySound(enableClip);
            }
        }

        public void SetAudioService(AudioService service)
        {
            audioService = service;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverClip != null && audioService != null)
                audioService.PlaySound(hoverClip);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (clickClip != null && audioService != null)
                audioService.PlaySound(clickClip);
        }
    }
}
