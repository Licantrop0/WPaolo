using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WizardMario
{
    public interface IWizardMarioElement
    {
        ElementColor Color { get; }

        bool Floating { get; set; }

        void Destroy();
    }
}
