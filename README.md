# Rumble API

Used in Unity to drop and drag in games and grab the URL that your Unity game is embedded into. Then we manually query the string by pulling out our 3 variables and then send the code accordingly to the URL as a json.


# Env

- Dev
- Stage
- Prod

# Statistics

- Session_ID
- Score
- Time

# Customize and Use

Change the levelsunlocked variable to one you have saved with playerprefs, or alternatively, delete this line and replace score with your score variable in game.

```score = PlayerPrefs.GetInt("Levelsunlocked");```

Replace 900 with a time you want to send your unityWebRequest

```if (sendupdate > 900) {```

Replace 800 with a start timer int before it hits 900 to send first data

``` public float sendupdate = 800;```

