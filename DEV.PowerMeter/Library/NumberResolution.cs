using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Formatting, "Controls the formatting of real numbers.\r\nThe setting selects the overall precision of the result,\r\nand NOT the number of digits right of the decimal point.\r\nThus the following numbers may result from a D3 setting:\r\n0.123, 1.23, 12.3, or 123.0. Because Legacy.\r\n")]
    public enum NumberResolution
    {
        [API("Format numbers with 3 significant digits.")] D3 = 3,
        [API("Default = D3")] Default = 3,
        [API("Format numbers with 4 significant digits.")] D4 = 4,
        [API("Format numbers with 5 significant digits.")] D5 = 5,
    }
}
