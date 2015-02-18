using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soccerama.Model
{
    public interface IShieldService
    {
        Task Save(IEnumerable<Shield> scudetti);
        Task<IEnumerable<Shield>> Load();
        Task<IEnumerable<Shield>> GetNew();
    }
}