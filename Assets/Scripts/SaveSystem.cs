using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{

    public static void SaveData (DataCollector dc)
    {
      BinaryFormatter formatter = new BinaryFormatter();
      string path = Application.persistentDataPath + "/data.pog";
      FileStream stream = new FileStream(path, FileMode.Create);

      GameData gameData = new GameData(dc);

      formatter.Serialize(stream, gameData);
      stream.Close();
    }

    public static GameData LoadData ()
    {
      string path = Application.persistentDataPath + "/data.pog";
      if (File.Exists(path))
      {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        GameData gameData = formatter.Deserialize(stream) as GameData;
        stream.Close();

        return gameData;
      }
      else
      {
        Debug.LogWarning("Save file not found in " + path + ". Save data there before loading");
        return null;
      }
    }

    public static void DeleteData()
    {
        string path = Application.persistentDataPath + "/data.pog";
        File.Delete(path);
    }

}
