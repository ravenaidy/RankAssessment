using MediatR;
using RES.ATM.API.Domain.ATM.Commands;
using RES.ATM.API.Domain.ATM.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RES.ATM.API.Domain.ATM.Handlers
{
    public class WithdrawalHandler : AsyncRequestHandler<WithdrawalCommand>
    {
        private readonly IATMService _atmService;

        public WithdrawalHandler(IATMService atmService)
        {
            _atmService = atmService ?? throw new ArgumentException(nameof(atmService));
        }        

        protected override Task Handle(WithdrawalCommand request, CancellationToken cancellationToken)
        {
            _atmService.ATMAmount -= request.WithdrawalAmount;
            return Task.CompletedTask;
        }
    }
}
