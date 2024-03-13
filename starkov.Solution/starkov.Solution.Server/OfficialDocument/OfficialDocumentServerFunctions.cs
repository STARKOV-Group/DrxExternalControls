using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Solution.OfficialDocument;

namespace starkov.Solution.Server
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
    /// Преобразовать первую страницу документа в изображение.
    /// </summary>
    [Remote]
    public virtual void ConvertPageToImage()
    {
      var version = _obj.LastVersion;
      if (version == null)
        return;
      
      var stampInfo = _obj.StampInfostarkov.FirstOrDefault() ?? _obj.StampInfostarkov.AddNew();
      using (var bodyStream = version.Body.Read())
        using (var pdfStream = Sungero.Docflow.IsolatedFunctions.PdfConverter.GeneratePdf(bodyStream, version.BodyAssociatedApplication.Extension))
      {
        var convertResult = Common.IsolatedFunctions.WorkWithAspose.ConvertFirstPageToImage(pdfStream);
        stampInfo.FirstPageAsImage = convertResult.Base64Image;
        stampInfo.IsLandscape = convertResult.IsLandscape;
        stampInfo.CoordX = 0;
        stampInfo.CoordY = 0;
      }
      _obj.Save();
    }
    
    /// <summary>
    /// Заполнить данные о штампе в виде строки Html.
    /// </summary>
    [Remote]
    public virtual void FillStampHtml()
    {
      var stampInfo = _obj.StampInfostarkov.FirstOrDefault() ?? _obj.StampInfostarkov.AddNew();
      var stamp = Sungero.Docflow.PublicFunctions.Module.GetSignatureMarkAsHtml(_obj, _obj.LastVersion?.Id ?? 0);
      if (stampInfo.StampHtml != stamp)
      {
        stampInfo.StampHtml = stamp;
        _obj.Save();
      }
    }

  }
}