using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistantLearningSystem.Models.DataModels
{
    //Будет служить для выставления оценки
    public enum MarkStatus
    {
        CheckedOk,
        CheckedNormal,
        CheckedMiddle,
        CheckedLow,
        CheckedWrong
    }
}