using UnityEngine;

namespace gameHub
{
    public class HealthBar : MonoBehaviour
    {
        private float _healthBarLength;
        public float curHealth;
        private float _maxHealth;
        private float _perHeal;
        private float _dmgPerAttack;

        public bool pausing = false;
        
        [SerializeField] private Vector3 offset;
        public GameObject player;

        private void Start()
        {
            _healthBarLength = transform.localScale.x;

            LevelHandler temp = GameObject.FindWithTag("levelhandler").GetComponent<LevelHandler>();
            _maxHealth = temp.maxHealth;
            _perHeal = temp.perHeal;
            _dmgPerAttack = temp.dmgPerAttack;
            curHealth = _maxHealth;
        }

        // Update is called once per frame
        void Update()
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position = player.transform.position + offset;
        }

        private void SetHealth(float factor)
        {
            transform.localScale = new Vector3((factor) * _healthBarLength,
                transform.localScale.y, transform.localScale.z);
        }
        
        public void TakeDamage()
        {
            if (curHealth > 0 && !pausing)
            {
                curHealth -= _dmgPerAttack;
                SetHealth(curHealth/_maxHealth);
            }
        }
        
        public void Heal()
        {
            if (curHealth < _maxHealth)
            {
                curHealth += _perHeal; 
                SetHealth(curHealth/_maxHealth);
            }
        }
    }
}