// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefinitionLimitFilter.cs" company="NURE">
//   BD
// </copyright>
// <summary>
//   Defines the ConnectionLimitFilter type. Checks for student added concepts overflow.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DistantLearningSystem.Filters
{
    using System.Web.Mvc;

    using DistantLearningSystem.Models.LogicModels.Managers;

    /// <summary>
    /// The connection limit filter. Checks for student added concepts overflow.
    /// </summary>
    public class DefinitionLimitFilter : ActionFilterAttribute
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
            var student = DataManager.Student.GetStudent(studentId);
            var limit = DataManager.Student.StudentActionLimits["Definition"];
            if (student.Definitions.Count <= limit)
            {
                return;
            }

            filterContext.Result = new EmptyResult();
        }
    }
}