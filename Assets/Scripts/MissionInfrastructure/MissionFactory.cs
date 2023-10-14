using UnityEngine;

namespace MissionInfrastructure
{
    public class MissionFactory
    {
        private readonly MissionView _viewPrefab;
        
        public MissionFactory(MissionView viewPrefab)
        {
            _viewPrefab = viewPrefab;
        }
        
        public (Mission model, MissionView view) Create(MissionData data, MissionData alternative = null)
        {
            var mission = alternative == null ? new Mission(data) : new DoubleMission(data);
            var view = Object.Instantiate(_viewPrefab);
            view.Init(mission);

            return (mission, view);
        }
    }
}