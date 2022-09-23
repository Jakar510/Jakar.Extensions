namespace Jakar.Extensions;


/// <Docs>
///   <summary>Indicates the altitude reference system to be used in defining a location.</summary>
///   <remarks>
///     <para>This enum is a copy of Windows.Devices.Geolocation.AltitudeReferenceSystem.</para>
///   </remarks>
/// </Docs>
public enum AltitudeReference
{
    /// <Docs>
    ///   <summary>The altitude reference system was not specified.</summary>
    /// </Docs>
    Unspecified,
    /// <Docs>
    ///   <summary>The altitude reference system is based on distance above terrain or ground level.</summary>
    /// </Docs>
    Terrain,
    /// <Docs>
    ///   <summary>The altitude reference system is based on an ellipsoid (usually WGS84), which is a mathematical approximation of the shape of the Earth.</summary>
    /// </Docs>
    Ellipsoid,
    /// <Docs>
    ///   <summary>The altitude reference system is based on the distance above sea level (parametrized by a so-called Geoid).</summary>
    /// </Docs>
    Geoid,
    /// <Docs>
    ///   <summary>The altitude reference system is based on the distance above the tallest surface structures, such as buildings, trees, roads, etc., above terrain or ground level.</summary>
    /// </Docs>
    Surface,
}