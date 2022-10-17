// Jakar.Extensions :: Jakar.Extensions
// 09/16/2022  11:08 AM

namespace Jakar.Extensions;


/// <summary>
///     <see href = "https://github.com/xamarin/Essentials/blob/main/Xamarin.Essentials/Types/UnitConverters.shared.cs" />
/// </summary>
public static class UnitConverters
{
    private const double TWO_PI                          = 2.0 * Math.PI;
    private const double TOTAL_DEGREES                   = 360.0;
    private const double ATMOSPHERE_PASCALS              = 101325.0;
    private const double DEGREES_TO_RADIANS              = Math.PI / 180.0;
    private const double MILES_TO_KILOMETERS             = 1.609344;
    private const double MILES_TO_METERS                 = 1609.344;
    private const double KILOMETERS_TO_MILES             = 1.0 / MILES_TO_KILOMETERS;
    private const double CELSIUS_TO_KELVIN               = 273.15;
    private const double POUNDS_TO_KG                    = 0.45359237;
    private const double POUNDS_TO_STONES                = 0.07142857;
    private const double STONES_TO_POUNDS                = 14;
    private const double KG_TO_POUNDS                    = 2.204623;
    private const double MEAN_EARTH_RADIUS_IN_KILOMETERS = 6371.0;
    private const double INTERNATIONAL_FOOT_DEFINITION   = 0.3048;
    private const double US_SURVEY_FOOT_DEFINITION       = 1200.0 / 3937;
    public static double AtmospheresToPascals( double atm ) => atm * ATMOSPHERE_PASCALS;
    public static double CelsiusToFahrenheit( double  celsius ) => celsius * 1.8 + 32.0;
    public static double CelsiusToKelvin( double      celsius ) => celsius + CELSIUS_TO_KELVIN;
    public static double CoordinatesToKilometers( double latStart, double longitudeStart, double latEnd, double longitudeEnd )
    {
        if (latStart.Equals( latEnd ) && longitudeStart.Equals( longitudeEnd )) { return 0; }

        double dLat = DegreesToRadians( latEnd - latStart );
        double dLon = DegreesToRadians( longitudeEnd - longitudeStart );

        latStart = DegreesToRadians( latStart );
        latEnd   = DegreesToRadians( latEnd );

        double dLat2 = Math.Sin( dLat / 2 ) * Math.Sin( dLat / 2 );
        double dLon2 = Math.Sin( dLon / 2 ) * Math.Sin( dLon / 2 );

        double a = dLat2 + dLon2 * Math.Cos( latStart ) * Math.Cos( latEnd );
        double c = 2 * Math.Asin( Math.Sqrt( a ) );

        return MEAN_EARTH_RADIUS_IN_KILOMETERS * c;
    }


    public static double CoordinatesToMiles( double                 latStart, double longitudeStart, double latEnd, double longitudeEnd ) => KilometersToMiles( CoordinatesToKilometers( latStart, longitudeStart, latEnd, longitudeEnd ) );
    public static double DegreesPerSecondToHertz( double            degrees ) => degrees / TOTAL_DEGREES;
    public static double DegreesPerSecondToRadiansPerSecond( double degrees ) => HertzToRadiansPerSecond( DegreesPerSecondToHertz( degrees ) );
    public static double DegreesToRadians( double                   degrees ) => degrees * DEGREES_TO_RADIANS;


    public static double FahrenheitToCelsius( double       fahrenheit ) => (fahrenheit - 32.0) / 1.8;
    public static double HectopascalsToKilopascals( double hpa ) => hpa / 10.0;
    public static double HectopascalsToPascals( double     hpa ) => hpa * 100.0;
    public static double HertzToDegreesPerSecond( double   hertz ) => hertz * TOTAL_DEGREES;
    public static double HertzToRadiansPerSecond( double   hertz ) => hertz * TWO_PI;

    /// <summary>
    ///     International survey foot defined as exactly 0.3048 meters by convention in 1959. This is the most common modern foot measure.
    /// </summary>
    public static double InternationalFeetToMeters( double internationalFeet ) => internationalFeet * INTERNATIONAL_FOOT_DEFINITION;
    public static double KelvinToCelsius( double           kelvin ) => kelvin - CELSIUS_TO_KELVIN;
    public static double KilogramsToPounds( double         kilograms ) => kilograms * KG_TO_POUNDS;
    public static double KilometersToMiles( double         kilometers ) => kilometers * KILOMETERS_TO_MILES;
    public static double KilopascalsToHectopascals( double kpa ) => kpa * 10.0;
    public static double KilopascalsToPascals( double      kpa ) => kpa * 1000.0;


    /// <summary>
    ///     International survey foot defined as exactly 0.3048 meters by convention in 1959. This is the most common modern foot measure.
    /// </summary>
    public static double MetersToInternationalFeet( double meters ) => meters / INTERNATIONAL_FOOT_DEFINITION;

    /// <summary>
    ///     Exactly 1200/3937 meters by definition. In decimal terms approximately 0.304 800 609 601 219 meters. Variation from the common international foot of exactly 0.3048 meters may only be considerable
    ///     over large survey distances.
    /// </summary>
    public static double MetersToUsSurveyFeet( double meters ) => meters / US_SURVEY_FOOT_DEFINITION;
    public static double MilesToKilometers( double                  miles ) => miles * MILES_TO_KILOMETERS;
    public static double MilesToMeters( double                      miles ) => miles * MILES_TO_METERS;
    public static double PascalsToAtmospheres( double               pascals ) => pascals / ATMOSPHERE_PASCALS;
    public static double PoundsToKilograms( double                  pounds ) => pounds * POUNDS_TO_KG;
    public static double PoundsToStones( double                     pounds ) => pounds * POUNDS_TO_STONES;
    public static double RadiansPerSecondToDegreesPerSecond( double radians ) => HertzToDegreesPerSecond( RadiansPerSecondToHertz( radians ) );
    public static double RadiansPerSecondToHertz( double            radians ) => radians / TWO_PI;
    public static double RadiansToDegrees( double                   radians ) => radians / DEGREES_TO_RADIANS;
    public static double StonesToPounds( double                     stones ) => stones * STONES_TO_POUNDS;

    /// <summary>
    ///     Exactly 1200/3937 meters by definition. In decimal terms approximately 0.304 800 609 601 219 meters. Variation from the common international foot of exactly 0.3048 meters may only be considerable
    ///     over large survey distances.
    /// </summary>
    public static double UsSurveyFeetToMeters( double usFeet ) => usFeet * US_SURVEY_FOOT_DEFINITION;
}
