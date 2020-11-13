/*using gameHub;
using gameObjects;
using ui;
using UnityEngine;

OLD FEATURE

public class CameraControl : MonoBehaviour
{

    private Vector3 _playerOffset;
    private bool _thisCameraActive;
    [SerializeField] private float _rotSpeed;
    [SerializeField] private float smoothVar;
    private LevelHandler _levelHandler;
    private GameObject _player;
    private Player _playerScript;
    private GameObject _camObject;
    private UI _ui;

    // Start is called before the first frame update
    void Start()
    {
        _camObject = GameObject.FindWithTag("secondarycam");
        _player = GameObject.FindWithTag("Player");
        _playerScript = _player.GetComponent<Player>();
        _ui = GameObject.FindWithTag("ui").GetComponent<UI>();
        _levelHandler = GameObject.FindWithTag("levelhandler").GetComponent<LevelHandler>();
        _thisCameraActive = false;
        _playerOffset = _player.transform.position - _camObject.transform.position;
        _camObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_levelHandler.active) 
            InputSwitchCamera();
    }

    private void LateUpdate()
    {
        MoveCamera();
    }

    private void InputSwitchCamera()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            _thisCameraActive = !_thisCameraActive;
            _camObject.SetActive(_thisCameraActive);
            if (_thisCameraActive) 
                _ui.SwitchCamera(_camObject.GetComponent<Camera>());
            else
            {
                _ui.SwitchCamera(Camera.main);
                _playerScript.SetCameraOffset(0);
            }
        }
    }

    private void MoveCamera()
    {
        if (_thisCameraActive)
        {
            Quaternion cameraAngleHorizontal = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * _rotSpeed, Vector3.up);
            Quaternion cameraAngleVertical = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * _rotSpeed, Vector3.left);

            Quaternion combinedAngle = cameraAngleHorizontal * cameraAngleVertical;

            _playerOffset = combinedAngle * _playerOffset;

            Vector3 camPosition = _player.transform.position - _playerOffset;
            
            if (camPosition.z > _player.transform.position.z)
            {
                camPosition.Set(camPosition.x, camPosition.y, _player.transform.position.z);
            }
            if (camPosition.y < _player.transform.position.y+4)
            {
                camPosition.Set(camPosition.x, _player.transform.position.y+4, camPosition.z);
            }
            
            _camObject.transform.position = Vector3.Slerp(_camObject.transform.position, camPosition, smoothVar);

            _camObject.transform.LookAt(_player.transform);

            float actualRotation = _camObject.transform.eulerAngles.y;
            if (actualRotation >= 270 && actualRotation <= 360)
            {
                actualRotation -= 360;
            }
            _playerScript.SetCameraOffset(actualRotation);
            print(actualRotation);
        }
    }
}
*/