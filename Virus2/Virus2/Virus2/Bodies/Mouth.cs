using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virus.Sprites
{
    public enum MouthState
    {
        idle,
        approching,
        preRotating,
        rotating,
        opening,
        mouthOpenBefore,
        mouthOpenAfter,
        closing,
        postRotating,
        leaving,
        dyingMouthClosing,
        dying,
        fading,
        died
    }

    public abstract class Mouth : Enemy
    {
        #region protected members parameters

        protected Sprite _whiteGlobuloSpritePrototype;
        protected List<Enemy> _enemies;  // reference to enemies list

        protected float _openingTime;  
        protected float _mouthOpenTime; 
        protected float _globulosSpeed;

        #endregion

        public MouthState State 
        {
            get { return _state; }
        }

        #region protected members state

        protected MouthState _state;
        protected int _hitPoints;

        #endregion

        #region costructors

        public Mouth(DynamicSystem dynamicSystem, Sprite sprite, Shape shape,
                     Sprite whiteGlobuloSpritePrototype, List<Enemy> enemies)
            :base(dynamicSystem, sprite, shape)
        {
            _whiteGlobuloSpritePrototype = whiteGlobuloSpritePrototype;
            _enemies = enemies;

            Touchable = true;
        }

        #endregion

        #region virtual methods

        protected abstract void FireGlobulo();

        #endregion

        public override bool Moving
        {
            get { return false; }
        }

        public override bool Died
        {
            get { return (_state == MouthState.died); }
        }

        public override bool ToBeCleared
        {
            get { return (_state == MouthState.died); }
        }
    }
}
