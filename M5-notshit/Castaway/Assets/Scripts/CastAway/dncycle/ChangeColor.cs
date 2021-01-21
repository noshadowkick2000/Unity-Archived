//DEPRECATED
/*using menuHub;
using UnityEngine;

namespace dncycle
{
    public class ChangeColor : MonoBehaviour
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
            _objRenderer = GetComponent<Renderer>();
            _isNight = GameObject.FindGameObjectWithTag("gamercore").GetComponent<DecorManager>().isNight;
            _startTime = Time.time + delay;
            if (!_isNight)
                _objRenderer.material.color = nightColor;
        }

        private void Update()
        {
            if (Time.time > _startTime)
            {

                float t = (Time.time - _startTime) * animSpeed;

                if (_isNight)
                {
                    _objRenderer.material.color = Color.Lerp(dayColor, nightColor, t);
                }
                else
                {
                    _objRenderer.material.color = Color.Lerp(nightColor, dayColor, t);
                }
            }
        }
    }
}
*/