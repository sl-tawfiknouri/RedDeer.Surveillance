namespace Surveillance.Engine.Rules.Tests.Judgements
{
    using System;

    using Domain.Surveillance.Judgement.Equity.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Judgements;
    using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    [TestFixture]
    public class JudgementServiceTests
    {
        private ICancelledOrderJudgement _cancelledOrderJudgementContext;

        private IHighProfitJudgementContext _highProfitJudgementContext;

        private IHighProfitJudgementMapper _highProfitJudgementMapper;

        private IFixedIncomeHighProfitJudgementMapper fixedIncomeJudgementMapper;

        private IHighVolumeJudgement _highVolumeJudgementContext;

        private IJudgementRepository _judgementRepository;

        private ILayeringJudgement _layeringJudgementContext;

        private ILogger<JudgementService> _logger;

        private IMarkingTheCloseJudgement _markingTheCloseJudgementContext;

        private IPlacingOrdersWithNoIntentToExecuteJudgement _placingOrdersWithNoIntentJudgementContext;

        private IRampingJudgement _rampingJudgementContext;

        private IRuleViolationService _ruleViolationService;

        private ISpoofingJudgement _spoofingJudgementContext;

        [Test]
        public void Ctor_NullHighProfitJudgementMapper_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementService(
                    this._judgementRepository,
                    this._ruleViolationService, 
                    null,
                    this.fixedIncomeJudgementMapper,
                    this._logger));
        }

        [Test]
        public void Ctor_NullFixedIncomeHighProfitJudgementMapper_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementService(
                    this._judgementRepository,
                    this._ruleViolationService,
                    this._highProfitJudgementMapper,
                    null,
                    this._logger));
        }

        [Test]
        public void Ctor_NullJudgementRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementService(
                    null,
                    this._ruleViolationService,
                    this._highProfitJudgementMapper,
                    this.fixedIncomeJudgementMapper,
                    this._logger));
        }

        [Test]
        public void Ctor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementService(
                    this._judgementRepository,
                    this._ruleViolationService,
                    this._highProfitJudgementMapper,
                    this.fixedIncomeJudgementMapper,
                    null));
        }

        [Test]
        public void Ctor_NullRuleViolationService_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementService(
                    this._judgementRepository,
                    null,
                    this._highProfitJudgementMapper,
                    this.fixedIncomeJudgementMapper,
                    this._logger));
        }

        [Test]
        public void Judgement_CancelledOrderJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._cancelledOrderJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<ICancelledOrderJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_CancelledOrderJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._cancelledOrderJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<ICancelledOrderJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_HighProfitJudgement_NullDoesNotThrowException()
        {
            var service = this.BuildService();

            this._highProfitJudgementContext.Judgement.OrderId = null;

            service.Judgement(this._highProfitJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<IHighProfitJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this._highProfitJudgementMapper.Map(A<IHighProfitJudgementContext>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_HighProfitJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            this._highProfitJudgementContext.RaiseRuleViolation = true;
            this._highProfitJudgementContext.Judgement.OrderId = "test-order-id";

            service.Judgement(this._highProfitJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<IHighProfitJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._highProfitJudgementMapper.Map(A<IHighProfitJudgementContext>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_HighProfitJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            this._highProfitJudgementContext.RaiseRuleViolation = false;
            this._highProfitJudgementContext.Judgement.OrderId = "test-order-id";

            service.Judgement(this._highProfitJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<IHighProfitJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._highProfitJudgementMapper.Map(A<IHighProfitJudgementContext>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_HighVolumeJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._highVolumeJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<IHighVolumeJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_HighVolumeJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._highVolumeJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<IHighVolumeJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_LayeringJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._layeringJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<ILayeringJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_LayeringJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._layeringJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<ILayeringJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_MarkingTheCloseJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._markingTheCloseJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<IMarkingTheCloseJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_MarkingTheCloseJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._markingTheCloseJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<IMarkingTheCloseJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_NullCancelledOrderJudgement_NullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrow(() => service.Judgement((ICancelledOrderJudgement)null));

            A.CallTo(() => this._judgementRepository.Save(A<ICancelledOrderJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_NullHighProfitJudgement_NullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrow(() => service.Judgement((IHighProfitJudgementContext)null));

            A.CallTo(() => this._judgementRepository.Save(A<IHighProfitJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this._highProfitJudgementMapper.Map(A<IHighProfitJudgementContext>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_NullHighVolumeJudgement_NullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrow(() => service.Judgement((IHighVolumeJudgement)null));

            A.CallTo(() => this._judgementRepository.Save(A<IHighVolumeJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_NullLayeringJudgement_NullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrow(() => service.Judgement((ILayeringJudgement)null));

            A.CallTo(() => this._judgementRepository.Save(A<ILayeringJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_NullMarkingTheCloseJudgement_NullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrow(() => service.Judgement((IMarkingTheCloseJudgement)null));

            A.CallTo(() => this._judgementRepository.Save(A<ILayeringJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_NullPlacingOrdersWithNoIntentJudgement_NullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrow(() => service.Judgement((IPlacingOrdersWithNoIntentToExecuteJudgement)null));

            A.CallTo(() => this._judgementRepository.Save(A<IPlacingOrdersWithNoIntentToExecuteJudgement>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_NullRampingJudgement_NullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrow(() => service.Judgement((IRampingJudgement)null));

            A.CallTo(() => this._judgementRepository.Save(A<IRampingJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_NullSpoofingJudgement_NullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrow(() => service.Judgement((ISpoofingJudgement)null));

            A.CallTo(() => this._judgementRepository.Save(A<ISpoofingJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_PlacingOrdersWithNoIntentJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._placingOrdersWithNoIntentJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<IPlacingOrdersWithNoIntentToExecuteJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_PlacingOrdersWithNoIntentJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._placingOrdersWithNoIntentJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<IPlacingOrdersWithNoIntentToExecuteJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_RampingJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._rampingJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<IRampingJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_RampingJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._rampingJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<IRampingJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_SpoofingJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._spoofingJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<ISpoofingJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_SpoofingJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            service.Judgement(this._spoofingJudgementContext);

            A.CallTo(() => this._judgementRepository.Save(A<ISpoofingJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void PassJudgement_Calls_RuleViolationsRepository()
        {
            var service = this.BuildService();

            service.PassJudgement();

            A.CallTo(() => this._ruleViolationService.ProcessRuleViolationCache()).MustHaveHappenedOnceExactly();
        }

        private JudgementService BuildService()
        {
            return new JudgementService(
                this._judgementRepository,
                this._ruleViolationService,
                this._highProfitJudgementMapper,
                this.fixedIncomeJudgementMapper,
                this._logger);
        }

        [SetUp]
        public void Setup()
        {
            this._judgementRepository = A.Fake<IJudgementRepository>();
            this._highProfitJudgementContext = A.Fake<IHighProfitJudgementContext>();
            this._cancelledOrderJudgementContext = A.Fake<ICancelledOrderJudgement>();
            this._highVolumeJudgementContext = A.Fake<IHighVolumeJudgement>();
            this._layeringJudgementContext = A.Fake<ILayeringJudgement>();
            this._placingOrdersWithNoIntentJudgementContext = A.Fake<IPlacingOrdersWithNoIntentToExecuteJudgement>();
            this._rampingJudgementContext = A.Fake<IRampingJudgement>();
            this._markingTheCloseJudgementContext = A.Fake<IMarkingTheCloseJudgement>();
            this._spoofingJudgementContext = A.Fake<ISpoofingJudgement>();
            this._highProfitJudgementMapper = A.Fake<IHighProfitJudgementMapper>();
            this.fixedIncomeJudgementMapper = A.Fake<IFixedIncomeHighProfitJudgementMapper>();
            this._ruleViolationService = A.Fake<IRuleViolationService>();
            this._logger = A.Fake<ILogger<JudgementService>>();
        }
    }
}