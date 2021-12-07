using System;

namespace RaceResults.Common.Models
{
    public interface IModel
    {
        public Guid Id { get; set; }

        public string GetPartitionKey();
    }

    public interface IModelz
    {
        public Guid Id { get; set; }

        public string GetPartitionKey();
    }
}
