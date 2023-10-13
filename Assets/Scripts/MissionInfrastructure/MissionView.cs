using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MissionInfrastructure
{
    public class MissionView : SerializedMonoBehaviour
    {
        public event Action<IReadOnlyMission> Selected;

        [SerializeField]
        private Dictionary<MissionStatus, Sprite> _stateToSprite;

        [SerializeField]
        private SpriteRenderer _stateRenderer;

        public IReadOnlyMission Model { get; private set; }

        public void Init(IReadOnlyMission model)
        {
            if (Model != null)
            {
                Debug.LogWarning("MissionView is already initialized!");
                return;
            }

            Model = model;
            model.StatusChanged += OnStateChanged;
            OnStateChanged(model);
        }

        private void OnStateChanged(IReadOnlyMission mission)
        {
            gameObject.SetActive(mission.Status != MissionStatus.Locked);

            _stateRenderer.sprite = _stateToSprite[mission.Status];
        }

        private void OnMouseDown()
        {
            if (Model.Status == MissionStatus.Active)
                Selected?.Invoke(Model);
        }
    }
}