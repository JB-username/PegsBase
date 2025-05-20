namespace PegsBase.Models.Constants
{
    public enum SaSrid
    {
        Wgs84 = 4326,

        // Hartebeesthoek94 “Lo” zones (Tranverse Mercator)
        HartLo15 = 2046, //Hartebeeshoek94 / Lo15
        HartLo17 = 2047,
        HartLo19 = 2048,
        HartLo21 = 2049,
        HartLo23 = 2050,
        HartLo25 = 2051,
        HartLo27 = 2052,
        HartLo29 = 2053,
        HartLo31 = 2054,
        HartLo33 = 2055,

        // Cape Datum “Lo” zones
        CapeLo17 = 22277, // Lo-17° E
        CapeLo19 = 22279,
        CapeLo21 = 22281,
        CapeLo23 = 22283,
        CapeLo25 = 22285,
        CapeLo27 = 22287,
        CapeLo29 = 22289,
        CapeLo31 = 22291,
        CapeLo33 = 22293,

        // UTM southern hemisphere
        Utm31S = 32731, // UTM zone 31S
        Utm32S = 32732,
        Utm33S = 32733,
        Utm34S = 32734,
        Utm35S = 32735,
        Utm36S = 32736
    }
}
