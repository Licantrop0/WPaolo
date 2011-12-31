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
        #region static members

        static public GameEventsManager GameManager { set; get; }
        static public MonsterGenerator MonsterFactory { set; get; }

        #endregion

        #region protected members parameters

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

        public Mouth(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
            :base(dynamicSystem, sprite, shape)
        {
            Touchable = true;
        }

        #endregion

        #region virtual methods

        protected abstract void FireGlobulo(TimeSpan t);

        #endregion

        public override bool Moving
        {
            get { return false; }
        }

        public override bool Died
        {
            get { return (_state == MouthState.died); }
        }
    }
}
