namespace DistantLearningSystem.Models.LogicModels.Services
{
    public interface ISender
    {
        string Send(string topic, string text, string userMail);
    }
}