using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HeroInfrastructure
{
    public class HeroView : MonoBehaviour, IPointerDownHandler
    {
        public event Action<IReadOnlyHero> Clicked;

        [SerializeField]
        private TextMeshProUGUI _pointsDisplay;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TextMeshProUGUI _name;
        [Header("Effects")]
        [SerializeField]
        private GameObject _selectionHighlight;
        [SerializeField]
        private float _selectionOffset;
        [SerializeField]
        private float _offsetDuration;

        public IReadOnlyHero Model { get; private set; }

        public void Init(IReadOnlyHero model)
        {
            Model = model;
            Model.StatusChanged += OnHeroChangedStatus;
            Model.PointsChanged += OnPointsChanged;

            _pointsDisplay.text = Model.Points.ToString();
            _image.sprite = Model.BaseData.Avatar;
            _name.text = Model.BaseData.Name;

            OnHeroChangedStatus(model);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Clicked?.Invoke(Model);
        }

        private void OnPointsChanged(int points)
        {
            _pointsDisplay.text = points.ToString();
        }

        private void OnHeroChangedStatus(IReadOnlyHero hero)
        {
            switch (hero.Status)
            {
                case HeroStatus.Unlocked:
                    DisableHighlight();
                    Enable();
                    break;
                case HeroStatus.Locked:
                    Disable();
                    break;
                case HeroStatus.Selected:
                    EnableHighlight();
                    break;
            }
        }

        private void EnableHighlight()
        {
            _selectionHighlight.SetActive(true);
            transform.DOComplete();
            transform.DOMoveY(transform.position.y + _selectionOffset, _offsetDuration).SetEase(Ease.Flash);
        }

        private void DisableHighlight()
        {
            if (_selectionHighlight.activeSelf)
            {
                transform.DOComplete();
                transform.DOMoveY(transform.position.y - _selectionOffset, _offsetDuration).SetEase(Ease.Flash);
                _selectionHighlight.SetActive(false);
            }
            else
            {
                _selectionHighlight.SetActive(false);
            }
        }

        private void Enable()
        {
            OnPointsChanged(Model.Points);
            gameObject.SetActive(true);
        }

        private void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}