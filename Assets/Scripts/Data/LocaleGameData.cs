using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEngine.Events;

namespace QT
{
    public class LocaleGameData : IGameData
    {
        public int Index { get; }
        public string StringKey { get; set; }
        
        public string LocaleKR { get; set; }
        public string LocaleUS { get; set; }
    }

    public enum Locale
    {
        KR,
        US,
    }

    [GameDataBase(typeof(LocaleGameData), "LocaleGameData")]
    public class LocaleGameDataBase : IGameDataBase
    {
        public string LocalePlayerPrefsKey => "Locale";
        public UnityEvent<LocaleGameDataBase> OnLocaleChanged = new ();
        
        private Locale _currentLocale = Locale.KR;
        public Locale CurrentLocale { get => _currentLocale;
            set
            {
                _currentLocale = value;
                OnLocaleChanged.Invoke(this);
                
                PlayerPrefs.SetInt(LocalePlayerPrefsKey, (int) _currentLocale);
            }
        }
        
        private readonly Dictionary<string, string[]> _datas = new();

        public LocaleGameDataBase()
        {
            CurrentLocale = PlayerPrefs.GetInt(LocalePlayerPrefsKey, 0) == 0 ? Locale.KR : Locale.US;
        }
        
        public void RegisterData(IGameData data)
        {
            var localeData = (LocaleGameData) data;
            _datas.Add(localeData.StringKey, new []{localeData.LocaleKR, localeData.LocaleUS});
        }

        public void OnInitialize(GameDataManager manager)
        {  
#if UNITY_EDITOR
            SetAlLDataString();
#endif
        }

        public string GetString(string key)
        {
            if (_datas.TryGetValue(key, out var value))
            {
                return value[(int)CurrentLocale];
            }

            return key;
        }
        
        public string[] GetStrings(string key)
        {
            if (_datas.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }


#if UNITY_EDITOR
        public string[] AllDataString { get; private set; }

        public void SetAlLDataString()
        {
            var data = new List<string>();
            
            foreach (var pair in _datas)
            {
                data.Add($"{pair.Key}/  |  {pair.Value[0]}/{pair.Value[1]}");
                //data.Add($"{pair.Key}");
            }
            
            AllDataString = data.ToArray();
        }
#endif
        
    }  
}