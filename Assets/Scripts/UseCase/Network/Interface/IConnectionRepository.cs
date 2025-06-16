using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Network;

namespace UseCase.Network
{
    public interface IConnectionRepository
    {
        void Save(PlayerSession session);
    }
}
