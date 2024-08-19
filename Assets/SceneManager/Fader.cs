using System;
using UnityEngine;

public class Fader : MonoBehaviour
{
    private const string PATH = "Fader";

    [SerializeField] private Animator animator;

    private static Fader _instance;

    public static Fader Instance
    {
        get
        {
            if (_instance == null)
            {
                var prefab = Resources.Load<Fader>(PATH);
                _instance = Instantiate(prefab);
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    private bool isFading;

    private Action _fadeInCallback;
    private Action _fadeOutCallback;

    public void FadeIn(Action fadeInCallback)
    {
        if (isFading) return;

        _fadeInCallback = fadeInCallback;
        isFading = true;
        animator.SetBool("faded", true);
    }

    public void FadeOut(Action fadeOutCallback)
    {
        if (isFading) return;

        _fadeOutCallback = fadeOutCallback;
        isFading = true;
        animator.SetBool("faded", false);
    }

    private void Handler_FadeInAnimationOver()
    {
        _fadeInCallback?.Invoke();
        _fadeInCallback = null;
        isFading = false;
    }

    private void Handler_FadeOutAnimationOver()
    {
        _fadeOutCallback?.Invoke();
        _fadeOutCallback = null;
        isFading = false;
    }
} 
