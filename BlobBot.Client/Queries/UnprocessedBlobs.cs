using BlobBot.Shared.Models;
using Dapper.QX.Abstract;
using Dapper.QX.Interfaces;

namespace BlobBot.Client.Queries
{
    internal class UnprocessedBlobs : TestableQuery<BlobCreated>
    {
        public UnprocessedBlobs() : base(
            @"SELECT [b].*
            FROM [eventgrid].[BlobCreated] [b]
            WHERE [Status]=0
            ORDER BY [DateCreated]")
        {
        }

        protected override IEnumerable<ITestableQuery> GetTestCasesInner()
        {
            yield return new UnprocessedBlobs();
        }
    }
}
