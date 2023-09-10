namespace PropPunkShared.Database.Models;

public record GlobalSettingsRaw(
    int Lock,
    int AotYearStart,
    float AotScaleFactor,
    int CountryResearchSlotCount,
    int CompanyResearchSlotCount,
    long OfficialServerId,
    int MoneyPerTechPoint
);

public record GlobalSettings(
    int AotYearStart,
    float AotScaleFactor,
    int LastCountryResearchSlotIndex,
    int LastCompanyResearchSlotIndex,
    long OfficialServerId,
    int MoneyPerTechPoint
)
{
    public GlobalSettings(GlobalSettingsRaw raw) :
        this(raw.AotYearStart,
            raw.AotScaleFactor, //shift research slot count to array notation
            raw.CountryResearchSlotCount - 1,
            raw.CompanyResearchSlotCount - 1,
            raw.OfficialServerId,
            raw.MoneyPerTechPoint
        )
    {
    }
}
