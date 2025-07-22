using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Unity singleton that allows you to execute code on the main thread from background threads.
/// This is needed because Unity's API can only be called from the main thread.
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    private static UnityMainThreadDispatcher _instance = null;

    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find existing instance
                _instance = FindObjectOfType<UnityMainThreadDispatcher>();
                
                // If no instance exists, create one
                if (_instance == null)
                {
                    GameObject go = new GameObject("UnityMainThreadDispatcher");
                    _instance = go.AddComponent<UnityMainThreadDispatcher>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Ensure only one instance exists
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    /// <summary>
    /// Enqueue an action to be executed on the main thread
    /// </summary>
    /// <param name="action">The action to execute</param>
    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    /// <summary>
    /// Enqueue an IEnumerator to be started as a coroutine on the main thread
    /// </summary>
    /// <param name="coroutine">The coroutine to start</param>
    public void EnqueueCoroutine(IEnumerator coroutine)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => StartCoroutine(coroutine));
        }
    }
}
