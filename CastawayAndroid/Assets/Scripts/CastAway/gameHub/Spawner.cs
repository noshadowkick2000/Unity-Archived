using gameObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace gameHub
{
    public class Spawner : MonoBehaviour
    {

        [SerializeField] private Vector3 center;
        [SerializeField] private float range;
        [SerializeField] private float startRange;
        [SerializeField] private float depth;

        [SerializeField] private GameObject zombiePrefab;
        [SerializeField] private GameObject specialZombiePrefab;
        [SerializeField] private GameObject bigZombiePrefab;
        [SerializeField] private float minDistanceBetweenSpawns;
        [SerializeField] private float minDistanceBetweenSpecialTarget;
        [SerializeField] private Vector3 bigZombieSpawnLocation;

        private Vector3 oldSpawnPoint = Vector3.zero;

        private Vector3 offset;

        private void Start()
        {
            transform.position = center;
        }

        public void spawnZombie(int comboAmount, float zombiespeed)
        {
            MoveSpawner();
            while (Vector3.Distance(oldSpawnPoint, transform.position) < minDistanceBetweenSpawns)
            {
                MoveSpawner();
            }
            
            oldSpawnPoint = transform.position;

            print("spawn");
            GameObject zombie;
            zombie = Instantiate(zombiePrefab, transform.position, transform.rotation);
            Zombie zombieScript = zombie.GetComponent<Zombie>();
            zombieScript.setValues(comboAmount, zombiespeed);
        }

        public void spawnSpecialZombie(int comboAmount, float zombiespeed)
        {
            MoveSpawner();
            while (Vector3.Distance(oldSpawnPoint, transform.position) < minDistanceBetweenSpawns)
            {
                MoveSpawner();
            }

            oldSpawnPoint = transform.position;
            
            GameObject specialZombie;
            specialZombie = Instantiate(specialZombiePrefab, transform.position, transform.rotation);
            SpecialZombie specialZombieScript = specialZombie.GetComponent<SpecialZombie>();
            
            MoveSpawner();
            while (Vector3.Distance(oldSpawnPoint, transform.position) < minDistanceBetweenSpecialTarget)
            {
                MoveSpawner();
            }
            
            oldSpawnPoint = transform.position;
            
            specialZombieScript.setValues(comboAmount, zombiespeed, transform.position);
        }

        public void spawnBigZombie()
        {
            Instantiate(bigZombiePrefab, bigZombieSpawnLocation, Quaternion.Euler(0,0,0));
        }

        private void MoveSpawner()
        {
            int side = Random.Range(0, 2);
            float xOffset;
            if (side==1)
                xOffset = Random.Range(-range, -startRange*range);
            else
            {
                xOffset = Random.Range(startRange*range, range);
            }
            float yOffset = -Mathf.Sqrt(((range*range)-(xOffset*xOffset)));
            offset = new Vector3(xOffset, depth, yOffset);
            transform.position = center + offset;
        }
    }
}