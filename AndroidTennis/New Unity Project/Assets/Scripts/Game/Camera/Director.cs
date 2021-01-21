using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Game.Camera
{
    public class Director : Singleton<Director>
    {
        [SerializeField] private float maxOffset;
        [SerializeField] private Transform mainCamera;

        [SerializeField] private GameObject ballOutUi;
        [SerializeField] private GameObject secondBounceUi;
        [SerializeField] private GameObject netUi;
        [SerializeField] private GameObject scoreUi;

        [SerializeField] private Transform sideLineCam;
        [SerializeField] private Transform endScreenCam;

        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI timesKnockedDownText;
        [SerializeField] private TextMeshProUGUI racketCountText;

        private float _yOffset;
        private float _fieldWidth;
        private Player _playerObject;
        private Vector3 _startPosition;

        public void SetParameters(float width, Player player)
        {
            _fieldWidth = width;
            _playerObject = player;

            _yOffset = (mainCamera.position.z - player.Get2DPosition().y);

            _startPosition = mainCamera.position;
        }

        public void MoveCamera(Vector2 ballPosition)
        {
            float goalXPlayer = (_playerObject.Get2DPosition().x / _fieldWidth) * maxOffset;
            float goalXBall = (ballPosition.x / _fieldWidth) * maxOffset;
            float combinedX = (goalXBall + goalXPlayer) / 2;

            float goalYPlayer = (_playerObject.Get2DPosition().y + _yOffset);
            
            mainCamera.position = Vector3.Lerp(mainCamera.position, new Vector3(combinedX, mainCamera.position.y, goalYPlayer), .1f);
        }

        public void ResetCamera()
        {
            endScreenCam.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(true);
            mainCamera.position = _startPosition;
            sideLineCam.gameObject.SetActive(false);
            netUi.SetActive(false);
            secondBounceUi.SetActive(false);
            ballOutUi.SetActive(false);
            scoreUi.SetActive(false);
        }

        public void Net()
        {
            netUi.SetActive(true);
            mainCamera.gameObject.SetActive(false);
            sideLineCam.gameObject.SetActive(true);
        }

        public void SecondBounce()
        {
            secondBounceUi.SetActive(true);
            mainCamera.gameObject.SetActive(false);
            sideLineCam.gameObject.SetActive(true);
        }

        public void BallOut()
        {
            ballOutUi.SetActive(true);
            mainCamera.gameObject.SetActive(false);
            sideLineCam.gameObject.SetActive(true);
        }

        public void AimSideLineCam(Vector3 ballPosition)
        {
            sideLineCam.LookAt(ballPosition);
        }

        public void DisplayEndScreen()
        {
            sideLineCam.gameObject.SetActive(false);
            endScreenCam.gameObject.SetActive(true);
        }

        public void OrbitEndScreenCamera()
        {
            endScreenCam.RotateAround(Vector3.zero, Vector3.up, .1f);
            endScreenCam.LookAt(Vector3.zero);
        }

        public void DisplayScore(string playerScore, string cpuScore)
        {
            scoreUi.SetActive(true);
            scoreUi.GetComponent<TextMeshProUGUI>().text = playerScore + " - " + cpuScore;
        }

        public void SetEndCard(string playerScore, string cpuScore, int timesKnockedDown, int totalRackets)
        {
            finalScoreText.text = playerScore + " - " + cpuScore;
            timesKnockedDownText.text = timesKnockedDown.ToString();
            racketCountText.text = totalRackets.ToString();
        }
    }
}