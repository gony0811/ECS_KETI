using CertLibrary;
using RuntimeLibrary;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DEV.PowerMeter.Library
{
public class CMC_CLA
  {
    private static CMC_CLA current;
    public const int FreeTrialPeriodDays = 30;
    public const string RuntimeVersionMinimum = "1.0.3";
    public const string AppName = "Coherent Meter Connection";
    protected readonly string[] UniversalCertificateLocations = new string[2]
    {
      "\\\\NA\\Groups\\POR\\Eng\\SPLC\\Meter_2013\\Software\\Coherent Meter Connection Setups\\",
      "N:\\POR\\Eng\\SPLC\\Meter_2013\\Software\\Coherent Meter Connection Setups\\"
    };
    public const string ProfessionalFeaturesPartNumber = "1415083";
    public const string LegacyEnergyMaxPartNumber = "1347058";
    public const string LegacyPowerMaxPartNumber = "1340621";
    public const string PulseAnalysisPartNumber = "1340622";
    public const string LimitsAndAlarmsPartNumber = "1340623";
    public readonly Dictionary<string, string> FeatureName = new Dictionary<string, string>()
    {
      {
        "1415083",
        "CMC Pro Software License"
      },
      {
        "1347058",
        "Energy Max USB/RS"
      },
      {
        "1340621",
        "Power Max USB/RS"
      },
      {
        "1340622",
        "Pulse Analysis"
      },
      {
        "1340623",
        "Limits and Alarms"
      }
    };
    public const string CmcProductName = "Coherent Meter Connection";
    public static readonly Guid CmcProductGuid = new Guid(2005463459, (short) 30556, (short) 19589, (byte) 190, (byte) 189, (byte) 225, (byte) 208, (byte) 135, (byte) 240, (byte) 14, (byte) 185);
    private RegistrationConfig registrationConfig;

    public static CMC_CLA Current
    {
      protected set => CMC_CLA.current = value;
      get
      {
        if (CMC_CLA.current == null)
          CMC_CLA.current = new CMC_CLA();
        return CMC_CLA.current;
      }
    }

    public CLA_Settings CLA_Settings { get; set; }

    public CMC_CLA()
    {
      this.CLA_Settings = new CLA_Settings();
      this.CLA_Settings.Load();
      this.Notify = new Action<string>(this.ShowNotificationDialog);
    }

    public virtual DateTime Today => new DateTime();

    public bool UnlimitedFreeTrial => false;

    public string MeterSerialNumber { get; private set; }

    public string MeterPartNumber { get; private set; }

    public Authorizations Authorizations { get; private set; }

    public bool IsAuthorized => this.Authorizations != null;

    public bool MeterIsLegacyMeterlessPower { get; protected set; }

    public bool MeterIsLegacyMeterlessEnergy { get; protected set; }

    protected bool LegacyPowerMax_IsLicensed { get; set; }

    protected bool LegacyEnergyMax_IsLicensed { get; set; }

    protected bool PulseAnalysis_IsLicensed { get; set; }

    protected bool LimitsAndAlarms_IsLicensed { get; set; }

    protected bool ProfessionalFeatures_AreLicensed { get; set; }

    public virtual void Clear()
    {
      this.Authorizations = (Authorizations) null;
      this.ProfessionalFeatures_AreLicensed = false;
      this.MeterSerialNumber = this.MeterPartNumber = string.Empty;
      this.MeterIsLegacyMeterlessPower = false;
      this.MeterIsLegacyMeterlessEnergy = false;
      this.LegacyPowerMax_IsLicensed = false;
      this.LegacyEnergyMax_IsLicensed = false;
      this.PulseAnalysis_IsLicensed = false;
      this.LimitsAndAlarms_IsLicensed = false;
    }

    public Registration Registration => this.CLA_Settings.Registration;

    public DateTime InstallDate => this.Registration.InstallDate;

    public virtual int FreeTrialRemainingDays => this.Registration == null ? -1 : this.Registration.FreeTrialRemainingDays;

    public bool FreeTrialActive => this.FreeTrialRemainingDays > 0;

    public virtual int FreeTrialDuration => this.Registration.Config.FreeTrialPeriodDays;

    public string Version => new AssemblyProperties((object) this).AssemblyVersion.ToString();

    public VersionMoniker RuntimeVersion => new VersionMoniker(new AssemblyProperties(typeof (CLA_Settings)).AssemblyVersion.ToString());

    public void Initialize()
    {
      this.Clear();
      if (this.RuntimeVersion < new VersionMoniker("1.0.3"))
        throw new NotSupportedException(string.Format("Runtime version {0} is not supported; need {1} or newer.", (object) this.RuntimeVersion, (object) "1.0.3"));
      this.CLA_Settings.InitializeRegistration(this.RegistrationConfig);
      this.Trace(string.Format("Initialize: FreeTrialActive = {0}", (object) this.FreeTrialActive));
    }

    public void RegisterInstallation()
    {
      this.Initialize();
      this.CLA_Settings.RegisterInstallation();
    }

    public void GetAuthorization(ILicensedMeter Meter)
    {
      string str1 = Meter != null && Meter.IsOpen ? Meter.PartNumber : throw new ArgumentException("GetAuthorization: null or closed Meter");
      string serialNumber = Meter.SerialNumber;
      if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(serialNumber))
        throw new ArgumentException("GetAuthorization: null or empty serial or part number");
      this.Trace("GetAuthorization: " + str1 + ", " + serialNumber);
      string str2 = str1.RemoveSurroundingQuotes();
      string str3 = serialNumber.RemoveSurroundingQuotes();
      this.Clear();
      this.MeterPartNumber = str2;
      this.MeterSerialNumber = str3;
      this.MeterIsLegacyMeterlessPower = Meter.IsLegacyMeterlessPower;
      this.MeterIsLegacyMeterlessEnergy = Meter.IsLegacyMeterlessEnergy;
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.CLA_Settings.Load();
      try
      {
        this.Authorizations = this.CLA_Settings.GetAuthorizations(this.MeterPartNumber, this.MeterSerialNumber);
        this.Trace(string.Format("Authorization[{0}]: {1}", (object) this.Authorizations.FeaturesCount, (object) this.Authorizations.ToString().Trim()));
        bool flag1 = this.UnlimitedFreeTrial || this.UniversalAccessAllowed;
        this.ProfessionalFeatures_AreLicensed = flag1 || this.Authorizations.HasFeaturePartNum("1415083") && !this.Authorizations.IsExpiredOn("1415083", this.Today);
        bool flag2 = flag1 | this.ProfessionalFeatures_AreLicensed;
        this.LegacyPowerMax_IsLicensed = flag2 || this.Authorizations.HasFeaturePartNum("1340621") && !this.Authorizations.IsExpiredOn("1340621", this.Today);
        this.LegacyEnergyMax_IsLicensed = flag2 || this.Authorizations.HasFeaturePartNum("1347058") && !this.Authorizations.IsExpiredOn("1347058", this.Today);
        this.PulseAnalysis_IsLicensed = flag2 || this.Authorizations.HasFeaturePartNum("1340622") && !this.Authorizations.IsExpiredOn("1340622", this.Today);
        this.LimitsAndAlarms_IsLicensed = flag2 || this.Authorizations.HasFeaturePartNum("1340623") && !this.Authorizations.IsExpiredOn("1340623", this.Today);
      }
      catch (Exception ex)
      {
        TraceLogger.Trace("CMC_CLA.GetAuthorization Exception: " + ex.Message);
      }
      this.Trace(string.Format("GetAuthorization load time: {0:N3} sec", (object) stopwatch.Elapsed.TotalSeconds));
    }

    public bool UniversalAccessAllowed { get; protected set; }

    public void EnableUniversalAccess(string certificatePath = null)
    {
      this.UniversalAccessAllowed = !string.IsNullOrEmpty(certificatePath) ? this.FindUniversalCertificate(certificatePath) : this.FindUniversalCertificate((IEnumerable<string>) this.UniversalCertificateLocations);
      if (!this.UniversalAccessAllowed)
        throw new UnauthorizedAccessException("No valid Universal Certificate found");
    }

    public bool FindUniversalCertificate(IEnumerable<string> locations)
    {
      foreach (string location in locations)
      {
        if (this.FindUniversalCertificate(location))
          return true;
      }
      return false;
    }

    public bool FindUniversalCertificate(string path)
    {
      if (File.Exists(path))
      {
        if (this.IsUniversalCertificate(path))
          return true;
      }
      else if (Directory.Exists(path) && this.FindUniversalCertificate(Directory.EnumerateFiles(path, "*.cert")))
        return true;
      return false;
    }

    public bool IsUniversalCertificate(string path)
    {
      Certificate certificate = new Certificate(File.ReadAllText(path));
      return certificate.Application.StartsWith("Coherent Meter Connection") && certificate.IsUniversalPossiblyExpired && certificate.IsUnexpiredUniversal() && certificate.VerifySignature(this.CLA_Settings.PublicKeysContainer);
    }

    public bool IsUnexpiredUniversalCertificate(Certificate certificate, DateTime date = default (DateTime))
    {
      if (date == new DateTime())
        date = DateTime.Today;
      return certificate.Application.StartsWith("Coherent Meter Connection") && certificate.IsUnexpiredUniversal(date) && certificate.VerifySignature(this.CLA_Settings.PublicKeysContainer);
    }

    public bool PulseAnalysis_CheckAuthorization()
    {
      if (!this.IsAuthorized)
        return false;
      if (!this.PulseAnalysis_IsLicensed)
        this.Notify("1340622");
      return this.PulseAnalysis_IsAuthorized;
    }

    public bool PulseAnalysis_IsAuthorized
    {
      get
      {
        if (!this.IsAuthorized)
          return false;
        return this.PulseAnalysis_IsLicensed || this.FreeTrialActive;
      }
    }

    public bool LimitsAndAlarms_CheckAuthorization()
    {
      if (!this.IsAuthorized)
        return false;
      if (!this.LimitsAndAlarms_IsLicensed)
        this.Notify("1340623");
      return this.LimitsAndAlarms_IsAuthorized;
    }

    public bool LimitsAndAlarms_IsAuthorized
    {
      get
      {
        if (!this.IsAuthorized)
          return false;
        return this.LimitsAndAlarms_IsLicensed || this.FreeTrialActive;
      }
    }

    public bool LegacyPowerMax_CheckAuthorization()
    {
      if (!this.IsAuthorized)
        return false;
      if (!this.LegacyPowerMax_IsLicensed)
        this.Notify("1340621");
      return this.LegacyPowerMax_IsAuthorized;
    }

    public bool LegacyPowerMax_IsAuthorized
    {
      get
      {
        if (!this.IsAuthorized)
          return false;
        return this.LegacyPowerMax_IsLicensed || this.FreeTrialActive;
      }
    }

    public bool LegacyEnergyMax_CheckAuthorization()
    {
      if (!this.IsAuthorized)
        return false;
      if (!this.LegacyEnergyMax_IsLicensed)
        this.Notify("1347058");
      return this.LegacyEnergyMax_IsAuthorized;
    }

    public bool LegacyEnergyMax_IsAuthorized
    {
      get
      {
        if (!this.IsAuthorized)
          return false;
        return this.LegacyEnergyMax_IsLicensed || this.FreeTrialActive;
      }
    }

    public void ThrowIfUnauthorizedLegacy(ILicensedMeter Meter)
    {
      this.GetAuthorization(Meter);
      if (Meter.IsOpen && (Meter.IsLegacyMeterlessPower && !this.LegacyPowerMax_CheckAuthorization() || Meter.IsLegacyMeterlessEnergy && !this.LegacyEnergyMax_CheckAuthorization()))
        throw new UnauthorizedAccessException();
    }

    public Action<string> Notify { get; set; }

    public void ShowNotificationDialog(string featurePartNumber)
    {
      featurePartNumber = "1415083";
      new CLA_Dialog(featurePartNumber, this.MeterPartNumber, this.MeterSerialNumber, this.FreeTrialRemainingDays).ShowDialog();
    }

    public virtual RegistrationConfig RegistrationConfig
    {
      get
      {
        RegistrationConfig registrationConfig1 = this.registrationConfig;
        if (registrationConfig1 != null)
          return registrationConfig1;
        RegistrationConfig registrationConfig2 = new RegistrationConfig();
        registrationConfig2.AppName = "Coherent Meter Connection";
        registrationConfig2.AppGuid = CMC_CLA.CmcProductGuid;
        registrationConfig2.AppVersion = this.Version;
        RegistrationConfig registrationConfig3 = registrationConfig2;
        this.registrationConfig = registrationConfig2;
        return registrationConfig3;
      }
    }

    [Conditional("TRACE_CMC_CLA")]
    private void Trace(string message)
    {
    }

    [Conditional("TRACE_UNIVERSAL")]
    private void TraceUniversal(string message)
    {
    }
  }
}
