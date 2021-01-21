using gameHub;
using UnityEngine;

namespace environments
{
    public class FireLight : MonoBehaviour
    {

        private Light _thisLight;
        [SerializeField] private float centerValue;
        [SerializeField] private float range;
        [SerializeField] private bool zombieFire;

        private Vector3 originalPosition;
        private LevelHandler _levelHandler;
        private bool positionSet = false;

        private void Awake()
        {
            _thisLight = GetComponent<Light>();
            _levelHandler = GameObject.FindWithTag("levelhandler").GetComponent<LevelHandler>();
            if (zombieFire)
                originalPosition = transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            if (_levelHandler.state != 0)
            {
                if (!zombieFire)
                {
                    if (positionSet)
                    {
                        float perlinVar = (Mathf.PerlinNoise(Time.time * 5, 0) * range);
                        _thisLight.intensity = centerValue - perlinVar;
                        transform.position = originalPosition + new Vector3(0, perlinVar / 4, 0);
                    }
                    else
                    {
                        originalPosition = transform.position;
                        positionSet = true;
                    }
                }
                else
                {
                    float perlinVar = (Mathf.PerlinNoise(Time.time * 5, 0) * range);
                    _thisLight.intensity = centerValue - perlinVar;
                    transform.localPosition = originalPosition + new Vector3(0, perlinVar / 4, 0);
                }
            }
        }
    }
}