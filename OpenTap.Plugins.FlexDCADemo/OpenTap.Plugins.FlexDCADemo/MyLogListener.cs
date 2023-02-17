using OpenTap;
using OpenTap.Diagnostic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenTap.Plugins.FlexDCADemo
{
    [Display("Training 2023 Listener", Group: "FlexDCADemo", Description: "This is a result listener that print to log.")]
    public class MyLogListener : ResultListener
    {

        private static TraceSource log = OpenTap.Log.CreateSource("Training 2023 Listener" + typeof(MyLogListener).Name);

        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change
        #endregion

        public MyLogListener()
        {
            Name = "Training 2023 Listener";
            // ToDo: Set default values for properties / settings.
        }

        public override void OnTestPlanRunStart(TestPlanRun planRun)
        {
            log.Info($"Test Plan: {planRun.TestPlanName} starts.");
        }

        public override void OnTestStepRunStart(TestStepRun stepRun)
        {
            log.Info($"Test step: {stepRun.TestStepName} starts.");
        }

        public override void OnResultPublished(Guid stepRun, ResultTable result)
        {
            // Add handling code for result data.
            OnActivity();
            
            foreach (var col in result.Columns)
            {
                foreach (var row in col.Data)
                {
                    log.Info($"{col.Name} : {row.ToString()}");
                }
            }
        }

        public override void OnTestStepRunCompleted(TestStepRun stepRun)
        {
            log.Info($"Test step: {stepRun.TestStepName} completed.");
        }

        public override void OnTestPlanRunCompleted(TestPlanRun planRun, Stream logStream)
        {
            log.Info($"Test Plan: {planRun.TestPlanName} completed.");

        }

        public override void Open()
        {
            base.Open();
            //Add resource open code.
        }

        public override void Close()
        {
            //Add resource close code.
            base.Close();
        }
    }
}
