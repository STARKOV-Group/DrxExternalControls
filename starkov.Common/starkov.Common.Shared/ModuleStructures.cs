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
    public byte[] Image { get; set; }
    public bool IsLandscape { get; set; }
  }
  
  [Public(Isolated=true)]
  partial class StampInfo
  {
    public string HtmlStamp { get; set; }
    public int PageNumber { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
  }
}