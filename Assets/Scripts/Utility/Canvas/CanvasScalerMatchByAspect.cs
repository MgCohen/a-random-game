using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.Utility
{
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasScalerMatchByAspect : MonoBehaviour
    {
        private CanvasScaler _scaler;

        private void Awake()
        {
            _scaler = GetComponent<CanvasScaler>();
        }

        private void OnEnable()
        {
            if (_scaler == null)
                _scaler = GetComponent<CanvasScaler>();
            if (_scaler != null)
                _scaler.matchWidthOrHeight = Screen.width > Screen.height ? 1f : 0f;
        }
    }
}
