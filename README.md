# Rumble API

Used in Unity, a class used in games to grab the URL that your Unity game is embedded into. Then we manually query the string by pulling out our 3 variables and then send the code accordingly to the URL as a json.


# Env

- Dev
- Stage
- Prod

# Statistics

- Session_ID
- Score
- Time

# Customize and Use

Drag the RumbleAPI.cs class onto a prefab on screen, preferably one that has a DontDestroy class on it so it persists between scene changes.

Change the levelsunlocked variable to one you have saved with playerprefs, or alternatively, delete this line and replace score with your score variable in game.

```score = PlayerPrefs.GetInt("Levelsunlocked");```

Replace 900 with a time you want to send your unityWebRequest

```if (sendupdate > 900) {```

Replace 800 with a start timer int before it hits 900 to send first data

``` public float sendupdate = 800;```

# Updates

- Added support for nested variables and nested json (Needs Newtonsoft.Json or it won't support nested json in Unity. Amazing bug there that was PAINFUL HOURS TO FIX!

- Decodes base64 and grabs out variables

- Send our decoded base64 into a static variable to be used for playerprefabs saving by Unity that will adapt to rooms
