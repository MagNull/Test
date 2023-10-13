using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MissionInfrastructure
{
    public class GameMap
    {
        [SerializeField]
        private Dictionary<MissionData, Mission> _missionDataMap;
        private readonly MapData _mapData;

        public GameMap(MapData mapData)
        {
            _mapData = mapData;
        }

        public void Init(IEnumerable<Mission> missionData)
        {
            if (_missionDataMap != null)
            {
                Debug.LogWarning("Map is already initialized!");
                return;
            }

            _missionDataMap = missionData.ToDictionary(mission => mission.BaseData, mission => mission);
            ResolveMissionGraph();
        }

        public void CompleteMission(MissionData missionFullData)
        {
            var mission = _missionDataMap[missionFullData];
            mission.ChangeStatus(MissionStatus.Complete);
        }

        private void ResolveMissionGraph()
        {
            foreach (var mission in _missionDataMap.Values)
            {
                var requiredMissions = mission.BaseData.RequiredMissions
                    .Select(requiredMissionData =>
                        _missionDataMap.TryGetValue(requiredMissionData, out var requiredMission)
                            ? requiredMission
                            : null)
                    .Where(requiredMission => requiredMission != null)
                    .Cast<IReadOnlyMission>();

                mission.Init(requiredMissions.ToHashSet());
                mission.StatusChanged += OnMissionStatusChanged;
            }
        }

        public bool HasMissionAlternative(IReadOnlyMission mission, out IReadOnlyMission alternativeMission)
        {
            alternativeMission = null;
            if (!_mapData.IsMissionDouble(mission.BaseData))
                return false;

            alternativeMission = _missionDataMap[_mapData.GetAlternativeMissionData(mission.BaseData)];
            return true;
        }

        private void OnMissionStatusChanged(IReadOnlyMission mission)
        {
            var isMissionDouble = _mapData.IsMissionDouble(mission.BaseData);
            var alternativeMission =
                isMissionDouble ? _missionDataMap[_mapData.GetAlternativeMissionData(mission.BaseData)] : null;
            switch (mission.Status)
            {
                case MissionStatus.Active:
                {
                    //Unlock alternative mission if it is double
                    if (isMissionDouble && alternativeMission.Status == MissionStatus.Complete)
                    {
                        _missionDataMap[mission.BaseData].ChangeStatus(MissionStatus.Locked);
                    }
                    else
                    {
                        //Lock temporary locking missions
                        foreach (var temporaryLockingMission in mission.BaseData.TemporaryLockingMissions.Select(data =>
                                     _missionDataMap[data]))
                        {
                            temporaryLockingMission.ChangeStatus(MissionStatus.TemporaryLocked);
                        }
                    }

                    break;
                }

                case MissionStatus.Complete:
                    //Lock alternative mission if it is double
                    if (isMissionDouble)
                    {
                        alternativeMission.ChangeStatus(MissionStatus.Locked);
                    }

                    //Unlock temporary locking missions
                    foreach (var temporaryLockingMission in mission.BaseData.TemporaryLockingMissions.Select(data =>
                                 _missionDataMap[data]))
                    {
                        temporaryLockingMission.ChangeStatus(MissionStatus.Active);
                    }

                    //Check required missions
                    foreach (var missionModel in _missionDataMap.Values)
                    {
                        missionModel.CheckRequiredMissions(mission);
                        missionModel.CheckDependentFractions(mission);
                    }

                    break;
            }
        }
    }
}