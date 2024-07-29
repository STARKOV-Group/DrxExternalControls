using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using starkov.Common.TaskForApproval;

namespace starkov.Common.Server.TaskForApprovalBlocks
{
  partial class PlaceStampHandlers
  {

    public virtual void PlaceStampExecute()
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (!document.HasVersions)
        return;
      
      var signatures = Signatures
        .Get(document.LastVersion, _ => _.Where(s => _block.OnlyApproval.GetValueOrDefault() ?
                                                Equals(s.SignatureType, SignatureType.Approval) :
                                                !Equals(s.SignatureType, SignatureType.NotEndorsing)))
        .Distinct();
      if (!signatures.Any())
        return;
      
      var stamps = new List<Structures.Module.IStampInfo>();
      foreach (var signature in signatures)
      {
        var signer = signature.Signatory;
        var stampInfo = document.StampInfostarkov.FirstOrDefault(_ => Equals(_.Signer, signer)) ??
          document.StampInfostarkov.Where(_ => !Sungero.Company.Employees.Is(_.Signer))
          .FirstOrDefault(_ => Sungero.Company.PublicFunctions.Module.GetEmployeesFromRecipients(new List<IRecipient>() { _.Signer }).Contains(signer));
        if (stampInfo == null)
          continue;
        
        var stamp = Structures.Module.StampInfo.Create();
        stamp.PageNumber = stampInfo.PageNumber.GetValueOrDefault();
        stamp.X = stampInfo.CoordX.GetValueOrDefault();
        stamp.Y = stampInfo.CoordY.GetValueOrDefault();
        stamp.HtmlStamp = Sungero.Docflow.PublicFunctions.Module.GetSignatureMarkAsHtml(document, signature);
        stamps.Add(stamp);
      }
      
      using (var bodyStream = document.LastVersion.Body.Read())
        using (var pdfStream = Sungero.Docflow.IsolatedFunctions.PdfConverter.GeneratePdf(bodyStream, document.LastVersion.BodyAssociatedApplication.Extension))
          using (var publicBodyStream = Common.IsolatedFunctions.WorkWithAspose.AddStampsByCoords(pdfStream, stamps))
      {
        document.LastVersion.PublicBody.Write(publicBodyStream);
        document.LastVersion.AssociatedApplication = Sungero.Content.AssociatedApplications.GetByExtension("pdf");
        document.Save();
      }
    }
  }

}