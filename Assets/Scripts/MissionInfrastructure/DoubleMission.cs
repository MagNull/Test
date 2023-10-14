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

        public override void CheckRequiredMissions(IReadOnlyMission completedMission)
        {
            if (CheckAlternativeComplete(completedMission))
                return;

            base.CheckRequiredMissions(completedMission);
        }

        private bool CheckAlternativeComplete(IReadOnlyMission completedMission)
        {
            if (_alternativeMission.Status == MissionStatus.Complete &&
                completedMission != _alternativeMission)
                return true;

            if (completedMission != _alternativeMission)
                return false;

            ChangeStatus(MissionStatus.Locked);
            return true;
        }
    }
}