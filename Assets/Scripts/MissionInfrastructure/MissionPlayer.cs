using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HeroInfrastructure;
using Panels;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace MissionInfrastructure
{
    public class MissionPlayer : MonoBehaviour
    {
        [SerializeField]
        private StartMissionPanel _startMissionPanel;
        [SerializeField]
        private StartMissionPanel _alternativeStartMissionPanel;
        [SerializeField]
        private PlayMissionPanel _playMissionPanel;
        
        [Header("Effects")]
        [SerializeField]
        private TextMeshProUGUI _selectHeroWarningText;
        [SerializeField]
        private float _warningFadeDelay;
        [SerializeField]
        private float _warningSizeDelay;
        [SerializeField]
        private float _warningSizeStrength;

        private GameMap _map;
        private HeroesPool _heroesPool;
        private IReadOnlyHero _selectedHero;
        private IReadOnlyMission _selectedMission;
        
        
        public void Init(List<MissionView> missionViews, GameMap map, HeroesPool heroesPool)
        {
            if (_map != null)
            {
                Debug.LogWarning("MissionPlayer is already initialized!");
                return;
            }

            _map = map;
            _heroesPool = heroesPool;

            foreach (var missionView in missionViews)
            {
                missionView.Selected += OnMissionSelected;
            }

            _startMissionPanel.ButtonClicked += OnMissionStarted;
            _alternativeStartMissionPanel.ButtonClicked += OnMissionStarted;
            _playMissionPanel.ButtonClicked += OnMissionCompleted;
        }

        private void OnHeroSelected(IReadOnlyHero hero)
        {
            if (_selectedHero == hero)
                return;

            if (_selectedHero != null)
                _heroesPool.DeselectHero(_selectedHero);

            _selectedHero = hero;
            _heroesPool.SelectHero(hero);
        }

        private void OnMissionSelected(IReadOnlyMission mission)
        {
            if (_selectedMission == mission)
                return;

            _selectedMission = mission;
            if (_map.HasMissionAlternative(mission, out var alternativeMission) && 
                alternativeMission.Status == MissionStatus.Active)
            {
                OnDoubleMissionSelected(mission, alternativeMission);
            }
            else
            {
                _alternativeStartMissionPanel.Deactivate();
                _startMissionPanel.Activate(mission);
            }

            InitHeroSelection();
        }

        private void OnDoubleMissionSelected(IReadOnlyMission missionA, IReadOnlyMission missionB)
        {
            _startMissionPanel.Activate(missionA);
            _alternativeStartMissionPanel.Activate(missionB);
        }

        private void InitHeroSelection()
        {
            if (_selectedHero != null)
            {
                _heroesPool.DeselectHero(_selectedHero);
                _selectedHero = null;
            }

            _heroesPool.HeroClicked -= OnHeroSelected;
            _heroesPool.HeroClicked += OnHeroSelected;
        }

        private void OnMissionStarted(IReadOnlyMission mission, MissionPanel panel)
        {
            if (_selectedHero == null)
            {
                SelectHeroWarning();
                return;
            }

            _heroesPool.HeroClicked -= OnHeroSelected;
            
            _startMissionPanel.Deactivate();
            _alternativeStartMissionPanel.Deactivate();
            _playMissionPanel.Activate(mission);
        }

        private void OnMissionCompleted(IReadOnlyMission mission, MissionPanel panel)
        {
            panel.Deactivate();
            _map.CompleteMission(mission.BaseData);

            GrantReward(mission.BaseData);
        }

        private void GrantReward(MissionData missionData)
        {
            if (missionData.HeroReward.Any())
            {
                foreach (var hero in missionData.HeroReward)
                {
                    _heroesPool.AddHero(hero);
                }
            }

            _heroesPool.DistributePoints(missionData.PointsReward, missionData.SelectedHeroPointReward);
            _heroesPool.DeselectHero(_selectedHero);
            _selectedHero = null;
        }
        
        private void SelectHeroWarning()
        {
            _selectHeroWarningText.DOComplete();
            _selectHeroWarningText.gameObject.SetActive(true);
            _selectHeroWarningText.transform.DOComplete();
            _selectHeroWarningText.transform.localScale = Vector3.one;
            _selectHeroWarningText.color = _selectHeroWarningText.color.WithAlpha(1);

            _selectHeroWarningText.transform
                .DOShakeScale(_warningSizeDelay, _warningSizeStrength).OnComplete(() =>
                {
                    _selectHeroWarningText.DOFade(0, _warningFadeDelay).OnComplete(() =>
                    {
                        _selectHeroWarningText.gameObject.SetActive(false);
                    });
                });
        }
    }
}