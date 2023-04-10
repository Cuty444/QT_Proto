using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace QT.Core
{
    public interface IGameData
    {
        public int Index { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class GameDataBaseAttribute : Attribute
    {
        public Type GameDataType { get; private set; }
        public string JsonFileName { get; private set; }

        public GameDataBaseAttribute(Type gameDataType, string fileName)
        {
            this.GameDataType = gameDataType;
            JsonFileName = fileName;
        }
    }

    public interface IGameDataBase
    {
        public void RegisterData(IGameData data);
    }


    public class GameDataManager
    {
        private const string JsonPath = "GameData/";

        public Dictionary<Type, IGameDataBase> Databases => _databases;
        private Dictionary<Type, IGameDataBase> _databases;

        public void Initialize()
        {
            _databases = new Dictionary<Type, IGameDataBase>();

            try
            {
                ParseGameDatas();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }


        private async UniTaskVoid ParseGameDatas()
        {
            var dataBaseTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IGameDataBase) != t && typeof(IGameDataBase).IsAssignableFrom(t));

            List<UniTask<(Type, IGameDataBase)>> tasks = new(dataBaseTypes.Count());
            
            foreach (var dataBaseType in dataBaseTypes)
            {
                var attribute = dataBaseType.GetCustomAttribute<GameDataBaseAttribute>();

                var json = await GetJson(attribute);
                
                if(json == null)
                {
                    continue;
                }
                
                tasks.Add(SetDataBase(dataBaseType, attribute, json));
            }
            
            var databases = await UniTask.WhenAll(tasks);

            foreach (var database in databases)
            {
                _databases.Add(database.Item1, database.Item2);
            }
            Debug.Log("로드완료"); //TODO 데이터 로드 후 씬 불러오기 부분 수정 필요
            if (SceneManager.GetActiveScene().buildIndex != 0)
                return;
            SceneManager.LoadScene(1);
        }

        private async UniTask<JArray> GetJson(GameDataBaseAttribute attribute)
        {
            var json = await SystemManager.Instance.ResourceManager.LoadAsset<TextAsset>(JsonPath + attribute.JsonFileName + ".json", false);
            
            if (json == null)
            {
                Debug.LogError($"{attribute.GameDataType}과 매칭되는 json 파일을 찾을 수 없습니다. JsonFilePath : {JsonPath}/{attribute.JsonFileName}");
                return null;
            }

            var result = JArray.Parse(json.text);

            if (result == null || result.Count != 2)
            {
                Debug.LogError($"json파일의 형식에 문제가 있습니다. JsonFilePath : {JsonPath}/{attribute.JsonFileName}");
                return null;
            }

            return result;
        }
        
        private async UniTask<(Type, IGameDataBase)> SetDataBase(Type dataBaseType, GameDataBaseAttribute attribute, JArray jsonResult)
        {
            var database = Activator.CreateInstance(dataBaseType) as IGameDataBase;

            var gameDataTypes = new Dictionary<string, PropertyInfo>();
            var propertyInfos = attribute.GameDataType.GetProperties();

            foreach (JToken gameData in jsonResult[0])
            {
                var property = propertyInfos.FirstOrDefault(t => t.Name == gameData.ToString());

                if (property != null)
                {
                    gameDataTypes.Add(gameData.ToString(), property);
                }
            }
                
            foreach (JObject gameData in jsonResult[1])
            {
                var data = Activator.CreateInstance(attribute.GameDataType) as IGameData;

                foreach (var type in gameDataTypes)
                {
                    if (gameData.TryGetValue(type.Key, out var value))
                    {
                        var parsedValue = ParseValue(type.Value.PropertyType, value.ToString());

                        if (parsedValue != null)
                        {
                            type.Value.SetValue(data, parsedValue);
                        }
                        else
                        {
                            //Debug.LogError($"{dataBaseType} : 지원되지 않는 형식의 변환입니다. Type : {type} Value : {value}");
                        }
                    }
                }

                database.RegisterData(data);
            }

            return (dataBaseType, database);
        }

        

        private object ParseValue(Type type, string value)
        {
            if (type == typeof(string))
            {
                return value;
            }
            if (type == typeof(int))
            {
                if (int.TryParse(value, out var result))
                {
                    return result;
                }
            }
            if (type == typeof(float))
            {
                if (float.TryParse(value, out var result))
                {
                    return result;
                }
            }
            if (type == typeof(bool))
            {
                if (bool.TryParse(value, out var result))
                {
                    return result;
                }
            }
            if (type.IsEnum && Enum.IsDefined(type, value))
            {
                return Enum.Parse(type, value, true);
            }

            return null;
        }


        // 조리예
        // GameDataManager.GetDataBase<TestGameDataBase>().GetData(1).Desc;
        public T GetDataBase<T>() where T : IGameDataBase
        {
            if (Databases.TryGetValue(typeof(T), out var dataBase))
            {
                return (T)dataBase;
            }

            return default(T);
        }
    }

}
