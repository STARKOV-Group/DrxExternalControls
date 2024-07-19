using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.ExtControl.OfficialDocument;

namespace starkov.ExtControl.Server
{
  partial class OfficialDocumentFunctions
  {

    /// <summary>
    /// Преобразовать документ в pdf с проставлением штампа по координатам.
    /// </summary>
    [Remote]
    public virtual void PlaceStampByCoords()
    {
      var version = _obj.LastVersion;
      var stampInfo = _obj.StampInfostarkov.FirstOrDefault();
      if (version == null || stampInfo == null)
        return;
      
      using (var bodyStream = version.Body.Read())
        using (var pdfStream = Sungero.Docflow.IsolatedFunctions.PdfConverter.GeneratePdf(bodyStream, version.BodyAssociatedApplication.Extension))
          using (var publicBodyStream = Common.IsolatedFunctions.WorkWithAspose.AddStampByCoords(pdfStream,
                                                                                                 stampInfo.StampHtml,
                                                                                                 1,
                                                                                                 stampInfo.CoordX.GetValueOrDefault(),
                                                                                                 stampInfo.CoordY.GetValueOrDefault()))
      {
        version.PublicBody.Write(publicBodyStream);
        version.AssociatedApplication = Sungero.Content.AssociatedApplications.GetByExtension("pdf");
        _obj.Save();
      }
    }
    
    /// <summary>
    /// Заполнить данные о штампе в виде строки Html.
    /// </summary>
    [Remote]
    public virtual void FillStampHtml()
    {
      var stampInfo = _obj.StampInfostarkov.AddNew();
      var stampParams = Sungero.Docflow.PublicFunctions.Module.GetDefaultSignatureStampParams(false);
      var stamp = Sungero.Docflow.Resources.HtmlStampTemplateForSignature.ToString();
      stamp = stamp.Replace("{SignatoryFullName}", "Подписывающий");
      stamp = stamp.Replace("{SignatoryId}", "1");
      stamp = stamp.Replace("{Logo}", stampParams.Logo);
      stamp = stamp.Replace("{SigningDate}", Calendar.Today.ToShortDateString());
      stamp = stamp.Replace("{Title}", stampParams.Title);
      
      stampInfo.PageNumber = 1;
      stampInfo.CoordX = 0;
      stampInfo.CoordY = 0;
      stampInfo.StampHtml = stamp;
    }

  }
}