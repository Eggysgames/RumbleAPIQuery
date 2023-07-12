using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System;

[System.Serializable]
public class StatisticsData {
    public string roomId;
    public class MetaData {
        public int level_completed;
        public int stars_won;
        public int userId;
    }
    public MetaData meta_data;
}

public class RumbleAPI : MonoBehaviour {
    private const string updateMatchDataURL = "https://api-stage.rumbleapp.gg/api/v1/game/developer/updateMatchData";
    private const string contentTypeHeader = "Content-Type";
    private const string jsonContentType = "application/json";

    private int sendupdate = 0;
    private float sessionDuration = 0f;
    private int score = 0;

    public void ReportStatistics() {
        // Example URL
        string currentURL = "https://gameserver.rumbleapp.gg/RumbleWebGL4/index.html?roomDetails=eyJyb29tSWQiOiI1MjU0XzEwNDYiLCJtYXhQbGF5ZXJzIjo0LCJtaW5QbGF5ZXJzIjoyLCJtYXhXYWl0IjoxNSwicm91bmRzIjoxLCJ0ZXh0IjoicGxheV9hZ2FpbiIsImFsbG93Qm90cyI6dHJ1ZSwidXNlciI6eyJuYW1lIjoiNzYwWFhYWDc3NCIsInBob3RvIjoiaHR0cHM6Ly9hc3NldHMtZGV2LnJ1bWJsZWFwcC5nZy9hdmF0YXJzX21hc3Rlci9hdmF0YXJfMS5wbmciLCJzdWIiOiIyMTAifX0=&session_id=fd4d0e21-6362-4a39-9e02-6127a1fece24&env=dev&source=cutysvcv167t63t4&gamingEnv=development";

        // Extract the room details from the URL
        Uri uri = new Uri(currentURL);
        string encodedRoomDetails = uri.Query.Substring(uri.Query.IndexOf('=') + 1);
        byte[] roomDetailsBytes = Convert.FromBase64String(encodedRoomDetails);
        string decodedRoomDetails = System.Text.Encoding.UTF8.GetString(roomDetailsBytes);

        // Parse the room details JSON
        StatisticsData statisticsData = JsonUtility.FromJson<StatisticsData>(decodedRoomDetails);

        // Set the room ID and user ID
        string roomId = statisticsData.roomId;
        int userId = statisticsData.meta_data.userId;

        // Update the meta data
        statisticsData.meta_data.level_completed = 2;
        statisticsData.meta_data.stars_won = 1;

        // Serialize the updated statistics data to JSON
        string jsonData = JsonUtility.ToJson(statisticsData);

        StartCoroutine(SendMatchData(roomId, userId, jsonData));
    }

    private IEnumerator SendMatchData(string roomId, int userId, string jsonData) {
        // Create the request payload
        string requestPayload = $"{{ \"meta_data\": {jsonData}, \"room_id\": \"{roomId}\" }}";

        // Create the UnityWebRequest
        UnityWebRequest request = UnityWebRequest.Post(updateMatchDataURL, requestPayload);

        // Set the content type header
        request.SetRequestHeader(contentTypeHeader, jsonContentType);

        // Set the authorization header
        request.SetRequestHeader("Authorization", "bradley123!@#");

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success) {
            Debug.LogError("Failed to send match data. Error: " + request.error);
            if (request.downloadHandler != null) {
                byte[] responseBody = request.downloadHandler.data;
                string responseBodyText = System.Text.Encoding.UTF8.GetString(responseBody);
                Debug.Log("Response Body: " + responseBodyText);
            }
        }
        else {
            Debug.Log("Match data sent successfully");
            Debug.Log("Response Data: " + request.downloadHandler.text);
        }
    }

    private void Update() {



        sendupdate++;
        sessionDuration += Time.deltaTime;
        score = PlayerPrefs.GetInt("Levelsunlocked");

        if (sendupdate > 900) {
            sessionDuration = Mathf.RoundToInt(sessionDuration);
            ReportStatistics();
            sendupdate = 0;
            //Debug.Log(sessionDuration);
        }
    }
}
