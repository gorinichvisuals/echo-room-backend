namespace EchoRoom.Shared.Services.Helpers;

public static class DateHelper
{
    public static int CalculateAge(this DateTime birthDate, DateTime? onDate = null)
    {
        DateTime today = (onDate ?? DateTime.UtcNow).Date;

        int age = today.Year - birthDate.Year;

        if (today < birthDate.AddYears(age))
            age--;

        return age;
    }
}