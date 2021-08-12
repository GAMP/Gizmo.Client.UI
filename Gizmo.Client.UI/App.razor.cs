using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Gizmo.Client.UI
{
    public partial class App : ComponentBase
    {
        #region CONSTRUTCOR
        public App() : base()
        {
        }
        #endregion
    }


    public sealed class DiscoveryService
    {
        //public DiscoveryService(IConfiguration configuration)
        //{

        //}

        #region FIELDS
        private IEnumerable<Assembly> _addtionalAssemblies = new HashSet<Assembly>();
        private Assembly _appAssembly;
        #endregion

        #region PROPERTIES

        public IEnumerable<Assembly> AdditionalAsseblies
        {
            get { return _addtionalAssemblies; }
        }

        public Assembly AppAssembly
        {
            get { return _appAssembly; }
        }
        
        #endregion
    }
}
