namespace Domain.Surveillance.Aws
{
    public enum SurveillanceSqsQueue
    {
        DistributedRule = 0,
        ScheduledRule = 1,
        DataSynchroniserRequest = 2,
        CaseMessage = 3,
        TestRuleRunUpdate = 4,
        UploadCoordinator = 5,
        ScheduleRuleCancellation = 6,
        DataImportS3Upload = 7,
    }
}
