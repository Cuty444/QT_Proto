using System;

namespace QT.InGame
{
    public class StatParameter
    {
        public enum StatValueType
        {
            Value,
            Base,
            Per
        }

        public readonly PlayerStats Stat;
        public readonly StatValueType Type;
        
        public StatParameter(PlayerStats stat, StatValueType type)
        {
            Stat = stat;
            Type = type;
        }

        
        public static bool ParseStatParam(string data, out StatParameter statParam)
        {
            var parts = data.Split('_');

            statParam = null;
            
            if (parts.Length <= 0 || parts.Length > 2)
            {
                return false;
            }
                
            if (!Enum.TryParse(parts[0], out PlayerStats stat))
            {
                return false;
            }

            StatValueType type = StatValueType.Value;
            
            if (parts.Length == 2)
            {
                if (!Enum.TryParse(parts[1], out type))
                {
                    return false;
                }
            }

            statParam = new StatParameter(stat, type);
            
            return true;
        }

        public static float GetStatValue(Stat stat, StatValueType type)
        {
            if(stat is Status)
            {
                var status = stat as Status;

                switch (type)
                {
                    case StatValueType.Base:
                        return status.Value;
                    case StatValueType.Value:
                        return status.StatusValue;
                    case StatValueType.Per:
                        return status.Value != 0 ? status.StatusValue / status.Value : 1;
                }
            }

            return stat;
        }
    }
}
