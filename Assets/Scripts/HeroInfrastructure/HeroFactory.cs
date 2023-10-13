using UnityEngine;

namespace HeroInfrastructure
{
    public class HeroFactory
    {
        private readonly HeroView _viewPrefab;

        public HeroFactory(HeroView viewPrefab)
        {
            _viewPrefab = viewPrefab;
        }
        
        public (Hero Model, HeroView View) Create(HeroBaseData data)
        {
            var model = new Hero(data);
            var view = CreateView(model);
            return (model, view);
        }

        public HeroView CreateView(Hero hero)
        {
            var view = Object.Instantiate(_viewPrefab);
            view.Init(hero);
            return view;
        }
    }
}