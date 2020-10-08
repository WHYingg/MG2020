using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public struct SaveInfo
{
    public Vector3 character1Position, character2Position;
    public float time;
    public List<TriggerState> states;
}

public enum LoadState
{
    Loading,
    Fail,
    Success,
}
public class SaveGame
{
    public IEnumerator Save(string saveName)
    {
        SaveInfo info = new SaveInfo();
        GameScene scene = GameScene.GetInstance();
        info.character1Position = scene.character1.transform.position;
        info.character2Position = scene.character2.transform.position;
        yield return false;
        info.time = scene.Timer.Time;
        yield return false;
        var triggers = scene.triggerManager.GetTriggers();
        foreach(var trigger in triggers)
        {
            info.states.Add(trigger.state);
            yield return false;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/"+saveName +".save");
        bf.Serialize(file, info);
        file.Close();
    }
    public IEnumerator Load(string saveName)
    {
        LoadState state = LoadState.Loading;
        string path = Application.persistentDataPath + saveName + ".save";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            try
            {
                SaveInfo save = (SaveInfo)bf.Deserialize(file);
            }
            catch
            {
                state = LoadState.Fail;
            }
            file.Close();

            SaveInfo info = new SaveInfo();
            GameScene scene = GameScene.GetInstance();
            scene.character1.transform.position = info.character1Position;
            scene.character2.transform.position = info.character2Position;
            yield return state;
            info.time = scene.Timer.Time;
            yield return state;
            var triggers = scene.triggerManager.GetTriggers();
            int i = 0;
            foreach (var trigger in triggers)
            {
                trigger.state = info.states[i];
                i++;
                yield return false;
            }

        }
        else
        {
            state = LoadState.Fail;
        }
    }
}