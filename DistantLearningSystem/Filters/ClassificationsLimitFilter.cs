// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassificationsLimitFilter.cs" company="NURE">
//   DistantLearningSystem.Filters.ClassificationLimitFilter class definition.
// </copyright>
// <summary>
//   The connection limit filter. Checks for student added concepts overflow.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DistantLearningSystem.Filters
{
    using System.Web.Mvc;

    using DistantLearningSystem.Models.LogicModels.Managers;

    /// <summary>
    /// The connection limit filter. Checks for student added concepts overflow.
    /// </summary>
    public class ClassificationLimitFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="filterContext">
        /// The filter context.
        /// </param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        /// <summary>
        /// Checks for student added concepts overflow.
        /// </summary>
        /// <param name="filterContext">
        /// The filter context.
        /// </param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var studentId = DataManager.DefineUser(filterContext.HttpContext).Id;
            if (DataManager.Student.CanAddClassification(studentId))
            {
                return;
            }

            var result = new JsonResult
            {
                Data = new { can = false },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            filterContext.Result = result;
        }
    }
}