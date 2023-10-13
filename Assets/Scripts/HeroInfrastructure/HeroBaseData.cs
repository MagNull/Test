using UnityEngine;

namespace HeroInfrastructure
{
    [CreateAssetMenu(fileName = "HeroBaseData", menuName = "Hero Infrastructure/Hero", order = 1)]
    public class HeroBaseData : ScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public Sprite Avatar { get; private set; }
        [field: SerializeField]
        public HeroStatus DefaultStatus { get; private set; }

        [field: SerializeField]
        public Fraction Fraction;
    }
}