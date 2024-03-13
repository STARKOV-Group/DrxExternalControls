using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Solution.OfficialDocument;

namespace starkov.Solution.Client
{
  partial class OfficialDocumentActions
  {
    public virtual void PlaceStampstarkov(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var version = _obj.LastVersion;
      if (version == null)
      {
        e.AddError("Документ не содержит версий");
        return;
      }
      var stampInfo = _obj.StampInfostarkov.FirstOrDefault();
      if (stampInfo == null)
      {
        e.AddError("Штамп не сформирован");
        return;
      }
      
      Functions.OfficialDocument.Remote.PlaceStampByCoords(_obj);
    }

    public virtual bool CanPlaceStampstarkov(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && _obj.AccessRights.CanUpdate();
    }

    public virtual void ConvertPageToImagestarkov(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var version = _obj.LastVersion;
      if (version == null)
      {
        e.AddError("Документ не содержит версий");
        return;
      }
      
      Functions.OfficialDocument.Remote.ConvertPageToImage(_obj);
    }

    public virtual bool CanConvertPageToImagestarkov(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && _obj.AccessRights.CanUpdate();
    }

    public virtual void ShowStampstarkov(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.OfficialDocument.Remote.FillStampHtml(_obj);
    }

    public virtual bool CanShowStampstarkov(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && _obj.AccessRights.CanUpdate();
    }

  }

}