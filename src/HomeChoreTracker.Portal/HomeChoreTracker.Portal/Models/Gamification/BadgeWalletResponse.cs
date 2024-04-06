namespace HomeChoreTracker.Portal.Models.Gamification
{
    public class BadgeWalletResponse
    {
        public int Id { get; set; }

        public bool DoneFirstTask { get; set; }

        public bool DoneFirstCleaningTask { get; set; }
        public bool DoneFirstLaundryTask { get; set; }
        public bool DoneFirstKitchenTask { get; set; }
        public bool DoneFirstBathroomTask { get; set; }
        public bool DoneFirstBedroomTask { get; set; }
        public bool DoneFirstOutdoorsTask { get; set; }
        public bool DoneFirstOrganizeTask { get; set; }

        public bool EarnedPerDayFiftyPoints { get; set; }
        public bool EarnedPerDayHundredPoints { get; set; }

        public bool DoneFiveTaskPerWeek { get; set; }
        public bool DoneTenTaskPerWeek { get; set; }
        public bool DoneTwentyFiveTaskPerWeek { get; set; }

        public bool CreatedTaskWasUsedOtherHome { get; set; }

        public bool CreateFirstPurchase { get; set; }
        public bool CreateFirstAdvice { get; set; }

        public bool CreateFirstIncome { get; set; }
        public bool CreateFirstExpense { get; set; }

    }
}
