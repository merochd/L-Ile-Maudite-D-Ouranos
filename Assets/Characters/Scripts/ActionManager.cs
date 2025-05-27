
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    private class ActionSubscriber
    {
        public InputAction action;
        public ActionState state;
        public Action<InputAction.CallbackContext> callback;
        private bool isSubscribe;

        public void subscribe()
        {
            if (isSubscribe) return;
            isSubscribe = true;

            if (!action.enabled)
            {
                action.Enable();
            }

            switch (state)
            {
                case ActionState.Started: action.started += callback; break;
                case ActionState.Canceled: action.canceled += callback; break;
                case ActionState.Performed: action.performed += callback; break;
            }
        }

        public void unsubscribe()
        {
            if (!isSubscribe) return;
            isSubscribe = false;

            switch (state)
            {
                case ActionState.Started: action.started -= callback; break;
                case ActionState.Canceled: action.canceled -= callback; break;
                case ActionState.Performed: action.performed -= callback; break;
            }
        }
    }

    [SerializeField] public InputActionAsset config;

    private List<ActionSubscriber> subscribers = new List<ActionSubscriber>();

    void Awake()
    {
        if (!config)
        {
            throw new Exception("ActionManager need config");
        }
    }

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (var subscriber in subscribers)
        {
            subscriber.subscribe();
        }
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (var subscriber in subscribers)
        {
            subscriber.unsubscribe();
        }

        foreach (var subscriber in subscribers)
        {
            if (subscriber.action.enabled)
            {
                subscriber.action.Disable();
            }
        }
    }

    public void AddAction(
        string name,
        Action<InputAction.CallbackContext> callback
    )
    {
        AddAction(name, ActionState.Started, callback);
    }
    
    public void AddAction(
        string name,
        ActionState state,
        Action<InputAction.CallbackContext> callback
    )
    {
        Debug.Log($"AddAction {name} {state}");

        var action = config.FindAction(name);

        if (action == null) throw new Exception($"no input action {name}");

        var subscriber = new ActionSubscriber()
        {
            action = action,
            state = state,
            callback = callback
        };

        subscribers.Add(subscriber);
        if (enabled) subscriber.subscribe();
    }
}

