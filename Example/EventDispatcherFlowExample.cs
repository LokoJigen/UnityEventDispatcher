using System;
using com.lokojigen.events;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// PlayerDiedEvent is an event example that is triggered when a player dies in your game.
/// It inherits from BaseEvent and carries the dead player's name as data.
/// </summary>
public class PlayerDiedEvent : BaseEvent
{
    public string PlayerName;

    public PlayerDiedEvent(string playerName, string senderTag = null) : base(senderTag)
    {
        PlayerName = playerName;
    }

    public override string ToString()
    {
        return base.ToString() + $" | PlayerName: {PlayerName}";
    }
}

/// <summary>
/// EventDispatcherStateListener is a utility example class that listens to the state changes of an EventDispatcher.
/// It logs the events of subscribing, unsubscribing, triggering, and failures.
/// </summary>
public class EventDispatcherStateListener
{
    public void AttachTo(EventDispatcher dispatcher)
    {
        if (dispatcher == null) return;

        Debug.Log($"[{GetType().Name}] Attaching to {dispatcher.GetType().Name} attached to GameObject {dispatcher.name}");

        dispatcher.OnSubscribed -= OnSubscribed;
        dispatcher.OnUnsubscribed -= OnUnsubscribed;
        dispatcher.OnTrigger -= OnTriggered;
        dispatcher.OnCallbackFailure -= OnCallbackFailure;
        dispatcher.OnTriggerFailure -= OnTriggerFailure;

        dispatcher.OnSubscribed += OnSubscribed;
        dispatcher.OnUnsubscribed += OnUnsubscribed;
        dispatcher.OnTrigger += OnTriggered;
        dispatcher.OnCallbackFailure += OnCallbackFailure;
        dispatcher.OnTriggerFailure += OnTriggerFailure;
    }

    public void DetachFrom(EventDispatcher dispatcher)
    {
        if (dispatcher == null) return;

        Debug.Log($"[{GetType().Name}] Detaching from {dispatcher.GetType().Name} attached to GameObject {dispatcher.name}");

        dispatcher.OnSubscribed -= OnSubscribed;
        dispatcher.OnUnsubscribed -= OnUnsubscribed;
        dispatcher.OnTrigger -= OnTriggered;
        dispatcher.OnCallbackFailure -= OnCallbackFailure;
        dispatcher.OnTriggerFailure -= OnTriggerFailure;
    }

    private void OnSubscribed(Type type, Delegate callback)
    {
        Debug.Log($"[{GetType().Name}] Subscribed {callback.Method.Name} to {type.Name}");
    }

    private void OnUnsubscribed(Type type, Delegate callback)
    {
        Debug.Log($"[{GetType().Name}] Unsubscribed {callback.Method.Name} from {type.Name}");
    }

    private void OnTriggered(Type type, BaseEvent evt)
    {
        Debug.Log($"[{GetType().Name}] Triggered {type.Name} with event: {evt}");
    }

    private void OnCallbackFailure(Type type, Delegate callback, CallbackFailureReason reason)
    {
        Debug.LogWarning($"[{GetType().Name}] Callback failure for {type.Name} with callback {callback.Method.Name}: {reason}");
    }

    private void OnTriggerFailure(Type type, BaseEvent evt)
    {
        Debug.LogWarning($"[{GetType().Name}] Trigger failure for {type.Name} with event: {evt}");
    }

}

/// <summary>
/// EventDispatcherFlowExample demonstrates how to use the EventDispatcher to subscribe, unsubscribe, and trigger events.
/// It includes options to control whether to unsubscribe on disable and whether the listener should unsubscribe on disable.
/// It also shows that is impossible to subscribe the same callback multiple times for the same event type.
/// If you try to do so, it will log a warning and not allow the duplicate subscription.
/// </summary>
public class EventDispatcherFlowExample : MonoBehaviour
{
    public bool IsUnsubscribeOnDisable = true;
    public bool IsListenerDetachOnDisable = true;
    public bool IsSingleSubscription = true;

    private EventDispatcher _dispatcher;
    private EventDispatcherStateListener m_eventStateDispatcherListener;

    private void OnEnable()
    {
        if (_dispatcher == null)
        {
            _dispatcher = transform.AddComponent<EventDispatcher>();
        }

        if (m_eventStateDispatcherListener == null)
        {
            m_eventStateDispatcherListener = new EventDispatcherStateListener();
        }

        m_eventStateDispatcherListener.AttachTo(_dispatcher);

        if (IsSingleSubscription)
        {
            _dispatcher.SubscribeToEvent<PlayerDiedEvent>(OnPlayerDiedLogic);

            _dispatcher.SubscribeToEvent<PlayerDiedEvent>(OnPlayerDiedGraphic);
        }
        else
        {
            // Subscribe more then once to demonstrate that is not allowed
            _dispatcher.SubscribeToEvent<PlayerDiedEvent>(OnPlayerDiedLogic);
            _dispatcher.SubscribeToEvent<PlayerDiedEvent>(OnPlayerDiedLogic);

            _dispatcher.SubscribeToEvent<PlayerDiedEvent>(OnPlayerDiedGraphic);
            _dispatcher.SubscribeToEvent<PlayerDiedEvent>(OnPlayerDiedGraphic);
        }

        // Simulate triggering the event
        var evt = new PlayerDiedEvent("LokoJigen the barbarian", "EventDispatcherFlowExample");
        _dispatcher.TriggerEvent(evt);
    }

    private void OnDisable()
    {
        if (_dispatcher == null) return; // GUARD CASE

        if (IsUnsubscribeOnDisable)
        {
            _dispatcher.UnsubscribeToEvent<PlayerDiedEvent>(OnPlayerDiedLogic);
            _dispatcher.UnsubscribeToEvent<PlayerDiedEvent>(OnPlayerDiedGraphic);
        }

        if (IsListenerDetachOnDisable && m_eventStateDispatcherListener != null)
        {
            m_eventStateDispatcherListener.DetachFrom(_dispatcher);
        }
    }

    private void OnPlayerDiedLogic(PlayerDiedEvent e)
    {
        Debug.Log($"[{GetType().Name}] OnPlayerDiedLogic -  PlayerDiedEvent : {e}");
    }

    private void OnPlayerDiedGraphic(PlayerDiedEvent e)
    {
        Debug.Log($"[{GetType().Name}] OnPlayerDiedGraphic - PlayerDiedEvent : {e}");
    }

}