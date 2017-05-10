using System.Collections.Generic;

namespace RavenTransformerTesting.Models {
  /// <summary>
  /// Measured data unit.
  /// </summary>
  public class Parameter {

    #region Constructor

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <remarks>
    /// Initializes list of tags.
    /// </remarks>
    public Parameter( ) {
      Tags = new List<string>( );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Name of the measured data.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Value of the measured data.
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// Additional measured data information.
    /// </summary>
    public List<string> Tags { get; set; }

    #endregion

  }
}
