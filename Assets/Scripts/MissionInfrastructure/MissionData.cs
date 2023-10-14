using System;
using System.Collections.Generic;
using HeroInfrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using Utils;

namespace MissionInfrastructure
{
    [Serializable]
    public class MissionData : SerializedScriptableObject
    {
        #region Properties

        //Others
        public Vector2 ScreenPosition => _screenPosition;

        public MissionStatus DefaultStatus => _defaultStatus;

        //Information
        public Sprite Image => _image;
        public string Title => _title;
        public string PreText => _preText;
        public string MainText => _mainText;
        public IReadOnlyList<Fraction> PlayerFractions => _playerFractions;

        public IReadOnlyList<(MissionData DependentMission, Fraction Fraction)> DependentPlayerFractions =>
            _dependentPlayerFractions;

        public IReadOnlyList<(MissionData DependentMission, Fraction Fraction)> DependentEnemyFractions =>
            _dependentEnemyFractions;

        public IReadOnlyList<Fraction> EnemyFractions => _enemyFractions;

        //Linked Missions
        public IReadOnlyList<MissionData> RequiredMissions => _requiredMissions;
        public LogicCondition UnlockCondition => _unlockCondition;
        public IReadOnlyList<MissionData> TemporaryLockingMissions => _temporaryLockingMissions;

        //Mission Rewards
        public IReadOnlyList<(Fraction, int RewardValue)> PointsReward => _pointsReward;
        public int SelectedHeroPointReward => _selectedHeroPointReward;
        public IReadOnlyList<HeroBaseData> HeroReward => _heroReward;

        #endregion

        #region Others

        [TitleGroup("Others", HorizontalLine = true, Alignment = TitleAlignments.Centered)]
        [HorizontalGroup("Others/Split")]
        [HideLabel]
        [Title("Screen Position", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [SerializeField]
        private Vector2 _screenPosition;

        [SerializeField]
        [HorizontalGroup("Others/Split")]
        [HideLabel]
        [Title("Default status", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        private MissionStatus _defaultStatus;

        #endregion

        #region Information

        [SerializeField]
        [TitleGroup("Mission Information", HorizontalLine = true, Alignment = TitleAlignments.Centered)]
        [HorizontalGroup("Mission Information/Image")]
        [PreviewField(Alignment = ObjectFieldAlignment.Center, Height = 100)]
        [Title("Image", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [HideLabel]
        private Sprite _image;

        [SerializeField]
        [HorizontalGroup("Mission Information/Title")]
        [Title("Title", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [HideLabel]
        [OnValueChanged(nameof(OnTitleChanged))]
        private string _title;

        [SerializeField]
        [Multiline(10)]
        [HorizontalGroup("Mission Information/Texts")]
        [Title("PreText", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [HideLabel]
        private string _preText;

        [SerializeField]
        [Multiline(10)]
        [HorizontalGroup("Mission Information/Texts")]
        [Title("Text", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [HideLabel]
        private string _mainText;

        [SerializeField]
        [HorizontalGroup("Mission Information/Sides")]
        [VerticalGroup("Mission Information/Sides/Player Fractions")]
        [Title("Player Fractions", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [LabelText("Fractions")]
        private List<Fraction> _playerFractions = new();

        [SerializeField]
        [HorizontalGroup("Mission Information/Sides")]
        [VerticalGroup("Mission Information/Sides/Player Fractions")]
        [Title("Dependent Fractions", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [LabelText("Fraction Missions")]
        private List<(MissionData DependentMission, Fraction Fraction)> _dependentPlayerFractions = new();


        [SerializeField]
        [HorizontalGroup("Mission Information/Sides")]
        [VerticalGroup("Mission Information/Sides/Enemy Fractions")]
        [Title("Enemy Fractions", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [LabelText("Fractions")]
        private List<Fraction> _enemyFractions = new();

        [SerializeField]
        [HorizontalGroup("Mission Information/Sides")]
        [VerticalGroup("Mission Information/Sides/Enemy Fractions")]
        [Title("Dependent Fractions", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [LabelText("Fraction Missions")]
        private List<(MissionData DependentMission, Fraction Fraction)> _dependentEnemyFractions = new();

        #endregion

        #region Linked Missions

        [TitleGroup("Linked Missions", HorizontalLine = true, Alignment = TitleAlignments.Centered)]
        [HorizontalGroup("Linked Missions/split")]
        [VerticalGroup("Linked Missions/split/Required")]
        [Title("Required Missions", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [LabelText("Missions")]
        [SerializeField]
        private List<MissionData> _requiredMissions = new();

        [SerializeField]
        [HorizontalGroup("Linked Missions/split")]
        [VerticalGroup("Linked Missions/split/Required")]
        [Title("Unlock Condition", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [LabelText("Conditions")]
        private LogicCondition _unlockCondition;

        [HorizontalGroup("Linked Missions/split")]
        [Title("Temporary Locking Missions", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [LabelText("Missions")]
        [SerializeField]
        private List<MissionData> _temporaryLockingMissions = new();

        #endregion

        #region Mission Rewards

        [SerializeField, OdinSerialize]
        [TitleGroup("Mission Rewards", HorizontalLine = true, Alignment = TitleAlignments.Centered)]
        [LabelText("Points To Fraction")]
        private List<(Fraction, int RewardValue)> _pointsReward = new();

        [SerializeField]
        private int _selectedHeroPointReward;

        [SerializeField]
        private List<HeroBaseData> _heroReward = new();

        #endregion

        private void OnTitleChanged()
        {
            name = Title == "" ? "Mission" : Title;
            AssetDatabase.SaveAssets();
        }
    }
}