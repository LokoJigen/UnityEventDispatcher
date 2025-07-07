using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.lokojigen.events
{
    /// <summary>
    /// Use this implementation for internal app communication with typed events.
    /// </summary>
    public class EventDispatcher : MonoBehaviour
    {
        // Contains all available events types
        protected Dictionary<Type, List<Action<object>>> eventDictionary = new();

        public static IEventLogger Logger { get; set; }

        /// <summary>
        /// Add the callback to T event when triggered.
        /// </summary>
        /// <typeparam name="T">Type of Event</typeparam>
        /// <param name="callback">Method to call when event T is triggered</param>
        public void SubscribeToEvent<T>(Action<T> callback) where T : BaseEvent
        {
            Type eventType = typeof(T);
            if (!eventDictionary.ContainsKey(eventType))
            {
                eventDictionary[eventType] = new List<Action<object>>();
            }

            Logger?.Log($"[EventManager] SubscribeToEvent - {eventType}\nFor obj (script) {callback.Target}\nMethod {callback.Method.Name}");

            eventDictionary[eventType].Add((param) => callback((T)param));
        }

        /// <summary>
        /// Unsubscribe callback from T event.
        /// </summary>
        /// <typeparam name="T">Type of event</typeparam>
        /// <param name="callback">Method to unsubscribe from event T</param>
        public void UnsubscribeToEvent<T>(Action<T> callback) where T : BaseEvent
        {
            Type eventType = typeof(T);
            if (eventDictionary.ContainsKey(eventType))
            {
                Logger?.Log($"[EventManager] UnsubscribeToEvent - {eventType}\nFor obj (script) {callback.Target}\nMethod {callback.Method.Name}");

                eventDictionary[eventType].Remove((param) => callback((T)param));
            }
        }

        /// <summary>
        /// Trigger all callbacks stored for event T, if any.
        /// </summary>
        /// <typeparam name="T">Type of event</typeparam>
        /// <param name="targetEvent">Event to trigger</param>
        public void TriggerEvent<T>(T targetEvent) where T : BaseEvent
        {
            Type eventType = typeof(T);
            if (eventDictionary.ContainsKey(eventType))
            {
                Logger?.Log($"[EventManager] TriggerEvent - {targetEvent.GetType()}");

                for (int i = 0; i < eventDictionary[eventType].Count; i++)
                {
                    eventDictionary[eventType][i].Invoke(targetEvent);
                }
            }
        }
    }
}