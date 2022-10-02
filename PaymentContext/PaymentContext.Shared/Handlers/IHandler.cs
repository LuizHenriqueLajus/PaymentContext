using PaymentContext.PaymentContext.Shared.Commands;

namespace PaymentContext.PaymentContext.Shared.Handlers
{
    public interface IHandler<T> where T : ICommand
    {
        ICommandResult Handle(T command);
    }
}
