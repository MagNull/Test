using System.Linq;
using HeroInfrastructure;
using MissionInfrastructure;
using TMPro;
using UnityEngine;

namespace Panels
{
    public class PlayMissionPanel : MissionPanel
    {
        [SerializeField]
        private TextMeshProUGUI _descriptionText;
        [SerializeField]
        private TextMeshProUGUI _playerSideText;
        [SerializeField]
        private TextMeshProUGUI _enemySideText;
        
        protected override void OnActivate(IReadOnlyMission mission)
        {
            _descriptionText.text = mission.BaseData.MainText;
            _playerSideText.text = string.Join(", ", mission.BaseData.PlayerFractions.Select(f => f.ToRussianName()));
            _enemySideText.text = string.Join(", ", mission.BaseData.EnemyFractions.Select(f => f.ToRussianName()));
        }
    }
}