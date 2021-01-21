using menuHub;
using UnityEngine;

namespace dncycle
{
    public class ChangeColorSkyBox : MonoBehaviour
    {
        private Renderer _objRenderer;

        private bool _isNight;

        [SerializeField] private Color nightColor;
        [SerializeField] private Color dayColor;
        [SerializeField] private float animSpeed;
        [SerializeField] private float delay;

        private float _startTime;
        
        private void Start()
        {
            _isNight = GameObject.FindGameObjectWithTag("gamecore").GetComponent<DecorManager>().isNight;
            _startTime = Time.time + delay;
            if (!_isNight)
                RenderSettings.skybox.SetColor("_Tint", nightColor);
        }

        private void Update()
        {

            if (Time.time > _startTime)
            {
                float t = (Time.time - _startTime) * animSpeed;

                if (_isNight)
                {
                    RenderSettings.skybox.SetColor("_Tint", Color.Lerp(dayColor, nightColor, t));
                }
                else
                {
                    RenderSettings.skybox.SetColor("_Tint", Color.Lerp(nightColor, dayColor, t));
                }
            }
        }
    }
}