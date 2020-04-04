using System;
using System.Threading.Tasks;
using Akka.Actor;
using PaymentsProcessor.ExternalSystems;
using PaymentsProcessor.Messages;

namespace PaymentsProcessor.Actors
{
    internal class PaymentWorkerActor : ReceiveActor
    {
        private readonly IPaymentGateway _paymentGateway;

        public PaymentWorkerActor(IPaymentGateway paymentGateway)
        {
            _paymentGateway = paymentGateway;

            Receive<SendPaymentMessage>(message => HandleSendPayment(message));
            Receive<PaymentReceipt>(message => HandlePaymentReceipt(message));
        }

        private void HandlePaymentReceipt(PaymentReceipt message)
        {
            Sender.Tell(new PaymentSentMessage(message.AccountNumber, message.PaymentConfirmationReceipt));
        }

        //private async void HandleSendPayment(SendPaymentMessage message)
        //{
        //    var sender = Sender;

        //    var result = await _paymentGateway.Pay(message.AccountNumber, message.Amount);

        //    sender.Tell(new PaymentSentMessage(result.AccountNumber, result.PaymentConfirmationReceipt));
        //}

        private void HandleSendPayment(SendPaymentMessage message)
        {
            _paymentGateway.Pay(message.AccountNumber, message.Amount).PipeTo(Self, Sender);

        }
    }
}