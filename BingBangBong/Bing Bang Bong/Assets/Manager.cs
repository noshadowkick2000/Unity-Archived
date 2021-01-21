using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Random = System.Random;

public class Manager : MonoBehaviour
{
    private enum States
    {
        PreMove = 0,
        PostMove = 1,
        Replay = 2
    }

    private States state = States.PreMove;
    private GameSetter gameSetter;
    private Transform cameraAnchor;
    private Animator cameraAnimator;
    [SerializeField] private int animationCount;
    private int animationIndex;

    [SerializeField] private Text description;
    [SerializeField] private GameObject MateUi;
    private PlayableDirector MateTimeline;
    [SerializeField] private GameObject DrawUi;
    private PlayableDirector DrawTimeline;
    [SerializeField] private GameObject ReplayUi;
    private PlayableDirector ReplayTimeline;

    private ReplayCam replayCam;

    private void Awake()
    {
        MateTimeline = MateUi.GetComponent<PlayableDirector>();
        DrawTimeline = DrawUi.GetComponent<PlayableDirector>();
        ReplayTimeline = ReplayUi.GetComponent<PlayableDirector>();

        cameraAnchor = transform.parent;
        cameraAnimator = GetComponent<Animator>();
        gameSetter = FindObjectOfType<GameSetter>();
        replayCam = FindObjectOfType<ReplayCam>();
        gameSetter.SetRandomBoard(description);
        AnchorCamera();
    }

    private void StartRandomAnimation()
    {
        StartAnimation(animationIndex);
        animationIndex++;
        if (animationIndex > animationCount - 1)
            animationIndex = 0;
    }

    private void StartAnimation(int index)
    {
        cameraAnchor.transform.Rotate(0, UnityEngine.Random.Range(30f, 60f), 0);
        cameraAnimator.SetInteger("CurrentAnimation", index);
        cameraAnimator.SetTrigger("TriggerAnimation");
    }

    private void AnchorCamera()
    {
        replayCam.DeAnchorDeActivateCamera();
        
        DisableAllUi();
        
        cameraAnchor.transform.parent = gameSetter.GetMovingPieceTransform();
        cameraAnchor.transform.localPosition = Vector3.zero;
        
        replayCam.AnchorCamera(gameSetter.GetMovingPieceTransform());
        
        StartRandomAnimation();
    }

    private void FreeCamera()
    {
        if (state == States.PreMove)
            StartRandomAnimation();
        else
            cameraAnchor.transform.parent = null;
    }

    public void Moved(bool mate)
    {
        StartAnimation(2);
        state = States.PostMove;
        if (mate)
        {
            MateUi.SetActive(true);
            MateTimeline.Stop();
            MateTimeline.Play();
        }
        else
        {
            DrawUi.SetActive(true);
            DrawTimeline.Stop();
            DrawTimeline.Play();
        }
    }

    public void Replay()
    {
        print("what");
        DisableAllUi();
        ReplayUi.SetActive(true);
        ReplayTimeline.Stop();
        ReplayTimeline.Play();
    }

    public void Reset()
    {
        state = States.PreMove;
        gameSetter.SetNextBoard(description);
        AnchorCamera();
    }

    private void DisableAllUi()
    {
        MateUi.SetActive(false);
        DrawUi.SetActive(false);
        ReplayUi.SetActive(false);
    }
    
    private void Update()
    {
        // Debug controls
        if (Input.GetMouseButtonDown(0) && state == States.PreMove)
            gameSetter.MovePiece(false);
    }
}
