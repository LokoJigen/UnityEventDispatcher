💡 **Quick Example: Creating and Triggering Events**

### 1. Create a custom event by inheriting `BaseEvent`

```csharp
using com.lokojigen.events;

public class PlayerDiedEvent : BaseEvent
{
    public string PlayerName { get; private set; }

    public PlayerDiedEvent(string playerName)
    {
        PlayerName = playerName;
    }

    public override string DebugInfo => $"Player: {PlayerName}";
}
```

---

### 2. Listen for that event (e.g. in a MonoBehaviour)

```csharp
public class GameManager : MonoBehaviour
{
    [SerializeField] private EventDispatcher eventDispatcher;

    private void OnEnable()
    {
        eventDispatcher.SubscribeToEvent<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnDisable()
    {
        eventDispatcher.UnsubscribeToEvent<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnPlayerDied(PlayerDiedEvent evt)
    {
        Debug.Log($"GameManager received death of {evt.PlayerName}");
    }
}
```

---

### 3. Trigger an event

```csharp
// From anywhere with access to EventDispatcher
eventDispatcher.TriggerEvent(new PlayerDiedEvent("Mario"));
```

Or with optional tag for debug/log purposes:

```csharp
eventDispatcher.TriggerEvent(new PlayerDiedEvent("Luigi", senderTag: "EnemyAI"));
```

---

🧩 **Optional: Custom Logger**

You can provide a logger to hook into your game's logging system:

```csharp
public class UnityDebugLogger : IEventLogger
{
    public void Log(string message)
    {
        Debug.Log(message);
    }
}

// Somewhere during initialization:
EventDispatcher.Logger = new UnityDebugLogger();
```
