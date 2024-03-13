using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sungero.Core;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using starkov.Common.Structures.Module;

namespace starkov.Common.Isolated.WorkWithAspose
{
  public class IsolatedFunctions
  {

    /// <summary>
    /// Преобразование первой страницы документа в изображение.
    /// </summary>
    /// <param name="documentStream">Поток документа.</param>
    /// <returns>Изображение в формате base64.</returns>
    [Public]
    public virtual IPageInfo ConvertFirstPageToImage(Stream documentStream)
    {
      try
      {
        var result = PageInfo.Create();
        byte[] bytes;
        
        using (var memoryStream = new MemoryStream())
        {
          var document = new Aspose.Pdf.Document(documentStream);
          var info = new Aspose.Pdf.Facades.PdfFileInfo(document);
          var page = document.Pages.FirstOrDefault(p => p.Number == 1);
          var pageWidth = Convert.ToInt32(info.GetPageWidth(page.Number));
          var pageHeight = Convert.ToInt32(info.GetPageHeight(page.Number));
          var pngDevice = new Aspose.Pdf.Devices.PngDevice();
          pngDevice.Process(page, memoryStream);
          memoryStream.Position = 0;
          bytes = memoryStream.ToArray();
          result.IsLandscape = pageWidth > pageHeight;
        }
        
        result.Base64Image = Convert.ToBase64String(bytes);
        return result;
      }
      catch (Exception ex)
      {
        Logger.Error("ConvertFirstPageToImage", ex);
        throw new AppliedCodeException(ex.Message);
      }
      finally
      {
        documentStream.Dispose();
      }
    }
    
    /// <summary>
    /// Проставить штамп на заданную страницу документа.
    /// </summary>
    /// <param name="inputStream">Поток с входным документом.</param>
    /// <param name="htmlStamp">Строка, содержащая штамп в виде html.</param>
    /// <param name="pageNumber">Страница, на которой необходимо проставить отметку.</param>
    /// <param name="x">Координата x.</param>
    /// <param name="y">Координата y.</param>
    /// <returns>Поток с документом.</returns>
    [Public]
    public virtual Stream AddStampByCoords(Stream inputStream, string htmlStamp, int pageNumber, double x, double y)
    {
      try
      {
        var document = new Aspose.Pdf.Document(inputStream);
        var info = new Aspose.Pdf.Facades.PdfFileInfo(document);
        var stamp = CreateStampFromHtml(htmlStamp);
        var page = document.Pages[pageNumber];
        stamp.XIndent = x;
        stamp.YIndent = info.GetPageHeight(page.Number) - y - stamp.Height;
        page.Dispose();
        return AddStampToDocumentPage(inputStream, pageNumber, stamp);
      }
      catch (Exception ex)
      {
        Logger.Error("Cannot add stamp by coords", ex);
        throw new AppliedCodeException("Cannot add stamp by coords");
      }
    }
    
    /// <summary>
    /// Создать штамп из шаблона html для вставки в pdf.
    /// </summary>
    /// <param name="html">Шаблон штампа в html.</param>
    /// <returns>Документ pdf со штампом.</returns>
    public virtual Aspose.Pdf.PdfPageStamp CreateStampFromHtml(string html)
    {
      try
      {
        Aspose.Pdf.HtmlLoadOptions objLoadOptions = new Aspose.Pdf.HtmlLoadOptions();
        objLoadOptions.PageInfo.Margin = new Aspose.Pdf.MarginInfo(0, 0, 0, 0);
        Aspose.Pdf.Document stampDoc;
        using (var htmlStamp = new MemoryStream(Encoding.UTF8.GetBytes(html)))
          stampDoc = new Aspose.Pdf.Document(htmlStamp, objLoadOptions);
        var firstPage = stampDoc.Pages[1];
        var contentBox = firstPage.CalculateContentBBox();
        objLoadOptions.PageInfo.Width = contentBox.Width;
        objLoadOptions.PageInfo.Height = contentBox.Height;
        using (var htmlStamp = new MemoryStream(Encoding.UTF8.GetBytes(html)))
          stampDoc = new Aspose.Pdf.Document(htmlStamp, objLoadOptions);
        if (stampDoc.Pages.Count > 0)
        {
          var mark = new Aspose.Pdf.PdfPageStamp(stampDoc.Pages[1]);
          mark.Background = false;
          return mark;
        }
        return null;
      }
      catch (Exception ex)
      {
        Logger.Error("Cannot create stamp from html", ex);
        throw new AppliedCodeException("Cannot create stamp from html");
      }
    }
    
