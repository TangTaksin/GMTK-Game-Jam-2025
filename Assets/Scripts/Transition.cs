using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    Animator _animator;

    public delegate void TransitionEvent();
    public static TransitionEvent CalledFadeIn,CallFadeOut;

    public static TransitionEvent FadeInOver, FadeOutOver;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        CalledFadeIn += PlayFadeIn;
    }

    private void OnDisable()
    {
        CalledFadeIn -= PlayFadeIn;
    }


    public void PlayFadeIn()
    {
        _animator.Play("generic_transition_in");
    }

    public void FinishFadeIn()
    {
        FadeInOver?.Invoke();
    }

    public void PlayFadeOut()
    {
        _animator.Play("generic_transition_out");
    }

    public void FinishFadeOut()
    {
        FadeOutOver?.Invoke();
    }
}
