using System;
using MissionInfrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    public abstract class MissionPanel : MonoBehaviour
    {
        public event Action<IReadOnlyMission, MissionPanel> ButtonClicked;
        
        [SerializeField]
        private TextMeshProUGUI _title;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Button _button;
        
        private IReadOnlyMission _currentMission;

        public void Activate(IReadOnlyMission mission)
        {
            _currentMission = mission;
            _title.text = mission.BaseData.Title;
            _image.sprite = mission.BaseData.Image;
            gameObject.SetActive(true);
            
            _button.onClick.RemoveListener(OnButtonClicked);
            _button.onClick.AddListener(OnButtonClicked);
            
            OnActivate(mission);
        }

        public void Deactivate()
        {
            _currentMission = null;
            _button.onClick.RemoveListener(OnButtonClicked);
            gameObject.SetActive(false);
        }
        
        private void OnButtonClicked()
        {
            ButtonClicked?.Invoke(_currentMission, this);
        }

        protected abstract void OnActivate(IReadOnlyMission mission);
    }
}