using System.Collections.Generic;

namespace MissionInfrastructure
{
    public class DoubleMission : Mission
    {
        private Mission _alternativeMission;

        public DoubleMission(MissionData data) : base(data)
        {
        }

        public DoubleMission AlternativeMission => (DoubleMission)_alternativeMission;

        public void Init(Mission alternativeMissionData, HashSet<IReadOnlyMission> requiredMissions)
        {
            _alternativeMission = alternativeMissionData;
            base.Init(requiredMissions);
        }

        public void CheckAlternativeMission(IReadOnlyMission completedMission)
        {
            if (completedMission != _alternativeMission) 
                return;
            
            ChangeStatus(MissionStatus.Locked);
        }
    }
}