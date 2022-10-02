using Flunt.Validations;
using PaymentContext.PaymentContext.Shared.Entities;
using PaymentContext.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace PaymentContext.PaymentContext.Domain.Entities
{
    public class Student : Entity
    {
        private IList<string> Notifications;
        private IList<Subscription> _subscriptions;
        public Student(Name name, Document document, Email email)
        {
            Name = name;
            Document = document;
            Email = email;
            _subscriptions = new List<Subscription>();

            AddNotifications(name, document, email);
        }

        public Name Name { get; private set; }
        public Document Document { get; private set; }
        public Email Email { get; private set; }
        public Address Address { get; private set; }
        public IReadOnlyCollection<Subscription> Subscriptions { get {return _subscriptions.ToArray();} }

        public void AddSubscription(Subscription subscription)
        {
            //Cancel all the subscriptions e put this one as principal.
            //foreach(var sub in Subscriptions)
            //{
            //    sub.Inactivate();
            //    _subscriptions.Add(subscription);
            //}

            var hasSubscriptionActive = false;
            foreach (var sub in _subscriptions)
            {
                if(sub.Active)
                    hasSubscriptionActive= true;
            }

            AddNotifications(new Contract()
                .Requires()
                .IsFalse(hasSubscriptionActive, "Student.Subscriptions", "Você já tem uma assinatura ativa")
                .AreEquals(0, subscription.Payments.Count, "Student.Subscription.Payments", "Esta assinatura não possui pagamentos")
                );

            //ALTERNATIVE
            //if (hasSubscriptionActive)
            //    AddNotification("Student.Subscriptions", "Você já tem uma assinatura ativa");
           
        }
    }
}
