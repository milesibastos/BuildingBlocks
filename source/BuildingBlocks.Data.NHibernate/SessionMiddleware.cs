using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using NHibernate;
using NHibernate.Context;

namespace BuildingBlocks.Data.NHibernate
{
    public class SessionMiddleware : OwinMiddleware
    {
        private readonly ISessionResolver _resolver;

        public SessionMiddleware(OwinMiddleware next, ISessionResolver resolver)
            : base(next)
        {
            _resolver = resolver;
        }

        public async override Task Invoke(IOwinContext context)
        {
            await Next.Invoke(context);

            foreach (var factory in _resolver.GetAllFactories())
            {
                if (!CurrentSessionContext.HasBind(factory))
                {
                    ISession session = CurrentSessionContext.Unbind(factory);

                    if (session.Transaction.IsActive)
                        session.Transaction.Rollback();

                    if (session != null)
                        session.Close();
                }
            }
        }
    }
}
