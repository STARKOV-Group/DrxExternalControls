using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace starkov.Common.Structures.Module
{

  [Public(Isolated=true)]
  partial class PageInfo
  {
    public string Image { get; set; }
    public bool IsLandscape { get; set; }
  }
}