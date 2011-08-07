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

    public abstract class Mouth : CircularSprite
    {
        #region static members

        static public GameEventsManager GameManager { set; get; }
        static public MonsterFactory MonsterFactory { set; get; }

        #endregion

        #region protected members parameters

        protected float _openingTime;  
        protected float _mouthOpenTime; 
        protected float _globulosSpeed;

        #endregion

        #region protected members state

        protected MouthState _state;
        protected int _hitPoints;

        #endregion

        #region properties

        public MouthState State
        { get { return _state; } }

        #endregion

        #region costructors

        public Mouth(Dictionary<string, Animation> animations, float radius, float touchRadius)
            :base(animations, radius, touchRadius)
        {

        }

        #endregion

        #region virtual methods

        protected abstract void FireGlobulo(TimeSpan t);

        #endregion

        #region physics initialization

        protected override void InitializePhysics()
        {
            _physicalPoint = new PhysicalMassSystemPoint();
        }

        #endregion
    }
}
