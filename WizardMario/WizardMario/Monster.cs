using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WizardMario
{
    public class Monster : IWizardMarioElement
    {
        ElementColor _color;

        bool _floating = false;

        public ElementColor Color
        {
            get { return _color; }
        }

        public bool Floating
        {
            get { return _floating; }

            set { throw new InvalidProgramException(); }
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
