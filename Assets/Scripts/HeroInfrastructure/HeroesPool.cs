using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeroInfrastructure
{
    [Serializable]
    public class HeroesPool
    {
        public event Action<IReadOnlyHero> HeroClicked;
        [SerializeField]
        private Transform _viewsContainer;

        private List<(Hero Model, HeroView View)> _heroesPool;
        private HeroFactory _heroFactory;

        public void DistributePoints(IReadOnlyList<(Fraction fraction, int RewardValue)> rewards, int selectedReward)
        {
            foreach (var reward in rewards)
            {
                var hero = _heroesPool
                    .FirstOrDefault(heroView => heroView.Model.BaseData.Fraction == reward.fraction);
                if(hero == default)
                    throw new Exception("Hero not found");
                
                if (hero.Model.Status != HeroStatus.Locked || reward.RewardValue < 0)
                    hero.Model.AddPoints(reward.RewardValue);
            }

            foreach (var hero in _heroesPool)
            {
                if (hero.Model.Status != HeroStatus.Selected)
                    continue;
                hero.Model.AddPoints(selectedReward);
            }
        }

        public HeroesPool(List<HeroBaseData> allHeroes, HeroFactory heroFactory, Transform viewsContainer)
        {
            _heroFactory = heroFactory;
            _heroesPool = allHeroes.Select(hero => _heroFactory.Create(hero)).ToList();
            _viewsContainer = viewsContainer;
        }

        public void Init()
        {
            foreach (var heroView in _heroesPool.Select(hero => hero.View))
            {
                heroView.transform.SetParent(_viewsContainer);
                heroView.Clicked += hero => HeroClicked?.Invoke(hero);
            }
        }

        public void SelectHero(IReadOnlyHero hero)
        {
            var heroModel = _heroesPool.FirstOrDefault(h => h.Model == hero).Model;
            if (heroModel == default)
                throw new Exception("Hero not found");
            if (hero.Status == HeroStatus.Locked)
                throw new Exception("Try select locked hero");
            heroModel.ChangeStatus(HeroStatus.Selected);
        }

        public void DeselectHero(IReadOnlyHero hero)
        {
            var heroModel = _heroesPool.FirstOrDefault(h => h.Model == hero).Model;
            if (heroModel == default)
                throw new Exception("Hero not found");
            if (heroModel.Status != HeroStatus.Selected)
                throw new Exception("Try deselect not selected hero");
            heroModel.ChangeStatus(HeroStatus.Unlocked);
        }

        public void AddHero(HeroBaseData heroData)
        {
            var hero = _heroesPool.FirstOrDefault(hero => hero.Model.BaseData == heroData);
            if (hero == default)
                throw new Exception("Hero not found");
            if (hero.Model.Status != HeroStatus.Locked)
            {
                Debug.LogWarning("Hero already added");
                return;
            }

            hero.Model.ChangeStatus(HeroStatus.Unlocked);
        }

        public void RemoveHero(Hero hero)
        {
            var heroObject = _heroesPool.FirstOrDefault(view => view.Model == hero);
            if (heroObject == default)
                throw new Exception("Hero not found");

            heroObject.Model.ChangeStatus(HeroStatus.Locked);
        }
    }
}