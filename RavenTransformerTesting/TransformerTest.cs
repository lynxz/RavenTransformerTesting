using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Raven.Client;
using Raven.Client.Linq;
using Raven.Tests.Helpers;
using RavenTransformerTesting.Indexes;
using RavenTransformerTesting.Models;
using Xunit;

namespace RavenTransformerTesting {
  public class TransformerTest : RavenTestBase {

    #region Fields

    readonly IDocumentStore _store;

    #endregion

    #region Constructor

    public TransformerTest( ) {
      _store = NewDocumentStore( );
      var index = new FailureModeSearch( );
      index.Execute( _store );
      var transformer = new FailureModeTransformer( );
      transformer.Execute( _store );

      GenerateDocuments( );
    }

    #endregion

    [Fact]
    public void GroupWithTransformerTest( ) {
      using( var session = _store.OpenSession( ) ) {
        var results = GetTransformedResults( session ).ToList( );
        ValidateResults( results );
      }
    }

    [Fact]
    public void GroupInCodeTest( ) {
      using( var session = _store.OpenSession( ) ) {
        var results = GetGroupedResults( session ).ToList( );
        ValidateResults( results );
      }
    }

    #region Methods

    void ValidateResults( List<FailureModeTransformer.Result> results ) {
      results.Should( ).HaveCount( 3 );
      results.Single( r => r.FailureMode == "Param1" ).FailureCount.Should( ).Be( 5000 );
      results.Single( r => r.FailureMode == "Param2" ).FailureCount.Should( ).Be( 5000 );
      results.Single( r => r.FailureMode == "Param3" ).FailureCount.Should( ).Be( 2500 );
    }

    static IEnumerable<FailureModeTransformer.Result> GetTransformedResults( IDocumentSession session ) {
      var query = GetTransformedQuery( session );

      using( var stream = session.Advanced.Stream( query ) ) {
        while( stream.MoveNext( ) ) {
          yield return stream.Current.Document;
        }
      }
    }

    static IEnumerable<FailureModeTransformer.Result> GetGroupedResults( IDocumentSession session ) {
      return from doc in GetResults( session )
             group doc by new { doc.BinningStep, doc.FailureMode, doc.ProductCode } into g
             select new FailureModeTransformer.Result {
               BinningStep = g.Key.BinningStep,
               FailureMode = g.Key.FailureMode,
               ProductCode = g.Key.ProductCode,
               FailureCount = g.Count( )
             };
    }

    static IEnumerable<FailureModeSearch.FailureModeResult> GetResults( IDocumentSession session ) {
      var query = GetQuery( session ).ProjectFromIndexFieldsInto<FailureModeSearch.FailureModeResult>( );

      using( var stream = session.Advanced.Stream( query ) ) {
        while( stream.MoveNext( ) ) {
          yield return stream.Current.Document;
        }
      }
    }

    static IRavenQueryable<FailureModeTransformer.Result> GetTransformedQuery( IDocumentSession session ) {
      return GetQuery( session ).TransformWith<FailureModeTransformer, FailureModeTransformer.Result>( );
    }

    static IRavenQueryable<FailureModeSearch.FailureModeResult> GetQuery( IDocumentSession session ) {
      return session.Query<FailureModeSearch.FailureModeResult, FailureModeSearch>( )
        .Where( doc => doc.BinningStep == "Bar.SomeLevel.Binning" )
        .Where( doc => doc.ProductCode == "24000" )
        .Where( doc => doc.BatchId == "0222-0540" );
    }

    public override void Dispose( ) {
      base.Dispose( );
      _store.Dispose( );
    }

    void GenerateDocuments( ) {
      using( var bulkInsert = _store.BulkInsert( ) ) {
        for( int i = 0; i < 5000; i++ ) {
          var doc = new ProcessStepDocument {
            Batch = "0540",
            Project = "0222",
            DeviceId = ( 99990000 + i ).ToString( ),
            StepId = "100.100.100",
            StepName = "Bar.SomeLevel.Binning",
            StartTime = DateTimeOffset.Now.AddHours( -1 ),
            StopTime = DateTimeOffset.Now,
            Parameters = new List<Parameter> {
            new Parameter { Name = "Param1", Value = 0, Tags = new List<string> { "Type=BinningCondition", "ProductCodeRev=24000.r015" } },
            new Parameter { Name = "Param2", Value = 0, Tags = new List<string> { "Type=BinningCondition", "ProductCodeRev=24000.r015" } },
            new Parameter { Name = "Param3", Value = i % 2, Tags = new List<string> { "Type=BinningCondition", "ProductCodeRev=24000.r015" } },
            new Parameter { Name = "Param4", Value = 1, Tags = new List<string> { "Type=BinningCondition", "ProductCodeRev=24000.r015" } },
          }
          };
          bulkInsert.Store( doc );
        }
      }

      WaitForIndexing( _store );
    }

    #endregion

  }
}
