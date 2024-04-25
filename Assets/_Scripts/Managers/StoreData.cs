﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct VehicleRuntimeData {
    public string vehicleName;
    public int TotalWaitTime;
    public float TotalDistanceTraveled;
    public int TotalTimeTaken;
}

public class StoreData : SingletonPersistent<MonoBehaviour> {

    [SerializeField] private GameSettingsScriptableObject gameSettings;
    private static readonly string PATH = Application.streamingAssetsPath + "/Runtime_Data/";
    private static string fileNameBase = "";
    private static readonly List<string> fileNamesWritten = new();
    private static float bufferTime;
    private void Start() {
        bufferTime = gameSettings.bufferTime;
    }

    //[MenuItem("Tools/Write file")]
    public static void WriteString(string dataToWrite, string fileName = null) {
        if (Time.time < bufferTime) {
            return;
        }

        if (string.IsNullOrEmpty(fileName)) {
            fileName = "Unknown";
        }

        if (fileNamesWritten.FindIndex(v => fileName == v) == -1) {
            if (fileName == "vehicle")
                dataToWrite = $"vehicleName,TotalWaitTime,TotalDistanceTraveled,TotalTimeTaken\n{dataToWrite}";
            if (fileName == "intersection_wait_time")
                dataToWrite = $"intersectionName,vehicleName,waitTime";
            fileNamesWritten.Add(fileName);
        }

        //Write some text to the file.txt file
        string path = $"{PATH}{fileNameBase}_{fileName}.csv";
        StreamWriter writer = new(path, append: true);

        writer.WriteLine(dataToWrite);
        writer.Flush();
        writer.Close();

        ////Re-import the file to update the reference in the editor
        //AssetDatabase.ImportAsset(path);
        //TextAsset asset = (TextAsset)Resources.Load("test");
        ////Print the text from the file
        //Debug.Log(asset.text);

    }

    //[MenuItem("Tools/Read file")]

    public static void ReadString() {
        //Read the text from directly from the test.txt file
        StreamReader reader = new(PATH);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }



    public static void WriteVehicleRuntimeData(VehicleRuntimeData data) {
        string dataToWrite = $"{data.vehicleName},{data.TotalWaitTime},{Math.Round(data.TotalDistanceTraveled, 2)},{data.TotalTimeTaken}";
        WriteString(dataToWrite, "vehicle");
    }

    public static void WriteIntesectionWaitTime(string intersectionName, string vehicleName, int waitTime) {
        string dataToWrite = $"{intersectionName},{vehicleName},{waitTime}";
        WriteString(dataToWrite, "intersection_wait_time");
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize() {
        fileNameBase = DateTime.Now.ToString("ddMMyyyy_HH-mm-ss");
    }
}




