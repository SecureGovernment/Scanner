using Moq;

namespace SecureGovernment.Domain.Tests
{
    public sealed class Utils
    {
        public static Mock<T> CreateMock<T>() where T : class
        {
            return new Mock<T>(MockBehavior.Strict);
        }
    }
}
