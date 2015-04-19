using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace BuildingBlocks.Data.NHibernate
{
    public interface ISessionResolver
    {
        ISession GetCurrentSessionFor(Type type);
    }
}
