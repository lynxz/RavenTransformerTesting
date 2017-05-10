using System.Linq;
using Raven.Client.Indexes;

namespace RavenTransformerTesting.Indexes {
  /// <summary>
  /// Groups failure modes.
  /// </summary>
  public class FailureModeTransformer : AbstractTransformerCreationTask<FailureModeSearch.FailureModeResult> {

    #region Result 

    /// <summary>
    /// Result of failure mode grouping;
    /// </summary>
    public class Result {

      /// <summary>
      /// Process step name.
      /// </summary>
      public string BinningStep { get; set; }

      /// <summary>
      /// Product code of device.
      /// </summary>
      public string ProductCode { get; set; }

      /// <summary>
      /// Failure mode of grouping.
      /// </summary>
      public string FailureMode { get; set; }

      /// <summary>
      /// Number of failure modes.
      /// </summary>
      public int FailureCount { get; set; }

    }

    #endregion

    #region Constructor

    /// <summary>
    /// DefaultConstructor
    /// </summary>
    public FailureModeTransformer( ) {
      TransformResults = failureModes => from failureMode in failureModes
                                         group failureMode by new { failureMode.FailureMode, failureMode.BinningStep, failureMode.ProductCode } into g
                                         select new Result {
                                           BinningStep = g.Key.BinningStep,
                                           ProductCode = g.Key.ProductCode,
                                           FailureMode = g.Key.FailureMode,
                                           FailureCount = g.Count( )
                                         };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Transformer name.
    /// </summary>
    public override string TransformerName => "FailureMode/Transformer";

    #endregion
  }
}
