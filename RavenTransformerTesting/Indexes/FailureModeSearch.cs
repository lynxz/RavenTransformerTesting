using System;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace RavenTransformerTesting.Indexes {
  /// <summary>
  /// Failure mode index.
  /// </summary>
  public class FailureModeSearch : AbstractIndexCreationTask<Models.ProcessStepDocument, FailureModeSearch.FailureModeResult> {

    #region FailureModeResult

    /// <summary>
    /// Failure mode index result.
    /// </summary>
    public class FailureModeResult {

      /// <summary>
      /// Process step name.
      /// </summary>
      public string BinningStep { get; set; }

      /// <summary>
      /// Product code of binning.
      /// </summary>
      public string ProductCode { get; set; }

      /// <summary>
      /// Failure mode.
      /// </summary>
      public string FailureMode { get; set; }

      /// <summary>
      /// Wafer ID of binned device.
      /// </summary>
      public string WaferId { get; set; }

      /// <summary>
      /// Batch ID for which the process step was 
      /// executed in.
      /// </summary>
      public string BatchId { get; set; }

      /// <summary>
      /// The number of days since <see cref="ProcessStepDocument.Epoch"/>
      /// date.
      /// </summary>
      public int DayNumberSinceMinDate { get; set; }

      /// <summary>
      /// Determines if this process step is the latest executed.
      /// </summary>
      public bool LatestRun { get; set; }

      /// <summary>
      /// Name of the station that the process step was executed
      /// on.
      /// </summary>
      public string Station { get; set; }

    }

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor.
    /// </summary>
    public FailureModeSearch( ) {

      Map = docs => from doc in docs
                    where doc.StepName.EndsWith( "Binning", StringComparison.Ordinal )
                    from param in doc.Parameters
                    where param.Tags != null && param.Tags.Any( x => x == "Type=BinningCondition" || x == "Type=ControlCondition" )
                    where ( uint )param.Value == 0
                    let productCode = param.Tags.First( x => x.StartsWith( "ProductCodeRev", StringComparison.Ordinal ) ).Split( '=' )[ 1 ].Split( '.' )[ 0 ]
                    select new FailureModeResult {
                      BinningStep = doc.StepName,
                      ProductCode = productCode,
                      FailureMode = param.Name,
                      DayNumberSinceMinDate = ( int )( ( DateTimeOffset )doc.StopTime ).Subtract( DateTimeOffset.MinValue ).TotalDays,
                      BatchId = $"{doc.Project}-{doc.Batch}"
                    };


      MaxIndexOutputsPerDocument = 100;

      StoreAllFields( FieldStorage.Yes );

    }

    #endregion

    #region Properties

    /// <summary>
    /// Index name.
    /// </summary>
    public override string IndexName => "FailureMode/Search";

    #endregion


  }
}
