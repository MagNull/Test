using System;
using System.Collections.Generic;
using System.Linq;
using HeroInfrastructure;
using UnityEngine;
using Utils;

namespace MissionInfrastructure
{
    public class Mission : IReadOnlyMission
    {
        public event Action<IReadOnlyMission> StatusChanged;
        public MissionStatus Status { get; private set; }
        public MissionData BaseData { get; }
        public IReadOnlyList<Fraction> CurrentPlayerFractions => _currentPlayerFractions;
        public IReadOnlyList<Fraction> CurrentEnemyFractions => _currentEnemyFractions;

        private HashSet<IReadOnlyMission> _requiredMissions;
        private readonly List<Fraction> _currentPlayerFractions;
        private readonly List<Fraction> _currentEnemyFractions;

        public Mission(MissionData data)
        {
            BaseData = data;
            _currentPlayerFractions = data.PlayerFractions.ToList();
            _currentEnemyFractions = data.EnemyFractions.ToList();
            Status = data.DefaultStatus;
        }

        public void Init(HashSet<IReadOnlyMission> requiredMissions)
        {
            _requiredMissions = requiredMissions;
            if (_requiredMissions.Count != 0 || Status == MissionStatus.Locked)
                ChangeStatus(MissionStatus.Locked);
            else
                ChangeStatus(MissionStatus.Active);
        }

        public void ChangeStatus(MissionStatus newStatus)
        {
            if (newStatus == Status)
            {
                Debug.LogWarning("Mission state is already " + newStatus + " for mission " + BaseData.Title + "!");
                return;
            }

            Status = newStatus;
            StatusChanged?.Invoke(this);
        }

        public virtual void CheckRequiredMissions(IReadOnlyMission completedMission)
        {
            if (Status != MissionStatus.Locked)
                return;

            if (!_requiredMissions.Contains(completedMission))
                return;

            _requiredMissions.Remove(completedMission);
            switch (BaseData.UnlockCondition)
            {
                case LogicCondition.Any:
                case LogicCondition.All when _requiredMissions.Count == 0:
                    ChangeStatus(MissionStatus.Active);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void CheckDependentFractions(IReadOnlyMission completedMission)
        {
            var playerFractions = BaseData.DependentPlayerFractions
                .Where(dependent => dependent.DependentMission == completedMission.BaseData)
                .Select(dependent => dependent.Fraction);
            _currentPlayerFractions.AddRange(playerFractions);

            var enemyFractions = BaseData.DependentEnemyFractions
                .Where(dependent => dependent.DependentMission == completedMission.BaseData)
                .Select(dependent => dependent.Fraction);
            _currentEnemyFractions.AddRange(enemyFractions);
        }
    }

    public interface IReadOnlyMission
    {
        public event Action<IReadOnlyMission> StatusChanged;
        public MissionStatus Status { get; }
        public IReadOnlyList<Fraction> CurrentPlayerFractions { get; }
        public IReadOnlyList<Fraction> CurrentEnemyFractions { get; }
        public MissionData BaseData { get; }
    }
}