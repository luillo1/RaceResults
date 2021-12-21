using System;

namespace RaceResults.Common.Models
{
    public interface IModel
    {
        Guid Id { get; set; }

        string GetPartitionKey();
    }
}
