﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.ModificationRequestModel.DeleteRequestModel
{
    public interface IDeleteRequestService
    {
        IEnumerable<DeleteRequest> GetAll();
        DeleteRequest Add(DeleteRequest request);
    }
}
