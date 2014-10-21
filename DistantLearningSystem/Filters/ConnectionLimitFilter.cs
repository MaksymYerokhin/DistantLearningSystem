// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionLimitFilter.cs" company="NURE">
//   BD
// </copyright>
// <summary>
//   Defines the ConnectionLimitFilter type. Checks for student added connections overflow.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DistantLearningSystem.Filters
{
    using System.Web.Mvc;

    using DistantLearningSystem.Models.LogicModels.Managers;

    /// <summary>
    /// The connection limit filter. Checks for student added connections overflow.
    /// </summary>
    public class ConnectionLimitFilter : ActionFilterAttribute
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
        /// Checks for student added connections overflow.
        /// </summary>
        /// <param name="filterContext">
        /// The filter context.
        /// </param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var studentId = DataManager.DefineUser(filterContext.HttpContext).Id;
            if (DataManager.Student.CanAddConnection(studentId))
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