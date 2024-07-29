using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace starkov.Common.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      GrantAccessRights();
    }
    
    /// <summary>
    /// Выдача прав на типы сущностей.
    /// </summary>
    public void GrantAccessRights()
    {
      var allUsers = Roles.AllUsers;
      
      TaskForApprovals.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      TaskForApprovals.AccessRights.Save();
    }
  }
}
