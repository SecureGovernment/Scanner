using Moq;

namespace SecureGovernment.Domain.Tests
{
    public sealed class Utils
    {
        public static Mock<T> CreateMock<T>(params object[] args) where T : class
        {
            return new Mock<T>(MockBehavior.Strict, args);
        }

        public static Mock<T> CreateMockOfSelf<T>(params object[] args) where T : class
        {
            return new Mock<T>(args) { CallBase = true };
        }
    }
}
