namespace DistantLearningSystem.Models.LogicModels.Services
{
    public interface ISender
    {
        bool Send(string topic, string text, string userMail);
    }
}