using System.Collections.Generic;
using QT.Core;
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
        public UnityEvent<LocaleGameDataBase> OnLocaleChanged = new ();
        
        private Locale _currentLocale = Locale.KR;
        public Locale CurrentLocale { get => _currentLocale;
            set
            {
                _currentLocale = value;
                OnLocaleChanged.Invoke(this);
            }
        }
        
        private readonly Dictionary<string, string[]> _datas = new();

        public void RegisterData(IGameData data)
        {
            var localeData = (LocaleGameData) data;
            _datas.Add(localeData.StringKey, new []{localeData.LocaleKR, localeData.LocaleUS});
        }

        public void OnInitialize(GameDataManager manager)
        {
            
        }

        public string GetString(string key)
        {
            if (_datas.TryGetValue(key, out var value))
            {
                return value[(int)CurrentLocale];
            }

            return key;
        }
    }  
}