using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.ExtControl.OfficialDocument;

namespace starkov.ExtControl.Client
{
  partial class OfficialDocumentActions
  {
    public virtual void ConvertToPDFstarkov(Sungero.Domain.Client.ExecuteActionArgs e)
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

    public virtual bool CanConvertToPDFstarkov(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && _obj.AccessRights.CanUpdate();
    }

    public virtual void AddStampstarkov(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("Добавление штампа");
      var pageNumField = dialog.AddSelect("Номер страницы", true).From(Enumerable.Range(1, Common.PublicFunctions.Module.GetDocumentPageCount(_obj.Id)).Select(_ => _.ToString()).ToArray());
      var recipientField = dialog.AddSelect("Подписывающий", true, Sungero.CoreEntities.Recipients.Null);
      if (dialog.Show() == DialogButtons.Ok)
        Functions.OfficialDocument.Remote.FillStampHtml(_obj, Int32.Parse(pageNumField.Value), recipientField.Value);
    }

    public virtual bool CanAddStampstarkov(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && _obj.AccessRights.CanUpdate();
    }

  }

}