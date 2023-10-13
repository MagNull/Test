using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace MissionInfrastructure
{
    [CreateAssetMenu(fileName = "Map", menuName = "Missions/Map", order = 1)]
    public class MapData : ScriptableObjectContainer
    {
        [SerializeField]
        [ReadOnly]
        [HorizontalGroup("Count", Width = 150)]
        [HideLabel, Title("Number of missions")]
        private int _numberOfMissions;

        [SerializeField]
        [ListDrawerSettings(ShowFoldout = false, HideAddButton = true, HideRemoveButton = true)]
        private List<MissionData> _missions;

        [SerializeField]
        [ListDrawerSettings(HideAddButton = true)]
        [HorizontalGroup("AltrnativeMissions")]
        private readonly List<(MissionData MissionA, MissionData MissionB)> _alternativeMissions = new();

        public IReadOnlyList<MissionData> Missions => _missions;

        public MissionData GetAlternativeMissionData(MissionData origin)
        {
            var alternative = _alternativeMissions.Find(x => x.MissionA == origin).MissionB;
            if (alternative != default)
                return alternative;

            alternative = _alternativeMissions.Find(x => x.MissionB == origin).MissionA;
            return alternative;
        }

        public bool IsMissionDouble(MissionData mission)
        {
            return _alternativeMissions.Exists(x => x.MissionA == mission || x.MissionB == mission);
        }

        [Button(Expanded = true)]
        [HorizontalGroup("Count")]
        [Title("Change number of missions")]
        private void ChangeNumber(int newNumber)
        {
            _numberOfMissions = newNumber;
            _missions.SetLength(_numberOfMissions);

            ClearExtraElements(_numberOfMissions);
            CreateNewMissions();
        }

        [HorizontalGroup("AltrnativeMissions")]
        [Button(Expanded = true)]
        private void AddDoubleMission(MissionData missionA, MissionData missionB)
        {
            //Validate input
            if (_alternativeMissions.Exists(x => x.MissionA == missionA || x.MissionB == missionA)
                || missionA == missionB || missionA == null || missionB == null
                || _alternativeMissions.Exists(x => x.MissionA == missionB || x.MissionB == missionB))
                return;

            _alternativeMissions.Add((missionA, missionB));
        }

        private void CreateNewMissions()
        {
            var newMissions = new List<Object>();
            for (var i = 0; i < _numberOfMissions; i++)
            {
                if (_missions[i] == null)
                {
                    _missions[i] = CreateInstance<MissionData>();
                    newMissions.Add(_missions[i]);
                }
            }

            AddChildren(newMissions);
        }
    }
}