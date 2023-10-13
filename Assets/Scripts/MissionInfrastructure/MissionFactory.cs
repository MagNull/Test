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
        
        public (Mission model, MissionView view) Create(MissionData data)
        {
            var mission = new Mission(data);
            var worldPosition = Camera.main.ScreenToWorldPoint(data.ScreenPosition) + Vector3.forward * 10f;
            var view = Object.Instantiate(_viewPrefab, worldPosition, Quaternion.identity);
            view.Init(mission);

            return (mission, view);
        }
    }
}