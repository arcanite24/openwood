using System;
using UnityEngine;

namespace OpenWood.Core.API
{
    /// <summary>
    /// API for controlling and querying time-related game state.
    /// Provides methods for managing days, seasons, years, and weather.
    /// </summary>
    /// <example>
    /// <code>
    /// // Advance to the next day
    /// TimeAPI.AdvanceDay();
    /// 
    /// // Set the season to summer
    /// TimeAPI.Season = Season.Summer;
    /// 
    /// // Toggle rain
    /// TimeAPI.ToggleRain();
    /// </code>
    /// </example>
    public static class TimeAPI
    {
        #region Enums

        /// <summary>
        /// Represents the four seasons in Littlewood.
        /// </summary>
        public enum Season
        {
            /// <summary>Spring (Season 0)</summary>
            Spring = 0,
            /// <summary>Summer (Season 1)</summary>
            Summer = 1,
            /// <summary>Autumn (Season 2)</summary>
            Autumn = 2,
            /// <summary>Winter (Season 3)</summary>
            Winter = 3
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current day of the week (1-7).
        /// </summary>
        public static int Day
        {
            get => GameScript.day;
            set => GameScript.day = Mathf.Clamp(value, 1, 7);
        }

        /// <summary>
        /// Gets or sets the current week of the season (0-3).
        /// </summary>
        public static int Week
        {
            get => GameScript.week;
            set => GameScript.week = Mathf.Clamp(value, 0, 3);
        }

        /// <summary>
        /// Gets or sets the current season as an integer (0-3).
        /// Use <see cref="CurrentSeason"/> for a typed enum version.
        /// </summary>
        public static int SeasonIndex
        {
            get => GameScript.season;
            set => GameScript.season = Mathf.Clamp(value, 0, 3);
        }

        /// <summary>
        /// Gets or sets the current season as a typed enum.
        /// </summary>
        public static Season CurrentSeason
        {
            get => (Season)GameScript.season;
            set => GameScript.season = (int)value;
        }

        /// <summary>
        /// Gets or sets the current year.
        /// </summary>
        public static int Year
        {
            get => GameScript.year;
            set => GameScript.year = Mathf.Max(1, value);
        }

        /// <summary>
        /// Gets the total number of days played in this save.
        /// </summary>
        public static int DaysPlayed => GameScript.daysPlayed;

        /// <summary>
        /// Gets or sets whether it is currently raining.
        /// </summary>
        public static bool IsRaining
        {
            get => GameScript.raining;
            set => GameScript.raining = value;
        }

        /// <summary>
        /// Gets the name of the current season as a string.
        /// </summary>
        public static string SeasonName => GetSeasonName(CurrentSeason);

        #endregion

        #region Public Methods

        /// <summary>
        /// Advances the game by one day, updating week, season, and year as needed.
        /// </summary>
        public static void AdvanceDay()
        {
            GameScript.daysPlayed++;
            GameScript.day++;
            
            if (GameScript.day > 7)
            {
                GameScript.day = 1;
                GameScript.week++;
                
                if (GameScript.week > 3)
                {
                    GameScript.week = 0;
                    GameScript.season++;
                    
                    if (GameScript.season > 3)
                    {
                        GameScript.season = 0;
                        GameScript.year++;
                    }
                }
            }
            
            Plugin.Log.LogDebug($"Advanced to Day {GameScript.day}, {GetSeasonName((Season)GameScript.season)}, Year {GameScript.year}");
        }

        /// <summary>
        /// Advances the game by the specified number of days.
        /// </summary>
        /// <param name="days">Number of days to advance.</param>
        public static void AdvanceDays(int days)
        {
            for (int i = 0; i < days; i++)
            {
                AdvanceDay();
            }
        }

        /// <summary>
        /// Advances the game by one week (7 days).
        /// </summary>
        public static void AdvanceWeek()
        {
            AdvanceDays(7);
        }

        /// <summary>
        /// Advances the game by one season (28 days).
        /// </summary>
        public static void AdvanceSeason()
        {
            AdvanceDays(28);
        }

        /// <summary>
        /// Toggles the rain on or off.
        /// </summary>
        public static void ToggleRain()
        {
            GameScript.raining = !GameScript.raining;
            Plugin.Log.LogDebug($"Rain toggled: {GameScript.raining}");
        }

        /// <summary>
        /// Sets the complete date at once.
        /// </summary>
        /// <param name="day">Day of the week (1-7).</param>
        /// <param name="season">Season (0-3 or use Season enum).</param>
        /// <param name="year">Year number.</param>
        public static void SetDate(int day, Season season, int year)
        {
            Day = day;
            CurrentSeason = season;
            Year = year;
            Plugin.Log.LogDebug($"Set date to Day {day}, {GetSeasonName(season)}, Year {year}");
        }

        /// <summary>
        /// Sets the complete date at once using integer values.
        /// </summary>
        /// <param name="day">Day of the week (1-7).</param>
        /// <param name="season">Season index (0-3).</param>
        /// <param name="year">Year number.</param>
        public static void SetDate(int day, int season, int year)
        {
            SetDate(day, (Season)Mathf.Clamp(season, 0, 3), year);
        }

        /// <summary>
        /// Gets the name of a season.
        /// </summary>
        /// <param name="season">The season to get the name for.</param>
        /// <returns>The season name as a string.</returns>
        public static string GetSeasonName(Season season)
        {
            switch (season)
            {
                case Season.Spring: return "Spring";
                case Season.Summer: return "Summer";
                case Season.Autumn: return "Autumn";
                case Season.Winter: return "Winter";
                default: return "Unknown";
            }
        }

        /// <summary>
        /// Gets the name of a season by index.
        /// </summary>
        /// <param name="seasonIndex">The season index (0-3).</param>
        /// <returns>The season name as a string.</returns>
        public static string GetSeasonName(int seasonIndex)
        {
            return GetSeasonName((Season)seasonIndex);
        }

        /// <summary>
        /// Gets a formatted date string.
        /// </summary>
        /// <returns>A string in the format "Day X, Season, Year Y".</returns>
        public static string GetFormattedDate()
        {
            return $"Day {Day}, {SeasonName}, Year {Year}";
        }

        /// <summary>
        /// Gets a short date string.
        /// </summary>
        /// <returns>A string in the format "D/S/Y" (e.g., "3/1/2").</returns>
        public static string GetShortDate()
        {
            return $"{Day}/{SeasonIndex + 1}/{Year}";
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Initializes the Time API. Called automatically by the plugin.
        /// </summary>
        internal static void Initialize()
        {
            Plugin.Log.LogDebug("TimeAPI initialized");
        }

        #endregion
    }
}
