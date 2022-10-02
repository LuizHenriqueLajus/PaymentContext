using Flunt.Notifications;
using PaymentContext.Enums;
using PaymentContext.PaymentContext.Domain.Commands;
using PaymentContext.PaymentContext.Domain.Entities;
using PaymentContext.PaymentContext.Domain.Repositories;
using PaymentContext.PaymentContext.Domain.Services;
using PaymentContext.PaymentContext.Shared.Commands;
using PaymentContext.PaymentContext.Shared.Handlers;
using PaymentContext.ValueObjects;
using System;

namespace PaymentContext.PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler : Notifiable, IHandler<CreateBoletoSubscriptionCommand>, IHandler<CreatePayPalSubscriptionCommand>
    {
        private readonly IStudentRepository _repository;
        private readonly IEmailService _emailService;

        public SubscriptionHandler(IStudentRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            //Fail Fast Validations
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possível realizar a sua assinatura");
            }

            //Verificar se Documento já esta cadastrado
            if (_repository.DocumentExists(command.Document))
                AddNotification("Document", "Este CPF já esta em uso");

            //Verificar se o E-mail já esta cadastrado
            if (_repository.DocumentExists(command.Email))
                AddNotification("Email", "Este E-mail já esta em uso");

            //Gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            //Gerar as Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(command.BarCode,
                command.BoletoNumber,
                command.PaidDate,
                command.ExpireDate,
                command.Total,
                command.TotalPaid,
                command.Payer,
                new Document(command.PayerDocument, command.PayerDocumentType),
                address,
                email
                );

            //Relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            //Agrupar as Validações
            AddNotifications(name, document, email, address, student, subscription, payment);

            //Salvar as Informações
            _repository.CreateSubscription(student);

            //Enivar e-mail de boas vindas
            _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo a nossa plataforma", "Sua assinatura foi criada");

            //Retornar informações

            return new CommandResult(true, "Assinatura realizada com sucesso!");
        }

        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {
            //Verificar se Documento já esta cadastrado
            if (_repository.DocumentExists(command.Document))
                AddNotification("Document", "Este CPF já esta em uso");

            //Verificar se o E-mail já esta cadastrado
            if (_repository.DocumentExists(command.Email))
                AddNotification("Email", "Este E-mail já esta em uso");

            //Gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            //Gerar as Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new PayPalPayment(command.TransactionCode,
                command.PaidDate,
                command.ExpireDate,
                command.Total,
                command.TotalPaid,
                command.Payer,
                new Document(command.PayerDocument, command.PayerDocumentType),
                address,
                email
                );

            //Relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            //Agrupar as Validações
            AddNotifications(name, document, email, address, student, subscription, payment);

            //Checar as Notificações
            if (Invalid)
                return new CommandResult(false, "Não foi possível realizar sua assinatura");

            //Salvar as Informações
            _repository.CreateSubscription(student);

            //Enivar e-mail de boas vindas
            _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo a nossa plataforma", "Sua assinatura foi criada");

            //Retornar informações

            return new CommandResult(true, "Assinatura realizada com sucesso!");
        }
    }
}
