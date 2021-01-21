using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    private GameObject _player;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _player.transform.position;
    }
}
