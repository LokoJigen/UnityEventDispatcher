# EventDispatcher Library

A simple and **type-safe Event Dispatcher** for Unity to manage custom events with callbacks without relying on static events or global singleton instances.  
Ideal for decoupling systems and game flow.

---

## How It Works

- **Define** an event by creating a class that inherits from `BaseEvent` and add any data you want to transfer.
- **Subscribe** to event types with `Action<T>` callbacks on the `EventDispatcher`.
- **Unsubscribe** to avoid memory leaks or unwanted callbacks.
- **Trigger** an event using `TriggerEvent`, which invokes all callbacks subscribed to that event type.

The dispatcher prevents duplicate subscriptions and signals errors on subscribe/unsubscribe/trigger through state events.

---

## Quick Example

```csharp
// 1. Define a custom event
public class PlayerDiedEvent : BaseEvent
{
    public string PlayerName { get; private set; }
    public PlayerDiedEvent(string playerName, string senderTag = null) : base(senderTag) 
        => PlayerName = playerName;
}

// 2. Listener in a MonoBehaviour
public class GameManager : MonoBehaviour
{
    [SerializeField] private EventDispatcher eventDispatcher;

    private void OnEnable() => eventDispatcher.SubscribeToEvent<PlayerDiedEvent>(OnPlayerDied);
    private void OnDisable() => eventDispatcher.UnsubscribeToEvent<PlayerDiedEvent>(OnPlayerDied);

    private void OnPlayerDied(PlayerDiedEvent evt)
    {
        Debug.Log($"GameManager received death of {evt.PlayerName}");
    }
}

// 3. Trigger the event
eventDispatcher.TriggerEvent(new PlayerDiedEvent("Mario"));
