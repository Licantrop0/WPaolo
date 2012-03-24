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

        public Monster(ElementColor color)
        {
            _color = color;
        }

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
            Floating = true;   // oppure posso gestirlo con la cancellazione dal board...

            throw new NotImplementedException();
        }
    }
}
