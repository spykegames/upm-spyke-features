# CLAUDE.md - Spyke Features Package

## What This Does
Reusable game features package providing common gameplay systems like chests, inbox, leaderboards, daily bonuses, tutorials, season passes, and team features.

## Package Structure

```
upm-spyke-features/
├── Runtime/
│   ├── Chest/         ← Reward chest opening with animations
│   ├── Inbox/         ← Player messages and notifications
│   ├── Leaderboard/   ← Ranking and competition displays
│   ├── Rewards/       ← Reward granting and display
│   ├── DailyBonus/    ← Daily login rewards
│   ├── Tutorial/      ← Tutorial system and steps
│   ├── SeasonPass/    ← Battle pass progression
│   ├── Team/          ← Team management features
│   └── Spyke.Features.asmdef
├── Editor/
│   └── Spyke.Features.Editor.asmdef
├── Tests/
│   ├── Runtime/
│   └── Editor/
├── package.json
└── CLAUDE.md
```

## Key Features

| Feature | Purpose | Status |
|---------|---------|--------|
| `Runtime/Chest/` | Reward chest system | 🚧 TODO |
| `Runtime/Inbox/` | Player inbox/messages | 🚧 TODO |
| `Runtime/Leaderboard/` | Rankings and competitions | 🚧 TODO |
| `Runtime/Rewards/` | Reward granting system | 🚧 TODO |
| `Runtime/DailyBonus/` | Daily login rewards | 🚧 TODO |
| `Runtime/Tutorial/` | Tutorial step system | 🚧 TODO |
| `Runtime/SeasonPass/` | Battle pass feature | 🚧 TODO |
| `Runtime/Team/` | Team management | 🚧 TODO |

## MVCN Pattern

Each feature follows the Model-View-Controller-Network pattern:

```
Features/{Feature}/
├── {Feature}Controller.cs    ← Business logic
├── {Feature}Model.cs         ← Data and state
├── {Feature}View.cs          ← UI presentation
├── {Feature}Network.cs       ← API communication (optional)
├── {Feature}Installer.cs     ← Zenject bindings (optional)
```

## How to Use

### Installation
```json
// Packages/manifest.json
{
  "dependencies": {
    "com.spykegames.features": "https://github.com/spykegames/upm-spyke-features.git#v0.1.0"
  }
}
```

### Basic Usage
```csharp
using Spyke.Features.Chest;
using Spyke.Features.Inbox;

// Chest
[Inject] private readonly IChestController _chest;
await _chest.OpenChestAsync(chestId);

// Inbox
[Inject] private readonly IInboxController _inbox;
var messages = await _inbox.GetMessagesAsync();
```

## Dependencies
- com.spykegames.core (required)
- com.spykegames.services (required)
- com.spykegames.ui (required)

## Depends On This
- Game-specific projects

## Source Files to Port

From `client-bootstrap`:
| Source | Destination |
|--------|-------------|
| `CoreFramework/Runtime/Chest/` | `Runtime/Chest/` |
| `CoreFramework/Runtime/Inbox/` | `Runtime/Inbox/` |
| `CoreFramework/Runtime/Leaderboard/` | `Runtime/Leaderboard/` |
| `CubeBusters/HourlyBonus/` | `Runtime/DailyBonus/` |
| `CubeBusters/Tutorial/` | `Runtime/Tutorial/` |
| `CubeBusters/SeasonPass/` | `Runtime/SeasonPass/` |
| `CoreFramework/Runtime/Team/` | `Runtime/Team/` |

## Status
🚧 **IN DEVELOPMENT** - Package structure created, features pending

### Completed
- ✅ Package structure created
- ✅ Assembly definitions configured
- ✅ CLAUDE.md documentation

### Planned Features
- 🚧 Chest (chest opening, rewards, animations)
- 🚧 Inbox (messages, notifications, claiming)
- 🚧 Leaderboard (rankings, tabs, rewards)
- 🚧 DailyBonus (daily rewards, streaks)
- 🚧 Tutorial (step system, highlighting)
- 🚧 SeasonPass (tiers, rewards, progression)
- 🚧 Team (create, join, chat, contributions)
