using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

[System.Serializable]

public class RoomData {
    public string roomId;
    public int maxPlayers;
    public int minPlayers;
    public int maxWait;
    public int rounds;
    public string text;
    public bool allowBots;
    public User user;

    public class User {
        public string name;
        public string photo;
        public string sub;
    }
}
public class StatisticsData {
    //public string session_id;
    //public int score;
    //public float session_duration;
    public string roomId;
    //public string sessionID;

    public MetaData meta_data;
    [System.Serializable] // This attribute is required by Newtonsoft.Json
    public class MetaData {
        public string level_completed;
        public string stars_won;
        public string userId;
    }
}


public class RumbleAPI : MonoBehaviour {

    private const string devURL = "https://api-devnew.rumbleapp.gg/api/v1/developer/updateMatchData";
    private const string stageURL = "https://api-stage.rumbleapp.gg/api/v1/developer/updateMatchData";
    private const string prodURL = "https://api.rumbleapp.gg/api/v1/developer/updateMatchData";


   
    private const string contentTypeHeader = "Content-Type";
    private const string jsonContentType = "application/json";

    public string sessionID;
    public int score;
    public float sessionDuration;
    public string env;

    public float sendupdate = 800;
    public string roomId;
    public string userId;
    public string level_completed;
    public string stars_won;

    public string level_completedint;
    public string stars_wonint;
    public string base64;
    public int holder;
    public int maxholder;

    private string scenename;

    public void UpdateMatchData() {

        // Set the appropriate URL based on the environment
        string url;

        string currentURL = Application.absoluteURL;
        //Testing URL
        //string currentURL = "https://gameserver.rumbleapp.gg/RumbleWebGL4/index.html?roomDetails=eyJyb29tSWQiOiI1MjU0XzEwNDYiLCJtYXhQbGF5ZXJzIjo0LCJtaW5QbGF5ZXJzIjoyLCJtYXhXYWl0IjoxNSwicm91bmRzIjoxLCJ0ZXh0IjoicGxheV9hZ2FpbiIsImFsbG93Qm90cyI6dHJ1ZSwidXNlciI6eyJuYW1lIjoiNzYwWFhYWDc3NCIsInBob3RvIjoiaHR0cHM6Ly9hc3NldHMtZGV2LnJ1bWJsZWFwcC5nZy9hdmF0YXJzX21hc3Rlci9hdmF0YXJfMS5wbmciLCJzdWIiOiIyMTAifX0=&session_id=fd4d0e21-6362-4a39-9e02-6127a1fece24&env=dev&source=cutysvcv167t63t4&gamingEnv=development";


        ///GRAB THE BASE64 OUT URL AND CONVERT IT TO READABLE STUFF////////////

        // Parse the URL
        Uri uri = new Uri(currentURL);

        // Get the query parameters
        string query = uri.Query;

        // Find the starting index of the roomDetails parameter
        int startIndex2 = query.IndexOf("roomDetails=") + 12;

        // Find the ending index of the roomDetails parameter
        int endIndex2 = query.IndexOf('&', startIndex2);

        // If the '&' character is not found, use the length of the query
        if (endIndex2 == -1)
            endIndex2 = query.Length;

        // Extract the roomDetails substring
        string roomDetails = query.Substring(startIndex2, endIndex2 - startIndex2);

        // Decode the roomDetails string if necessary
        roomDetails = query.Substring(startIndex2, endIndex2 - startIndex2);

         //base64
        base64 = roomDetails;
        MyStaticClass.base64levelsunlocked = base64;
        MyStaticClass.base64_2stars = base64 + "2";

        Debug.Log("Base64 " + MyStaticClass.base64levelsunlocked);
        Debug.Log("Base642 " + MyStaticClass.base64_2stars);

        byte[] data = Convert.FromBase64String(roomDetails);
        string decodedString = System.Text.Encoding.UTF8.GetString(data);

        Debug.Log("ROOMID" + decodedString);

        RoomData roomData = JsonConvert.DeserializeObject<RoomData>(decodedString);
        roomId = roomData.roomId;
        userId = roomData.user.sub;

        



        ///////////////////////////////////////////////////////////




        ///GRAB SESSIONID OUT OF THE URL//////////////////////
        // Parse the URL
        Uri uri2 = new Uri(currentURL);

        // Get the query parameters
        string query2 = uri.Query;

        // Find the starting index of the session ID
        int startIndex = query.IndexOf("session_id=") + 11;

        // Find the ending index of the session ID
        int endIndex = query.IndexOf('&', startIndex);

        // If the '&' character is not found, use the length of the query
        if (endIndex == -1)
            endIndex = query.Length;

        // Extract the session ID substring
        sessionID = query.Substring(startIndex, endIndex - startIndex);
        //////////////////////////////////////////////////////////


        string envValue = "";

        if (!string.IsNullOrEmpty(uri.Query)) {
            // Remove the leading '?' character from the query string
            string queryString = uri.Query.Substring(1);

            // Split the query string into key-value pairs
            string[] queryParams = queryString.Split('&');

            // Loop through the key-value pairs and extract the value of the env variable
            foreach (string param in queryParams) {
                string[] keyValue = param.Split('=');
                string key = keyValue[0];
                string value = keyValue[1];

                if (key == "env") {
                    envValue = value;
                    break;
                }
            }
        }

        // Update the env variable in your code with the extracted value
        env = envValue;

        switch (env) {
            case "dev":
                url = devURL;
                break;
            case "stage":
                url = stageURL;
                break;
            case "prod":
                url = prodURL;
                break;
            default:
                Debug.LogError("Invalid environment specified");
                return;
        }

        StartCoroutine(SendStatistics(url));
    }


