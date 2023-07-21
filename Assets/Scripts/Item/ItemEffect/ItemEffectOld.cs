using System.Collections.Generic;
using System.Text;
using QT.InGame;
using ApplyTypes = QT.ItemEffectGameData.ApplyTypes;
using ApplyPoints = QT.ItemEffectGameData.ApplyPoints;

namespace QT
{
    public abstract class ItemEffectOld
    {
        public virtual ApplyTypes ApplyType => ApplyTypes.None;
        
        public ApplyPoints ApplyPoints { get; protected set; }
        public readonly bool IsAvailable = false;
        
        public ItemEffectOld(ItemEffectGameData effectData)
        {
            if(effectData == null || effectData.ApplyType != ApplyType)
            {
                return;
            }
            
            ApplyPoints = effectData.ApplyPoint;
            
            IsAvailable = Process(effectData);
        }

        protected abstract bool Process(ItemEffectGameData effectData);
        
        public abstract void ApplyEffect(Player player, object source);
        public abstract void RemoveEffect(Player player, object source);
        
        protected static List<string> ParseApplyValue(ref string applyValue)
        {
            bool IsAlpha(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
            bool IsNumber(char c) => c is >= '0' and <= '9';

            var param = new List<string>();

            var value = new StringBuilder(applyValue.Length);
            var temp = new StringBuilder(20);

            bool isLastCharIsAlpha = false;
            foreach (var c in applyValue)
            {
                var isAlpha = IsAlpha(c);
                
                if (!isAlpha && c != '_' && !(isLastCharIsAlpha && IsNumber(c)))
                {
                    if (temp.Length > 0)
                    {
                        param.Add(temp.ToString());
                        value.Append($"[{param.Count - 1}]");
                        
                        temp.Clear();
                    }

                    value.Append(c);
                }
                else
                {
                    temp.Append(c);
                }

                isLastCharIsAlpha = isAlpha;
            }

            if (temp.Length > 0)
            {
                param.Add(temp.ToString());
                value.Append($"[{param.Count - 1}]");
            }

            applyValue = value.ToString();
            
            return param;
        }
    }
}
