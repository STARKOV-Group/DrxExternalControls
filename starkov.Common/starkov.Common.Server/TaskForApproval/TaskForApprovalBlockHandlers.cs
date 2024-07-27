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
//      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
//      var signers = Signatures.Get(document.LastVersion).Select(_ => _.Signatory).Distinct();
//      if (!signers.Any())
//        return;
//      
//      foreach (var stampInfo in document.StampInfostarkov)
//      {
//        var recipients = Sungero.Company.PublicFunctions.Module.GetEmployeesFromRecipients(new List<IRecipient>() { stampInfo.Signer });
//        if (recipients.Any
//        }
    }
  }

}