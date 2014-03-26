using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace ReplicatedSite.Models
{
    public class WebPrincipal : IPrincipal
    {
        CustomerIdentity _identity;
        public WebPrincipal(CustomerIdentity identity)
        {
            _identity = identity;
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }
}