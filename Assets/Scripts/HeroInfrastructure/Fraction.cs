namespace HeroInfrastructure
{
    public enum Fraction
    {
        None,
        Owl,
        Crow,
        Jackdaw,
        Jay,
        Sparrow,
        Gull,
        Eagle,
        Phoenix
    }

    public static class FractionExtension
    {
        public static string ToRussianName(this Fraction fraction)
        {
            switch (fraction)
            {
                case Fraction.None:
                    return "Нет гнезда";
                case Fraction.Owl:
                    return "Совы";
                case Fraction.Crow:
                    return "Вороны";
                case Fraction.Jackdaw:
                    return "Галки";
                case Fraction.Jay:
                    return "Сойки";
                case Fraction.Sparrow:
                    return "Воробьи";
                case Fraction.Gull:
                    return "Чайки";
                case Fraction.Eagle:
                    return "Орлы";
                case Fraction.Phoenix:
                    return "Фениксы";
                default:
                    return "Нет";
            }
        }
    }
}