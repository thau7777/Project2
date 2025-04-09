using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System.IO;


public class DataPersistenceManager : Singleton<DataPersistenceManager>
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private void Start()
    {
        InitializeDataHandler();
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    private void InitializeDataHandler()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }

    public void UpdateFileName(string newFileName)
    {
        fileName = newFileName;
        InitializeDataHandler();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }
        
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(gameData);
        }    
    }   
    
    public void SaveGame() 
    {
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
        //CaptureScreenshot();
    }

    public void RemoveData()
    {
        dataHandler.DeleteData();

        string screenshotPath = Path.Combine(Application.persistentDataPath, fileName + "_screenshot.png");
        if (File.Exists(screenshotPath))
        {
            File.Delete(screenshotPath);
        }

        NewGame();
        Debug.Log("Data and screenshot removed.");
    }

    public void OnApplicationQuit()
    {
        SaveGame();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    //private void CaptureScreenshot()
    //{
    //    string screenshotPath = Path.Combine(Application.persistentDataPath, fileName + "_screenshot.png");

    //    // Tạo RenderTexture với độ phân giải của màn hình
    //    RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
    //    Camera.main.targetTexture = renderTexture;
    //    Camera.main.Render();

    //    // Chuyển RenderTexture thành Texture2D
    //    RenderTexture.active = renderTexture;
    //    Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    //    screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
    //    screenshot.Apply();

    //    // Gỡ RenderTexture và giải phóng tài nguyên
    //    Camera.main.targetTexture = null;
    //    RenderTexture.active = null;
    //    Destroy(renderTexture);

    //    // Lưu ảnh dưới dạng PNG
    //    byte[] bytes = screenshot.EncodeToPNG();
    //    File.WriteAllBytes(screenshotPath, bytes);
    //}
}
