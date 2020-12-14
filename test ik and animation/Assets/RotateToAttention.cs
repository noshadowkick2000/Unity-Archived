using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToAttention : MonoBehaviour
{
    private Collider[] _buffer = new Collider[5];
    [SerializeField] private float range;
    private Vector3 _rangeExtent;
    private int _layerMask;
    private Vector3 _midMousePos;
    private Vector2 _screenDimensions;

    // Start is called before the first frame update
    void Start()
    {
        _rangeExtent = new Vector3(range, range, range);
        _layerMask = LayerMask.GetMask("LookAt");
        
        _screenDimensions = new Vector2(Screen.width/2, Screen.height/2);
    }

    // Update is called once per frame
    void Update()
    {
        int size = Physics.OverlapBoxNonAlloc(transform.position, _rangeExtent, _buffer, Quaternion.identity, _layerMask);
        
        List<Vector3> _attentionPoints = new List<Vector3>();
        for (int i=0; i<size; i++)
        {
            if (_buffer[i].gameObject != gameObject)
            {
                _attentionPoints.Add(_buffer[i].transform.position);
            }
        }

        transform.LookAt(GetClosestAttention(_attentionPoints.ToArray()), Vector3.left);
    }

    Vector3 GetClosestAttention(Vector3[] attentionPoints)
    {
        if (attentionPoints.Length > 0)
        {
            float minDistance = 1000;
            int closestId = 0;
            for (int i = 0; i < attentionPoints.Length; i++)
            {
                float dist = Vector3.Distance(transform.position, attentionPoints[i]);
                if (dist < minDistance)
                {
                    closestId = i;
                    minDistance = dist;
                }
            }
            
            return attentionPoints[closestId];
        }
        
        _midMousePos = new Vector3(Input.mousePosition.x - _screenDimensions.x, Input.mousePosition.y - _screenDimensions.y, 0);
        Vector3 returnPosition = Camera.main.transform.position + Camera.main.transform.rotation * _midMousePos.normalized;
        return returnPosition;
    }
}
