using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Reflection;

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
        public Dictionary<int, IGameData> datas { get; set; }
    }


    public class GameDataManager
    {
        private const string JsonPath = "GameData/";

        public Dictionary<Type, IGameDataBase> Databases => _databases;
        private Dictionary<Type, IGameDataBase> _databases;

        public void Initialize()
        {
            _databases = new Dictionary<Type, IGameDataBase>();
            ParseGameDatas();
        }


        private void ParseGameDatas()
        {
            var dataBaseTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IGameDataBase) != t && typeof(IGameDataBase).IsAssignableFrom(t));

            foreach (var dataBaseType in dataBaseTypes)
            {
                var attribute = dataBaseType.GetCustomAttribute<GameDataBaseAttribute>();

                if(!GetJson(attribute, out var result))
                {
                    continue;
                }

                var database = Activator.CreateInstance(dataBaseType) as IGameDataBase;
                database.datas = new Dictionary<int, IGameData>();

                var gameDataTypes = new Dictionary<string, PropertyInfo>();
                var propertyInfos = attribute.GameDataType.GetProperties();

                foreach (JToken gameData in result[0])
                {
                    var property = propertyInfos.FirstOrDefault(t => t.Name == gameData.ToString());

                    if (property != null)
                    {
                        gameDataTypes.Add(gameData.ToString(), property);
                    }
                }

                foreach (JObject gameData in result[1])
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
                                Debug.LogError($"{dataBaseType} : 지원되지 않는 형식의 변환입니다. Type : {type} Value : {value}");
                            }
                        }
                    }

                    database.datas.Add(data.Index, data);
                }

                _databases.Add(dataBaseType, database);
            }
        }

        private bool GetJson(GameDataBaseAttribute attribute, out JArray result)
        {
            result = null;

            var json = Resources.Load<TextAsset>(JsonPath + attribute.JsonFileName); // todo : 리소스 매니저와 연동

            if (json == null)
            {
                Debug.LogError($"{attribute.GameDataType}과 매칭되는 json 파일을 찾을 수 없습니다. JsonFilePath : {JsonPath}/{attribute.JsonFileName}");
                return false;
            }

            result = JArray.Parse(json.text);

            if (result == null || result.Count != 2)
            {
                Debug.LogError($"json파일의 형식에 문제가 있습니다. JsonFilePath : {JsonPath}/{attribute.JsonFileName}");
                return false;
            }

            return true;
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
