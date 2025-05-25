using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Data;
using System.Collections.Generic;
using System;



public struct CombineDatas
{
    public List<PlayData> playData;
    public List<LevelData> levelData;
    public PlayerData playerData;
    public List<LevelData> employeeData;
}

public class SaveLoadManager
{
    public enum SaveState
    {
        ONLY_SAVE_UPGRADE,
        ONLY_SAVE_PLAYERDATA,
        ONLY_SAVE_EMPLOYEEDATA,
        ALL_SAVES
    }

    public bool isLoading;

    private static string saveFilePath = Application.persistentDataPath;// + "/save/savefile.json";

    public static void SaveGame(bool saveCheck = true, bool levelCheck = true)
    {   
        List<PlayData> playDataList = new List<PlayData>();
        List<LevelData> levelDataList = new List<LevelData>();
     
     //   GameInstance instance = new GameInstance();

        for (int i = 0; i < GameInstance.GameIns.restaurantManager.levels.Length; i++)
        {
            if (GameInstance.GameIns.restaurantManager.levels[i].TryGetComponent(out FoodMachine machine))
            {
                PlayData playData = new PlayData(i + 1, 2, i <= GameInstance.GameIns.restaurantManager.level - 1);
                playDataList.Add(playData);

                int level = machine.level > 0 ? machine.level : 1; 
                LevelData levelData = new LevelData(i + 1, level,0);
                levelDataList.Add(levelData);
                // json += JsonConvert.SerializeObject(playData);
            }
            else if (GameInstance.GameIns.restaurantManager.levels[i].TryGetComponent(out Table table))
            {
                PlayData playData = new PlayData(i + 1, 1, i <= GameInstance.GameIns.restaurantManager.level - 1);
                playDataList.Add(playData);
                // json += JsonConvert.SerializeObject(playData);
            }
            else if (GameInstance.GameIns.restaurantManager.levels[i].TryGetComponent(out Counter counter))
            {
                PlayData playData = new PlayData(i + 1, 3, i <= GameInstance.GameIns.restaurantManager.level - 1);
                playDataList.Add(playData);
                //   json += JsonConvert.SerializeObject(playData);
            }
            else
            {
                PlayData playData = new PlayData(i + 1, 0, i <= GameInstance.GameIns.restaurantManager.level - 1);
                playDataList.Add(playData);
            }
        }

     

        if (saveCheck)
        {
            var json = JsonConvert.SerializeObject(playDataList);

            string path = Path.Combine(saveFilePath, "save/savefile.json");

            File.WriteAllText(path, json);
        }

        if (levelCheck)
        {
            var json_level = JsonConvert.SerializeObject(levelDataList);

            string path_level = Path.Combine(saveFilePath, "save/savelevel.json");

            File.WriteAllText(path_level, json_level);
        }
    }

    public static void PlayerStateSave(bool isNotlood = false)
    {
        string path_state = Path.Combine(saveFilePath, "save/savestate.dat");
        string key = "AAAAAA";
        //  string path_state = Path.Combine(saveFilePath, "save/savestate.json");
        if (isNotlood)
        {
            PlayerData playerData = new PlayerData(1, 100, 0, 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 0);
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(playerData.level);
                    writer.Write(playerData.money);
                    writer.Write(playerData.fishesNum);
                    writer.Write(playerData.employeeNum);
                    writer.Write(playerData.time);
                    writer.Write(playerData.fishesNum_InBox);
                }
                EncryptData.SaveBinaryFile(ms.ToArray(), key, path_state);
              
            }

