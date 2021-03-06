﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Different Plot types.
    /// </summary>
    public enum PlotDataType
    {
        Magnitude,
        Direction,
        Amplitude,
        BottomTrackRange,
        BottomTrackEarthVelocity,
        Voltage
    }

    /// <summary>
    /// Plot View Model interface.
    /// </summary>
    public interface IPlotLayer
    {
        /// <summary>
        /// Load the project to the plot.
        /// </summary>
        /// <param name="fileName">File Path to the project file.</param>
        /// <param name="ssConfig">Subsystem configuration.</param>
        /// <param name="minIndex">Minimum ensemble index to display.</param>
        /// <param name="maxIndex">Minimum ensemble index to display.</param>
        void LoadProject(string fileName, SubsystemConfiguration ssConfig, int minIndex = 0, int maxIndex = 0);

        /// <summary>
        /// Update the min and max ensembles selected.
        /// </summary>
        /// <param name="minIndex">Minimum ensemble index.</param>
        /// <param name="maxIndex">Maximum ensemble index.</param>
        void ReplotData(int minIndex, int maxIndex);

        /// <summary>
        /// Update plot.
        /// </summary>
        void ReplotData();
    }
}
