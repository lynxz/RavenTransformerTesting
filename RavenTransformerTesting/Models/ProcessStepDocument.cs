using System;
using System.Collections.Generic;

namespace RavenTransformerTesting.Models {
  /// <summary>
  /// Represents a single test step. 
  /// </summary>
  public class ProcessStepDocument {

    #region Constructor

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ProcessStepDocument( ) {
      Parameters = new List<Parameter>( );
      StartTime = new DateTimeOffset( new DateTime( 2010, 10, 10 ), TimeSpan.Zero );
      StopTime = DateTimeOffset.UtcNow;
    }

    #endregion

    #region Properties

    /// <summary>
    /// RavenDB document ID.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Serial number of device.
    /// </summary>
    public string DeviceId { get; set; }

    /// <summary>
    /// ID of the process step.
    /// </summary>
    public string StepId { get; set; }

    /// <summary>
    /// Name of the process step.
    /// </summary>
    public string StepName { get; set; }

    /// <summary>
    /// Project that the process step was performed in.
    /// </summary>
    public string Project { get; set; }

    /// <summary>
    /// Batch that the process step was performed in.
    /// </summary>
    public string Batch { get; set; }

    /// <summary>
    /// Name of batch that the process step was performed in.
    /// </summary>
    public string BatchName { get; set; }

    /// <summary>
    /// When the execution of the process step began.
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// When the execution of the process step concluded.
    /// </summary>
    public DateTimeOffset StopTime { get; set; }

    /// <summary>
    /// Measurement or binning data of the process step.
    /// </summary>
    public List<Parameter> Parameters { get; set; }

    #endregion

  }
}
