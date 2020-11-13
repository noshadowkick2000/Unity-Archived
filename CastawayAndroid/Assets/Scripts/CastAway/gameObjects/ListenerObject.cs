using UnityEngine;

public class ListenerObject : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
