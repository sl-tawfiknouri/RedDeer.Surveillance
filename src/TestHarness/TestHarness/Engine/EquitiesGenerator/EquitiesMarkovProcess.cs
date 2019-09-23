namespace TestHarness.Engine.EquitiesGenerator
{
    using System;
    using System.Linq;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;
    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.EquitiesGenerator.Interfaces;
    using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;
    using TestHarness.Engine.Heartbeat.Interfaces;

    /// <summary>
    ///     Apply a random walk to securities in your stream
    ///     Not multi thread safe, only use with transitory life style
    /// </summary>
    public class EquitiesMarkovProcess : IEquityDataGenerator
    {
        private readonly IEquityDataGeneratorStrategy _dataStrategy;

        private readonly IExchangeSeriesInitialiser _exchangeTickInitialiser;

        private readonly IHeartbeat _heartBeat;

        private readonly ILogger _logger;

        private readonly object _stateTransitionLock = new object();

        private readonly object _walkingLock = new object();

        private EquityIntraDayTimeBarCollection _activeFrame;

        private IStockExchangeStream _stream;

        private volatile bool _tickLocked;

        private volatile bool _walkInitiated;

        public EquitiesMarkovProcess(
            IExchangeSeriesInitialiser exchangeTickInitialiser,
            IEquityDataGeneratorStrategy dataStrategy,
            IHeartbeat heartbeat,
            ILogger logger)
        {
            this._exchangeTickInitialiser = exchangeTickInitialiser
                                            ?? throw new ArgumentNullException(nameof(exchangeTickInitialiser));

            this._dataStrategy = dataStrategy ?? throw new ArgumentNullException(nameof(dataStrategy));

            this._heartBeat = heartbeat ?? throw new ArgumentNullException(nameof(heartbeat));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void InitiateWalk(IStockExchangeStream stream)
        {
            this._logger.LogInformation("Walk initiated in equity generator");

            if (stream == null) throw new ArgumentNullException(nameof(stream));

            lock (this._stateTransitionLock)
            {
                this._walkInitiated = true;

                this._stream = stream;
                this._activeFrame = this._exchangeTickInitialiser.InitialFrame();
                this._stream.Add(this._activeFrame);

                this._heartBeat.OnBeat(this.Tick);
                this._heartBeat.Start();
            }
        }

        public void TerminateWalk()
        {
            lock (this._stateTransitionLock)
            {
                this._walkInitiated = false;

                this._heartBeat.Stop();
                this._heartBeat.Dispose();

                this._logger.LogInformation("Random walk generator terminating walk");
            }
        }

        private void Tick(object sender, EventArgs e)
        {
            if (this._tickLocked)
            {
                // don't tick if we're getting overwhelmed
                this._logger.LogInformation("Ticks tocking too fast for equity generator");
                return;
            }

            lock (this._walkingLock)
            {
                this._tickLocked = true;

                if (!this._walkInitiated)
                {
                    this._heartBeat?.Stop();
                    this._tickLocked = false;
                    return;
                }

                var tockedSecurities = this._activeFrame.Securities.Select(this.TickSecurity).ToArray();

                var tickTock = new EquityIntraDayTimeBarCollection(
                    this._activeFrame.Exchange,
                    DateTime.UtcNow,
                    tockedSecurities);
                this._activeFrame = tickTock;

                this._stream.Add(tickTock);

                this._tickLocked = false;
            }
        }

        private EquityInstrumentIntraDayTimeBar TickSecurity(EquityInstrumentIntraDayTimeBar tick)
        {
            return this._dataStrategy.AdvanceFrame(tick, DateTime.UtcNow, true);
        }
    }
}