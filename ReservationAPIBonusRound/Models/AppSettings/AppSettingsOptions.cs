namespace ReservationAPIBonusRound.Models.AppSettings
{
    public class AppSettingsOptions
    {
        public int ReservationTimeoutInSeconds { get; set; }
        public int MinimumTimeBlockInSeconds { get; set; }
        public int MinimumAdvancedReservationInDays { get; set; }
    }
}
