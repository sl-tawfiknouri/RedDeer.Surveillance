using Surveillance;
using System;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.App
{
    public class MediatorBootstrapper : IStartUpTaskRunner
    {
        private IMediator _mediator;

        public MediatorBootstrapper(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Run()
        {
            await Task.Run(() => 
            {
                // trades on 69, stocks on 70 [ports]
                _mediator.Initiate();
            });
        }
    }
}
