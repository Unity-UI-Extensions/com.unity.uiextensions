using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace UnityEngine.UI.Extensions
{
    public static class SaveLoad
    {

        //You may define any path you like, such as "c:/Saved Games"
        //remember to use slashes instead of backslashes! ("/" instead of "\")
        //Application.DataPath: http://docs.unity3d.com/ScriptReference/Application-dataPath.html
        //Application.persistentDataPath: http://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
        public static string saveGamePath = Application.persistentDataPath + "/Saved Games/";

        public static void Save(SaveGame saveGame)
        {

            BinaryFormatter bf = new BinaryFormatter();

            // 1. Construct a SurrogateSelector object
            SurrogateSelector ss = new SurrogateSelector();
            // 2. Add the ISerializationSurrogates to our new SurrogateSelector
            AddSurrogates(ref ss);
            // 3. Have the formatter use our surrogate selector
            bf.SurrogateSelector = ss;

            //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
            //You can also use any path you like
            CheckPath(saveGamePath);

            FileStream file = File.Create(saveGamePath + saveGame.savegameName + ".sav"); //you can call it anything you want including the file extension
            bf.Serialize(file, saveGame);
            file.Close();
            Debug.Log("Saved Game: " + saveGame.savegameName);

        }

        public static SaveGame Load(string gameToLoad)
        {
            if (File.Exists(saveGamePath + gameToLoad + ".sav"))
            {

                BinaryFormatter bf = new BinaryFormatter();
                // 1. Construct a SurrogateSelector object
                SurrogateSelector ss = new SurrogateSelector();
                // 2. Add the ISerializationSurrogates to our new SurrogateSelector
                AddSurrogates(ref ss);
                // 3. Have the formatter use our surrogate selector
                bf.SurrogateSelector = ss;

                FileStream file = File.Open(saveGamePath + gameToLoad + ".sav", FileMode.Open);
                SaveGame loadedGame = (SaveGame)bf.Deserialize(file);
                file.Close();
                Debug.Log("Loaded Game: " + loadedGame.savegameName);
                return loadedGame;
            }
            else
            {
                Debug.Log(gameToLoad + " does not exist!");
                return null;
            }
        }

        private static void AddSurrogates(ref SurrogateSelector ss)
        {
            Vector2Surrogate Vector2_SS = new Vector2Surrogate();
            ss.AddSurrogate(typeof(Vector2),
                            new StreamingContext(StreamingContextStates.All),
                            Vector2_SS);

            Vector3Surrogate Vector3_SS = new Vector3Surrogate();
            ss.AddSurrogate(typeof(Vector3),
                            new StreamingContext(StreamingContextStates.All),
                            Vector3_SS);

            Vector4Surrogate Vector4_SS = new Vector4Surrogate();
            ss.AddSurrogate(typeof(Vector4),
                            new StreamingContext(StreamingContextStates.All),
                            Vector4_SS);

            ColorSurrogate Color_SS = new ColorSurrogate();
            ss.AddSurrogate(typeof(Color),
                            new StreamingContext(StreamingContextStates.All),
                            Color_SS);

            QuaternionSurrogate Quaternion_SS = new QuaternionSurrogate();
            ss.AddSurrogate(typeof(Quaternion),
                            new StreamingContext(StreamingContextStates.All),
                            Quaternion_SS);

            //Reserved for future implementation
            //Texture2DSurrogate Texture2D_SS = new Texture2DSurrogate();
            //ss.AddSurrogate(typeof(Texture2D),
            //                new StreamingContext(StreamingContextStates.All),
            //                Texture2D_SS);
            //GameObjectSurrogate GameObject_SS = new GameObjectSurrogate();
            //ss.AddSurrogate(typeof(GameObject),
            //                new StreamingContext(StreamingContextStates.All),
            //                GameObject_SS);
            //TransformSurrogate Transform_SS = new TransformSurrogate();
            //ss.AddSurrogate(typeof(Transform),
            //                new StreamingContext(StreamingContextStates.All),
            //                Transform_SS);
        }

        private static void CheckPath(string path)
        {
            try
            {
                // Determine whether the directory exists. 
                if (Directory.Exists(path))
                {
                    //Debug.Log("That path exists already.");
                    return;
                }

                // Try to create the directory.
                //DirectoryInfo dir = Directory.CreateDirectory(path);
                Directory.CreateDirectory(path);
                Debug.Log("The directory was created successfully at " + path);

            }
            catch (Exception e)
            {
                Debug.Log("The process failed: " + e.ToString());
            }
            finally { }
        }
    }
}