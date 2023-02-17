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

        [Display("2023 FlexDCA", Group: "Resources", Order: 0)]
        public FlexDCAInstrument MyFlexDCA { get; set; }

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
            var settings = new FlexDCAInstrument.Settings(WaveformCount, WaveformModulationSelected, WaveformAmplitude,
                ModuleSelected, OperatingMode.eye, WaveformType.data);
            
            MyFlexDCA.Configure(settings);

            // Get the result
            var result = MyFlexDCA.MeasureEyeDiagram(settings);

            Results.Publish("Data Publish", new { height = result[0], width = result[1] });

            foreach (var item in result)
            {
                if(!(item < 9.91E+37))
                {
                    this.UpgradeVerdict(Verdict.Error);
                }
            }

            this.UpgradeVerdict(Verdict.Pass);
            
        }

        public override void PostPlanRun()
        {
            base.PostPlanRun();
            Log.Debug($"This is PostPlanRun of {nameof(EyeDiagramStep)}");
        }
    }
}
