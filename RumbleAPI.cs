using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System;

[System.Serializable]
public class StatisticsData {
    public string session_id;
    public int score;
    public float session_duration;
}

public class RumbleAPI : MonoBehaviour {

    private const string devURL = "https://api-devnew.rumbleapp.gg/api/v1/game/updateStatistics";
    private const string stageURL = "https://api-stage.rumbleapp.gg/api/v1/game/updateStatistics";
    private const string prodURL = "https://api.rumbleapp.gg/api/v1/game/updateStatistics";

    private const string contentTypeHeader = "Content-Type";
    private const string jsonContentType = "application/json";

    public string sessionID;
    public int score;
    public float sessionDuration;
    public string env;

    public float sendupdate = 800;


    public void ReportStatistics() {


        // Set the appropriate URL based on the environment
        string url;

        string currentURL = Application.absoluteURL;

        //Testing URL
        //string currentURL = "https://gameserver.rumbleapp.gg/RumbleWebGLv2/index.html?session_id=12c7e0b8-e38e-4f72-9a1c-6e869d630046&env=dev&source=appvbh756246h";

        // Parse the URL
        Uri uri = new Uri(currentURL);

        // Get the query parameters
        string query = uri.Query;

        // Find the starting index of the session ID
        int startIndex = query.IndexOf("session_id=") + 11;

        // Find the ending index of the session ID
        int endIndex = query.IndexOf('&', startIndex);

        // If the '&' character is not found, use the length of the query
        if (endIndex == -1)
            endIndex = query.Length;

        // Extract the session ID substring
        sessionID = query.Substring(startIndex, endIndex - startIndex);



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

        // Create the statistics data object
        StatisticsData statisticsData = new StatisticsData();
        statisticsData.session_id = sessionID;
        statisticsData.score = score;
        statisticsData.session_duration = sessionDuration;

        // Serialize the statistics data to JSON
        string jsonData = JsonUtility.ToJson(statisticsData);

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


    private void Update() {

        Debug.Log(sessionID);

        sendupdate++;

        sessionDuration += Time.deltaTime;

        score = PlayerPrefs.GetInt("Levelsunlocked");

        if (sendupdate > 900) {
            sessionDuration = Mathf.RoundToInt(sessionDuration);
            ReportStatistics();
            sendupdate = 0;
            Debug.Log(sessionDuration);
        }
    }
}
