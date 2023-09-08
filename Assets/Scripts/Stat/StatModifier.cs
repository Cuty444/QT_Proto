namespace QT
{
    public class StatModifier
    {
        public enum ModifierType
        {
            Multiply,
            Addition,
            Hard
        }
        
        public readonly float Value;
        public readonly ModifierType ModType;
        public readonly int Order;
        public readonly object Source;
        
        public StatModifier(float value, ModifierType type, object source)
        {
            Value = value;
            ModType = type;
            Order = (int) type;
            Source = source;
        }
    }
}