    private IEnumerator SendStatistics(string url) {

        StatisticsData statisticsData = new StatisticsData();
        // Set other fields in the statisticsData object
        //statisticsData.sessionID = sessionID;
        statisticsData.roomId = roomId;

        statisticsData.meta_data = new StatisticsData.MetaData();
        statisticsData.meta_data.level_completed = level_completed;
        statisticsData.meta_data.stars_won = stars_won;
        statisticsData.meta_data.userId = userId;


        string jsonData = JsonConvert.SerializeObject(statisticsData);

        //Debug.Log(roomId);
        Debug.Log(userId);

        ///Grab roomID from the extracted base64 code above and set as roomID
        //grab userID from the extracted base64 code above and set as roomID
        //Set levels completed as myplayerprefs
        ///Set stars as my playerprefabs




        Debug.Log("URL: " + url);
        Debug.Log("JSON Data: " + jsonData);

        // Create the UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // Set the content type header
        request.SetRequestHeader(contentTypeHeader, jsonContentType);

        // Set the authorization header
        request.SetRequestHeader("Authorization", "bradley123!@#");

        // Attach the JSON payload to the request body
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success) {
            Debug.LogError("Failed to send statistics. Error: " + request.error);
            if (request.downloadHandler != null) {
                byte[] responseBody = request.downloadHandler.data;
                string responseBodyText = System.Text.Encoding.UTF8.GetString(responseBody);
                Debug.Log("Response Body: " + responseBodyText);
            }
        }
        else {
            Debug.Log("Statistics sent successfully");
            Debug.Log("Response Data: " + request.downloadHandler.text);
        }
    }

    public void CallOnce() {

        
            
            

        scenename = SceneManager.GetActiveScene().name;
        int intholder = int.Parse(scenename);

        holder = PlayerPrefs.GetInt(scenename + MyStaticClass.base64_2stars);

        //Debug.Log(PlayerPrefs.GetInt(scenename + MyStaticClass.base64_2stars));


        stars_won = holder.ToString();

        int levelholder = PlayerPrefs.GetInt(MyStaticClass.base64levelsunlocked);
        level_completed = levelholder.ToString();

        //Debug.Log(level_completed);

        holder = 0;
        maxholder = 0;
        ////////////////////////////////
        ///
        sessionDuration = Mathf.RoundToInt(sessionDuration);
        UpdateMatchData();
        

    }


    private void Update() {

        //Debug.Log(sessionID);

        

        sendupdate++;

        sessionDuration += Time.deltaTime;

        ///Set the levels to a base64 unlock
        //int levelholder = PlayerPrefs.GetInt(MyStaticClass.base64levelsunlocked);
        //level_completed = levelholder.ToString();


        /*if (sendupdate > 900) {

            ///Update Stars Once
            //for (int i = 1; i < 41; i++) {
                holder = PlayerPrefs.GetInt(i + MyStaticClass.base64_2stars);
                //maxholder += holder;
            //}

            //Debug.Log(holder);
            stars_won = maxholder.ToString();

            holder = 0;
            maxholder = 0;
            ////////////////////////////////
            ///
            sessionDuration = Mathf.RoundToInt(sessionDuration);
            UpdateMatchData();
            sendupdate = 0;
            //Debug.Log(sessionDuration);
        }*/
    }
}