    /// <summary>
    /// Добавить отметку на страницу документа.
    /// </summary>
    /// <param name="inputStream">Поток с входным документом.</param>
    /// <param name="pageNumber">Номер страницы документа, на которую нужно проставить отметку.</param>
    /// <param name="stamp">Отметка.</param>
    /// <returns>Страница документа с отметкой.</returns>
    public virtual Stream AddStampToDocumentPage(Stream inputStream, int pageNumber, Aspose.Pdf.PdfPageStamp stamp)
    {
      // Создание нового потока, в который будет записан документ с отметкой (во входной поток записывать нельзя).
      var outputStream = new MemoryStream();
      try
      {
        var document = new Aspose.Pdf.Document(inputStream);
        // Поднимаем версию и переполучаем документ из потока,
        // чтобы гарантировать читаемость штампа после вставки.
        using (var documentStream = GetUpgradedPdf(document))
        {
          document = new Aspose.Pdf.Document(documentStream);

          var documentPage = document.Pages[pageNumber];
          var rectConsiderRotation = documentPage.GetPageRect(true);
          if (stamp.Width > rectConsiderRotation.Width || stamp.Width > (rectConsiderRotation.Height - 20))
          {
            inputStream.CopyTo(outputStream);
          }
          else
          {
            documentPage.AddStamp(stamp);
            document.Save(outputStream);
          }
        }
        
        return outputStream;
      }
      catch (Exception ex)
      {
        outputStream.Dispose();
        Logger.Error("Cannot add stamp to document page", ex);
        throw new AppliedCodeException("Cannot add stamp to document page");
      }
      finally
      {
        inputStream.Close();
      }
    }

    /// <summary>
    /// Для документов версии ниже 1.4 поднять версию до 1.4 перед вставкой отметки.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>PDF документ, сконвертированный до версии 1.4, или исходный, если версию поднимать не требовалось.</returns>
    /// <remarks>При вставке отметки в pdf версии ниже, чем 1.4, портятся шрифты в документе.
    /// В Adobe Reader такие документы либо не открываются совсем, либо отображаются некорректно.
    /// Для корректного отображения отметки pdf-документ будет сконвертирован до версии pdf 1.4.
    /// Документы в формате pdf/a не конвертируем, т.к. формат основан на версии pdf 1.4 и не требует конвертации.</remarks>
    public Stream GetUpgradedPdf(Aspose.Pdf.Document document)
    {
      if (!document.IsPdfaCompliant)
      {
        // Получить версию стандарта PDF из свойств документа. Достаточно первых двух чисел, разделённых точкой.
        var versionRegex = new Regex(@"^\d{1,2}\.\d{1,2}");
        var pdfVersionAsString = versionRegex.Match(document.Version).Value;
        var minCompatibleVersion = Version.Parse("1.4");

        if (Version.TryParse(pdfVersionAsString, out Version version) && version < minCompatibleVersion)
        {
          Logger.DebugFormat("GetUpgradedPdf. Convert Pdf version to 1.4. Current version is {0}", version);
          using (var convertLog = new MemoryStream())
          {
            var options = new Aspose.Pdf.PdfFormatConversionOptions(Aspose.Pdf.PdfFormat.v_1_4);
            options.LogStream = convertLog;
            document.Convert(options);
          }
        }
      }
      // Необходимо пересохранить документ в поток, чтобы изменение версии применилось до простановки отметки, а не после.
      var docStream = new MemoryStream();
      document.Save(docStream);
      return docStream;
    }
  }
}