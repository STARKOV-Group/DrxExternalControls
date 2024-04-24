using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.ExtControl.OfficialDocument;

namespace starkov.ExtControl
{
  partial class OfficialDocumentStampInfostarkovSharedCollectionHandlers
  {

    public virtual void StampInfostarkovAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      var stampParams = Sungero.Docflow.PublicFunctions.Module.GetDefaultSignatureStampParams(false);
      var stamp = Sungero.Docflow.Resources.HtmlStampTemplateForSignature.ToString();
      stamp = stamp.Replace("{SignatoryFullName}", "Подписывающий");
      stamp = stamp.Replace("{SignatoryId}", _added.Id.ToString());
      stamp = stamp.Replace("{Logo}", stampParams.Logo);
      stamp = stamp.Replace("{SigningDate}", Calendar.Today.ToShortDateString());
      stamp = stamp.Replace("{Title}", stampParams.Title);
      _added.StampHtml = stamp;
    }
  }


  partial class OfficialDocumentSharedHandlers
  {

  }
}