
namespace DEV.LaserControl
{
    public struct StatusCode
    {
        public static string NO_ERROR = "0";
        public static string INTERLOCK_SW_I2C_ERROR = "1";
        public static string INTERLOCK_SW_PRESET_ENERGY_TOO_HIGH = "2";
        public static string INTERLOCK_SW_TUBE_PRESSURE_OUT_OF_RANGE = "6";
        public static string INTERLOCK_HW_TUBE_TEMPERATURE_TOO_HIGH = "10";
        public static string INTERLOCK_HW_VENTILATION_MOTOR_FAILED = "11";
        public static string INTERLOCK_HW_REMOTE_INTERLOCK_SWITCH_OPEN = "16";
        public static string INTERLOCK_HW_HV_POWER_SUPPLY_ERROR = "18";
        public static string WARNING_NO_GAS_FLOW = "23";
        public static string WARNING_PRESET_ENERGY_TOO_LOW = "25";
        public static string INTERLOCK_SW_LOW_LIGHT = "26";
        public static string INTERLOCK_SW_NO_GAS_FLOW_2 = "27";
        public static string INTERLOCK_SW_CONFIGURE_ERROR_DETECTED = "30";
        public static string INTERLOCK_SW_REBOOT_REQUIRED = "31";
        public static string WARNING_NO_VACUUM = "32";
        public static string INTERLOCK_HW_LEFT_SIDE_SERVICE_PANEL_OPEN = "42";
        public static string INTERLOCK_HW_NO_AIR_FLOW = "43";
        public static string WARNING_INTERNAL_GAS_PURIFIER_ERROR = "51";
        public static string INTERLOCK_SW_NO_EXTERNAL_TRIGGER_SIGNAL_DETECTED = "54";
        public static string INTERLOCK_SW_HALOGEN_FILTER_EXCHANGE_REQUIRED = "62";
        public static string WARNING_TUBE_TEMPERATURE_TOO_HIGH = "64";
        public static string WARNING_CHECK_SAFETY_RELAY = "69";
        public static string WARNING_DUTY_CYCLE_TOO_HIGH = "73";
        public static string INTERLOCK_SW_DUTY_CYCLE_TOO_HIGH = "74";
        public static string WARNING_DUTY_CYCLE_TOO_HIGH_PRE_INTERLOCK = "75";
        public static string WARNING_PRESET_ENERGY_TOO_HIGH = "89";
        public static string INTERLOCK_SW_MAX_POWER = "95";
        public static string WARNING_HALOGEN_FILTER_EXCHANGE_REQUIRE_SOON = "103";
        public static string INTERLOCK_HW_ASSP_TEMPERATURE_TOO_HIGH = "106";
        public static string INTERLOCK_HW_ASSP_RESET_CURRENT = "107";
        public static string INTERLOCK_HW_RIGHT_SIDE_SERVICE_PANEL_OPEN = "120";
        public static string INTERLOCK_HW_SAFETY_CONTROL_MODULE_OFF = "122";
        public static string WARNING_TUBE_PRESSURE_TOO_HIGH = "123";
        public static string WARNING_TUBE_PRESSURE_TOO_LOW = "124";
        public static string INTERLOCK_SW_TUBE_TEMPERATURE_TOO_HIGH = "125";
        public static string WARNING_LEAK_TEST_FAIL = "126";
        public static string INTERLOCK_SW_COMMUNICATION_TIME_OUT = "127";
        public static string INTERLOCK_SW_TUBE_PRESSURE_SENSOR_FAILED = "128";
        public static string WARNING_MANIFOLD_PRESSURE_SENSOR_FAILED = "129";
        public static string INTERLOCK_SW_TUBE_TEMPERATURE_SENSOR_FAILED = "130";
        public static string INTERLOCK_SW_GAS_ACTI0N_TIMEOUT = "157";
        public static string INTERLOCK_SW_GAS_MISMATCH = "182";
        public static string WARNING_HALOGEN_LINE_PRESSURE_TO0_LOW = "192";
        public static string WARNING_HALOGEN_LINE_PRESSURE_TOO_HIGH = "193";
        public static string WARNING_INERT_LINE_PRESSURE_TOO_LOW = "198";
        public static string WARNING_INERT_LINE_PRESSURE_TOO_HIGH = "199";
        public static string INTERLOCK_SW_MANIFOLD_LEAK_TEST_FAILED = "203";
        public static string INTERLOCK_SW_HALOGEN_LINE_PRESSURE_TOO_LOW = "204";
        public static string INTERLOCK_SW_HALOGEN_LINE_PRESSURE_TOO_HIGH = "205";
        public static string INTERLOCK_SW_INERT_LINE_PRESSURE_TOO_LOW = "210";
        public static string INTERLOCK_SW_INERT_LINE_PRESSURE_TOO_HIGH = "211";
        public static string INTERLOCK_SW_WATCHDOG_ERROR = "220";
        public static string INTERLOCK_SW_EXTERNAL_GAS_FAILURE = "221";
        public static string WARNING_TUBE_PRESSURE_HIGH = "223";
        public static string INTERLOCK_SW_TUBE_PRESSURE_MAX = "224";
        public static string WARNING_REAL_TIME_CLOCK_BATTERY_EMPTY = "231";
        public static string WARNING_BOTTLE_EMPTY = "250";
    }
}