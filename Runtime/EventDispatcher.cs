using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.lokojigen.events
{
    public enum CallbackFailureReason
    {
        Unknown,
        UnsubscriptionMissingDelegate,
        UnsubscriptionMissingEventType,
        AlreadySubscribed,
    }

    /// <summary>
    /// EventDispatcher is a Unity MonoBehaviour that allows you to manage events in a type-safe manner.
    /// It supports subscribing, unsubscribing, and triggering events with callbacks.
    /// It also provides events to notify subscribers about changes in the event dispatcher state and failures in subscription, unsubscription, or triggering.
    /// BE AWARE: This class is not thread-safe and should be used in the main Unity thread only.
    /// BE AWARE: This class does not support duplicate subscriptions, meaning if you try to subscribe the same callback multiple times, it will fail and notify the failure event.
    /// BE AWARE: The delegates subscription list order matters for the order of execution when triggering events. 
    /// </summary>
    public class EventDispatcher : MonoBehaviour
    {
        private Dictionary<Type, Delegate> eventTable = new();

        // Events to notify subscribers about changes in the event dispatcher state
        public event Action<Type, Delegate> OnSubscribed;
        public event Action<Type, Delegate> OnUnsubscribed;
        public event Action<Type, BaseEvent> OnTrigger;

        // Events to notify subscribers about failures in subscription, unsubscription, or triggering
        public event Action<Type, Delegate, CallbackFailureReason> OnCallbackFailure;
        public event Action<Type, BaseEvent> OnTriggerFailure;

        /// <summary>
        /// SubscribeToEvent allows you to register a callback for a specific event type.
        /// If the event type is already registered, it will notify the failed subscription event with a reason.
        /// </summary>
        public void SubscribeToEvent<T>(Action<T> callback) where T : BaseEvent
        {
            var type = typeof(T);

            if (eventTable.TryGetValue(type, out var existingDelegate))
            {
                foreach (var del in existingDelegate.GetInvocationList())
                {
                    if (del == (Delegate)callback)
                    {
                        OnCallbackFailure?.Invoke(type, callback, CallbackFailureReason.AlreadySubscribed);
                        return; // Already registered
                    }
                }

                eventTable[type] = Delegate.Combine(existingDelegate, callback);
            }
            else
            {
                eventTable[type] = callback;
            }

            OnSubscribed?.Invoke(type, callback);
        }

        /// <summary>
        /// UnsubscribeToEvent allows you to remove a previously registered callback for a specific event type.
        /// If the callback is not found, it will notify the failed unsubscription event with a reason.
        /// </summary>
        public void UnsubscribeToEvent<T>(Action<T> callback) where T : BaseEvent
        {
            var type = typeof(T);

            if (eventTable.TryGetValue(type, out var existingDelegate))
            {
                var invocationList = existingDelegate.GetInvocationList();
                bool found = false;

                foreach (var del in invocationList)
                {
                    if (del == (Delegate)callback)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    OnCallbackFailure?.Invoke(type, callback, CallbackFailureReason.UnsubscriptionMissingDelegate);
                    return;
                }

                // Remove only the first occurrence (safe, since duplicate subscriptions are no longer allowed)
                var newDelegate = Delegate.Remove(existingDelegate, callback);

                if (newDelegate == null)
                {
                    eventTable.Remove(type);
                }
                else
                {
                    eventTable[type] = newDelegate;
                }

                OnUnsubscribed?.Invoke(type, callback);
            }
            else
            {
                OnCallbackFailure?.Invoke(type, callback, CallbackFailureReason.UnsubscriptionMissingEventType);
            }
        }

        /// <summary>
        /// TriggerEvent allows you to invoke all callbacks registered for a specific event type.
        /// If no callbacks are registered, it will notify the trigger failure event.
        /// </summary>
        public void TriggerEvent<T>(T evt) where T : BaseEvent
        {
            var type = typeof(T);
            if (eventTable.TryGetValue(type, out var del) && del is Action<T> action)
            {
                OnTrigger?.Invoke(type, evt);
                action.Invoke(evt);
            }
            else
            {
                OnTriggerFailure?.Invoke(type, evt);
            }
        }
    }
}
