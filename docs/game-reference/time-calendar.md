# Time & Calendar System

## Overview

Littlewood uses a day/week/season/year calendar system. Each season has 28 days (4 weeks of 7 days).

## Time Variables

All stored in `GameScript` as static fields:

```csharp
public static int day;          // Day of week (1-7, Monday-Sunday)
public static int week;         // Week of season (0-3)
public static int season;       // Season (0-3)
public static int year;         // Current year
public static int daysPlayed;   // Total days played
```

## Calendar Structure

### Days Per Season
- 28 days per season (4 weeks × 7 days)
- Total year: 112 days (4 seasons × 28 days)

### Day of Week
| Value | Day |
|-------|-----|
| 1 | Monday |
| 2 | Tuesday |
| 3 | Wednesday |
| 4 | Thursday |
| 5 | Friday |
| 6 | Saturday |
| 7 | Sunday |

### Seasons
| Value | Season |
|-------|--------|
| 0 | Spring |
| 1 | Summer |
| 2 | Autumn |
| 3 | Winter |

## Date Advancement

The `IncreaseDate()` method handles day progression:

```csharp
private void IncreaseDate()
{
    daysPlayed++;
    day++;
    
    if (day > 7)
    {
        day = 1;
        week++;
        
        if (week > 3)
        {
            week = 0;
            season++;
            
            if (season > 3)
            {
                season = 0;
                year++;
                NewYear();
            }
        }
    }
    
    // Season change visual updates
    if (season == 2) SetAutumn();
    else if (season == 3) SetWinter();
    else if (season == 0) SetSpring();
}
```

## Day/Night Cycle

The game has day ending mechanics but no real-time day/night:

```csharp
// End of day menu
private int endDayMenuCounter;
public GameObject menuEndOfDay;
public GameObject menuNewDay;

// Day ends when player goes to bed
// Or when knocked out (energy depleted)
```

### Energy System
```csharp
public static int dayEXP;        // Current day experience
public static int maxDayEXP = 100;  // Max before player gets tired
```

## Weather System

### Rain
```csharp
public static bool raining;      // Currently raining
private bool wasRaining;         // Was raining (for save)

// Rain control methods
private void RainStart();
private void RainStop();
private IEnumerator RainDrops();
```

### Lighting
```csharp
public GameObject dayLightObj;        // Normal daylight
public GameObject dayLightRainObj;    // Rainy day light
public GameObject nightLightObj;      // Night lighting
public GameObject nightLightRainObj;  // Rainy night light
```

## Calendar Events

Events are stored in a 112-element array (28 days × 4 seasons):

```csharp
public static int[] calendarEvents = new int[112];
public static int Current_Event = 0;

// Calculate day index
int dayIndex = day + week * 7 + season * 28 - 1;
int todaysEvent = calendarEvents[dayIndex];
```

### Event IDs (Partial List)
| ID | Event |
|----|-------|
| 0 | No Event |
| 1 | Fishing Tournament |
| 2 | Bug Catching Contest |
| 3 | Cooking Competition |
| 4 | Market Day |
| 43 | Festival (Generic) |
| 81 | Willow's Arrival |
| 100 | Wedding Day |

### Event Methods
```csharp
private int GetTodaysEvent();           // Get current day's event
private bool CheckForEventToday();      // Check if event today
private string GetEventName(int id);    // Get event display name
private void SetCalendarEvents();       // Initialize calendar

// Adding events to calendar
private void ScheduleEvent(int eventId);
```

## New Day Flow

1. Player goes to bed → `EndOfDayMenu()`
2. Show day summary with exp gained
3. `NewDayMenu()` called → `IncreaseDate()`
4. Display new day/season
5. Check for events with `CheckForEventToday()`
6. `NewDay()` → reset daily flags
7. `SecondPartOfNewDay()` → spawn resources, etc.

### Key Coroutines
```csharp
private IEnumerator NewDayCoroutine()
{
    // Reset daily states
    loveUnlockedToday = false;
    // ... other resets
}

private IEnumerator SecondPartOfNewDay()
{
    GC.Collect();
    // Spawn farm resources
    // Reset NPC states
    // Check weather
}
```

## Helper Methods

```csharp
// Get day name from number
private string GetDayName(int a);

// Get season name from number  
private string GetSeasonName(int a);

// Get day number within season (1-28)
private string GetDayNameFromDayOfSeason(int a);

// Get total day of year (0-111)
private int GetDayOfYear()
{
    return day + week * 7 + season * 28 - 1;
}
```

## Modding the Time System

### Get Current Date
```csharp
int currentDay = GameScript.day;      // 1-7
int currentWeek = GameScript.week;    // 0-3
int currentSeason = GameScript.season; // 0-3
int currentYear = GameScript.year;

int dayOfSeason = GameScript.week * 7 + GameScript.day;  // 1-28
int dayOfYear = GameScript.day + GameScript.week * 7 + GameScript.season * 28 - 1;  // 0-111
```

### Hook Day Change
```csharp
[HarmonyPatch(typeof(GameScript), "IncreaseDate")]
class DatePatch
{
    static void Postfix()
    {
        int day = GameScript.day;
        int season = GameScript.season;
        // React to day change
    }
}
```

### Hook New Day
```csharp
[HarmonyPatch(typeof(GameScript), "NewDay")]
class NewDayPatch
{
    static void Postfix()
    {
        // Day has started
        // Good place to add daily events
    }
}
```

### Hook Weather
```csharp
// Check if raining
bool isRaining = GameScript.raining;

// Hook rain start
[HarmonyPatch(typeof(GameScript), "RainStart")]
class RainPatch
{
    static void Postfix()
    {
        // It started raining
    }
}
```

### Add Custom Events
```csharp
[HarmonyPatch(typeof(GameScript), "SetCalendarEvents")]
class CalendarPatch
{
    static void Postfix()
    {
        // Add custom event on Spring Day 15
        int dayIndex = 14; // Day 15 of Spring (0-indexed)
        GameScript.calendarEvents[dayIndex] = 999; // Custom event ID
    }
}
```

## Season Visual Changes

The game changes town appearance based on season:

```csharp
private void SetSpring();   // Green trees, flowers
private void SetSummer();   // Lush green (default)
private void SetAutumn();   // Orange/red leaves
private void SetWinter();   // Snow, bare trees
```

These are called when:
- Season changes in `IncreaseDate()`
- Player enters town during new season
- Game loads with different season

## Daily Reset Flags

Many flags reset each day:
```csharp
public bool loveUnlockedToday;
public bool acceptedTravelerRewardToday;
public bool confirmedCatacombToday;
public bool gainedUltimateHeartEXPToday;
public bool[] farmAnimalHeart;     // Pet/farm animal interactions
public bool[] townsfolkTalkedTo;   // NPC talked today
```
