using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.PaymentContext.Domain.Commands;

namespace PaymentContext.PaymentContext.Tests.Commands
{
    public class CreateBoletoSubscriptionCommandTests
    {
        // Red, Green, Refactor

        [TestMethod]
        public void ShouldReturnErrorWhenNameIsInvalid()
        {
            var command = new CreateBoletoSubscriptionCommand();
            command.FirstName = "";

            command.Validate();
            Assert.AreEqual(false, command.Valid);
        }

    }
}
