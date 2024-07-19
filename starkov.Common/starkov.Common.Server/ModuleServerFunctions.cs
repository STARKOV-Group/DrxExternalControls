using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace starkov.Common.Server
{
  public class ModuleFunctions
  {
    
    /// <summary>
    /// Преобразование страницы документа в изображение.
    /// </summary>
    /// <param name="docId">ID документа.</param>
    /// <param name="pageNum">Номер страницы.</param>
    /// <returns>Json с информацией о странице документа.</returns>
    [Public(WebApiRequestType = RequestType.Get)]
    public virtual string GetDocumentPage(long docId, int pageNum)
    {
      var document = ExtControl.OfficialDocuments.GetAll(_ => _.Id == docId).FirstOrDefault();
      if (document == null)
        return string.Empty;
      
      var version = document.LastVersion;
      if (version == null)
        return string.Empty;
      
      var pageInfo = Structures.Module.PageInfo.Create();
      var stampInfo = document.StampInfostarkov.FirstOrDefault() ?? document.StampInfostarkov.AddNew();
      using (var bodyStream = version.Body.Read())
        using (var pdfStream = Sungero.Docflow.IsolatedFunctions.PdfConverter.GeneratePdf(bodyStream, version.BodyAssociatedApplication.Extension))
      {
        var data = Common.IsolatedFunctions.WorkWithAspose.ConvertPageToImage(pdfStream, pageNum);
        pageInfo.Image = data.Image;
        pageInfo.IsLandscape = data.IsLandscape;
      }
      
      return Common.IsolatedFunctions.Json.SerializePageStruct(pageInfo);
    }
    
    /// <summary>
    /// Получение общего кол-ва страниц.
    /// </summary>
    /// <param name="docId">ID документа.</param>
    /// <returns>Общее кол-во страниц.</returns>
    [Public(WebApiRequestType = RequestType.Get)]
    public virtual int GetDocumentPageCount(long docId)
    {
      var document = ExtControl.OfficialDocuments.GetAll(_ => _.Id == docId).FirstOrDefault();
      if (document == null)
        return 0;
      
      var version = document.LastVersion;
      if (version == null)
        return 0;
      
      using (var bodyStream = version.Body.Read())
        using (var pdfStream = Sungero.Docflow.IsolatedFunctions.PdfConverter.GeneratePdf(bodyStream, version.BodyAssociatedApplication.Extension))
      {
        return Common.IsolatedFunctions.WorkWithAspose.GetPagesCount(pdfStream);
      }
    }
  }
}