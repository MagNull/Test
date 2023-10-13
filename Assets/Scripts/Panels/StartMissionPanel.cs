using MissionInfrastructure;
using TMPro;
using UnityEngine;

namespace Panels
{
    public class StartMissionPanel : MissionPanel
    {
        [SerializeField]
        private TextMeshProUGUI _preText;

        protected override void OnActivate(IReadOnlyMission mission)
        {
            _preText.text = mission.BaseData.PreText;
        }
    }
}