            // Json 방식 + AES 암호화
            /*PlayerData playerData = new PlayerData(1, 100, 0, 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 0);
            GameInstance.GameIns.restaurantManager.playerData = playerData;
            string json_state = JsonConvert.SerializeObject(playerData);
            File.WriteAllText(path_state, json_state);
             
            string hash = EncryptData.AddHashToJson(path_state);
            // string hash = EncryptData.ComputeHash(json_state);
            string cc = "AAAAAA";
            string encrypted = EncryptData.Encrypt(File.ReadAllText(path_state), cc);
            string path_encrypted = Path.Combine(saveFilePath, "save/savestate.json");
            File.WriteAllText(path_state, encrypted);*/

        }
        else
        {
         /*   PlayerData playerData = GameInstance.GameIns.restaurantManager.playerData;
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(playerData.level);
                    writer.Write(playerData.money);
                    writer.Write(playerData.fishesNum);
                    writer.Write(playerData.employeeNum);
                    writer.Write(playerData.time);
                    writer.Write(playerData.fishesNum_InBox);
                }
                EncryptData.SaveBinaryData(ms.ToArray(), key, path_state);

            }*/


            /*  if(EncryptData.VerifyFileIntegrity(path_state, key))
              {
                  using (MemoryStream ms = new MemoryStream(EncryptData.Decrypt(File.ReadAllBytes(path_state), key)))
                  {
                      using (BinaryReader reader = new BinaryReader(ms))
                      {


                      }
                  }


              }
              else Debug.Log("fail");*/

            //json + AES
            /* string cc = "AAAAAA";
             string pathData = File.ReadAllText(path_state);
             byte[] checkSize = Convert.FromBase64String(pathData);

             if (checkSize.Length % 16 == 0)
             {
                 string decrypt = EncryptData.Decrypt(pathData, cc);


                 if (EncryptData.VerifyJsonHash(decrypt))
                 {
                     GameInstance.GameIns.restaurantManager.playerData.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                     string json_state = JsonConvert.SerializeObject(GameInstance.GameIns.restaurantManager.playerData);

                     File.WriteAllText(path_state, json_state);

                     EncryptData.AddHashToJson(path_state);

                     string encrypted = EncryptData.Encrypt(File.ReadAllText(path_state), cc);
                     string path_encrypted = Path.Combine(saveFilePath, "save/savestate.json");
                     File.WriteAllText(path_state, encrypted);
                 }
             }*/


        }
    }

    public static void Save(SaveState state)
    {
      /*  switch (state)
        {
            case SaveState.ONLY_SAVE_UPGRADE:
                SaveGame();
                break;
            case SaveState.ONLY_SAVE_PLAYERDATA:
                PlayerStateSave();
                break;
            case SaveState.ONLY_SAVE_EMPLOYEEDATA:
                EmployeeLevelSave();
                break;
            case SaveState.ALL_SAVES:
                SaveGame();
                PlayerStateSave();
                EmployeeLevelSave();
                break;
        }*/
    }

    public static PlayerData PlayerStateLoad()
    {
        string path_state = Path.Combine(saveFilePath, "save/savestate.dat");
        string key = "AAAAAA";
        //W     string levelInfo = File.ReadAllText(path_state);

        /*    if (EncryptData.VerifyFileIntegrity(path_state, key))
            {
                Debug.Log("checked");
            }
            else Debug.Log("fail");*/

        PlayerData playerData =  EncryptData.LoadBinaryData(path_state, key);


      //  PlayerData playerData = new PlayerData(1, 100, 0, 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 0);
        return playerData;
        /*    string cc = "AAAAAA";
            string pathData = File.ReadAllText(path_state);
            byte[] checkSize = Convert.FromBase64String(pathData);

            if (checkSize.Length % 16 == 0)
            {
                string decrypt = EncryptData.Decrypt(pathData, cc);
                //File.WriteAllText(path_state, decrypt);
                // Debug.Log(decrypt);
                if (EncryptData.VerifyJsonHash(decrypt))
                {
                    Debug.Log("데이터를 불러옴");
                    //PlayerData playerData = new PlayerData(1, 100, 0, 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 0);
                    //  return playerData;
                    PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(decrypt);
                    playerData.fishesNum += playerData.fishesNum_InBox;
                    playerData.fishesNum_InBox = 0;
                    return playerData;
                }
                else
                {
                    Debug.Log("잘못된 데이터");
                    PlayerData playerData = new PlayerData(1, 100, 0, 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 0);
                    return playerData;
                }
            }
            else
            {

                Debug.Log("잘못된 데이터");
                PlayerData playerData = new PlayerData(1, 100, 0, 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 0);
                return playerData;
            }*/
    }
    public static void EmployeeLevelSave(bool exist = true)
    {
        List<LevelData> levelData = new List<LevelData>();

        if (exist)
        {
            for (int i = 0; i < 8; i++)
            {
                if(i >= GameInstance.GameIns.animalManager.employeeControllers.Count)
                {
                    levelData.Add(new LevelData(i + 1, 1,0));
                }
                else levelData.Add(new LevelData(GameInstance.GameIns.animalManager.employeeControllers[i].id, GameInstance.GameIns.animalManager.employeeControllers[i].EmployeeData.level, GameInstance.GameIns.animalManager.employeeControllers[i].EXP));
            }
       
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                levelData.Add(new LevelData(i + 1, 1,0));
            }
        }
        string path_state = Path.Combine(saveFilePath, "save/employeelevel.json");

        string json = JsonConvert.SerializeObject(levelData);

        File.WriteAllText(path_state, json);
    }
    public static List<LevelData> EmployeeLevelLoad()
    {
        string path_state = Path.Combine(saveFilePath, "save/employeelevel.json");

        string levelInfo = File.ReadAllText(path_state);

        List<LevelData> employeeLevelData = JsonConvert.DeserializeObject<List<LevelData>>(levelInfo);

        return employeeLevelData;
    }

    public static CombineDatas LoadGame()
    {
        List<PlayData> data = new List<PlayData>();
        List<LevelData> level = new List<LevelData>();
        List<LevelData> employee = new List<LevelData>();

        string directoryPath = Path.Combine(saveFilePath, "save");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);        
        }
        //    saveFilePath = Path.Combine(Application.persistentDataPath, "save/savefile.json");
        string savePath = Path.Combine(saveFilePath, "save/savefile.json");
        string saveLevelPath = Path.Combine(saveFilePath, "save/savelevel.json");
        string employeeLevelPath = Path.Combine(saveFilePath, "save/employeelevel.json");

        bool dosentSavePathExist = !File.Exists(savePath);
        bool dosentLevelPathExist = !File.Exists(saveLevelPath);
    
        if (dosentSavePathExist || dosentLevelPathExist) SaveGame(dosentSavePathExist, dosentLevelPathExist);

        string gameInfo = File.ReadAllText(savePath);

        data = JsonConvert.DeserializeObject<List<PlayData>>(gameInfo);

        string levelInfo = File.ReadAllText(saveLevelPath);

        level = JsonConvert.DeserializeObject<List<LevelData>>(levelInfo);

        bool saveEmployeeLevelPathExist = File.Exists(employeeLevelPath);
        if(saveEmployeeLevelPathExist)
        {
            employee = EmployeeLevelLoad();
           
        }
        else
        {
            EmployeeLevelSave(saveEmployeeLevelPathExist);
            employee = EmployeeLevelLoad();
        }
      
       

        PlayerData returnPD = new PlayerData(0,0,0,0, null, 0);
     //   string saveStatePath = Path.Combine(saveFilePath, "save/savestate.json");
        string saveStatePath = Path.Combine(saveFilePath, "save/savestate.dat");
        bool saveStatePathExist = File.Exists(saveStatePath);
        if (saveStatePathExist)
        {
         /*   PlayerData pd = PlayerStateLoad();

            GameInstance.GameIns.restaurantManager.playerData = pd; 
            Save(SaveState.ONLY_SAVE_PLAYERDATA);

            returnPD = GameInstance.GameIns.restaurantManager.playerData;*/
        }
        else
        {
            PlayerStateSave(true);
            //Save(SaveState.ONLY_SAVE_PLAYERDATA);

            PlayerData pd = PlayerStateLoad();       
            
         //   returnPD = GameInstance.GameIns.restaurantManager.playerData;
        }



        CombineDatas combineDatas = new CombineDatas();
        combineDatas.playData = data;
        combineDatas.levelData = level;
        combineDatas.playerData = returnPD;
        combineDatas.employeeData = employee;
        return combineDatas;



    }    

}
