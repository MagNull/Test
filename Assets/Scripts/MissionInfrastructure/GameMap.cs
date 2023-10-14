using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MissionInfrastructure
{
    public class GameMap
    {
        private readonly Dictionary<MissionData, (Mission Model, MissionView View)> _missionDataMap;
        private readonly MapData _mapData;
        private bool _isInitialized;
        public GameMap(MapData mapData)
        {
            _mapData = mapData;
            _missionDataMap = new Dictionary<MissionData, (Mission Model, MissionView View)>();
        }

        public void Init(MissionFactory missionFactory)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("Map is already initialized!");
                return;
            }

            _isInitialized = true;

            foreach (var missionData in _mapData.Missions)
            {
                var alternativeMission = _mapData.GetAlternativeMissionData(missionData);
                var mission = missionFactory.Create(missionData, alternativeMission);
                _missionDataMap.Add(missionData, mission);
            }

            ResolveMissionGraph();
        }

        public IEnumerable<MissionView> GetViews()
        {
            return _missionDataMap.Values.Select(x => x.View).ToArray();
        }

        public void CompleteMission(MissionData missionFullData)
        {
            var mission = _missionDataMap[missionFullData];
            mission.Model.ChangeStatus(MissionStatus.Complete);
        }

        private void ResolveMissionGraph()
        {
            foreach (var mission in _missionDataMap.Values)
            {
                var worldPosition = Camera.main.ScreenToWorldPoint(mission.Model.BaseData.ScreenPosition) +
                                    Vector3.forward * 10f;
                mission.View.transform.position = worldPosition;

                var requiredMissions = mission.Model.BaseData.RequiredMissions
                    .Select(requiredMissionData =>
                        _missionDataMap.TryGetValue(requiredMissionData, out var requiredMission)
                            ? requiredMission.Model
                            : null)
                    .Where(requiredMission => requiredMission != null)
                    .Cast<IReadOnlyMission>();

                if (mission.Model is DoubleMission doubleMission)
                {
                    var alternativeMission = _missionDataMap[_mapData.GetAlternativeMissionData(mission.Model.BaseData)]
                        .Model;
                    doubleMission.Init(alternativeMission, requiredMissions.ToHashSet());
                }
                else
                {
                    mission.Model.Init(requiredMissions.ToHashSet());
                }

                mission.Model.StatusChanged += OnMissionStatusChanged;
            }
        }

        public bool HasMissionAlternative(IReadOnlyMission mission, out IReadOnlyMission alternativeMission)
        {
            alternativeMission = null;
            if (!_mapData.IsMissionDouble(mission.BaseData))
                return false;

            alternativeMission = _missionDataMap[_mapData.GetAlternativeMissionData(mission.BaseData)].Model;
            return true;
        }

        private void OnMissionStatusChanged(IReadOnlyMission mission)
        {
            switch (mission.Status)
            {
                case MissionStatus.Active:
                {
                    if (mission is DoubleMission doubleMission &&
                        doubleMission.AlternativeMission.Status == MissionStatus.Complete)
                    {
                        _missionDataMap[mission.BaseData].Model.ChangeStatus(MissionStatus.Locked);
                    }
                    else
                    {
                        foreach (var temporaryLockingMission in mission.BaseData.TemporaryLockingMissions.Select(data =>
                                     _missionDataMap[data]))
                        {
                            temporaryLockingMission.Model.ChangeStatus(MissionStatus.TemporaryLocked);
                        }
                    }

                    break;
                }

                case MissionStatus.Complete:
                    foreach (var temporaryLockingMission in mission.BaseData.TemporaryLockingMissions.Select(data =>
                                 _missionDataMap[data]))
                    {
                        temporaryLockingMission.Model.ChangeStatus(MissionStatus.Active);
                    }
                    
                    foreach (var missionModel in _missionDataMap.Values)
                    {
                        missionModel.Model.CheckRequiredMissions(mission);
                        missionModel.Model.CheckDependentFractions(mission);
                        if(missionModel.Model is DoubleMission doubleMission)
                            doubleMission.CheckAlternativeMission(mission);
                    }

                    break;
            }
        }
    }
}