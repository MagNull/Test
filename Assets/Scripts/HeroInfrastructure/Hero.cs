using System;
using UnityEngine;

namespace HeroInfrastructure
{
    public enum HeroStatus
    {
        Locked,
        Unlocked,
        Selected
    }

    [Serializable]
    public class Hero : IReadOnlyHero
    {
        public event Action<IReadOnlyHero> StatusChanged;
        public event Action<int> PointsChanged;

        private readonly HeroBaseData _baseData;
        [SerializeField]
        private int _points;
        private HeroStatus _status;

        public HeroBaseData BaseData => _baseData;

        public HeroStatus Status => _status;

        public int Points => _points;

        public Hero(HeroBaseData baseData)
        {
            _baseData = baseData;
            _status = baseData.DefaultStatus;
            _points = 0;
        }

        public void AddPoints(int points)
        {
            _points += points;
            PointsChanged?.Invoke(_points);
        }

        public void ChangeStatus(HeroStatus status)
        {
            _status = status;
            StatusChanged?.Invoke(this);
        }
    }

    public interface IReadOnlyHero
    {
        public event Action<IReadOnlyHero> StatusChanged;
        public event Action<int> PointsChanged;
        HeroBaseData BaseData { get; }
        HeroStatus Status { get; }
        public int Points { get; }
    }
}