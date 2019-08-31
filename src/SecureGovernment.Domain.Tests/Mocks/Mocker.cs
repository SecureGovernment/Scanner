using Moq;

namespace SecureGovernment.Domain.Tests.Mocks
{
    public static class Mocker
    {
        public static Mock<T> CreateMock<T>(params object[] constructor) where T : class
        {
            var mock = new Mock<T>(MockBehavior.Strict, constructor);
            mock.SetupAllProperties();

            return mock;
        }
    }
}
