using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTap;   // Use OpenTAP infrastructure/core components (log,TestStep definition, etc)

namespace OpenTap.Plugins.FlexDCADemo
{
    [Display("Eye Diagram", Group: "FlexDCADemo", Description: "This step measure and analyze eye diagram")]
    public class EyeDiagramStep : TestStep
    {
        #region Settings
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

        [Display("Extended Module", Group: "Module", Order: 1)]
        public ExtendedModuleType ModuleSelected { get; set; }


        [Display("Number of Waveform", Group: "Waveform", Order: 2)]
        public double WaveformCount { get; set; }


        [Display("Waveform Type", Group: "Waveform", Order: 3)]
        public WaveformModulation WaveformModulationSelected { get; set; }

        [Display("Waveform Amplitude", Group: "Waveform", Order: 4)]
        [Unit("V", true)]
        public double WaveformAmplitude { get; set; }

        #endregion

        public EyeDiagramStep()
        {

            WaveformCount = 100;
            WaveformModulationSelected = WaveformModulation.NonReturnToZero;
            ModuleSelected = ExtendedModuleType.DualElectrical;
            WaveformAmplitude = 0.5;

            Rules.Add(() => WaveformCount > 0, "Waveform count size must be greater than zero", nameof(WaveformCount));
            Rules.Add(() => WaveformAmplitude > 0, "Waveform amplitude must be greater than zero", nameof(WaveformAmplitude));
        }

        public override void PrePlanRun()
        {
            base.PostPlanRun();
            Log.Debug($"This is PostPlanRun of {nameof(EyeDiagramStep)}");
        }

        public override void Run()
        {
            configure(WaveformModulationSelected, WaveformCount, ModuleSelected, WaveformAmplitude);
            var result = measureEyeDiagram();

            Results.Publish("Eye diagram", new { EyeDiagramResult = result });

            if (result < 9.91E+37)
                UpgradeVerdict(Verdict.Pass);
            else
                UpgradeVerdict(Verdict.Fail);

            RunChildSteps();
        }

        public override void PostPlanRun()
        {
            base.PostPlanRun();
            Log.Debug($"This is PostPlanRun of {nameof(EyeDiagramStep)}");
        }


        private void configure(WaveformModulation mod, double waveformCount, ExtendedModuleType moduleType, double waveformAmplitude)
        {
            Log.Debug("Setup waveform type to" + mod.ToString());
            Log.Debug("Setup waveform count to" + waveformCount);
            Log.Debug("Setup module type to" + moduleType.ToString());
            Log.Debug("Setup waveform amplitude to" + waveformAmplitude);
        }

        private double measureEyeDiagram()
        {
            return 6.47E+2;
        }
    }
}
