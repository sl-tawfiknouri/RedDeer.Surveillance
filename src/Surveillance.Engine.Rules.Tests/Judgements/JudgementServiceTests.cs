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

namespace Surveillance.Engine.Rules.Tests.Judgements
{
    [TestFixture]
    public class JudgementServiceTests
    {
        private IJudgementRepository _judgementRepository;
        private IHighProfitJudgementContext _highProfitJudgementContext;
        private ICancelledOrderJudgement _cancelledOrderJudgementContext;
        private IHighVolumeJudgement _highVolumeJudgementContext;
        private IMarkingTheCloseJudgement _markingTheCloseJudgementContext;
        private IRampingJudgement _rampingJudgementContext;
        private ILayeringJudgement _layeringJudgementContext;
        private ISpoofingJudgement _spoofingJudgementContext;
        private IPlacingOrdersWithNoIntentToExecuteJudgement _placingOrdersWithNoIntentJudgementContext;
        private IHighProfitJudgementMapper _highProfitJudgementMapper;
        private IRuleViolationService _ruleViolationService;
        private ILogger<JudgementService> _logger;

        [SetUp]
        public void Setup()
        {
            _judgementRepository = A.Fake<IJudgementRepository>();
            _highProfitJudgementContext = A.Fake<IHighProfitJudgementContext>();
            _cancelledOrderJudgementContext = A.Fake<ICancelledOrderJudgement>();
            _highVolumeJudgementContext = A.Fake<IHighVolumeJudgement>();
            _layeringJudgementContext = A.Fake<ILayeringJudgement>();
            _placingOrdersWithNoIntentJudgementContext = A.Fake<IPlacingOrdersWithNoIntentToExecuteJudgement>();
            _rampingJudgementContext = A.Fake<IRampingJudgement>();
            _markingTheCloseJudgementContext = A.Fake<IMarkingTheCloseJudgement>();
            _spoofingJudgementContext = A.Fake<ISpoofingJudgement>();
            _highProfitJudgementMapper = A.Fake<IHighProfitJudgementMapper>();
            _ruleViolationService = A.Fake<IRuleViolationService>();
            _logger = A.Fake<ILogger<JudgementService>>();
        }

        [Test]
        public void Ctor_NullJudgementRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new JudgementService(null, _ruleViolationService, _highProfitJudgementMapper, _logger));
        }

        [Test]
        public void Ctor_NullRuleViolationService_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new JudgementService(_judgementRepository, null, _highProfitJudgementMapper, _logger));
        }

        [Test]
        public void Ctor_NullHighProfitJudgementMapper_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new JudgementService(_judgementRepository, _ruleViolationService, null, _logger));
        }

        [Test]
        public void Ctor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, null));
        }

        [Test]
        public void Judgement_NullHighProfitJudgement_NullDoesNotThrowException()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            Assert.DoesNotThrow(() => service.Judgement((IHighProfitJudgementContext)null));

            A.CallTo(() => _judgementRepository.Save(A<IHighProfitJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _highProfitJudgementMapper.Map(A<IHighProfitJudgementContext>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_HighProfitJudgement_NullDoesNotThrowException()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);
            _highProfitJudgementContext.Judgement.OrderId = null;

            service.Judgement(_highProfitJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<IHighProfitJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _highProfitJudgementMapper.Map(A<IHighProfitJudgementContext>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
       }

        [Test]
        public void Judgement_HighProfitJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);
            _highProfitJudgementContext.ProjectToAlert = false;
            _highProfitJudgementContext.Judgement.OrderId = "test-order-id";

            service.Judgement(_highProfitJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<IHighProfitJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _highProfitJudgementMapper.Map(A<IHighProfitJudgementContext>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }


        [Test]
        public void Judgement_HighProfitJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);
            _highProfitJudgementContext.ProjectToAlert = true;
            _highProfitJudgementContext.Judgement.OrderId = "test-order-id";

            service.Judgement(_highProfitJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<IHighProfitJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _highProfitJudgementMapper.Map(A<IHighProfitJudgementContext>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_NullCancelledOrderJudgement_NullDoesNotThrowException()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            Assert.DoesNotThrow(() => service.Judgement((ICancelledOrderJudgement)null));

            A.CallTo(() => _judgementRepository.Save(A<ICancelledOrderJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_CancelledOrderJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_cancelledOrderJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<ICancelledOrderJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }
        
        [Test]
        public void Judgement_CancelledOrderJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_cancelledOrderJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<ICancelledOrderJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_NullHighVolumeJudgement_NullDoesNotThrowException()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            Assert.DoesNotThrow(() => service.Judgement((IHighVolumeJudgement)null));

            A.CallTo(() => _judgementRepository.Save(A<IHighVolumeJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_HighVolumeJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_highVolumeJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<IHighVolumeJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_HighVolumeJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_highVolumeJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<IHighVolumeJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_NullLayeringJudgement_NullDoesNotThrowException()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            Assert.DoesNotThrow(() => service.Judgement((ILayeringJudgement)null));

            A.CallTo(() => _judgementRepository.Save(A<ILayeringJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_LayeringJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_layeringJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<ILayeringJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_LayeringJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_layeringJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<ILayeringJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_NullMarkingTheCloseJudgement_NullDoesNotThrowException()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            Assert.DoesNotThrow(() => service.Judgement((IMarkingTheCloseJudgement)null));

            A.CallTo(() => _judgementRepository.Save(A<ILayeringJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_MarkingTheCloseJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_markingTheCloseJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<IMarkingTheCloseJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_MarkingTheCloseJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_markingTheCloseJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<IMarkingTheCloseJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_NullPlacingOrdersWithNoIntentJudgement_NullDoesNotThrowException()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            Assert.DoesNotThrow(() => service.Judgement((IPlacingOrdersWithNoIntentToExecuteJudgement)null));

            A.CallTo(() => _judgementRepository.Save(A<IPlacingOrdersWithNoIntentToExecuteJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_PlacingOrdersWithNoIntentJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_placingOrdersWithNoIntentJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<IPlacingOrdersWithNoIntentToExecuteJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_PlacingOrdersWithNoIntentJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_placingOrdersWithNoIntentJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<IPlacingOrdersWithNoIntentToExecuteJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_NullRampingJudgement_NullDoesNotThrowException()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            Assert.DoesNotThrow(() => service.Judgement((IRampingJudgement)null));

            A.CallTo(() => _judgementRepository.Save(A<IRampingJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_RampingJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_rampingJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<IRampingJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_RampingJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_rampingJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<IRampingJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_NullSpoofingJudgement_NullDoesNotThrowException()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            Assert.DoesNotThrow(() => service.Judgement((ISpoofingJudgement)null));

            A.CallTo(() => _judgementRepository.Save(A<ISpoofingJudgement>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Judgement_SpoofingJudgement_SaveDoesNotAddToViolationIfNoProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_spoofingJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<ISpoofingJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Judgement_SpoofingJudgement_SaveDoesAddToViolationIfProjectToAlert()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.Judgement(_spoofingJudgementContext);

            A.CallTo(() => _judgementRepository.Save(A<ISpoofingJudgement>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void PassJudgement_Calls_RuleViolationsRepository()
        {
            var service = new JudgementService(_judgementRepository, _ruleViolationService, _highProfitJudgementMapper, _logger);

            service.PassJudgement();

            A.CallTo(() => _ruleViolationService.ProcessRuleViolationCache()).MustHaveHappenedOnceExactly();
        }
    }
}
