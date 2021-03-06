﻿namespace Surveillance.Engine.Rules.Tests.Judgements
{
    using System;
    using System.Threading.Tasks;

    using Domain.Surveillance.Judgement.Equity.Interfaces;
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Judgements;
    using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    /// The judgement service tests.
    /// </summary>
    [TestFixture]
    public class JudgementServiceTests
    {
        /// <summary>
        /// The cancelled order judgement context.
        /// </summary>
        private ICancelledOrderJudgement cancelledOrderJudgementContext;

        /// <summary>
        /// The high profit judgement context.
        /// </summary>
        private IHighProfitJudgementContext highProfitJudgementContext;

        /// <summary>
        /// The high profit judgement mapper.
        /// </summary>
        private IHighProfitJudgementMapper highProfitJudgementMapper;

        /// <summary>
        /// The fixed income high profit judgement context.
        /// </summary>
        private IFixedIncomeHighProfitJudgementContext fixedIncomeProfitJudgementContext;

        /// <summary>
        /// The fixed income judgement mapper.
        /// </summary>
        private IFixedIncomeHighProfitJudgementMapper fixedIncomeProfitJudgementMapper;

        /// <summary>
        /// The high volume judgement context.
        /// </summary>
        private IHighVolumeJudgement highVolumeJudgementContext;

        /// <summary>
        /// The fixed income high volume judgement mapper.
        /// </summary>
        private IFixedIncomeHighVolumeJudgementMapper fixedIncomeHighVolumeJudgementMapper;

        /// <summary>
        /// The fixed income high volume judgement context.
        /// </summary>
        private IFixedIncomeHighVolumeJudgementContext fixedIncomeHighVolumeJudgementContext;

        /// <summary>
        /// The judgement repository.
        /// </summary>
        private IJudgementRepository judgementRepository;

        /// <summary>
        /// The layering judgement context.
        /// </summary>
        private ILayeringJudgement layeringJudgementContext;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<JudgementService> logger;

        /// <summary>
        /// The marking the close judgement context.
        /// </summary>
        private IMarkingTheCloseJudgement markingTheCloseJudgementContext;

        /// <summary>
        /// The placing orders with no intent judgement context.
        /// </summary>
        private IPlacingOrdersWithNoIntentToExecuteJudgement placingOrdersWithNoIntentJudgementContext;

        /// <summary>
        /// The ramping judgement context.
        /// </summary>
        private IRampingJudgement rampingJudgementContext;

        /// <summary>
        /// The rule violation service.
        /// </summary>
        private IRuleViolationService ruleViolationService;

        /// <summary>
        /// The spoofing judgement context.
        /// </summary>
        private ISpoofingJudgement spoofingJudgementContext;

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.judgementRepository = A.Fake<IJudgementRepository>();
            this.highProfitJudgementContext = A.Fake<IHighProfitJudgementContext>();
            this.cancelledOrderJudgementContext = A.Fake<ICancelledOrderJudgement>();
            this.highVolumeJudgementContext = A.Fake<IHighVolumeJudgement>();
            this.layeringJudgementContext = A.Fake<ILayeringJudgement>();
            this.placingOrdersWithNoIntentJudgementContext = A.Fake<IPlacingOrdersWithNoIntentToExecuteJudgement>();
            this.rampingJudgementContext = A.Fake<IRampingJudgement>();
            this.markingTheCloseJudgementContext = A.Fake<IMarkingTheCloseJudgement>();
            this.spoofingJudgementContext = A.Fake<ISpoofingJudgement>();
            this.highProfitJudgementMapper = A.Fake<IHighProfitJudgementMapper>();
            this.fixedIncomeProfitJudgementMapper = A.Fake<IFixedIncomeHighProfitJudgementMapper>();
            this.fixedIncomeProfitJudgementContext = A.Fake<IFixedIncomeHighProfitJudgementContext>();
            this.fixedIncomeHighVolumeJudgementMapper = A.Fake<IFixedIncomeHighVolumeJudgementMapper>();
            this.fixedIncomeHighVolumeJudgementContext = A.Fake<IFixedIncomeHighVolumeJudgementContext>();
            this.ruleViolationService = A.Fake<IRuleViolationService>();
            this.logger = A.Fake<ILogger<JudgementService>>();
        }

        /// <summary>
        /// The constructor null high profit judgement mapper is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullHighProfitJudgementMapperIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementService(
                    this.judgementRepository,
                    this.ruleViolationService, 
                    null,
                    this.fixedIncomeProfitJudgementMapper,
                    this.fixedIncomeHighVolumeJudgementMapper,
                    this.logger));
        }

        /// <summary>
        /// The constructor null fixed income high profit judgement mapper is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullFixedIncomeHighProfitJudgementMapperIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementService(
                    this.judgementRepository,
                    this.ruleViolationService,
                    this.highProfitJudgementMapper,
                    null,
                    this.fixedIncomeHighVolumeJudgementMapper,
                    this.logger));
        }

        /// <summary>
        /// The constructor_ null judgement repository_ is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullJudgementRepositoryIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementService(
                    null,
                    this.ruleViolationService,
                    this.highProfitJudgementMapper,
                    this.fixedIncomeProfitJudgementMapper,
                    this.fixedIncomeHighVolumeJudgementMapper,
                    this.logger));
        }

        /// <summary>
        /// The constructor_ null logger_ is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullLoggerIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementService(
                    this.judgementRepository,
                    this.ruleViolationService,
                    this.highProfitJudgementMapper,
                    this.fixedIncomeProfitJudgementMapper,
                    this.fixedIncomeHighVolumeJudgementMapper,
                    null));
        }

        /// <summary>
        /// The ctor_ null rule violation service_ is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullRuleViolationServiceIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementService(
                    this.judgementRepository,
                    null,
                    this.highProfitJudgementMapper,
                    this.fixedIncomeProfitJudgementMapper,
                    this.fixedIncomeHighVolumeJudgementMapper,
                    this.logger));
        }

        /// <summary>
        /// The judgement_ cancelled order judgement_ save does add to violation if project to alert.
        /// </summary>
        [Test]
        public async Task JudgementCancelledOrderJudgementSaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.cancelledOrderJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<ICancelledOrderJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ cancelled order judgement_ save does not add to violation if no project to alert.
        /// </summary>
        [Test]
        public async Task JudgementCancelledOrderJudgementSaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.cancelledOrderJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<ICancelledOrderJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ high profit judgement_ null does not throw exception.
        /// </summary>
        [Test]
        public async Task JudgementHighProfitJudgementNullDoesNotThrowException()
        {
            var service = this.BuildService();

            this.highProfitJudgementContext.Judgement.OrderId = null;

            await service.Judgement(this.highProfitJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IHighProfitJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.highProfitJudgementMapper.Map(A<IHighProfitJudgementContext>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The judgement_ high profit judgement_ save does add to violation if project to alert.
        /// </summary>
        [Test]
        public async Task JudgementHighProfitJudgementSaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            this.highProfitJudgementContext.RaiseRuleViolation = true;
            this.highProfitJudgementContext.Judgement.OrderId = "test-order-id";

            await service.Judgement(this.highProfitJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IHighProfitJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.highProfitJudgementMapper.Map(A<IHighProfitJudgementContext>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ high profit judgement_ save does not add to violation if no project to alert.
        /// </summary>
        [Test]
        public async Task JudgementHighProfitJudgementSaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            this.highProfitJudgementContext.RaiseRuleViolation = false;
            this.highProfitJudgementContext.Judgement.OrderId = "test-order-id";

            await service.Judgement(this.highProfitJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IHighProfitJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.highProfitJudgementMapper.Map(A<IHighProfitJudgementContext>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The judgement_ high volume judgement_ save does add to violation if project to alert.
        /// </summary>
        [Test]
        public async Task JudgementHighVolumeJudgementSaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.highVolumeJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IHighVolumeJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ high volume judgement_ save does not add to violation if no project to alert.
        /// </summary>
        [Test]
        public async Task JudgementHighVolumeJudgementSaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.highVolumeJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IHighVolumeJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ layering judgement_ save does add to violation if project to alert.
        /// </summary>
        [Test]
        public async Task JudgementLayeringJudgementSaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.layeringJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<ILayeringJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ layering judgement_ save does not add to violation if no project to alert.
        /// </summary>
        [Test]
        public async Task JudgementLayeringJudgementSaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.layeringJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<ILayeringJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ marking the close judgement_ save does add to violation if project to alert.
        /// </summary>
        [Test]
        public async Task JudgementMarkingTheCloseJudgementSaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.markingTheCloseJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IMarkingTheCloseJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ marking the close judgement_ save does not add to violation if no project to alert.
        /// </summary>
        [Test]
        public async Task JudgementMarkingTheCloseJudgementSaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.markingTheCloseJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IMarkingTheCloseJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ null cancelled order judgement_ null does not throw exception.
        /// </summary>
        [Test]
        public void JudgementNullCancelledOrderJudgementNullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrowAsync(() => service.Judgement((ICancelledOrderJudgement)null));

            A.CallTo(() => this.judgementRepository.SaveAsync(A<ICancelledOrderJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The judgement_ null high profit judgement_ null does not throw exception.
        /// </summary>
        [Test]
        public void JudgementNullHighProfitJudgementNullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrowAsync(() => service.Judgement((IHighProfitJudgementContext)null));

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IHighProfitJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.highProfitJudgementMapper.Map(A<IHighProfitJudgementContext>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The judgement_ null high volume judgement_ null does not throw exception.
        /// </summary>
        [Test]
        public void JudgementNullHighVolumeJudgementNullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrowAsync(() => service.Judgement((IHighVolumeJudgement)null));

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IHighVolumeJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The judgement_ null layering judgement_ null does not throw exception.
        /// </summary>
        [Test]
        public void JudgementNullLayeringJudgementNullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrowAsync(() => service.Judgement((ILayeringJudgement)null));

            A.CallTo(() => this.judgementRepository.SaveAsync(A<ILayeringJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The judgement_ null marking the close judgement_ null does not throw exception.
        /// </summary>
        [Test]
        public void JudgementNullMarkingTheCloseJudgementNullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrowAsync(() => service.Judgement((IMarkingTheCloseJudgement)null));

            A.CallTo(() => this.judgementRepository.SaveAsync(A<ILayeringJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The judgement null placing orders with no intent judgement null does not throw exception.
        /// </summary>
        [Test]
        public void JudgementNullPlacingOrdersWithNoIntentJudgementNullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrowAsync(() => service.Judgement((IPlacingOrdersWithNoIntentToExecuteJudgement)null));

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IPlacingOrdersWithNoIntentToExecuteJudgement>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The judgement null ramping judgement null does not throw exception.
        /// </summary>
        [Test]
        public void JudgementNullRampingJudgementNullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrowAsync(() => service.Judgement((IRampingJudgement)null));

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IRampingJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The judgement_ null spoofing judgement_ null does not throw exception.
        /// </summary>
        [Test]
        public void JudgementNullSpoofingJudgementNullDoesNotThrowException()
        {
            var service = this.BuildService();

            Assert.DoesNotThrowAsync(() => service.Judgement((ISpoofingJudgement)null));

            A.CallTo(() => this.judgementRepository.SaveAsync(A<ISpoofingJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The judgement placing orders with no intent judgement save does add to violation if project to alert.
        /// </summary>
        [Test]
        public async Task JudgementPlacingOrdersWithNoIntentJudgementSaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.placingOrdersWithNoIntentJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IPlacingOrdersWithNoIntentToExecuteJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement placing orders with no intent judgement save does not add to violation if no project to alert.
        /// </summary>
        [Test]
        public async Task JudgementPlacingOrdersWithNoIntentJudgementSaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.placingOrdersWithNoIntentJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IPlacingOrdersWithNoIntentToExecuteJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ ramping judgement_ save does add to violation if project to alert.
        /// </summary>
        [Test]
        public async Task JudgementRampingJudgementSaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.rampingJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IRampingJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ ramping judgement_ save does not add to violation if no project to alert.
        /// </summary>
        [Test]
        public async Task JudgementRampingJudgementSaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.rampingJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IRampingJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ spoofing judgement_ save does add to violation if project to alert.
        /// </summary>
        [Test]
        public async Task JudgementSpoofingJudgementSaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.spoofingJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<ISpoofingJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ spoofing judgement_ save does not add to violation if no project to alert.
        /// </summary>
        [Test]
        public async Task JudgementSpoofingJudgementSaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = this.BuildService();

            await service.Judgement(this.spoofingJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<ISpoofingJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement_ fixed income high profit judgement_ null does not throw exception.
        /// </summary>
        [Test]
        public async Task JudgementFixedIncomeHighProfitJudgementNullDoesNotThrowException()
        {
            var service = this.BuildService();

            this.fixedIncomeProfitJudgementContext.Judgement.OrderId = null;

            await service.Judgement(this.fixedIncomeProfitJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IFixedIncomeHighProfitJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.fixedIncomeProfitJudgementMapper.Map(A<IFixedIncomeHighProfitJudgementContext>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The judgement_ fixed income high profit judgement_ save does add to violation if project to alert.
        /// </summary>
        [Test]
        public async Task JudgementFixedIncomeHighProfitJudgementSaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            this.fixedIncomeProfitJudgementContext.RaiseRuleViolation = true;
            this.fixedIncomeProfitJudgementContext.Judgement.OrderId = "test-order-id";

            await service.Judgement(this.fixedIncomeProfitJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IFixedIncomeHighProfitJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.fixedIncomeProfitJudgementMapper.Map(A<IFixedIncomeHighProfitJudgementContext>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The judgement fixed income high volume judgement null does not throw exception.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task JudgementFixedIncomeHighVolumeJudgementNullDoesNotThrowException()
        {
            var service = this.BuildService();

            this.fixedIncomeHighVolumeJudgementContext.Judgement.OrderId = null;

            await service.Judgement(this.fixedIncomeHighVolumeJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IFixedIncomeHighProfitJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.fixedIncomeHighVolumeJudgementMapper.MapContextToBreach(A<IFixedIncomeHighVolumeJudgementContext>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The judgement fixed income high volume judgement save does add to violation if project to alert.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task JudgementFixedIncomeHighVolumeJudgementSaveDoesAddToViolationIfProjectToAlert()
        {
            var service = this.BuildService();

            this.fixedIncomeHighVolumeJudgementContext.RaiseRuleViolation = true;
            this.fixedIncomeHighVolumeJudgementContext.Judgement.OrderId = "test-order-id";

            await service.Judgement(this.fixedIncomeHighVolumeJudgementContext);

            A.CallTo(() => this.judgementRepository.SaveAsync(A<IFixedIncomeHighVolumeJudgement>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.fixedIncomeHighVolumeJudgementMapper.MapContextToBreach(A<IFixedIncomeHighVolumeJudgementContext>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }
        
        /// <summary>
        /// The pass judgement_ calls_ rule violations repository.
        /// </summary>
        [Test]
        public void PassJudgementCallsRuleViolationsRepository()
        {
            var service = this.BuildService();

            service.PassJudgement();

            A.CallTo(() => this.ruleViolationService.ProcessRuleViolationCache()).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The build service.
        /// </summary>
        /// <returns>
        /// The <see cref="JudgementService"/>.
        /// </returns>
        private JudgementService BuildService()
        {
            return new JudgementService(
                this.judgementRepository,
                this.ruleViolationService,
                this.highProfitJudgementMapper,
                this.fixedIncomeProfitJudgementMapper,
                this.fixedIncomeHighVolumeJudgementMapper,
                this.logger);
        }
    }
}