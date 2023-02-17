using OpenTap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

//Note this template assumes that you have a SCPI based instrument, and accordingly
//extends the ScpiInstrument base class.

//If you do NOT have a SCPI based instrument, you should modify this instance to extend
//the (less powerful) Instrument base class.

namespace OpenTap.Plugins.FlexDCADemo
{

    public enum WaveformModulation
    {
        [Scpi("UNSP")]
        Unspecified,
        [Scpi("NRZ")]
        NonReturnToZero,
        [Scpi("PAM4")]
        FourLevelPAM
    }

    public enum ExtendedModuleType
    {
        [Scpi("DEM")]
        DualElectrical,
        [Scpi("DOM")]
        DualOptical,
        [Scpi("OEM")]
        ElectricalOptical,
        [Scpi("QEM")]
        QuadElectricalOptical
    }

    public enum WaveformType
    {
        [Scpi("CLOC")]
        clock,
        [Scpi("DATA")]
        data,
        [Scpi("FUNC")]
        function,
        [Scpi("FILE")]
        file
    }

    public enum OperatingMode
    {
        [Scpi("EYE")]
        eye,
        [Scpi("OSC")]
        oscilloscope,
        [Scpi("JITT")]
        jitter,
        [Scpi("TDR")]
        timeDomainReflectometry
    }

    [Display("FlexDCA Instrument", Group: "FLexDCADemo", Description: "Flex DCA Sampling Oscilloscope")]
    public class FlexDCAInstrument : ScpiInstrument
    {
        #region Settings
        public class Settings
        {
            public double WaveformCount { get; private set; }
            public WaveformModulation WaveformModulationSelected { get; private set; }
            public double WaveformAmplitude { get; private set; }
            public ExtendedModuleType ModuleSelected { get; private set; }
            public WaveformType waveformTypeSelected { get; private set; }
            public OperatingMode operatingModeSelected { get; private set; }

            public Settings(double waveformCount, WaveformModulation modulationType, double waveformAmplitude,
                ExtendedModuleType moduleSelected, OperatingMode operatingModeSelected, WaveformType waveformType)
            {
                this.WaveformCount = waveformCount;
                this.WaveformModulationSelected = modulationType;
                this.WaveformAmplitude = waveformAmplitude;
                this.ModuleSelected = moduleSelected;
                this.waveformTypeSelected = waveformType;
                this.operatingModeSelected = operatingModeSelected;
            }
        }
        #endregion

        // Constructor
        public FlexDCAInstrument()
        {
            Name = "FlexDCA";
            // ToDo: Set default values for properties / settings.
        }

        public void Configure(Settings settings)
        {
            //Configure source
            ScpiCommand(":SYSTEM:DEFault");
            ScpiCommand(":EMODules:SLOT5:SELection " + Scpi.Format("{0}", settings.ModuleSelected));
            ScpiCommand(":SOUR5A:FORMat " + Scpi.Format("{0}", settings.WaveformModulationSelected));
            ScpiCommand($":SOUR5A:AMPLitude {settings.WaveformAmplitude}");
            ScpiCommand(":CHAN5A:DISPlay ON"); // Turn channel 5A on
            ScpiCommand($":SOUR5A:WTYPe {settings.waveformTypeSelected}");
            ScpiCommand($":SYSTem:MODE {settings.operatingModeSelected}");
        }
        
        private double MeasureWidth()
        {
            ScpiCommand(":MEASure:EYE:EWIDth"); // Eye width measurement
            double eyewidth = ScpiQuery<double>(":MEASure:EYE:EWIDth:MEAN?");
            Log.Info("Eye width (mean): " + eyewidth.ToString() + "s");
            return eyewidth;
        }

        private double MeasureEyeHeight()
        {
            ScpiCommand(":MEASure:EYE:EHEight"); // Eye height measurement
            double eyeHeight = ScpiQuery<double>(":MEASure:EYE:EHEight:MEAN?");
            Log.Info("Eye Height (mean): " + eyeHeight.ToString() + "V");
            return eyeHeight;
        }

        public double[] MeasureEyeDiagram(Settings settings)
        {
            ScpiCommand(":ACQuire:SINGle"); // single acquisition mode
            ScpiCommand(":ACQuire:CDISplay"); // Clear display
            ScpiCommand($":LTESt:ACQuire:CTYPe:WAVeforms {settings.WaveformCount}");
            ScpiCommand(":LTESt:ACQuire:STATe ON");
            if (!ScpiQuery(":ACQuire:RUN;*OPC?").Equals("1\n"))
            {
                string error = ScpiQuery(":SYSTem:ERRor?");
                Log.Error("Acquire run with error" + error);
                throw new Exception("Fail to :ACQuire:RUN.");
            }
            ScpiCommand(":LTESt:ACQuire:STATe OFF");
            ScpiCommand(":SYSTem:GTLocal");
            return new double[] { MeasureEyeHeight(), MeasureWidth() };
        }

        /// <summary>
        /// Open procedure for the instrument.
        /// </summary>
        public override void Open()
        {

            base.Open();
            // TODO:  Open the connection to the instrument here

            if (!IdnString.Contains("N1010A"))
            {
                Log.Error("This instrument driver does not support the connected instrument.");
                throw new ArgumentException("Wrong instrument type.");
            }

        }

        /// <summary>
        /// Close procedure for the instrument.
        /// </summary>
        public override void Close()
        {
            // TODO:  Shut down the connection to the instrument here.
            base.Close();
        }
    }
}
