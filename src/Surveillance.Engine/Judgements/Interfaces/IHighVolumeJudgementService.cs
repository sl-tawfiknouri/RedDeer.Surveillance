﻿using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface IHighVolumeJudgementService
    {
        void Judgement(IHighVolumeJudgement highVolume);
    }
}
